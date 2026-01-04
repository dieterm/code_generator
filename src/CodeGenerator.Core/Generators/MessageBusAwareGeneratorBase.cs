using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Events;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using CodeGenerator.Core.Models.Schema;
using CodeGenerator.Core.Services;
using CodeGenerator.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.Generators;

/// <summary>
/// Base class for generators that use the message bus
/// </summary>
public abstract class MessageBusAwareGeneratorBase : IMessageBusAwareGenerator
{
    private readonly Lazy<ITemplateEngine> _templateEngine;
    private readonly Lazy<IFileService> _fileService;
    private readonly Lazy<GeneratorSettingsDescription> _generatorSettingsDescription;
    protected IGeneratorMessageBus? MessageBus { get; private set; }
    protected ILogger Logger { get; }
    protected ITemplateEngine TemplateEngine { get { return _templateEngine.Value; } }
    protected IFileService FileService { get { return _fileService.Value; } }
    protected MessageBusAwareGeneratorBase(ILogger logger)
    {
        Logger = logger;
        _templateEngine = new Lazy<ITemplateEngine>(() => ServiceProviderHolder.ServiceProvider.GetRequiredService<ITemplateEngine>());
        _fileService = new Lazy<IFileService>(() => ServiceProviderHolder.ServiceProvider.GetRequiredService<IFileService>());
        _generatorSettingsDescription = new Lazy<GeneratorSettingsDescription>(CreateGeneratorSettingsDescription);
    }

    protected abstract GeneratorSettingsDescription CreateGeneratorSettingsDescription();

    public GeneratorSettingsDescription SettingsDescription { get { return _generatorSettingsDescription.Value; } }

    public virtual void Initialize(IGeneratorMessageBus messageBus)
    {
        MessageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
    }

    public abstract void SubscribeToEvents(IGeneratorMessageBus messageBus);


    public virtual void UnsubscribeFromEvents(IGeneratorMessageBus messageBus)
    {
        // Override in derived classes to unsubscribe from events
    }

    /// <summary>
    /// Request content for a placeholder from other generators
    /// </summary>
    protected async Task<string> RequestPlaceholderContentAsync(
        PlaceholderContentRequestedEventArgs eventArgs,
        CancellationToken cancellationToken = default)
    {
        if (MessageBus == null)
        {
            Logger.LogWarning("MessageBus is not initialized. Cannot request placeholder content.");
            return string.Empty;
        }

        await MessageBus.PublishAsync(eventArgs, cancellationToken);
        return eventArgs.GetCombinedContent();
    }

    /// <summary>
    /// Publish a CreatingFile event and check if it was cancelled
    /// </summary>
    protected async Task<bool> PublishCreatingFileEventAsync(
        CreatingFileEventArgs eventArgs,
        CancellationToken cancellationToken = default)
    {
        if (MessageBus == null)
        {
            return true; // Continue if no message bus
        }

        await MessageBus.PublishAsync(eventArgs, cancellationToken);
        
        if (eventArgs.Cancel)
        {
            Logger.LogDebug("File creation cancelled: {FileName}. Reason: {Reason}", 
                eventArgs.File.FileName, eventArgs.CancelReason);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Publish a CreatedFile event
    /// </summary>
    protected async Task PublishCreatedFileEventAsync(
        CreatedFileEventArgs eventArgs,
        CancellationToken cancellationToken = default)
    {
        if (MessageBus != null)
        {
            await MessageBus.PublishAsync(eventArgs, cancellationToken);
        }
    }

    protected virtual string GetRootNamespace()
    {
        var settings = GetSettings();
        var rootNamespace = string.IsNullOrWhiteSpace(settings.RootNamespace) ? "<Settings_RootNamespace_NotSet>" : settings.RootNamespace;
        return rootNamespace;
    }

    protected virtual string GetSchemaNamespace(DomainSchema schema)
    {
        var schemaNamespace = string.IsNullOrWhiteSpace(schema.CodeGenMetadata?.Namespace) ? "<Schema_XCodegen_Namespace_NotSet>" : schema.CodeGenMetadata?.Namespace;
        return schemaNamespace;
    }

    protected GeneratorSettings GetSettings()
    {
        var settingsService = ServiceProviderHolder.GetRequiredService<ISettingsService>();
        return settingsService.Settings;
    }

    protected bool IsProject<T>(DomainSchema schema, ProjectRegistration project) where T : BaseProjectGenerator
    {
        var projectGenerator = ServiceProviderHolder.GetRequiredService<T>();
        var projectName = projectGenerator.GetProjectName(schema);
        return string.Equals(projectName, project.ProjectName, StringComparison.OrdinalIgnoreCase);
    }
}
