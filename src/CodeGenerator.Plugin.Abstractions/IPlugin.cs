namespace CodeGenerator.Plugin.Abstractions;

/// <summary>
/// Contract for plugins that extend the CodeGenerator application.
/// Plugin assemblies must contain exactly one public class implementing this interface.
/// </summary>
public interface IPlugin : IDisposable
{
    /// <summary>
    /// Unique identifier for this plugin (e.g., "com.example.myplugin")
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Display name shown in the UI and logs
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Plugin version
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// Description of what this plugin provides
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Whether this plugin targets global (application) or workspace scope
    /// </summary>
    PluginScope Scope { get; }

    /// <summary>
    /// Called when the plugin is loaded.
    /// Use the context to register services, generators, datasources, views, etc.
    /// </summary>
    void Initialize(IPluginContext context);

    /// <summary>
    /// Called before the plugin is unloaded.
    /// Clean up any resources, subscriptions, and references.
    /// </summary>
    void Shutdown();
}
