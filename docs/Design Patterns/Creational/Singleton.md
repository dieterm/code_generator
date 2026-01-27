# Singleton Pattern

## Intentie
Zorgt ervoor dat een class **exact één instance** heeft en biedt een **globaal toegangspunt** tot die instance.

## Wanneer gebruiken?
- Wanneer er precies één object nodig is om acties te coördineren in het hele systeem
- Voor logging, configuratie, connection pools, caches
- Wanneer de instance gedeeld moet worden tussen meerdere onderdelen

## Structuur

```
???????????????????????????????
?        Singleton            ?
???????????????????????????????
? - instance: Singleton       ?
? - Singleton()               ?
???????????????????????????????
? + GetInstance(): Singleton  ?
? + BusinessLogic()           ?
???????????????????????????????
```

## Implementatie in C#

### Basis Implementatie (Lazy, Thread-Safe)

```csharp
public sealed class Singleton
{
    // Lazy<T> zorgt voor thread-safe lazy initialization
    private static readonly Lazy<Singleton> _instance = 
        new Lazy<Singleton>(() => new Singleton());

    // Private constructor voorkomt externe instantiatie
    private Singleton()
    {
        // Initialisatie logica
    }

    // Globaal toegangspunt
    public static Singleton Instance => _instance.Value;

    // Business methods
    public void DoSomething()
    {
        Console.WriteLine("Singleton is working");
    }
}

// Gebruik
Singleton.Instance.DoSomething();
```

### Alternatief: Static Constructor

```csharp
public sealed class Singleton
{
    private static readonly Singleton _instance = new Singleton();

    // Explicit static constructor zorgt voor thread-safety
    static Singleton() { }

    private Singleton() { }

    public static Singleton Instance => _instance;
}
```

### Met Dependency Injection (Aanbevolen in moderne C#)

```csharp
// Service definitie
public interface IConfigurationService
{
    string GetSetting(string key);
}

public class ConfigurationService : IConfigurationService
{
    private readonly Dictionary<string, string> _settings;

    public ConfigurationService()
    {
        _settings = LoadSettings();
    }

    public string GetSetting(string key) => 
        _settings.TryGetValue(key, out var value) ? value : string.Empty;

    private Dictionary<string, string> LoadSettings() => 
        new Dictionary<string, string>
        {
            ["AppName"] = "My Application",
            ["Version"] = "1.0.0"
        };
}

// Registratie in DI Container
services.AddSingleton<IConfigurationService, ConfigurationService>();

// Gebruik via constructor injection
public class MyController
{
    private readonly IConfigurationService _config;

    public MyController(IConfigurationService config)
    {
        _config = config;
    }
}
```

## Praktisch Voorbeeld: Logger

```csharp
public sealed class Logger
{
    private static readonly Lazy<Logger> _instance = 
        new Lazy<Logger>(() => new Logger());
    
    private readonly object _lock = new object();
    private readonly string _logPath;

    private Logger()
    {
        _logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "app.log");
    }

    public static Logger Instance => _instance.Value;

    public void Log(LogLevel level, string message)
    {
        var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
        
        lock (_lock)
        {
            File.AppendAllText(_logPath, logEntry + Environment.NewLine);
        }
        
        Console.WriteLine(logEntry);
    }

    public void Info(string message) => Log(LogLevel.Info, message);
    public void Warning(string message) => Log(LogLevel.Warning, message);
    public void Error(string message) => Log(LogLevel.Error, message);
}

public enum LogLevel
{
    Info,
    Warning,
    Error
}

// Gebruik
Logger.Instance.Info("Application started");
Logger.Instance.Error("Something went wrong");
```

## Voordelen
- ? Gegarandeerd één instance
- ? Globaal toegangspunt
- ? Lazy initialization mogelijk
- ? Thread-safe met juiste implementatie

## Nadelen
- ? Moeilijker te unit testen (tight coupling)
- ? Kan misbruikt worden als "global state"
- ? Verbergt dependencies
- ? Single Responsibility Principle schending (beheert eigen lifetime)

## Alternatieven
- **Dependency Injection**: `services.AddSingleton<T>()` - aanbevolen!
- **Static class**: Wanneer er geen state is
- **Monostate Pattern**: Alle instances delen dezelfde state

## Anti-Pattern Waarschuwing

Vermijd overmatig gebruik van Singleton. Het wordt vaak misbruikt als vervanging voor global variables. Gebruik bij voorkeur **Dependency Injection** voor beter testbare en onderhoudbare code.

```csharp
// ? Vermijd: directe Singleton toegang
public class OrderService
{
    public void ProcessOrder()
    {
        Logger.Instance.Info("Processing order"); // Tight coupling
    }
}

// ? Beter: Dependency Injection
public class OrderService
{
    private readonly ILogger _logger;

    public OrderService(ILogger logger)
    {
        _logger = logger; // Loose coupling, testbaar
    }

    public void ProcessOrder()
    {
        _logger.Info("Processing order");
    }
}
```
