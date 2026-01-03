using Microsoft.Extensions.Logging;

namespace CodeGenerator.WinForms.Services;

/// <summary>
/// Custom logger provider that routes log messages to a callback
/// </summary>
public class ListBoxLoggerProvider : ILoggerProvider
{
    private readonly Action<string, LogLevel> _logCallback;
    private readonly LogLevel _minimumLevel;

    public ListBoxLoggerProvider(Action<string, LogLevel> logCallback, LogLevel minimumLevel = LogLevel.Information)
    {
        _logCallback = logCallback ?? throw new ArgumentNullException(nameof(logCallback));
        _minimumLevel = minimumLevel;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new ListBoxLogger(categoryName, _logCallback, _minimumLevel);
    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}

/// <summary>
/// Custom logger that sends messages to a callback
/// </summary>
public class ListBoxLogger : ILogger
{
    private readonly string _categoryName;
    private readonly Action<string, LogLevel> _logCallback;
    private readonly LogLevel _minimumLevel;

    public ListBoxLogger(string categoryName, Action<string, LogLevel> logCallback, LogLevel minimumLevel)
    {
        _categoryName = categoryName;
        _logCallback = logCallback;
        _minimumLevel = minimumLevel;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _minimumLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var message = formatter(state, exception);
        
        // Extract short category name (last part after the dot)
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

        _logCallback(formattedMessage, logLevel);
    }
}

/// <summary>
/// Extension methods for adding ListBoxLogger to ILoggingBuilder
/// </summary>
public static class ListBoxLoggerExtensions
{
    public static ILoggingBuilder AddListBoxLogger(this ILoggingBuilder builder, Action<string, LogLevel> logCallback, LogLevel minimumLevel = LogLevel.Information)
    {
        builder.AddProvider(new ListBoxLoggerProvider(logCallback, minimumLevel));
        return builder;
    }
}
