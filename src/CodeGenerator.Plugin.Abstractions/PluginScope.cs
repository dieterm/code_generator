namespace CodeGenerator.Plugin.Abstractions;

/// <summary>
/// Defines the scope in which a plugin operates
/// </summary>
public enum PluginScope
{
    /// <summary>
    /// Global plugins are loaded at application startup from the default plugin folder
    /// and remain loaded for the entire application lifetime.
    /// </summary>
    Global,

    /// <summary>
    /// Workspace plugins are loaded when a workspace is opened from &lt;workspacefolder&gt;/Plugins
    /// and unloaded when the workspace is closed.
    /// </summary>
    Workspace
}
