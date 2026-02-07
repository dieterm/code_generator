using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CodeGenerator.Core.Copilot.ViewModels
{
    public class CopilotChatViewModel : ViewModelBase
    {
        private string _inputText = string.Empty;
        private bool _isProcessing;
        private bool _isConnected;
        private string _statusText = "Not connected";

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

        public void ClearMessages()
        {
            Messages.Clear();
        }
    }

    public enum ChatMessageRole
    {
        User,
        Assistant,
        System
    }

    public class ChatMessageViewModel : ViewModelBase
    {
        private string _content;

        public ChatMessageRole Role { get; }
        public DateTime Timestamp { get; }

        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public string RoleLabel => Role switch
        {
            ChatMessageRole.User => "You",
            ChatMessageRole.Assistant => "Copilot",
            ChatMessageRole.System => "System",
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
