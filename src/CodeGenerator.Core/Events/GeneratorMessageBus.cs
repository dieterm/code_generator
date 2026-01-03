using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.Events;

/// <summary>
/// Default implementation of the generator message bus
/// </summary>
public class GeneratorMessageBus : IGeneratorMessageBus
{
    private readonly Dictionary<Type, List<Delegate>> _syncHandlers = new();
    private readonly Dictionary<Type, List<Delegate>> _asyncHandlers = new();
    private readonly object _lock = new();
    private readonly ILogger<GeneratorMessageBus>? _logger;

    public GeneratorMessageBus(ILogger<GeneratorMessageBus>? logger = null)
    {
        _logger = logger;
    }

    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : GeneratorEventArgs
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        lock (_lock)
        {
            var eventType = typeof(TEvent);
            if (!_syncHandlers.ContainsKey(eventType))
            {
                _syncHandlers[eventType] = new List<Delegate>();
            }
            _syncHandlers[eventType].Add(handler);
            
            _logger?.LogDebug("Subscribed sync handler to {EventType}", eventType.Name);
        }
    }

    public void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : GeneratorEventArgs
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        lock (_lock)
        {
            var eventType = typeof(TEvent);
            if (!_asyncHandlers.ContainsKey(eventType))
            {
                _asyncHandlers[eventType] = new List<Delegate>();
            }
            _asyncHandlers[eventType].Add(handler);
            
            _logger?.LogDebug("Subscribed async handler to {EventType}", eventType.Name);
        }
    }

    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : GeneratorEventArgs
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        lock (_lock)
        {
            var eventType = typeof(TEvent);
            if (_syncHandlers.TryGetValue(eventType, out var handlers))
            {
                handlers.Remove(handler);
                _logger?.LogDebug("Unsubscribed sync handler from {EventType}", eventType.Name);
            }
        }
    }

    public void Unsubscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : GeneratorEventArgs
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        lock (_lock)
        {
            var eventType = typeof(TEvent);
            if (_asyncHandlers.TryGetValue(eventType, out var handlers))
            {
                handlers.Remove(handler);
                _logger?.LogDebug("Unsubscribed async handler from {EventType}", eventType.Name);
            }
        }
    }

    public void Publish<TEvent>(TEvent eventArgs) where TEvent : GeneratorEventArgs
    {
        if (eventArgs == null) throw new ArgumentNullException(nameof(eventArgs));

        List<Delegate> syncHandlersCopy;
        List<Delegate> asyncHandlersCopy;

        lock (_lock)
        {
            var eventType = typeof(TEvent);
            syncHandlersCopy = _syncHandlers.TryGetValue(eventType, out var syncList) 
                ? new List<Delegate>(syncList) 
                : new List<Delegate>();
            asyncHandlersCopy = _asyncHandlers.TryGetValue(eventType, out var asyncList) 
                ? new List<Delegate>(asyncList) 
                : new List<Delegate>();
        }

        _logger?.LogDebug("Publishing {EventType} to {SyncCount} sync and {AsyncCount} async handlers", 
            typeof(TEvent).Name, syncHandlersCopy.Count, asyncHandlersCopy.Count);

        foreach (var handler in syncHandlersCopy)
        {
            try
            {
                ((Action<TEvent>)handler)(eventArgs);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in sync event handler for {EventType}", typeof(TEvent).Name);
            }
        }

        foreach (var handler in asyncHandlersCopy)
        {
            try
            {
                ((Func<TEvent, Task>)handler)(eventArgs).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in async event handler for {EventType}", typeof(TEvent).Name);
            }
        }
    }

    public async Task PublishAsync<TEvent>(TEvent eventArgs, CancellationToken cancellationToken = default) where TEvent : GeneratorEventArgs
    {
        if (eventArgs == null) throw new ArgumentNullException(nameof(eventArgs));

        List<Delegate> syncHandlersCopy;
        List<Delegate> asyncHandlersCopy;

        lock (_lock)
        {
            var eventType = typeof(TEvent);
            syncHandlersCopy = _syncHandlers.TryGetValue(eventType, out var syncList) 
                ? new List<Delegate>(syncList) 
                : new List<Delegate>();
            asyncHandlersCopy = _asyncHandlers.TryGetValue(eventType, out var asyncList) 
                ? new List<Delegate>(asyncList) 
                : new List<Delegate>();
        }

        _logger?.LogDebug("Publishing async {EventType} to {SyncCount} sync and {AsyncCount} async handlers", 
            typeof(TEvent).Name, syncHandlersCopy.Count, asyncHandlersCopy.Count);

        foreach (var handler in syncHandlersCopy)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                ((Action<TEvent>)handler)(eventArgs);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in sync event handler for {EventType}", typeof(TEvent).Name);
            }
        }

        foreach (var handler in asyncHandlersCopy)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await ((Func<TEvent, Task>)handler)(eventArgs);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in async event handler for {EventType}", typeof(TEvent).Name);
            }
        }
    }

    public void ClearSubscriptions()
    {
        lock (_lock)
        {
            _syncHandlers.Clear();
            _asyncHandlers.Clear();
            _logger?.LogDebug("Cleared all subscriptions");
        }
    }

    public void ClearSubscriptions<TEvent>() where TEvent : GeneratorEventArgs
    {
        lock (_lock)
        {
            var eventType = typeof(TEvent);
            _syncHandlers.Remove(eventType);
            _asyncHandlers.Remove(eventType);
            _logger?.LogDebug("Cleared subscriptions for {EventType}", eventType.Name);
        }
    }
}
