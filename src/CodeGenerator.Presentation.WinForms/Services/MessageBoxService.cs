using CodeGenerator.Application.Services;
using System;
using System.Windows.Forms;
using WinFormsMessageBox = System.Windows.Forms.MessageBox;
using WinFormsDialogResult = System.Windows.Forms.DialogResult;
using WinFormsMessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
using WinFormsMessageBoxIcon = System.Windows.Forms.MessageBoxIcon;
using MessageBoxButtons = CodeGenerator.Application.Services.MessageBoxButtons;
using MessageBoxIcon = CodeGenerator.Application.Services.MessageBoxIcon;

namespace CodeGenerator.Presentation.WinForms.Services
{
    /// <summary>
    /// WinForms implementation of IMessageBoxService
    /// </summary>
    public class MessageBoxService : IMessageBoxService
    {
        public void ShowInformation(string message, string title = "Information")
        {
            WinFormsMessageBox.Show(
                message,
                title,
                WinFormsMessageBoxButtons.OK,
                WinFormsMessageBoxIcon.Information);
        }

        public void ShowWarning(string message, string title = "Warning")
        {
            WinFormsMessageBox.Show(
                message,
                title,
                WinFormsMessageBoxButtons.OK,
                WinFormsMessageBoxIcon.Warning);
        }

        public void ShowError(string message, string title = "Error")
        {
            WinFormsMessageBox.Show(
                message,
                title,
                WinFormsMessageBoxButtons.OK,
                WinFormsMessageBoxIcon.Error);
        }

        public bool AskYesNo(string message, string title = "Question")
        {
            var result = WinFormsMessageBox.Show(
                message,
                title,
                WinFormsMessageBoxButtons.YesNo,
                WinFormsMessageBoxIcon.Question);

            return result == WinFormsDialogResult.Yes;
        }

        public MessageBoxResult AskYesNoCancel(string message, string title = "Question")
        {
            var result = WinFormsMessageBox.Show(
                message,
                title,
                WinFormsMessageBoxButtons.YesNoCancel,
                WinFormsMessageBoxIcon.Question);

            return ConvertDialogResult(result);
        }

        public bool Confirm(string message, string title = "Confirm")
        {
            var result = WinFormsMessageBox.Show(
                message,
                title,
                WinFormsMessageBoxButtons.OKCancel,
                WinFormsMessageBoxIcon.Warning);

            return result == WinFormsDialogResult.OK;
        }

        public string? PromptForText(string message, string title = "Input", string defaultValue = "")
        {
            // Create a custom input dialog
            using var inputForm = new Form
            {
                Text = title,
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MinimizeBox = false,
                MaximizeBox = false
            };

            var label = new Label
            {
                Text = message,
                Left = 10,
                Top = 10,
                Width = 370,
                Height = 20
            };

            var textBox = new TextBox
            {
                Text = defaultValue,
                Left = 10,
                Top = 35,
                Width = 370
            };

            var okButton = new Button
            {
                Text = "OK",
                Left = 220,
                Top = 70,
                Width = 75,
                DialogResult = WinFormsDialogResult.OK
            };

            var cancelButton = new Button
            {
                Text = "Cancel",
                Left = 305,
                Top = 70,
                Width = 75,
                DialogResult = WinFormsDialogResult.Cancel
            };

            inputForm.Controls.Add(label);
            inputForm.Controls.Add(textBox);
            inputForm.Controls.Add(okButton);
            inputForm.Controls.Add(cancelButton);

            inputForm.AcceptButton = okButton;
            inputForm.CancelButton = cancelButton;

            // Select all text when form loads
            inputForm.Shown += (s, e) =>
            {
                textBox.Focus();
                textBox.SelectAll();
            };

            return inputForm.ShowDialog() == WinFormsDialogResult.OK ? textBox.Text : null;
        }

        public MessageBoxResult ShowCustom(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            var winFormsButtons = ConvertButtons(buttons);
            var winFormsIcon = ConvertIcon(icon);

            var result = WinFormsMessageBox.Show(message, title, winFormsButtons, winFormsIcon);

            return ConvertDialogResult(result);
        }

        #region Helper methods

        private static WinFormsMessageBoxButtons ConvertButtons(MessageBoxButtons buttons)
        {
            return buttons switch
            {
                MessageBoxButtons.OK => WinFormsMessageBoxButtons.OK,
                MessageBoxButtons.OKCancel => WinFormsMessageBoxButtons.OKCancel,
                MessageBoxButtons.YesNo => WinFormsMessageBoxButtons.YesNo,
                MessageBoxButtons.YesNoCancel => WinFormsMessageBoxButtons.YesNoCancel,
                MessageBoxButtons.RetryCancel => WinFormsMessageBoxButtons.RetryCancel,
                MessageBoxButtons.AbortRetryIgnore => WinFormsMessageBoxButtons.AbortRetryIgnore,
                _ => WinFormsMessageBoxButtons.OK
            };
        }

        private static WinFormsMessageBoxIcon ConvertIcon(MessageBoxIcon icon)
        {
            return icon switch
            {
                MessageBoxIcon.None => WinFormsMessageBoxIcon.None,
                MessageBoxIcon.Information => WinFormsMessageBoxIcon.Information,
                MessageBoxIcon.Warning => WinFormsMessageBoxIcon.Warning,
                MessageBoxIcon.Error => WinFormsMessageBoxIcon.Error,
                MessageBoxIcon.Question => WinFormsMessageBoxIcon.Question,
                _ => WinFormsMessageBoxIcon.None
            };
        }

        private static MessageBoxResult ConvertDialogResult(WinFormsDialogResult result)
        {
            return result switch
            {
                WinFormsDialogResult.OK => MessageBoxResult.OK,
                WinFormsDialogResult.Cancel => MessageBoxResult.Cancel,
                WinFormsDialogResult.Yes => MessageBoxResult.Yes,
                WinFormsDialogResult.No => MessageBoxResult.No,
                WinFormsDialogResult.Abort => MessageBoxResult.Abort,
                WinFormsDialogResult.Retry => MessageBoxResult.Retry,
                WinFormsDialogResult.Ignore => MessageBoxResult.Ignore,
                _ => MessageBoxResult.Cancel
            };
        }

        #endregion
    }
}
