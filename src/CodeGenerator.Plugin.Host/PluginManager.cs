using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Plugin.Abstractions;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.Loader;

namespace CodeGenerator.Plugin.Host;

/// <summary>
/// Manages the lifecycle of global and workspace plugins.
/// <para>
/// <b>Global plugins</b> are loaded at application startup from the default plugin folder
/// and stay loaded for the entire application lifetime (loaded into the default ALC).
/// </para>
/// <para>
/// <b>Workspace plugins</b> are loaded when a workspace is opened from
/// &lt;workspacefolder&gt;/Plugins and unloaded when the workspace is closed
/// (loaded into a collectible ALC so memory can be reclaimed).
/// </para>
/// </summary>
public class PluginManager : IDisposable
{
    public const string PluginsFolderName = "Plugins";

    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<PluginManager> _logger;
    private readonly IDatasourceFactory _datasourceFactory;
    private readonly GeneratorOrchestrator _generatorOrchestrator;
    private readonly TemplateEngineManager _templateEngineManager;
    private readonly TemplateManager _templateManager;
    private readonly WorkspaceMessageBus _workspaceMessageBus;

    // Global plugin tracking
    private readonly List<LoadedPlugin> _globalPlugins = new();

    // Workspace plugin tracking
    private readonly List<LoadedPlugin> _workspacePlugins = new();
    private PluginLoadContext? _workspaceLoadContext;
    private WeakReference? _workspaceLoadContextRef;

