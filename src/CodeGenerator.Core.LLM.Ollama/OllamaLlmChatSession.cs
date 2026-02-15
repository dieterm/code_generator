using CodeGenerator.Core.LLM.Ollama.Api;
using CodeGenerator.Core.LLM.Providers;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeGenerator.Core.LLM.Ollama
{
    /// <summary>
    /// Ollama chat session with streaming and tool-calling support.
    /// Implements the Ollama tool-calling protocol:
    /// 1. Send messages + tools definition
    /// 2. If response contains tool_calls, execute them and send results back
    /// 3. Repeat until the model responds with plain content
    /// </summary>
    internal class OllamaLlmChatSession : ILlmChatSession
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _model;
        private readonly LlmSessionConfig _config;
        private readonly ILogger _logger;
        private readonly List<OllamaMessage> _conversationHistory = new();
        private readonly Dictionary<string, AIFunction> _toolLookup = new();

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public string SessionId { get; } = Guid.NewGuid().ToString();

        public OllamaLlmChatSession(
            HttpClient httpClient,
            string baseUrl,
            string model,
            LlmSessionConfig config,
            ILogger logger)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
            _model = model;
            _config = config;
            _logger = logger;

            // Add system message to conversation history
            if (!string.IsNullOrEmpty(_config.SystemMessage))
            {
                _conversationHistory.Add(new OllamaMessage
                {
                    Role = "system",
                    Content = _config.SystemMessage
                });
            }

            // Build tool lookup for execution
            foreach (var tool in _config.Tools)
            {
                _toolLookup[tool.Name] = tool;
            }
        }

        public async Task SendAsync(string prompt, Action<LlmChatEvent> onEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _conversationHistory.Add(new OllamaMessage
                {
                    Role = "user",
                    Content = prompt
                });

                // Tool-calling loop: the model may request tool calls multiple times
                while (true)
                {
                    var request = new OllamaChatRequest
                    {
                        Model = _model,
                        Messages = _conversationHistory,
                        Stream = _config.Streaming,
                        Tools = BuildToolsDefinition()
                    };

                    var json = JsonSerializer.Serialize(request, _jsonOptions);
                    _logger.LogDebug("Ollama request: {Url}, model: {Model}", $"{_baseUrl}/api/chat", _model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync($"{_baseUrl}/api/chat", content, cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                        _logger.LogError("Ollama API returned {StatusCode}: {ErrorBody}", response.StatusCode, errorBody);
                        throw new HttpRequestException(
                            $"Ollama API returned {(int)response.StatusCode} ({response.StatusCode}): {errorBody}");
                    }

                    OllamaMessage? assistantMessage;

                    if (_config.Streaming)
                    {
                        assistantMessage = await ProcessStreamingResponseAsync(response, onEvent, cancellationToken);
                    }
                    else
                    {
                        assistantMessage = await ProcessNonStreamingResponseAsync(response, onEvent, cancellationToken);
                    }

                    if (assistantMessage == null)
                        break;

                    // Add assistant message to history
                    _conversationHistory.Add(assistantMessage);

                    // Check if the assistant wants to call tools
                    // 
                    if(assistantMessage.ToolCalls == null && assistantMessage.Content!=null)
                    {
                        if(assistantMessage.Content.StartsWith("```json") && assistantMessage.Content.EndsWith("```"))
                        {
                            // Strip code block markers if present
                            assistantMessage.Content = assistantMessage.Content[7..^3].Trim();
                        }
                        try
                        {
                            // try deserialize content as tool calls (for backward compatibility with older Ollama versions)
                            var toolCall = JsonSerializer.Deserialize<OllamaToolCallFunction>(assistantMessage.Content, _jsonOptions);
                            if(toolCall != null)
                            {
                                assistantMessage.ToolCalls = new List<OllamaToolCall>
                                {
                                    new OllamaToolCall
                                    {
                                        Function = toolCall
                                    }
                                };
                                _logger.LogInformation("Ollama response included a tool call in content");
                            }
                            else
                            {
                                _logger.LogInformation("Ollama response complete with no tool calls");
                            }
                        }
                        catch (Exception ex)
                        {
                             _logger.LogError(ex, "Error deserializing Ollama tool call");
                            throw;
                        }
                        
                    }
                    if (assistantMessage.ToolCalls != null && assistantMessage.ToolCalls.Count > 0)
                    {
                        _logger.LogInformation("Ollama requested {Count} tool call(s)", assistantMessage.ToolCalls.Count);

                        foreach (var toolCall in assistantMessage.ToolCalls)
                        {
                            var functionName = toolCall.Function?.Name ?? "";
                            var arguments = toolCall.Function?.Arguments ?? new Dictionary<string, object>();
                            var argsJson = JsonSerializer.Serialize(arguments, _jsonOptions);

                            onEvent(new LlmToolCallEvent(functionName, argsJson));

                            var toolResult = await ExecuteToolCallAsync(toolCall);

                            onEvent(new LlmToolResultEvent(functionName, toolResult));

                            // Add tool result to conversation
                            _conversationHistory.Add(new OllamaMessage
                            {
                                Role = "tool",
                                Content = toolResult
                            });
                        }

                        // Continue the loop to let the model process the tool results
                        continue;
                    }

                    // No tool calls, we're done
                    break;
                }

                onEvent(new LlmSessionIdleEvent());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Ollama chat session");
                throw;
            }
        }

        private async Task<OllamaMessage?> ProcessStreamingResponseAsync(
            HttpResponseMessage response,
            Action<LlmChatEvent> onEvent,
            CancellationToken cancellationToken)
        {
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            var fullContent = new StringBuilder();
            List<OllamaToolCall>? toolCalls = null;

            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(line)) continue;

                var chunk = JsonSerializer.Deserialize<OllamaChatResponse>(line, _jsonOptions);
                if (chunk?.Message != null)
                {
                    if (!string.IsNullOrEmpty(chunk.Message.Content))
                    {
                        fullContent.Append(chunk.Message.Content);
                        onEvent(new LlmMessageDeltaEvent(chunk.Message.Content));
                    }

                    if (chunk.Message.ToolCalls != null && chunk.Message.ToolCalls.Count > 0)
                    {
                        toolCalls ??= new List<OllamaToolCall>();
                        toolCalls.AddRange(chunk.Message.ToolCalls);
                    }
                }

                if (chunk?.Done == true)
                    break;
            }

            return new OllamaMessage
            {
                Role = "assistant",
                Content = fullContent.ToString(),
                ToolCalls = toolCalls
            };
        }

        private async Task<OllamaMessage?> ProcessNonStreamingResponseAsync(
            HttpResponseMessage response,
            Action<LlmChatEvent> onEvent,
            CancellationToken cancellationToken)
        {
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<OllamaChatResponse>(json, _jsonOptions);

            if (result?.Message == null)
                return null;

            if (!string.IsNullOrEmpty(result.Message.Content))
            {
                onEvent(new LlmMessageCompleteEvent(result.Message.Content));
            }

            return new OllamaMessage
            {
                Role = "assistant",
                Content = result.Message.Content ?? "",
                ToolCalls = result.Message.ToolCalls
            };
        }

        private async Task<string> ExecuteToolCallAsync(OllamaToolCall toolCall)
        {
            var functionName = toolCall.Function?.Name ?? "";
            var arguments = toolCall.Function?.Arguments ?? new Dictionary<string, object>();

            _logger.LogInformation("Executing tool: {FunctionName} with args: {Args}",
                functionName, JsonSerializer.Serialize(arguments));

            if (!_toolLookup.TryGetValue(functionName, out var aiFunction))
            {
                var error = $"Unknown tool: {functionName}";
                _logger.LogWarning(error);
                return error;
            }

            try
            {
                // Build AIFunctionArguments from the dictionary
                var aiArgs = new AIFunctionArguments(arguments);
                var result = await aiFunction.InvokeAsync(aiArgs);
                var resultStr = result?.ToString() ?? "";
                _logger.LogInformation("Tool {FunctionName} returned: {Result}",
                    functionName, resultStr.Length > 200 ? resultStr[..200] + "..." : resultStr);
                return resultStr;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing tool {FunctionName}", functionName);
                return $"Error executing {functionName}: {ex.Message}";
            }
        }

        private List<OllamaTool>? BuildToolsDefinition()
        {
            if (_config.Tools.Count == 0)
                return null;

            var tools = new List<OllamaTool>();

            foreach (var aiFunction in _config.Tools)
            {
                var tool = new OllamaTool
                {
                    Type = "function",
                    Function = new OllamaToolFunction
                    {
                        Name = aiFunction.Name,
                        Description = aiFunction.Description,
                        Parameters = BuildParametersFromAIFunction(aiFunction)
                    }
                };
                tools.Add(tool);
            }

            return tools;
        }

        private static OllamaToolParameters BuildParametersFromAIFunction(AIFunction aiFunction)
        {
            var parameters = new OllamaToolParameters();

            if (aiFunction.JsonSchema is JsonElement schemaElement &&
                schemaElement.ValueKind == JsonValueKind.Object)
            {
                // Use the JSON schema from the AIFunction directly
                if (schemaElement.TryGetProperty("properties", out var propsElement) &&
                    propsElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in propsElement.EnumerateObject())
                    {
                        var propType = "string";
                        var propDesc = "";

                        if (prop.Value.TryGetProperty("type", out var typeElem))
                            propType = typeElem.GetString() ?? "string";
                        if (prop.Value.TryGetProperty("description", out var descElem))
                            propDesc = descElem.GetString() ?? "";

                        parameters.Properties[prop.Name] = new OllamaToolProperty
                        {
                            Type = propType,
                            Description = propDesc
                        };
                    }
                }

                if (schemaElement.TryGetProperty("required", out var reqElement) &&
                    reqElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var req in reqElement.EnumerateArray())
                    {
                        var reqName = req.GetString();
                        if (reqName != null)
                            parameters.Required.Add(reqName);
                    }
                }
            }

            return parameters;
        }

        public ValueTask DisposeAsync()
        {
            _conversationHistory.Clear();
            _toolLookup.Clear();
            return ValueTask.CompletedTask;
        }
    }
}
