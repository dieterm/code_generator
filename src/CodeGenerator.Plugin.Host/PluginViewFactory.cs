using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;

namespace CodeGenerator.Plugin.Host;

/// <summary>
/// A secondary view factory that holds View ? ViewModel mappings registered by plugins.
/// The host <see cref="IViewFactory"/> should fall back to this factory when it cannot
/// resolve a view from the main DI container.
/// </summary>
public class PluginViewFactory
{
    private static readonly Lazy<PluginViewFactory> _instance = new(() => new PluginViewFactory());
    public static PluginViewFactory Instance => _instance.Value;

    private readonly Dictionary<Type, Type> _viewMappings = new();
    private readonly object _lock = new();

    private PluginViewFactory() { }

    /// <summary>
    /// Register a ViewModel ? View type mapping from a plugin
    /// </summary>
    internal void RegisterMapping(Type viewModelType, Type viewType)
    {
        lock (_lock)
        {
            _viewMappings[viewModelType] = viewType;
        }
    }

    /// <summary>
    /// Unregister a ViewModel ? View type mapping
    /// </summary>
    internal void UnregisterMapping(Type viewModelType)
    {
        lock (_lock)
        {
            _viewMappings.Remove(viewModelType);
        }
    }

    /// <summary>
    /// Try to create a view for the given ViewModel using plugin-registered mappings.
    /// Returns null if no mapping exists.
    /// </summary>
    public IView? TryCreateView(ViewModelBase viewModel)
    {
        Type? viewType;
        lock (_lock)
        {
            if (!_viewMappings.TryGetValue(viewModel.GetType(), out viewType))
                return null;
        }

        try
        {
            var view = Activator.CreateInstance(viewType) as IView;
            view?.BindViewModel(viewModel);
            return view;
        }
        catch
        {
            return null;
        }
    }
}
