using CodeGenerator.Core.Copilot.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CodeGenerator.Core.Copilot
{
    public partial class CopilotChatView : UserControl, IView<CopilotChatViewModel>
    {
        private CopilotChatViewModel? _viewModel;

        public CopilotChatView()
        {
            InitializeComponent();

            _sendButton.Click += (s, e) => _viewModel?.RequestSendMessage();
            _inputTextBox.KeyDown += OnInputKeyDown;
            _inputTextBox.TextChanged += (s, e) => UpdateSendButtonState();
        }

        private void OnInputKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                _viewModel?.RequestSendMessage();
            }
        }

        public void BindViewModel(CopilotChatViewModel viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                _viewModel.Messages.CollectionChanged -= OnMessagesCollectionChanged;
            }

            _viewModel = viewModel;

            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            _viewModel.Messages.CollectionChanged += OnMessagesCollectionChanged;

            _inputTextBox.DataBindings.Clear();
            _inputTextBox.DataBindings.Add(nameof(TextBox.Text), _viewModel, nameof(CopilotChatViewModel.InputText),
                false, DataSourceUpdateMode.OnPropertyChanged);

            _statusLabel.Text = _viewModel.StatusText;
            UpdateSendButtonState();

            // Render any existing messages
            RenderAllMessages();
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((CopilotChatViewModel)(object)viewModel);
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnViewModelPropertyChanged(sender, e));
                return;
            }

            switch (e.PropertyName)
            {
                case nameof(CopilotChatViewModel.StatusText):
                    _statusLabel.Text = _viewModel?.StatusText ?? "";
                    break;
                case nameof(CopilotChatViewModel.IsProcessing):
                case nameof(CopilotChatViewModel.IsConnected):
                case nameof(CopilotChatViewModel.CanSend):
                    UpdateSendButtonState();
                    break;
            }
        }

        private void OnMessagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => OnMessagesCollectionChanged(sender, e));
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (ChatMessageViewModel message in e.NewItems)
                {
                    AddMessageControl(message);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _messagesPanel.Controls.Clear();
            }
        }

        private void RenderAllMessages()
        {
            _messagesPanel.Controls.Clear();
            if (_viewModel == null) return;
            foreach (var message in _viewModel.Messages)
            {
                AddMessageControl(message);
            }
        }

        private void AddMessageControl(ChatMessageViewModel message)
        {
            var messagePanel = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowOnly,
                MinimumSize = new Size(_messagesPanel.ClientSize.Width - 30, 0),
                MaximumSize = new Size(_messagesPanel.ClientSize.Width - 30, 0),
                Padding = new Padding(8, 6, 8, 6),
                Margin = new Padding(2, 2, 2, 2),
                BackColor = message.Role switch
                {
                    ChatMessageRole.User => Color.FromArgb(230, 240, 255),
                    ChatMessageRole.Assistant => Color.FromArgb(240, 255, 240),
                    ChatMessageRole.System => Color.FromArgb(255, 255, 230),
                    _ => SystemColors.Control
                }
            };

            var roleLabel = new Label
            {
                Text = $"{message.RoleLabel}  {message.Timestamp:HH:mm}",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 2)
            };

            var contentLabel = new Label
            {
                Text = message.Content,
                Font = new Font("Segoe UI", 9.5F),
                AutoSize = true,
                Dock = DockStyle.Top,
                MaximumSize = new Size(_messagesPanel.ClientSize.Width - 50, 0)
            };

            // Update content when streaming
            message.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ChatMessageViewModel.Content))
                {
                    if (contentLabel.InvokeRequired)
                        contentLabel.Invoke(() => UpdateContentLabel(contentLabel, message.Content));
                    else
                        UpdateContentLabel(contentLabel, message.Content);
                }
            };

            messagePanel.Controls.Add(contentLabel);
            messagePanel.Controls.Add(roleLabel);

            _messagesPanel.Controls.Add(messagePanel);

            // Scroll to bottom
            _messagesPanel.ScrollControlIntoView(messagePanel);
        }

        private void UpdateContentLabel(Label label, string content)
        {
            label.Text = content;
            // Scroll parent panel to bottom
            if (_messagesPanel.Controls.Count > 0)
            {
                _messagesPanel.ScrollControlIntoView(_messagesPanel.Controls[_messagesPanel.Controls.Count - 1]);
            }
        }

        private void UpdateSendButtonState()
        {
            _sendButton.Enabled = _viewModel?.CanSend ?? false;
        }
    }
}
