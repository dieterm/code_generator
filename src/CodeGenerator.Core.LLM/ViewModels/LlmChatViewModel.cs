using CodeGenerator.Shared.ViewModels;
using System.Collections.ObjectModel;

namespace CodeGenerator.Core.LLM.ViewModels
{
    public class LlmChatViewModel : ViewModelBase
    {
        private string _inputText = string.Empty;
        private bool _isProcessing;
        private bool _isConnected;
        private string _statusText = "Not connected";
        private string _providerName = "LLM";

        public ObservableCollection<ChatMessageViewModel> Messages { get; } = new();

        public string InputText
        {
            get => _inputText;
            set => SetProperty(ref _inputText, value);
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                if (SetProperty(ref _isProcessing, value))
                    OnPropertyChanged(nameof(CanSend));
            }
        }

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (SetProperty(ref _isConnected, value))
                {
                    OnPropertyChanged(nameof(CanSend));
                    StatusText = value ? "Connected" : "Not connected";
                }
            }
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        /// <summary>
        /// Display name of the active LLM provider (e.g. "GitHub Copilot", "Ollama (Local)")
        /// </summary>
        public string ProviderName
        {
            get => _providerName;
            set => SetProperty(ref _providerName, value);
        }

        public bool CanSend => IsConnected && !IsProcessing && !string.IsNullOrWhiteSpace(InputText);

        /// <summary>
        /// Raised when the user wants to send a message.
        /// The controller subscribes to this event.
        /// </summary>
        public event EventHandler<string>? SendMessageRequested;

        public void RequestSendMessage()
        {
            if (!CanSend) return;
            var text = InputText.Trim();
            InputText = string.Empty;
            SendMessageRequested?.Invoke(this, text);
        }

        public void AddUserMessage(string text)
        {
            Messages.Add(new ChatMessageViewModel(ChatMessageRole.User, text));
        }

        public ChatMessageViewModel AddAssistantMessage(string text = "")
        {
            var message = new ChatMessageViewModel(ChatMessageRole.Assistant, text);
            Messages.Add(message);
            return message;
        }

        public void AddSystemMessage(string text)
        {
            Messages.Add(new ChatMessageViewModel(ChatMessageRole.System, text));
        }

        public void AddToolCallMessage(string toolName, string arguments, string? result = null)
        {
            var message = new ChatMessageViewModel(ChatMessageRole.ToolCall, "")
            {
                ToolName = toolName,
                ToolArguments = arguments,
                ToolResult = result
            };
            Messages.Add(message);
        }

        /// <summary>
        /// Updates the result of the last tool call message matching the given tool name.
        /// </summary>
        public void UpdateToolCallResult(string toolName, string result)
        {
            for (int i = Messages.Count - 1; i >= 0; i--)
            {
                if (Messages[i].Role == ChatMessageRole.ToolCall
                    && Messages[i].ToolName == toolName
                    && Messages[i].ToolResult == null)
                {
                    Messages[i].ToolResult = result;
                    return;
                }
            }
        }

        public void ClearMessages()
        {
            Messages.Clear();
        }
    }

    public enum ChatMessageRole
    {
        User,
        Assistant,
        System,
        ToolCall
    }

    public class ChatMessageViewModel : ViewModelBase
    {
        private string _content;
        private string? _toolName;
        private string? _toolArguments;
        private string? _toolResult;

        public ChatMessageRole Role { get; }
        public DateTime Timestamp { get; }

        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public string? ToolName
        {
            get => _toolName;
            set => SetProperty(ref _toolName, value);
        }

        public string? ToolArguments
        {
            get => _toolArguments;
            set => SetProperty(ref _toolArguments, value);
        }

        public string? ToolResult
        {
            get => _toolResult;
            set => SetProperty(ref _toolResult, value);
        }

        public string RoleLabel => Role switch
        {
            ChatMessageRole.User => "You",
            ChatMessageRole.Assistant => "Assistant",
            ChatMessageRole.System => "System",
            ChatMessageRole.ToolCall => "Tool",
            _ => "Unknown"
        };

        public ChatMessageViewModel(ChatMessageRole role, string content)
        {
            Role = role;
            _content = content;
            Timestamp = DateTime.Now;
        }

        public void AppendContent(string delta)
        {
            Content += delta;
        }
    }
}
