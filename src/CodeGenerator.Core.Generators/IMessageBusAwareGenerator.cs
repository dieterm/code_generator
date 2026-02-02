
using CodeGenerator.Core.Generators.Settings;

namespace CodeGenerator.Core.Generators;

/// <summary>
/// Interface for generators that can participate in the message bus
/// </summary>
public interface IMessageBusAwareGenerator 
{
    /// <summary>
    /// Unique identifier of this generator.
    /// Shortcut for SettingsDescription.Id
    /// </summary>
    string Id { get; }

    GeneratorSettingsDescription SettingsDescription { get; }

    /// <summary>
    /// Initialize the generator with the message bus
    /// </summary>
    void Initialize(GeneratorMessageBus messageBus);
    
    /// <summary>
    /// Subscribe to events on the message bus
    /// </summary>
    void SubscribeToEvents(GeneratorMessageBus messageBus);
    
    /// <summary>
    /// Unsubscribe from events on the message bus
    /// </summary>
    void UnsubscribeFromEvents(GeneratorMessageBus messageBus);
}
