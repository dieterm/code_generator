using CodeGenerator.Core.Events;

namespace CodeGenerator.Core.Interfaces;

/// <summary>
/// Interface for generators that can participate in the message bus
/// </summary>
public interface IMessageBusAwareGenerator //: ICodeGenerator
{
    /// <summary>
    /// Unique identifier for this generator
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Display name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Description of what this generator produces
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Initialize the generator with the message bus
    /// </summary>
    void Initialize(IGeneratorMessageBus messageBus);
    
    /// <summary>
    /// Subscribe to events on the message bus
    /// </summary>
    void SubscribeToEvents(IGeneratorMessageBus messageBus);
    
    /// <summary>
    /// Unsubscribe from events on the message bus
    /// </summary>
    void UnsubscribeFromEvents(IGeneratorMessageBus messageBus);
}
