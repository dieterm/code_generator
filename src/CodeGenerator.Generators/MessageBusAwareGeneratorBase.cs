using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Events;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Generators;

/// <summary>
/// Base class for generators that use the message bus
/// </summary>
public abstract class MessageBusAwareGeneratorBase : IMessageBusAwareGenerator
{
    protected IGeneratorMessageBus? MessageBus { get; private set; }
    protected ILogger Logger { get; }

    protected MessageBusAwareGeneratorBase(ILogger logger)
    {
        Logger = logger;
    }

    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract GeneratorType Type { get; }
    public abstract ArchitectureLayer Layer { get; }
    public abstract IReadOnlyList<TargetLanguage> SupportedLanguages { get; }

    public virtual void Initialize(IGeneratorMessageBus messageBus)
    {
        MessageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
    }

    public virtual void SubscribeToEvents(IGeneratorMessageBus messageBus)
    {
        // Override in derived classes to subscribe to events
    }

    public virtual void UnsubscribeFromEvents(IGeneratorMessageBus messageBus)
    {
        // Override in derived classes to unsubscribe from events
    }

    public abstract Task<GenerationResult> GenerateAsync(DomainContext context, GeneratorSettings settings, CancellationToken cancellationToken = default);
    
    public abstract Task<GenerationResult> GenerateForEntityAsync(EntityModel entity, DomainContext context, GeneratorSettings settings, CancellationToken cancellationToken = default);
    
    public abstract Task<GenerationPreview> PreviewAsync(DomainContext context, GeneratorSettings settings, CancellationToken cancellationToken = default);
    
    public abstract ValidationResult Validate(GeneratorConfiguration configuration);

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
}
