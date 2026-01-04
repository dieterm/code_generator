using CodeGenerator.Core.Events;
using CodeGenerator.Core.Models.Configuration;

namespace CodeGenerator.Core.Interfaces;

/// <summary>
/// Interface for generators that can participate in the message bus
/// </summary>
public interface IMessageBusAwareGenerator //: ICodeGenerator
{

    GeneratorSettingsDescription SettingsDescription { get; }

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
