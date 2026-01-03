using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace CodeGenerator.WinForms.Services;

/// <summary>
/// Service that manages log messages and allows UI components to subscribe
/// </summary>
public class LoggingService
{
    private readonly ConcurrentQueue<LogEntry> _pendingMessages = new();
    private Action<LogEntry>? _logHandler;
    private readonly object _lock = new();

    /// <summary>
    /// Register a handler to receive log messages
    /// </summary>
    public void RegisterHandler(Action<LogEntry> handler)
    {
        lock (_lock)
        {
            _logHandler = handler;
            
            // Send any pending messages
            while (_pendingMessages.TryDequeue(out var entry))
            {
                handler(entry);
            }
        }
    }

    /// <summary>
    /// Unregister the current handler
    /// </summary>
    public void UnregisterHandler()
    {
        lock (_lock)
        {
            _logHandler = null;
        }
    }

    /// <summary>
    /// Log a message
    /// </summary>
    public void Log(string message, LogLevel level = LogLevel.Information)
    {
        var entry = new LogEntry
        {
            Timestamp = DateTime.Now,
            Message = message,
            Level = level
        };

        lock (_lock)
        {
            if (_logHandler != null)
            {
                _logHandler(entry);
            }
            else
            {
                // Buffer messages until a handler is registered
                _pendingMessages.Enqueue(entry);
            }
        }
    }
}

/// <summary>
/// Represents a log entry
/// </summary>
public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public string Message { get; set; } = string.Empty;
    public LogLevel Level { get; set; }

    public string GetFormattedMessage()
    {
        var levelPrefix = Level switch
        {
            LogLevel.Error => "?",
            LogLevel.Warning => "??",
            LogLevel.Information => "??",
            LogLevel.Debug => "??",
            LogLevel.Trace => "??",
            _ => ""
        };
        
        return $"[{Timestamp:HH:mm:ss}] {levelPrefix} {Message}";
    }
}

/// <summary>
/// Logger provider that uses LoggingService
/// </summary>
public class LoggingServiceLoggerProvider : ILoggerProvider
{
    private readonly LoggingService _loggingService;
    private readonly LogLevel _minimumLevel;

    public LoggingServiceLoggerProvider(LoggingService loggingService, LogLevel minimumLevel = LogLevel.Information)
    {
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        _minimumLevel = minimumLevel;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new LoggingServiceLogger(categoryName, _loggingService, _minimumLevel);
    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}

/// <summary>
/// Logger that uses LoggingService
/// </summary>
public class LoggingServiceLogger : ILogger
{
    private readonly string _categoryName;
    private readonly LoggingService _loggingService;
    private readonly LogLevel _minimumLevel;

    public LoggingServiceLogger(string categoryName, LoggingService loggingService, LogLevel minimumLevel)
    {
        _categoryName = categoryName;
        _loggingService = loggingService;
        _minimumLevel = minimumLevel;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _minimumLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var message = formatter(state, exception);
        
        // Extract short category name
        var shortCategory = _categoryName;
        var lastDot = _categoryName.LastIndexOf('.');
        if (lastDot >= 0 && lastDot < _categoryName.Length - 1)
        {
            shortCategory = _categoryName.Substring(lastDot + 1);
        }

        var formattedMessage = $"[{shortCategory}] {message}";
        
        if (exception != null)
        {
            formattedMessage += $"\n  Exception: {exception.Message}";
        }

        _loggingService.Log(formattedMessage, logLevel);
    }
}

/// <summary>
/// Extension methods for adding LoggingServiceLogger to ILoggingBuilder
/// </summary>
public static class LoggingServiceLoggerExtensions
{
    public static ILoggingBuilder AddLoggingService(this ILoggingBuilder builder, LoggingService loggingService, LogLevel minimumLevel = LogLevel.Information)
    {
        builder.AddProvider(new LoggingServiceLoggerProvider(loggingService, minimumLevel));
        return builder;
    }
}
