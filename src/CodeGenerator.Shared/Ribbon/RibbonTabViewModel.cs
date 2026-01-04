using System.Drawing;

namespace CodeGenerator.Shared.Ribbon;

/// <summary>
/// ViewModel for a Ribbon Tab
/// </summary>
public class RibbonTabViewModel : RibbonItemViewModel
{
    public List<RibbonToolStripViewModel> ToolStrips { get; set; } = new();
    public int Position { get; set; }
    public string? Tag { get; set; }
}
