using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CodeGenerator.Shared.Ribbon;

/// <summary>
/// ViewModel for a ToolStripDropDownButton - a button with a dropdown menu of items
/// </summary>
public class RibbonDropDownButtonViewModel : RibbonItemViewModel, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public RibbonButtonDisplayStyle DisplayStyle { get; set; } = RibbonButtonDisplayStyle.ImageAndText;
    public RibbonButtonSize Size { get; set; } = RibbonButtonSize.Large;

    /// <summary>
    /// The dropdown menu items. This collection is observable so the renderer can react to changes.
    /// </summary>
    public ObservableCollection<RibbonDropDownItemViewModel> DropDownItems { get; set; } = new();

    /// <summary>
    /// Optional: a provider function that returns the current dropdown items.
    /// Called each time the dropdown is opened, allowing lazy/dynamic population.
    /// </summary>
    public Func<IEnumerable<RibbonDropDownItemViewModel>>? DropDownItemsProvider { get; set; }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// ViewModel for an individual item in a dropdown menu
/// </summary>
public class RibbonDropDownItemViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public object? ImageData { get; set; }
    public string? ToolTipText { get; set; }
    public bool Enabled { get; set; } = true;
    public bool Visible { get; set; } = true;
    public bool IsSeparator { get; set; }

    /// <summary>
    /// Tag for storing additional data (e.g., the index in the undo/redo stack)
    /// </summary>
    public object? Tag { get; set; }

    /// <summary>
    /// Click handler for this dropdown item
    /// </summary>
    public Action<RibbonDropDownItemViewModel>? ClickHandler { get; set; }

    /// <summary>
    /// Creates a separator item
    /// </summary>
    public static RibbonDropDownItemViewModel Separator => new() { IsSeparator = true, Name = "separator" };
}
