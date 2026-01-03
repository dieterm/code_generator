using System.Drawing;

namespace ProjectXYZ.Shared.Ribbon;

/// <summary>
/// Fluent builder for ribbon buttons
/// </summary>
public class RibbonButtonBuilder
{
    private readonly RibbonButtonViewModel _viewModel;
    private readonly RibbonToolStripBuilder _parent;

    internal RibbonButtonBuilder(RibbonToolStripBuilder parent, string name, string text)
    {
        _parent = parent;
        _viewModel = new RibbonButtonViewModel
        {
            Name = name,
            Text = text
        };
    }

    /// <summary>
    /// Sets the click handler for the button
    /// </summary>
    public RibbonButtonBuilder OnClick(Action<EventArgs> handler)
    {
        _viewModel.ClickHandler = handler;
        return this;
    }

    /// <summary>
    /// Sets the image for the button using byte array
    /// </summary>
    public RibbonButtonBuilder WithImage(byte[] imageData)
    {
        _viewModel.ImageData = imageData;
        return this;
    }

    /// <summary>
    /// Sets the tooltip text for the button
    /// </summary>
    public RibbonButtonBuilder WithToolTip(string toolTipText)
    {
        _viewModel.ToolTipText = toolTipText;
        return this;
    }

    /// <summary>
    /// Sets the display style for the button
    /// </summary>
    public RibbonButtonBuilder WithDisplayStyle(RibbonButtonDisplayStyle style)
    {
        _viewModel.DisplayStyle = style;
        return this;
    }

    /// <summary>
    /// Sets the button size
    /// </summary>
    public RibbonButtonBuilder WithSize(RibbonButtonSize size)
    {
        _viewModel.Size = size;
        return this;
    }

    /// <summary>
    /// Sets whether the button is enabled
    /// </summary>
    public RibbonButtonBuilder Enabled(bool enabled = true)
    {
        _viewModel.Enabled = enabled;
        return this;
    }

    /// <summary>
    /// Sets whether the button is visible
    /// </summary>
    public RibbonButtonBuilder Visible(bool visible = true)
    {
        _viewModel.Visible = visible;
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
