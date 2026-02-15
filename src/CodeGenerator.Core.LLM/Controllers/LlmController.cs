using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Copilot;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.LLM.Providers;
using CodeGenerator.Core.LLM.Services;
using CodeGenerator.Core.LLM.Tools;
using CodeGenerator.Core.LLM.ViewModels;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Operations;
using CodeGenerator.Shared.Ribbon;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.LLM.Controllers
{
    /// <summary>
    /// Provider-agnostic LLM controller. Uses ILlmProvider to connect
    /// to any LLM backend (Copilot, Ollama, etc.)
    /// </summary>
    public class LlmController : CoreControllerBase, ILlmController
    {
        private readonly LlmChatViewModel _chatViewModel;
        private readonly ILlmWindowManagerService _windowManagerService;
        private readonly IEnumerable<ILlmProvider> _providers;
        private IWorkspaceContextProvider? _workspaceContextProvider;
        private WorkspaceTools? _workspaceTools;
        private LlmOperationBridge? _operationBridge;
        private ILlmProvider? _activeProvider;
        private ILlmChatSession? _activeSession;

        private const string SystemPromptContent =
            "You are a workspace assistant for a code generator application. " +
            "You help users manipulate their workspace by adding domains, entities, properties and value types.\n\n" +
            "The workspace is organized as: Workspace > Scopes (e.g. Shared, Application) > Domains > Entities/ValueTypes. " +
            "Each component in the workspace (eg. Workspace, Scope, Domain, Entity, ValueType) is an abstract Artifact and has a unique ID. " +
            "You can retrieve the workspace structure and artifact IDs using the GetWorkspaceInfo and ListScopes tools. " +
            "You should retrieve the workspace structure before performing any mutations to ensure you have the correct IDs and understand the current state. " +
            "Most tools require the relevant IDs to perform actions, so understanding the current workspace structure is crucial.\n\n" +
            "Each Entity can have properties with data types like: varchar, int, bigint, decimal, float, bool, datetime, guid, text.\n\n" +
            "When the user asks you to create domains, entities or properties, use the available tools to perform the actions. " +
            "Always confirm what you've done after performing actions. " +
            "Use PascalCase for all names (domains, entities, properties). " +
            "First call GetWorkspaceInfo and ListScopes to understand the workspace structure before making changes.";

        public LlmController(
            OperationExecutor operationExecutor,
            LlmChatViewModel chatViewModel,
            ILlmWindowManagerService windowManagerService,
            IEnumerable<ILlmProvider> providers,
            RibbonBuilder ribbonBuilder,
            ApplicationMessageBus messageBus,
            IMessageBoxService messageboxService,
            IFileSystemDialogService fileSystemDialogService,
            ILogger<LlmController> logger)
            : base(operationExecutor, ribbonBuilder, messageBus, messageboxService, fileSystemDialogService, logger)
        {
            _chatViewModel = chatViewModel;
            _windowManagerService = windowManagerService;
            _providers = providers;
        }

        public override void Initialize()
        {
            _workspaceContextProvider = ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>();
            _chatViewModel.SendMessageRequested += OnSendMessageRequested;

            // Register all IOperation implementations in the executor
            var operations = ServiceProviderHolder.GetServices<IOperation>();
            foreach (var operation in operations)
            {
                _operationExecutor.Register(operation);
            }
        }

        public void ShowCopilot()
        {
            _windowManagerService.ShowLlmChatView(_chatViewModel);

            if (!_chatViewModel.IsConnected)
            {
                _ = ConnectAsync();
            }
        }

        /// <summary>
        /// Switch to a different LLM provider by its ProviderId
        /// </summary>
        public async Task SwitchProviderAsync(string providerId)
        {
            // Disconnect current
            if (_activeSession != null)
            {
                await _activeSession.DisposeAsync();
                _activeSession = null;
            }

            _chatViewModel.IsConnected = false;
            _chatViewModel.ClearMessages();

            _activeProvider = _providers.FirstOrDefault(p => p.ProviderId == providerId);
            if (_activeProvider == null)
            {
                _chatViewModel.AddSystemMessage($"Provider '{providerId}' not found.");
                return;
            }

            await ConnectAsync();
        }

        /// <summary>
        /// Returns all registered provider display names and IDs
        /// </summary>
        public IEnumerable<(string ProviderId, string DisplayName)> GetAvailableProviders()
        {
            return _providers.Select(p => (p.ProviderId, p.DisplayName));
        }

        private async Task ConnectAsync()
        {
            try
            {
                // If no active provider, pick the first available
                _activeProvider ??= _providers.FirstOrDefault();

                if (_activeProvider == null)
                {
                    _chatViewModel.StatusText = "No LLM providers registered";
                    _chatViewModel.AddSystemMessage("No LLM providers are available. Please configure at least one provider.");
                    return;
                }

                _chatViewModel.StatusText = $"Connecting to {_activeProvider.DisplayName}...";
                _chatViewModel.ProviderName = _activeProvider.DisplayName;

                _workspaceTools = new WorkspaceTools(_workspaceContextProvider!, InvokeOnUiThread);
                var loggerFactory = ServiceProviderHolder.GetRequiredService<ILoggerFactory>();
                _operationBridge = new LlmOperationBridge(_operationExecutor, InvokeOnUiThread, loggerFactory.CreateLogger<LlmOperationBridge>());

                await _activeProvider.ConnectAsync();

                // Combine read-only query tools and operation-based mutation tools
                var tools = new List<AIFunction>();
                tools.AddRange(_workspaceTools.GetAllTools());
                tools.AddRange(_operationBridge.GenerateTools());

                _activeSession = await _activeProvider.CreateSessionAsync(new LlmSessionConfig
                {
                    Streaming = true,
                    SystemMessage = SystemPromptContent,
                    Tools = tools
                });

                _chatViewModel.IsConnected = true;
                _chatViewModel.AddSystemMessage($"Connected to {_activeProvider.DisplayName}. How can I help you with your workspace?");
                _logger.LogInformation("LLM session started (Provider={Provider}, SessionId={SessionId})",
                    _activeProvider.ProviderId, _activeSession.SessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to LLM provider {Provider}", _activeProvider?.ProviderId);
                _chatViewModel.StatusText = $"Connection failed: {ex.Message}";
                _chatViewModel.AddSystemMessage($"Failed to connect: {ex.Message}");
            }
        }

        private async void OnSendMessageRequested(object? sender, string message)
        {
            if (_activeSession == null || _chatViewModel.IsProcessing)
                return;

            _chatViewModel.AddUserMessage(message);
            _chatViewModel.IsProcessing = true;

            try
            {
                var assistantMessage = _chatViewModel.AddAssistantMessage();

                await _activeSession.SendAsync(message, ev =>
                {
                    if (ev is LlmMessageDeltaEvent delta)
                    {
                        InvokeOnUiThread(() => assistantMessage.AppendContent(delta.DeltaContent));
                    }
                    else if (ev is LlmMessageCompleteEvent complete)
                    {
                        InvokeOnUiThread(() =>
                        {
                            if (string.IsNullOrEmpty(assistantMessage.Content))
                                assistantMessage.Content = complete.Content;
                        });
                    }
                    else if (ev is LlmToolCallEvent toolCall)
                    {
                        InvokeOnUiThread(() => _chatViewModel.AddToolCallMessage(toolCall.ToolName, toolCall.Arguments));
                    }
                    else if (ev is LlmToolResultEvent toolResult)
                    {
                        InvokeOnUiThread(() => _chatViewModel.UpdateToolCallResult(toolResult.ToolName, toolResult.Result));
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to LLM");
                _chatViewModel.AddSystemMessage($"Error: {ex.Message}");
            }
            finally
            {
                _chatViewModel.IsProcessing = false;
            }
        }

        private void InvokeOnUiThread(Action action)
        {
            if (System.Windows.Forms.Application.OpenForms.Count > 0)
            {
                var form = System.Windows.Forms.Application.OpenForms[0];
                if (form != null && !form.IsDisposed && form.InvokeRequired)
                {
                    form.Invoke(action);
                    return;
                }
            }
            action();
        }

        private string InvokeOnUiThread(Func<string> func)
        {
            if (System.Windows.Forms.Application.OpenForms.Count > 0)
            {
                var form = System.Windows.Forms.Application.OpenForms[0];
                if (form != null && !form.IsDisposed && form.InvokeRequired)
                {
                    return (string)form.Invoke(func);
                }
            }
            return func();
        }

        public override void Dispose()
        {
            _chatViewModel.SendMessageRequested -= OnSendMessageRequested;

            if (_activeSession != null)
            {
                _ = _activeSession.DisposeAsync();
                _activeSession = null;
            }

            if (_activeProvider != null)
            {
                _ = _activeProvider.DisposeAsync();
                _activeProvider = null;
            }
        }
    }
}
