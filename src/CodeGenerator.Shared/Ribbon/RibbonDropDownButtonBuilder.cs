using System.Windows.Input;

namespace CodeGenerator.Shared.Ribbon;

/// <summary>
/// Fluent builder for ribbon dropdown buttons
/// </summary>
public class RibbonDropDownButtonBuilder
{
    private readonly RibbonDropDownButtonViewModel _viewModel;
    private readonly RibbonToolStripBuilder _parent;

    internal RibbonDropDownButtonBuilder(RibbonToolStripBuilder parent, string name, string text)
    {
        _parent = parent;
        _viewModel = new RibbonDropDownButtonViewModel
        {
            Name = name,
            Text = text
        };
    }

    /// <summary>
    /// Sets the click handler for the main button area
    /// </summary>
    public RibbonDropDownButtonBuilder OnClick(Action<EventArgs> handler)
    {
        _viewModel.ClickHandler = handler;
        return this;
    }

    /// <summary>
    /// Sets the command for the main button area
    /// </summary>
    public RibbonDropDownButtonBuilder WithCommand(ICommand command, object? commandParameter = null)
    {
        _viewModel.Command = command;
        _viewModel.CommandParameter = commandParameter;
        return this;
    }

    /// <summary>
    /// Default is false. If set to true, the button will be hidden when disabled
    /// </summary>
    public RibbonDropDownButtonBuilder HideWhenDisabled(bool hiddenWhenDisabled = true)
    {
        _viewModel.HiddenWhenDisabled = hiddenWhenDisabled;
        return this;
    }

    /// <summary>
    /// Sets the image for the button using byte array
    /// </summary>
    public RibbonDropDownButtonBuilder WithImage(byte[] imageData)
    {
        _viewModel.ImageData = imageData;
        return this;
    }

    /// <summary>
    /// Sets the image for the button using Image
    /// </summary>
    public RibbonDropDownButtonBuilder WithImage(System.Drawing.Image image)
    {
        _viewModel.ImageData = image;
        return this;
    }

    /// <summary>
    /// Sets an image key of the ResourceFile or ImageList used for this ribbon
    /// </summary>
    public RibbonDropDownButtonBuilder WithImage(string imageKey)
    {
        _viewModel.ImageData = imageKey;
        return this;
    }

    /// <summary>
    /// Sets the tooltip text for the button
    /// </summary>
    public RibbonDropDownButtonBuilder WithToolTip(string toolTipText)
    {
        _viewModel.ToolTipText = toolTipText;
        return this;
    }

    /// <summary>
    /// Sets the display style for the button
    /// </summary>
    public RibbonDropDownButtonBuilder WithDisplayStyle(RibbonButtonDisplayStyle style)
    {
        _viewModel.DisplayStyle = style;
        return this;
    }

    /// <summary>
    /// Sets the button size
    /// </summary>
    public RibbonDropDownButtonBuilder WithSize(RibbonButtonSize size)
    {
        _viewModel.Size = size;
        return this;
    }

    /// <summary>
    /// Sets whether the button is enabled
    /// </summary>
    public RibbonDropDownButtonBuilder Enabled(bool enabled = true)
    {
        _viewModel.Enabled = enabled;
        return this;
    }

    /// <summary>
    /// Sets whether the button is visible
    /// </summary>
    public RibbonDropDownButtonBuilder Visible(bool visible = true)
    {
        _viewModel.Visible = visible;
        return this;
    }

    /// <summary>
    /// Sets a dynamic provider function for dropdown items.
    /// This function is called each time the dropdown is opened.
    /// </summary>
    public RibbonDropDownButtonBuilder WithDropDownItemsProvider(Func<IEnumerable<RibbonDropDownItemViewModel>> provider)
    {
        _viewModel.DropDownItemsProvider = provider;
        return this;
    }

    /// <summary>
    /// Adds a static dropdown item
    /// </summary>
    public RibbonDropDownButtonBuilder AddDropDownItem(string name, string text, Action<RibbonDropDownItemViewModel>? clickHandler = null)
    {
        _viewModel.DropDownItems.Add(new RibbonDropDownItemViewModel
        {
            Name = name,
            Text = text,
            ClickHandler = clickHandler
        });
        return this;
    }

    /// <summary>
    /// Adds a separator to the dropdown
    /// </summary>
    public RibbonDropDownButtonBuilder AddDropDownSeparator()
    {
        _viewModel.DropDownItems.Add(RibbonDropDownItemViewModel.Separator);
        return this;
    }

    /// <summary>
    /// Adds another button to the toolstrip
    /// </summary>
    public RibbonButtonBuilder AddButton(string name, string text)
    {
        Build();
        return _parent.AddButton(name, text);
    }

    /// <summary>
    /// Adds another dropdown button to the toolstrip
    /// </summary>
    public RibbonDropDownButtonBuilder AddDropDownButton(string name, string text)
    {
        Build();
        return _parent.AddDropDownButton(name, text);
    }

    /// <summary>
    /// Returns to the parent toolstrip builder
    /// </summary>
    public RibbonToolStripBuilder EndToolStrip()
    {
        Build();
        return _parent;
    }

    /// <summary>
    /// Builds the RibbonViewModel
    /// </summary>
    public RibbonViewModel Build()
    {
        if (!_parent.ToolStrip.Items.Contains(_viewModel))
        {
            _parent.ToolStrip.Items.Add(_viewModel);
        }
        return _parent.Build();
    }
}
