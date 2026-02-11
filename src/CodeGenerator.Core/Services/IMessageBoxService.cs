using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Services
{
    /// <summary>
    /// Service for displaying message boxes and prompting user input
    /// </summary>
    public interface IMessageBoxService
    {
        /// <summary>
        /// Show an information message
        /// </summary>
        void ShowInformation(string message, string title = "Information");

        /// <summary>
        /// Show a warning message
        /// </summary>
        void ShowWarning(string message, string title = "Warning");

        /// <summary>
        /// Show an error message
        /// </summary>
        void ShowError(string message, string title = "Error");

        /// <summary>
        /// Ask a yes/no question
        /// </summary>
        /// <returns>True if user clicked Yes, false if No</returns>
        bool AskYesNo(string message, string title = "Question");

        /// <summary>
        /// Ask a yes/no/cancel question
        /// </summary>
        /// <returns>Result of the dialog</returns>
        MessageBoxResult AskYesNoCancel(string message, string title = "Question");

        /// <summary>
        /// Show a confirmation dialog with OK/Cancel
        /// </summary>
        /// <returns>True if user clicked OK, false if Cancel</returns>
        bool Confirm(string message, string title = "Confirm");

        /// <summary>
        /// Prompt user for text input
        /// </summary>
        /// <param name="message">The prompt message</param>
        /// <param name="title">Dialog title</param>
        /// <param name="defaultValue">Default value in the textbox</param>
        /// <returns>The entered text, or null if cancelled</returns>
        string? PromptForText(string message, string title = "Input", string defaultValue = "");

        /// <summary>
        /// Show a message with custom buttons
        /// </summary>
        MessageBoxResult ShowCustom(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon);
    }

    /// <summary>
    /// Result of a message box dialog
    /// </summary>
    public enum MessageBoxResult
    {
        OK,
        Cancel,
        Yes,
        No,
        Abort,
        Retry,
        Ignore
    }

    /// <summary>
    /// Buttons to display in message box
    /// </summary>
    public enum MessageBoxButtons
    {
        OK,
        OKCancel,
        YesNo,
        YesNoCancel,
        RetryCancel,
        AbortRetryIgnore
    }

    /// <summary>
    /// Icon to display in message box
    /// </summary>
    public enum MessageBoxIcon
    {
        None,
        Information,
        Warning,
        Error,
        Question
    }
}