    public PluginManager(
        ILoggerFactory loggerFactory,
        IDatasourceFactory datasourceFactory,
        GeneratorOrchestrator generatorOrchestrator,
        TemplateEngineManager templateEngineManager,
        TemplateManager templateManager,
        WorkspaceMessageBus workspaceMessageBus)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<PluginManager>();
        _datasourceFactory = datasourceFactory;
        _generatorOrchestrator = generatorOrchestrator;
        _templateEngineManager = templateEngineManager;
        _templateManager = templateManager;
        _workspaceMessageBus = workspaceMessageBus;
    }

    /// <summary>
    /// Get all currently loaded global plugins
    /// </summary>
    public IReadOnlyList<PluginInfo> GlobalPlugins =>
        _globalPlugins.Select(p => p.Info).ToList().AsReadOnly();

    /// <summary>
    /// Get all currently loaded workspace plugins
    /// </summary>
    public IReadOnlyList<PluginInfo> WorkspacePlugins =>
        _workspacePlugins.Select(p => p.Info).ToList().AsReadOnly();

    /// <summary>
    /// Load and initialize global plugins from the default plugin folder.
    /// Should be called once at application startup after the DI container is built.
    /// </summary>
    public void LoadGlobalPlugins(string? globalPluginsFolder = null)
    {
        var folder = globalPluginsFolder
            ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginsFolderName);

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
            _logger.LogInformation("Created global plugins folder: {Folder}", folder);
            return;
        }

        _logger.LogInformation("Scanning global plugins folder: {Folder}", folder);
        LoadPluginsFromFolder(folder, PluginScope.Global, loadContext: null, _globalPlugins);
    }

    /// <summary>
    /// Load and initialize workspace plugins from the workspace's Plugins folder.
    /// Should be called when a workspace is opened.
    /// </summary>
    public void LoadWorkspacePlugins(string workspaceDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspaceDirectory);

        // Ensure any previous workspace plugins are unloaded
        UnloadWorkspacePlugins();

        var pluginsFolder = Path.Combine(workspaceDirectory, PluginsFolderName);
        if (!Directory.Exists(pluginsFolder))
        {
            _logger.LogDebug("No workspace plugins folder found at {Folder}", pluginsFolder);
            return;
        }

        _logger.LogInformation("Scanning workspace plugins folder: {Folder}", pluginsFolder);

        // Create a collectible load context for workspace plugins
        _workspaceLoadContext = new PluginLoadContext(pluginsFolder);
        _workspaceLoadContextRef = new WeakReference(_workspaceLoadContext);

        LoadPluginsFromFolder(pluginsFolder, PluginScope.Workspace, _workspaceLoadContext, _workspacePlugins);
    }

    /// <summary>
    /// Unload all workspace plugins and free the collectible AssemblyLoadContext.
    /// Should be called when a workspace is closed.
    /// </summary>
    public void UnloadWorkspacePlugins()
    {
        if (_workspacePlugins.Count == 0)
            return;

        _logger.LogInformation("Unloading {Count} workspace plugin(s)", _workspacePlugins.Count);

        foreach (var loadedPlugin in _workspacePlugins)
        {
            ShutdownPlugin(loadedPlugin);
        }
        _workspacePlugins.Clear();

        // Unload the AssemblyLoadContext
        if (_workspaceLoadContext != null)
        {
            _workspaceLoadContext.Unload();
            _workspaceLoadContext = null;

            // Optionally verify unload (for diagnostics)
            if (_workspaceLoadContextRef != null)
            {
                for (int i = 0; i < 10 && _workspaceLoadContextRef.IsAlive; i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                if (_workspaceLoadContextRef.IsAlive)
                {
                    _logger.LogWarning("Workspace plugin AssemblyLoadContext was not fully unloaded. " +
                        "This may indicate reference leaks from plugin types.");
                }
                else
                {
                    _logger.LogDebug("Workspace plugin AssemblyLoadContext successfully unloaded");
                }
            }
        }
    }

    private void LoadPluginsFromFolder(
        string pluginsFolder,
        PluginScope expectedScope,
        AssemblyLoadContext? loadContext,
        List<LoadedPlugin> targetList)
    {
        foreach (var pluginDir in Directory.GetDirectories(pluginsFolder))
        {
            var dirName = Path.GetFileName(pluginDir);
            try
            {
                var loadedPlugin = LoadPluginFromDirectory(pluginDir, expectedScope, loadContext);
                if (loadedPlugin != null)
                {
                    targetList.Add(loadedPlugin);
                    _logger.LogInformation(
                        "Loaded {Scope} plugin '{Name}' v{Version} from {Directory}",
                        expectedScope, loadedPlugin.Info.Plugin.Name,
                        loadedPlugin.Info.Plugin.Version, pluginDir);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load plugin from directory: {Directory}", pluginDir);
            }
        }
    }

    private LoadedPlugin? LoadPluginFromDirectory(
        string pluginDirectory,
        PluginScope expectedScope,
        AssemblyLoadContext? loadContext)
    {
        // Find the main plugin DLL (convention: same name as the folder)
        var dirName = Path.GetFileName(pluginDirectory);
        var dllPath = Path.Combine(pluginDirectory, $"{dirName}.dll");

        if (!File.Exists(dllPath))
        {
            // Try to find any DLL that contains an IPlugin implementation
            var dlls = Directory.GetFiles(pluginDirectory, "*.dll");
            if (dlls.Length == 0)
            {
                _logger.LogWarning("No DLL files found in plugin directory: {Directory}", pluginDirectory);
                return null;
            }

            dllPath = dlls.FirstOrDefault(d =>
                Path.GetFileNameWithoutExtension(d)
                    .Equals(dirName, StringComparison.OrdinalIgnoreCase)) ?? dlls[0];
        }

        // Load the assembly
        Assembly assembly;
        if (loadContext != null)
        {
            assembly = loadContext.LoadFromAssemblyPath(Path.GetFullPath(dllPath));
        }
        else
        {
            // Global plugins load into the default context
            assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(dllPath));
        }

        // Find the IPlugin implementation
        var pluginType = assembly.GetExportedTypes()
            .FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

        if (pluginType == null)
        {
            _logger.LogWarning("No IPlugin implementation found in {DllPath}", dllPath);
            return null;
        }

        // Create the plugin instance
        var plugin = (IPlugin)Activator.CreateInstance(pluginType)!;

        // Validate scope
        if (plugin.Scope != expectedScope)
        {
            _logger.LogWarning(
                "Plugin '{Name}' declares scope {DeclaredScope} but was found in {ExpectedScope} plugins folder. Skipping.",
                plugin.Name, plugin.Scope, expectedScope);
            (plugin as IDisposable)?.Dispose();
            return null;
        }

        // Create the plugin context for registration
        var context = new PluginContext(
            pluginDirectory,
            _loggerFactory,
            _datasourceFactory,
            _generatorOrchestrator,
            _templateEngineManager,
            _templateManager,
            _workspaceMessageBus);

        // Initialize the plugin
        var info = new PluginInfo
        {
            Plugin = plugin,
            PluginDirectory = pluginDirectory,
            IsActive = false
        };

        try
        {
            plugin.Initialize(context);
            info.IsActive = true;
        }
        catch (Exception ex)
        {
            info.Error = ex.Message;
            _logger.LogError(ex, "Plugin '{Name}' failed to initialize", plugin.Name);
        }

        return new LoadedPlugin(info, context);
    }

    private void ShutdownPlugin(LoadedPlugin loadedPlugin)
    {
        try
        {
            _logger.LogDebug("Shutting down plugin '{Name}'", loadedPlugin.Info.Plugin.Name);

            // Call plugin's own shutdown
            loadedPlugin.Info.Plugin.Shutdown();

            // Unregister all contributions
            loadedPlugin.Context.UnregisterAll();

            // Dispose the plugin
            loadedPlugin.Info.Plugin.Dispose();
            loadedPlugin.Info.IsActive = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error shutting down plugin '{Name}'", loadedPlugin.Info.Plugin.Name);
        }
    }

    public void Dispose()
    {
        // Shutdown workspace plugins
        UnloadWorkspacePlugins();

        // Shutdown global plugins
        foreach (var plugin in _globalPlugins)
        {
            ShutdownPlugin(plugin);
        }
        _globalPlugins.Clear();
    }

    /// <summary>
    /// Internal record to associate a plugin with its registration context
    /// </summary>
    private record LoadedPlugin(PluginInfo Info, PluginContext Context);
}
