namespace CodeGenerator.Core.Copilot
{
    partial class CopilotChatView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            // Main layout
            _mainLayout = new TableLayoutPanel();
            _mainLayout.Dock = DockStyle.Fill;
            _mainLayout.RowCount = 3;
            _mainLayout.ColumnCount = 1;
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            _mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            _mainLayout.Padding = new Padding(4);

            // Messages panel
            _messagesPanel = new FlowLayoutPanel();
            _messagesPanel.Dock = DockStyle.Fill;
            _messagesPanel.AutoScroll = true;
            _messagesPanel.FlowDirection = FlowDirection.TopDown;
            _messagesPanel.WrapContents = false;
            _messagesPanel.BackColor = System.Drawing.SystemColors.Window;
            _messagesPanel.BorderStyle = BorderStyle.FixedSingle;
            _mainLayout.Controls.Add(_messagesPanel, 0, 0);

            // Input panel
            var inputPanel = new TableLayoutPanel();
            inputPanel.Dock = DockStyle.Fill;
            inputPanel.ColumnCount = 2;
            inputPanel.RowCount = 1;
            inputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            inputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            inputPanel.Padding = new Padding(0, 4, 0, 0);

            _inputTextBox = new TextBox();
            _inputTextBox.Dock = DockStyle.Fill;
            _inputTextBox.Multiline = true;
            _inputTextBox.ScrollBars = ScrollBars.Vertical;
            _inputTextBox.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            _inputTextBox.PlaceholderText = "Ask Copilot to manipulate your workspace...";
            inputPanel.Controls.Add(_inputTextBox, 0, 0);

            _sendButton = new Button();
            _sendButton.Text = "Send";
            _sendButton.Dock = DockStyle.Fill;
            _sendButton.Enabled = false;
            _sendButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            inputPanel.Controls.Add(_sendButton, 1, 0);

            _mainLayout.Controls.Add(inputPanel, 0, 1);

            // Status bar
            _statusLabel = new Label();
            _statusLabel.Dock = DockStyle.Fill;
            _statusLabel.Text = "Not connected";
            _statusLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            _statusLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            _statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _mainLayout.Controls.Add(_statusLabel, 0, 2);

            this.Controls.Add(_mainLayout);
        }

        #endregion

        private TableLayoutPanel _mainLayout;
        private FlowLayoutPanel _messagesPanel;
        private TextBox _inputTextBox;
        private Button _sendButton;
        private Label _statusLabel;
    }
}
