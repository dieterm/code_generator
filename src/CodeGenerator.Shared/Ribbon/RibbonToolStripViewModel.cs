namespace CodeGenerator.Shared.Ribbon;

/// <summary>
/// ViewModel for a ToolStripEx (panel within a tab)
/// </summary>
public class RibbonToolStripViewModel : RibbonItemViewModel
{
    public List<RibbonItemViewModel> Items { get; set; } = new();
}
