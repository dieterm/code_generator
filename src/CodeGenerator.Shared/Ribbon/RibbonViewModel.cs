namespace CodeGenerator.Shared.Ribbon;

/// <summary>
/// ViewModel for the complete Ribbon control
/// </summary>
public class RibbonViewModel
{
    public List<RibbonTabViewModel> Tabs { get; set; } = new();
    public string? ApplicationButtonText { get; set; }
    public Action<EventArgs>? SelectedTabChangedHandler { get; set; }
}
