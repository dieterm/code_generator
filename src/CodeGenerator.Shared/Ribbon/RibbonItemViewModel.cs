using System.Drawing;

namespace CodeGenerator.Shared.Ribbon;

/// <summary>
/// Base class for all ribbon item view models
/// </summary>
public abstract class RibbonItemViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Image as byte array for cross-platform compatibility
    /// </summary>
    public byte[]? ImageData { get; set; }

    public bool Enabled { get; set; } = true;
    public bool Visible { get; set; } = true;
    public Action<EventArgs>? ClickHandler { get; set; }
    public string? ToolTipText { get; set; }
}
