using System.Reflection;
using System.Runtime.Loader;

namespace CodeGenerator.Plugin.Host;

/// <summary>
/// A collectible AssemblyLoadContext for workspace plugins.
/// Using <c>isCollectible: true</c> allows the assemblies to be unloaded
/// when the workspace is closed, freeing memory.
/// </summary>
internal class PluginLoadContext : AssemblyLoadContext
{
    private readonly string _pluginDirectory;

    public PluginLoadContext(string pluginDirectory)
        : base(isCollectible: true)
    {
        _pluginDirectory = pluginDirectory;
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        // Try to resolve from the plugin's own directory first
        var assemblyPath = Path.Combine(_pluginDirectory, $"{assemblyName.Name}.dll");
        if (File.Exists(assemblyPath))
        {
            return LoadFromAssemblyPath(assemblyPath);
        }

        // Fall back to the default context (host assemblies)
        return null;
    }
}
