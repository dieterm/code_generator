namespace CodeGenerator.Plugin.Abstractions;

/// <summary>
/// Metadata describing a loaded plugin instance
/// </summary>
public class PluginInfo
{
    /// <summary>
    /// The plugin instance
    /// </summary>
    public required IPlugin Plugin { get; init; }

    /// <summary>
    /// The directory from which the plugin was loaded
    /// </summary>
    public required string PluginDirectory { get; init; }

    /// <summary>
    /// Whether the plugin is currently initialized and active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Any error that occurred during plugin loading or initialization
    /// </summary>
    public string? Error { get; set; }
}
