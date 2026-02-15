using CodeGenerator.Core.LLM.Ollama.Settings;
using CodeGenerator.Core.LLM.Providers;
using CodeGenerator.Core.LLM.Settings;
using CodeGenerator.Core.Settings.Interfaces;
using CodeGenerator.Core.Settings.Models;
using CodeGenerator.Shared;
using CodeGenerator.UserControls.ViewModels;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace CodeGenerator.Core.LLM.Ollama
{
    /// <summary>
    /// Ollama LLM provider implementation. Connects to a local Ollama instance.
    /// </summary>
    public class OllamaLlmProvider : ILlmProvider
    {
        public const string PROVIDER_ID = "Ollama";
        private readonly HttpClient _httpClient;
        private readonly ILogger<OllamaLlmProvider> _logger;
        //private readonly OllamaSettings _settings;
        private List<string> _availableModels = new();

        public string ProviderId => PROVIDER_ID;
        public string DisplayName => "Ollama (Local)";
        public bool IsConnected { get; private set; }

        public OllamaLlmProvider(
            IHttpClientFactory httpClientFactory,
            OllamaSettings settings,
            ILogger<OllamaLlmProvider> logger)
        {
            _httpClient = httpClientFactory.CreateClient("Ollama");
            //_settings = settings;
            _logger = logger;

            // Start background loading of model list (fire-and-forget, does not block constructor)
            _ = Task.Run(async () =>
            {
                try
                {
                    await LoadAvailableModelsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to pre-load Ollama model list in background. Will retry on connect.");
                }
            });
        }

        private async Task LoadAvailableModelsAsync(CancellationToken cancellationToken = default)
        {
            var baseUrl = GetSettingsParameter<string>(BASE_URL_PARAMETER_KEY);
            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = "http://localhost:11434";
            }

            var response = await _httpClient.GetAsync($"{baseUrl}/api/tags", cancellationToken);
            response.EnsureSuccessStatusCode();

            var tags = await response.Content.ReadFromJsonAsync<Api.OllamaTagsResponse>(cancellationToken: cancellationToken);
            _availableModels = tags?.Models?.Select(m => m.Name).ToList() ?? new();
        }

        public T? GetSettingsParameter<T>(string key)
        {
            var settingsManager = ServiceProviderHolder.GetKeyedService<ISettingsManager>("LlmSettingsManager") as LlmSettingsManager;
            return settingsManager.GetParameter<T>(PROVIDER_ID, key);
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            await LoadAvailableModelsAsync(cancellationToken);

            var baseUrl = GetSettingsParameter<string>(BASE_URL_PARAMETER_KEY);
            IsConnected = true;
            _logger.LogInformation("Connected to Ollama at {BaseUrl} ({ModelCount} models available: {Models})",
                baseUrl, _availableModels.Count, string.Join(", ", _availableModels));
        }

        public Task<ILlmChatSession> CreateSessionAsync(LlmSessionConfig config, CancellationToken cancellationToken = default)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to Ollama. Call ConnectAsync first.");

            var requestedModel = config.Model ?? GetSettingsParameter<string>(DEFAULT_MODEL_PARAMETER_KEY);

            // Ollama model names can be "model:tag" or just "model" (defaults to ":latest").
            // Match both exact name and with ":latest" suffix.
            var modelExists = _availableModels.Any(m =>
                m.Equals(requestedModel, StringComparison.OrdinalIgnoreCase) ||
                m.Equals($"{requestedModel}:latest", StringComparison.OrdinalIgnoreCase));

            if (!modelExists)
            {
                throw new InvalidOperationException(
                    $"Model '{requestedModel}' is not available in Ollama. " +
                    $"Available models: {string.Join(", ", _availableModels)}. " +
                    $"Pull it first with: ollama pull {requestedModel}");
            }

            var session = new OllamaLlmChatSession(
                _httpClient,
                GetSettingsParameter<string>(BASE_URL_PARAMETER_KEY)!.TrimEnd('/'),
                requestedModel,
                config,
                _logger);

            return Task.FromResult<ILlmChatSession>(session);
        }

        public ValueTask DisposeAsync()
        {
            IsConnected = false;
            return ValueTask.CompletedTask;
        }
        public void Dispose()
        {
            IsConnected = false;
        }

        public List<ISettingsItem> GetSettingsItems(LlmProviderSettings settings)
        {
            var settingsItems = new List<ISettingsItem>();
            var defaultModelDefault = "qwen2.5-coder:7b";

            // Use cached model list if available, otherwise use fallback
            var modelList = _availableModels.Count > 0 
                ? _availableModels 
                : new List<string> { defaultModelDefault };

            if (_availableModels.Count == 0)
            {
                _logger.LogWarning("Model list not yet loaded. Settings will show default model only. Connect to Ollama to populate model list.");
            }

            var defaultModelValue = settings.Parameters.TryGetValue(DEFAULT_MODEL_PARAMETER_KEY, out object? defaultModel) 
                ? defaultModel ?? defaultModelDefault 
                : defaultModelDefault;

            var defaultModelField = new ComboboxFieldModel
            {
                Label = "Default Model",
                Name = DEFAULT_MODEL_PARAMETER_KEY,
                Items = modelList.Select(m => new ComboboxItem { DisplayName = m, Value = m }).ToList(),
                Value = defaultModelValue,
            };
            
            settingsItems.Add(new SettingsItem<ComboboxFieldModel>(defaultModelField, DEFAULT_MODEL_PARAMETER_KEY, "Default Model"));

            var defaultBaseUrl = "http://localhost:11434";
            var baseUrlField = new SingleLineTextFieldModel
            {
                Name = BASE_URL_PARAMETER_KEY,
                Label = "Ollama Base URL",
                Value = settings.Parameters.TryGetValue(BASE_URL_PARAMETER_KEY, out object? baseUrl) ? baseUrl ?? defaultBaseUrl : defaultBaseUrl
            }; 
            settingsItems.Add(new SettingsItem<SingleLineTextFieldModel>(baseUrlField, BASE_URL_PARAMETER_KEY, "Ollama Base URL"));
            return settingsItems;
        }

        

        public const string DEFAULT_MODEL_PARAMETER_KEY = "DefaultModel";
        public const string BASE_URL_PARAMETER_KEY = "BaseUrl";
    }
}
