using CodeGenerator.Core.LLM.Providers;
using CodeGenerator.Core.LLM.Settings;
using CodeGenerator.Core.Settings.Interfaces;
using CodeGenerator.Core.Settings.Models;
using CodeGenerator.Shared;
using CodeGenerator.UserControls.ViewModels;
using GitHub.Copilot.SDK;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.LLM.Copilot
{
    /// <summary>
    /// GitHub Copilot SDK implementation of ILlmProvider
    /// </summary>
    public class CopilotLlmProvider : ILlmProvider
    {
        public const string PROVIDER_ID = "GitHubCopilot";
        private readonly ILogger<CopilotLlmProvider> _logger;
        private CopilotClient? _client;

        public string ProviderId => PROVIDER_ID;
        public string DisplayName => "GitHub Copilot";
        public bool IsConnected { get; private set; }
        private List<ModelInfo> _availableModels = new();
        public CopilotLlmProvider(ILogger<CopilotLlmProvider> logger)
        {
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
                    _logger.LogWarning(ex, "Failed to pre-load GitHub Copilot model list in background. Will retry on connect.");
                }
            });
        }

        private async Task LoadAvailableModelsAsync()
        {
            await ConnectAsync();
            _availableModels = await _client!.ListModelsAsync();
            await _client.DisposeAsync();
            _client = null;
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            _client = new CopilotClient();
            await _client.StartAsync();
            IsConnected = true;
            _logger.LogInformation("Connected to GitHub Copilot");
        }

        public async Task<ILlmChatSession> CreateSessionAsync(LlmSessionConfig config, CancellationToken cancellationToken = default)
        {
            if (_client == null || !IsConnected)
                throw new InvalidOperationException("Not connected to GitHub Copilot. Call ConnectAsync first.");

            var sessionConfig = new SessionConfig
            {
                Model = config.Model ?? GetSettingsParameter<string>(DEFAULT_MODEL_PARAMETER_KEY),
                Streaming = config.Streaming,
                SystemMessage = new SystemMessageConfig
                {
                    Mode = SystemMessageMode.Append,
                    Content = config.SystemMessage ?? ""
                },
                Tools = config.Tools
            };

            var session = await _client.CreateSessionAsync(sessionConfig);
            return new CopilotLlmChatSession(session, _logger);
        }

        public async ValueTask DisposeAsync()
        {
            if (_client != null)
            {
                await _client.DisposeAsync();
                _client = null;
            }
            IsConnected = false;
        }
        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }

        public T? GetSettingsParameter<T>(string key)
        {
            var settingsManager = ServiceProviderHolder.GetKeyedService<ISettingsManager>("LlmSettingsManager") as LlmSettingsManager;
            return settingsManager.GetParameter<T>(PROVIDER_ID, key);
        }

        public List<ISettingsItem> GetSettingsItems(LlmProviderSettings settings)
        {
            var settingsItems = new List<ISettingsItem>();

            // TODO: get available models dynamically from the CopilotClient when that functionality is available. For now we'll hardcode the options.
            var availableModels = _availableModels.Select(m => new ComboboxItem { DisplayName = m.Name, Value = m.Id }).ToList();


            var defaultModelField = new ComboboxFieldModel
            {
                Label = "Default Model",
                Name = DEFAULT_MODEL_PARAMETER_KEY,
                Items = availableModels,
                Value = settings.Parameters.TryGetValue(DEFAULT_MODEL_PARAMETER_KEY, out object? defaultModel) ? defaultModel?? "gpt-4.1" : "gpt-4.1",
            };
            settingsItems.Add(new SettingsItem<ComboboxFieldModel>(defaultModelField, DEFAULT_MODEL_PARAMETER_KEY, "Default Model"));

            return settingsItems;
        }

        

        public const string DEFAULT_MODEL_PARAMETER_KEY = "DefaultModel";
    }
}
