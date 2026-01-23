using System.Drawing;
using System.Windows.Input;

namespace CodeGenerator.Shared.Ribbon;

/// <summary>
/// Base class for all ribbon item view models
/// </summary>
public abstract class RibbonItemViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Image as byte array or Image for cross-platform compatibility
    /// </summary>
    public object? ImageData { get; set; }

    public bool Enabled { get; set; } = true;
    public bool Visible { get; set; } = true;
    public Action<EventArgs>? ClickHandler { get; set; }
    public ICommand? Command { get; set; }
    public object? CommandParameter { get; set; }
    public bool HiddenWhenDisabled { get; set; } = false;
    public string? ToolTipText { get; set; }
}
