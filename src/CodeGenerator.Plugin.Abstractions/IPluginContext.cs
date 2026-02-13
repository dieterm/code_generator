using Microsoft.Extensions.Logging;

namespace CodeGenerator.Plugin.Abstractions;

/// <summary>
/// Provides a controlled API surface for plugins to register their contributions
/// with the host application. Plugins receive this during <see cref="IPlugin.Initialize"/>.
/// </summary>
public interface IPluginContext
{
    /// <summary>
    /// Register a datasource provider (e.g., a new database type).
    /// The provider will be available in the workspace datasource factory.
    /// </summary>
    void RegisterDatasourceProvider(object datasourceProvider);

    /// <summary>
    /// Register a code generator that participates in the generator message bus.
    /// </summary>
    void RegisterGenerator(object generator);

    /// <summary>
    /// Register a template engine for processing custom template types.
    /// </summary>
    void RegisterTemplateEngine(object templateEngine);

    /// <summary>
    /// Register a workspace message bus subscriber for reacting to workspace events.
    /// </summary>
    void RegisterWorkspaceSubscriber(object subscriber);

    /// <summary>
    /// Register a View ? ViewModel mapping so the ViewFactory can resolve plugin views.
    /// </summary>
    /// <param name="viewModelType">The ViewModel type (must extend ViewModelBase)</param>
    /// <param name="viewType">The View type (must implement IView&lt;TViewModel&gt; and be a UserControl)</param>
    void RegisterViewMapping(Type viewModelType, Type viewType);

    /// <summary>
    /// Register a template folder for the TemplateManager to scan.
    /// </summary>
    void RegisterTemplateFolder(string folderPath);

    /// <summary>
    /// Create a logger for the given category name.
    /// </summary>
    ILogger CreateLogger(string categoryName);

    /// <summary>
    /// Create a typed logger.
    /// </summary>
    ILogger<T> CreateLogger<T>();

    /// <summary>
    /// The directory from which this plugin was loaded.
    /// </summary>
    string PluginDirectory { get; }
}
