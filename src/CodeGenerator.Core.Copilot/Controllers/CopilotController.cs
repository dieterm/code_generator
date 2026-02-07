using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Copilot;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.Copilot.Settings;
using CodeGenerator.Core.Copilot.Tools;
using CodeGenerator.Core.Copilot.ViewModels;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Ribbon;
using GitHub.Copilot.SDK;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.Copilot.Controllers
{
    public class CopilotController : CoreControllerBase, ICopilotController
    {
        private readonly CopilotChatViewModel _copilotChatViewModel;
        private IWorkspaceContextProvider? _workspaceContextProvider;
        private CopilotClient? _client;
        private CopilotSession? _session;
        private WorkspaceTools? _workspaceTools;

        private const string SystemPromptContent =
            "You are a workspace assistant for a code generator application. " +
            "You help users manipulate their workspace by adding domains, entities, properties and value types.\n\n" +
            "The workspace is organized as: Workspace > Scopes (e.g. Shared, Application) > Domains > Entities/ValueTypes. " +
            "Each Entity can have properties with data types like: varchar, int, bigint, decimal, float, bool, datetime, guid, text.\n\n" +
            "When the user asks you to create domains, entities or properties, use the available tools to perform the actions. " +
            "Always confirm what you've done after performing actions. " +
            "If an entity needs properties, prefer using AddEntityWithProperties to add them all at once. " +
            "Use PascalCase for all names (domains, entities, properties). " +
            "First call GetWorkspaceInfo and ListScopes to understand the workspace structure before making changes.";

        public CopilotController(
            CopilotChatViewModel copilotChatViewModel,
            IWindowManagerService windowManagerService,
            RibbonBuilder ribbonBuilder,
            ApplicationMessageBus messageBus,
            IMessageBoxService messageboxService,
            IFileSystemDialogService fileSystemDialogService,
            ILogger<CopilotController> logger)
            : base(windowManagerService, ribbonBuilder, messageBus, messageboxService, fileSystemDialogService, logger)
        {
            _copilotChatViewModel = copilotChatViewModel;
        }

        public override void Initialize()
        {
            _workspaceContextProvider = ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>();
            _copilotChatViewModel.SendMessageRequested += OnSendMessageRequested;
        }

        public void ShowCopilot()
        {
            _windowManagerService.ShowCopilotChatView(_copilotChatViewModel);

            if (!_copilotChatViewModel.IsConnected)
            {
                _ = ConnectAsync();
            }
        }

        private async Task ConnectAsync()
        {
            try
            {
                _copilotChatViewModel.StatusText = "Connecting...";

                _workspaceTools = new WorkspaceTools(_workspaceContextProvider!, InvokeOnUiThread);

                _client = new CopilotClient();
                await _client.StartAsync();

                _session = await _client.CreateSessionAsync(new SessionConfig
                {
                    Model = "gpt-4.1",
                    Streaming = true,
                    SystemMessage = new SystemMessageConfig
                    {
                        Mode = SystemMessageMode.Append,
                        Content = SystemPromptContent
                    },
                    Tools = _workspaceTools.GetAllTools()
                });

                _copilotChatViewModel.IsConnected = true;
                _copilotChatViewModel.AddSystemMessage("Connected to GitHub Copilot. How can I help you with your workspace?");
                _logger.LogInformation("Copilot session started (SessionId={SessionId})", _session.SessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to Copilot");
                _copilotChatViewModel.StatusText = $"Connection failed: {ex.Message}";
                _copilotChatViewModel.AddSystemMessage($"Failed to connect: {ex.Message}");
            }
        }

        private async void OnSendMessageRequested(object? sender, string message)
        {
            if (_session == null || _copilotChatViewModel.IsProcessing)
                return;

            _copilotChatViewModel.AddUserMessage(message);
            _copilotChatViewModel.IsProcessing = true;

            try
            {
                var assistantMessage = _copilotChatViewModel.AddAssistantMessage();
                var done = new TaskCompletionSource();

                var subscription = _session.On(ev =>
                {
                    if (ev is AssistantMessageDeltaEvent deltaEvent)
                    {
                        InvokeOnUiThread(() => assistantMessage.AppendContent(deltaEvent.Data.DeltaContent ?? ""));
                    }
                    else if (ev is AssistantMessageEvent msgEvent)
                    {
                        InvokeOnUiThread(() =>
                        {
                            if (string.IsNullOrEmpty(assistantMessage.Content))
                                assistantMessage.Content = msgEvent.Data.Content ?? "";
                        });
                    }
                    else if (ev is SessionIdleEvent)
                    {
                        done.TrySetResult();
                    }
                    else if(ev is PendingMessagesModifiedEvent pendingMessagesModifiedEvent)
                    {
                        _logger.LogInformation($"Pending messages modified: {pendingMessagesModifiedEvent.Data.ToString()} pending messages");
                    }
                    else
                    {
                        _logger.LogWarning("Received unexpected event type: {EventType}", ev.GetType().Name);
                    }
                });

                await _session.SendAsync(new MessageOptions { Prompt = message });
                await done.Task;
                subscription.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to Copilot");
                _copilotChatViewModel.AddSystemMessage($"Error: {ex.Message}");
            }
            finally
            {
                _copilotChatViewModel.IsProcessing = false;
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
            _copilotChatViewModel.SendMessageRequested -= OnSendMessageRequested;

            if (_session != null)
            {
                _ = _session.DisposeAsync();
                _session = null;
            }

            if (_client != null)
            {
                _ = _client.DisposeAsync();
                _client = null;
            }
        }
    }
}
