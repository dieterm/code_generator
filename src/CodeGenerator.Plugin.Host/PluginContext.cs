using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Plugin.Abstractions;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Views;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Plugin.Host;

/// <summary>
/// Concrete implementation of <see cref="IPluginContext"/> that bridges
/// plugin registrations to the host application's managers and factories.
/// Tracks all registrations so they can be cleanly removed on plugin unload.
/// </summary>
internal class PluginContext : IPluginContext
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IDatasourceFactory _datasourceFactory;
    private readonly GeneratorOrchestrator _generatorOrchestrator;
    private readonly TemplateEngineManager _templateEngineManager;
    private readonly TemplateManager _templateManager;
    private readonly WorkspaceMessageBus _workspaceMessageBus;

    // Track registrations for cleanup
    private readonly List<IDatasourceProvider> _registeredDatasourceProviders = new();
    private readonly List<IMessageBusAwareGenerator> _registeredGenerators = new();
    private readonly List<IWorkspaceMessageBusSubscriber> _registeredWorkspaceSubscribers = new();
    private readonly List<string> _registeredTemplateFolders = new();
    private readonly List<(Type viewModelType, Type viewType)> _registeredViewMappings = new();
    private readonly object _lock = new();

    public string PluginDirectory { get; }

    public PluginContext(
        string pluginDirectory,
        ILoggerFactory loggerFactory,
        IDatasourceFactory datasourceFactory,
        GeneratorOrchestrator generatorOrchestrator,
        TemplateEngineManager templateEngineManager,
        TemplateManager templateManager,
        WorkspaceMessageBus workspaceMessageBus)
    {
        PluginDirectory = pluginDirectory;
        _loggerFactory = loggerFactory;
        _datasourceFactory = datasourceFactory;
        _generatorOrchestrator = generatorOrchestrator;
        _templateEngineManager = templateEngineManager;
        _templateManager = templateManager;
        _workspaceMessageBus = workspaceMessageBus;
    }

    public void RegisterDatasourceProvider(object datasourceProvider)
    {
        if (datasourceProvider is not IDatasourceProvider provider)
            throw new ArgumentException($"Object must implement {nameof(IDatasourceProvider)}", nameof(datasourceProvider));

        lock (_lock)
        {
            _datasourceFactory.RegisterProvider(provider);
            _registeredDatasourceProviders.Add(provider);
        }
    }

    public void RegisterGenerator(object generator)
    {
        if (generator is not IMessageBusAwareGenerator mbGenerator)
            throw new ArgumentException($"Object must implement {nameof(IMessageBusAwareGenerator)}", nameof(generator));

        lock (_lock)
        {
            mbGenerator.Initialize(_generatorOrchestrator.MessageBus);
            mbGenerator.SubscribeToEvents(_generatorOrchestrator.MessageBus);
            _registeredGenerators.Add(mbGenerator);
        }
    }

    public void RegisterTemplateEngine(object templateEngine)
    {
        if (templateEngine is not ITemplateEngine engine)
            throw new ArgumentException($"Object must implement {nameof(ITemplateEngine)}", nameof(templateEngine));

        // TemplateEngineManager doesn't expose a dynamic add method,
        // but we can initialize the engine so it's ready for use
        engine.Initialize();
    }

    public void RegisterWorkspaceSubscriber(object subscriber)
    {
        if (subscriber is not IWorkspaceMessageBusSubscriber wSubscriber)
            throw new ArgumentException($"Object must implement {nameof(IWorkspaceMessageBusSubscriber)}", nameof(subscriber));

        lock (_lock)
        {
            wSubscriber.Subscribe(_workspaceMessageBus);
            _registeredWorkspaceSubscribers.Add(wSubscriber);
        }
    }

    public void RegisterViewMapping(Type viewModelType, Type viewType)
    {
        ArgumentNullException.ThrowIfNull(viewModelType);
        ArgumentNullException.ThrowIfNull(viewType);

        lock (_lock)
        {
            _registeredViewMappings.Add((viewModelType, viewType));
            // Register with the PluginViewFactory so the host ViewFactory can resolve plugin views
            PluginViewFactory.Instance.RegisterMapping(viewModelType, viewType);
        }
    }

    public void RegisterTemplateFolder(string folderPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"Template folder not found: {folderPath}");

        lock (_lock)
        {
            _templateManager.RegisterTemplateFolder(folderPath);
            _registeredTemplateFolders.Add(folderPath);
        }
    }

    public ILogger CreateLogger(string categoryName) => _loggerFactory.CreateLogger(categoryName);

    public ILogger<T> CreateLogger<T>() => _loggerFactory.CreateLogger<T>();

    /// <summary>
    /// Unregister all contributions made by the plugin through this context.
    /// Called during plugin shutdown/unload.
    /// </summary>
    internal void UnregisterAll()
    {
        lock (_lock)
        {
            // Unregister datasource providers
            foreach (var provider in _registeredDatasourceProviders)
            {
                _datasourceFactory.UnregisterProvider(provider.TypeId);
            }
            _registeredDatasourceProviders.Clear();

            // Unsubscribe generators from message bus
            foreach (var generator in _registeredGenerators)
            {
                generator.UnsubscribeFromEvents(_generatorOrchestrator.MessageBus);
            }
            _registeredGenerators.Clear();

            // Unsubscribe workspace message bus subscribers
            foreach (var subscriber in _registeredWorkspaceSubscribers)
            {
                subscriber.Unsubscribe(_workspaceMessageBus);
            }
            _registeredWorkspaceSubscribers.Clear();

            // Unregister template folders
            foreach (var folder in _registeredTemplateFolders)
            {
                _templateManager.UnregisterTemplateFolder(folder);
            }
            _registeredTemplateFolders.Clear();

            // Unregister view mappings
            foreach (var (viewModelType, _) in _registeredViewMappings)
            {
                PluginViewFactory.Instance.UnregisterMapping(viewModelType);
            }
            _registeredViewMappings.Clear();
        }
    }
}
