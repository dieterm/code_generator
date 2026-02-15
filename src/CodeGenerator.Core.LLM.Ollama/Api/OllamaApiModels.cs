using System.Text.Json.Serialization;

namespace CodeGenerator.Core.LLM.Ollama.Api
{
    internal class OllamaChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<OllamaMessage> Messages { get; set; } = new();

        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = true;

        [JsonPropertyName("tools")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<OllamaTool>? Tools { get; set; }
    }

    internal class OllamaMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("tool_calls")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<OllamaToolCall>? ToolCalls { get; set; }
    }

    internal class OllamaChatResponse
    {
        [JsonPropertyName("message")]
        public OllamaMessage? Message { get; set; }

        [JsonPropertyName("done")]
        public bool Done { get; set; }
    }

    internal class OllamaTool
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "function";

        [JsonPropertyName("function")]
        public OllamaToolFunction Function { get; set; } = new();
    }

    internal class OllamaToolFunction
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("parameters")]
        public OllamaToolParameters Parameters { get; set; } = new();
    }

    internal class OllamaToolParameters
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "object";

        [JsonPropertyName("properties")]
        public Dictionary<string, OllamaToolProperty> Properties { get; set; } = new();

        [JsonPropertyName("required")]
        public List<string> Required { get; set; } = new();
    }

    internal class OllamaToolProperty
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "string";

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("enum")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Enum { get; set; }
    }

    internal class OllamaToolCall
    {
        [JsonPropertyName("function")]
        public OllamaToolCallFunction? Function { get; set; }
    }

    internal class OllamaToolCallFunction
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("arguments")]
        public Dictionary<string, object>? Arguments { get; set; }
    }

    internal class OllamaTagsResponse
    {
        [JsonPropertyName("models")]
        public List<OllamaModelInfo>? Models { get; set; }
    }

    internal class OllamaModelInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
