namespace CodeGenerator.Core.Events;

/// <summary>
/// Interface for the generator message bus
/// </summary>
public interface IGeneratorMessageBus
{
    /// <summary>
    /// Subscribe to an event type
    /// </summary>
    void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : GeneratorEventArgs;
    
    /// <summary>
    /// Subscribe to an event type with async handler
    /// </summary>
    void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : GeneratorEventArgs;
    
    /// <summary>
    /// Unsubscribe from an event type
    /// </summary>
    void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : GeneratorEventArgs;
    
    /// <summary>
    /// Unsubscribe from an event type with async handler
    /// </summary>
    void Unsubscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : GeneratorEventArgs;
    
    /// <summary>
    /// Publish an event to all subscribers
    /// </summary>
    void Publish<TEvent>(TEvent eventArgs) where TEvent : GeneratorEventArgs;
    
    /// <summary>
    /// Publish an event to all subscribers asynchronously
    /// </summary>
    Task PublishAsync<TEvent>(TEvent eventArgs, CancellationToken cancellationToken = default) where TEvent : GeneratorEventArgs;
    
    /// <summary>
    /// Clear all subscriptions
    /// </summary>
    void ClearSubscriptions();
    
    /// <summary>
    /// Clear subscriptions for a specific event type
    /// </summary>
    void ClearSubscriptions<TEvent>() where TEvent : GeneratorEventArgs;
}
