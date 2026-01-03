namespace ProjectXYZ.Shared.Ribbon;

/// <summary>
/// Fluent builder for creating a Ribbon control configuration
/// </summary>
public class RibbonBuilder
{
    internal readonly RibbonViewModel ViewModel = new();

    /// <summary>
    /// Creates a new RibbonBuilder instance
    /// </summary>
    public static RibbonBuilder Create()
    {
        return new RibbonBuilder();
    }

    /// <summary>
    /// Adds a new tab to the ribbon
    /// </summary>
    public RibbonTabBuilder AddTab(string name, string text)
    {
        return new RibbonTabBuilder(this, name, text);
    }

    /// <summary>
    /// Sets the application button text
    /// </summary>
    public RibbonBuilder WithApplicationButton(string text)
    {
        ViewModel.ApplicationButtonText = text;
        return this;
    }

    /// <summary>
    /// Sets the handler for when the selected tab changes
    /// </summary>
    public RibbonBuilder OnSelectedTabChanged(Action<EventArgs> handler)
    {
        ViewModel.SelectedTabChangedHandler = handler;
        return this;
    }

    /// <summary>
    /// Builds and returns the RibbonViewModel
    /// </summary>
    public RibbonViewModel Build()
    {
        return ViewModel;
    }
}
