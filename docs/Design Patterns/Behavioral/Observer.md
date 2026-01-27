# Observer Pattern

## Intentie
Definieert een **één-naar-veel dependency** tussen objecten zodat wanneer één object van state verandert, alle afhankelijke objecten automatisch worden **genotificeerd en bijgewerkt**.

## Wanneer gebruiken?
- Wanneer een verandering in één object andere objecten moet beïnvloeden
- Wanneer je niet weet hoeveel objecten genotificeerd moeten worden
- Wanneer je loose coupling wilt tussen subject en observers
- Event-driven systemen, UI updates, pub/sub messaging

## Structuur

```
???????????????????         ???????????????????
?     Subject     ?????????>?    Observer     ?
???????????????????         ???????????????????
? +Attach()       ?         ? +Update()       ?
? +Detach()       ?         ???????????????????
? +Notify()       ?                 ?
???????????????????                 ?
        ?                   ?????????????????
        ?                   ?               ?
?????????????????    ??????????????? ???????????????
?ConcreteSubject?    ? ObserverA   ? ?  ObserverB  ?
?????????????????    ??????????????? ???????????????
? -state        ?
? +GetState()   ?
? +SetState()   ?
?????????????????
```

## Implementatie in C#

### Met C# Events (Aanbevolen)

```csharp
// Subject met events
public class Stock
{
    private string _symbol;
    private decimal _price;

    public Stock(string symbol, decimal price)
    {
        _symbol = symbol;
        _price = price;
    }

    public string Symbol => _symbol;

    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                var oldPrice = _price;
                _price = value;
                OnPriceChanged(new StockPriceChangedEventArgs(this, oldPrice, value));
            }
        }
    }

    // Event declaratie
    public event EventHandler<StockPriceChangedEventArgs>? PriceChanged;

    protected virtual void OnPriceChanged(StockPriceChangedEventArgs e)
    {
        PriceChanged?.Invoke(this, e);
    }
}

public class StockPriceChangedEventArgs : EventArgs
{
    public Stock Stock { get; }
    public decimal OldPrice { get; }
    public decimal NewPrice { get; }
    public decimal ChangePercent => (NewPrice - OldPrice) / OldPrice * 100;

    public StockPriceChangedEventArgs(Stock stock, decimal oldPrice, decimal newPrice)
    {
        Stock = stock;
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
}

// Observer 1: Alert System
public class StockAlertService
{
    private readonly decimal _threshold;

    public StockAlertService(decimal thresholdPercent = 5)
    {
        _threshold = thresholdPercent;
    }

    public void OnPriceChanged(object? sender, StockPriceChangedEventArgs e)
    {
        if (Math.Abs(e.ChangePercent) >= _threshold)
        {
            var direction = e.ChangePercent > 0 ? "?? UP" : "?? DOWN";
            Console.WriteLine($"ALERT: {e.Stock.Symbol} {direction} {e.ChangePercent:F2}%!");
        }
    }
}

// Observer 2: Logger
public class StockLogger
{
    public void OnPriceChanged(object? sender, StockPriceChangedEventArgs e)
    {
        Console.WriteLine(
            $"[{DateTime.Now:HH:mm:ss}] {e.Stock.Symbol}: " +
            $"€{e.OldPrice} -> €{e.NewPrice} ({e.ChangePercent:+0.00;-0.00}%)");
    }
}

// Observer 3: Portfolio Tracker
public class PortfolioTracker
{
    private readonly Dictionary<string, int> _holdings = new();
    
    public void AddHolding(string symbol, int shares)
    {
        _holdings[symbol] = shares;
    }

    public void OnPriceChanged(object? sender, StockPriceChangedEventArgs e)
    {
        if (_holdings.TryGetValue(e.Stock.Symbol, out int shares))
        {
            var oldValue = e.OldPrice * shares;
            var newValue = e.NewPrice * shares;
            var change = newValue - oldValue;
            Console.WriteLine(
                $"Portfolio impact: {shares} x {e.Stock.Symbol} = " +
                $"€{change:+0.00;-0.00}");
        }
    }
}

// Gebruik
var appleStock = new Stock("AAPL", 150.00m);

var alertService = new StockAlertService(thresholdPercent: 3);
var logger = new StockLogger();
var portfolio = new PortfolioTracker();
portfolio.AddHolding("AAPL", 100);

// Subscribe observers
appleStock.PriceChanged += alertService.OnPriceChanged;
appleStock.PriceChanged += logger.OnPriceChanged;
appleStock.PriceChanged += portfolio.OnPriceChanged;

// Price changes trigger all observers
appleStock.Price = 155.00m; // +3.33%
appleStock.Price = 145.00m; // -6.45%

// Unsubscribe
appleStock.PriceChanged -= alertService.OnPriceChanged;
```

### Generic Observer Pattern

```csharp
// Generic Subject
public interface ISubject<T>
{
    void Attach(IObserver<T> observer);
    void Detach(IObserver<T> observer);
    void Notify(T data);
}

public interface IObserver<T>
{
    void Update(T data);
}

public class Subject<T> : ISubject<T>
{
    private readonly List<IObserver<T>> _observers = new();

    public void Attach(IObserver<T> observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public void Detach(IObserver<T> observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(T data)
    {
        foreach (var observer in _observers.ToList()) // ToList prevents modification during iteration
        {
            observer.Update(data);
        }
    }
}

// Concrete implementation
public class WeatherStation : Subject<WeatherData>
{
    public void UpdateMeasurements(float temp, float humidity, float pressure)
    {
        var data = new WeatherData(temp, humidity, pressure, DateTime.Now);
        Notify(data);
    }
}

public record WeatherData(float Temperature, float Humidity, float Pressure, DateTime Timestamp);

public class TemperatureDisplay : IObserver<WeatherData>
{
    public void Update(WeatherData data)
    {
        Console.WriteLine($"??? Temperature: {data.Temperature}°C");
    }
}

public class HumidityDisplay : IObserver<WeatherData>
{
    public void Update(WeatherData data)
    {
        Console.WriteLine($"?? Humidity: {data.Humidity}%");
    }
}

public class WeatherForecast : IObserver<WeatherData>
{
    private float _lastPressure = 0;

    public void Update(WeatherData data)
    {
        if (data.Pressure > _lastPressure)
            Console.WriteLine("?? Forecast: Improving weather!");
        else if (data.Pressure < _lastPressure)
            Console.WriteLine("??? Forecast: Expect rain");
        else
            Console.WriteLine("? Forecast: No change");
        
        _lastPressure = data.Pressure;
    }
}

// Gebruik
var weatherStation = new WeatherStation();

weatherStation.Attach(new TemperatureDisplay());
weatherStation.Attach(new HumidityDisplay());
weatherStation.Attach(new WeatherForecast());

weatherStation.UpdateMeasurements(22.5f, 65f, 1013.25f);
weatherStation.UpdateMeasurements(23.0f, 60f, 1010.00f);
```

### Met IObservable<T> (Reactive Extensions Style)

```csharp
// .NET's built-in IObservable/IObserver
public class MessageBroker : IObservable<Message>
{
    private readonly List<IObserver<Message>> _observers = new();

    public IDisposable Subscribe(IObserver<Message> observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
        
        return new Unsubscriber(_observers, observer);
    }

    public void Publish(Message message)
    {
        foreach (var observer in _observers.ToList())
        {
            observer.OnNext(message);
        }
    }

    public void Complete()
    {
        foreach (var observer in _observers.ToList())
        {
            observer.OnCompleted();
        }
        _observers.Clear();
    }

    public void Error(Exception error)
    {
        foreach (var observer in _observers.ToList())
        {
            observer.OnError(error);
        }
    }

    private class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<Message>> _observers;
        private readonly IObserver<Message> _observer;

        public Unsubscriber(List<IObserver<Message>> observers, IObserver<Message> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            _observers.Remove(_observer);
        }
    }
}

public record Message(string Topic, string Content, DateTime Timestamp);

public class MessageSubscriber : IObserver<Message>
{
    private readonly string _name;
    private readonly string? _topicFilter;

    public MessageSubscriber(string name, string? topicFilter = null)
    {
        _name = name;
        _topicFilter = topicFilter;
    }

    public void OnNext(Message message)
    {
        if (_topicFilter == null || message.Topic == _topicFilter)
        {
            Console.WriteLine($"[{_name}] Received: {message.Topic} - {message.Content}");
        }
    }

    public void OnError(Exception error)
    {
        Console.WriteLine($"[{_name}] Error: {error.Message}");
    }

    public void OnCompleted()
    {
        Console.WriteLine($"[{_name}] Subscription completed");
    }
}

// Gebruik
var broker = new MessageBroker();

// Subscribe met automatic cleanup via IDisposable
using var sub1 = broker.Subscribe(new MessageSubscriber("All Topics"));
using var sub2 = broker.Subscribe(new MessageSubscriber("Orders Only", "orders"));

broker.Publish(new Message("orders", "New order #123", DateTime.Now));
broker.Publish(new Message("inventory", "Stock updated", DateTime.Now));
broker.Publish(new Message("orders", "Order #123 shipped", DateTime.Now));
```

### Praktisch Voorbeeld: Form Validation

```csharp
public class FormField
{
    private string _value = string.Empty;
    
    public string Name { get; }
    
    public string Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                ValueChanged?.Invoke(this, new FieldChangedEventArgs(Name, value));
            }
        }
    }

    public event EventHandler<FieldChangedEventArgs>? ValueChanged;

    public FormField(string name)
    {
        Name = name;
    }
}

public class FieldChangedEventArgs : EventArgs
{
    public string FieldName { get; }
    public string NewValue { get; }

    public FieldChangedEventArgs(string fieldName, string newValue)
    {
        FieldName = fieldName;
        NewValue = newValue;
    }
}

public class FormValidator
{
    private readonly Dictionary<string, List<Func<string, string?>>> _rules = new();
    private readonly Dictionary<string, List<string>> _errors = new();

    public void AddRule(string fieldName, Func<string, string?> rule)
    {
        if (!_rules.ContainsKey(fieldName))
            _rules[fieldName] = new List<Func<string, string?>>();
        
        _rules[fieldName].Add(rule);
    }

    public void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        ValidateField(e.FieldName, e.NewValue);
    }

    private void ValidateField(string fieldName, string value)
    {
        _errors[fieldName] = new List<string>();

        if (_rules.TryGetValue(fieldName, out var rules))
        {
            foreach (var rule in rules)
            {
                var error = rule(value);
                if (error != null)
                {
                    _errors[fieldName].Add(error);
                }
            }
        }

        OnValidationChanged(new ValidationChangedEventArgs(fieldName, _errors[fieldName]));
    }

    public event EventHandler<ValidationChangedEventArgs>? ValidationChanged;

    protected virtual void OnValidationChanged(ValidationChangedEventArgs e)
    {
        ValidationChanged?.Invoke(this, e);
    }

    public bool IsValid => _errors.Values.All(e => e.Count == 0);
}

public class ValidationChangedEventArgs : EventArgs
{
    public string FieldName { get; }
    public IReadOnlyList<string> Errors { get; }
    public bool IsValid => Errors.Count == 0;

    public ValidationChangedEventArgs(string fieldName, IReadOnlyList<string> errors)
    {
        FieldName = fieldName;
        Errors = errors;
    }
}

public class ValidationDisplay
{
    public void OnValidationChanged(object? sender, ValidationChangedEventArgs e)
    {
        if (e.IsValid)
        {
            Console.WriteLine($"? {e.FieldName}: Valid");
        }
        else
        {
            Console.WriteLine($"? {e.FieldName}: {string.Join(", ", e.Errors)}");
        }
    }
}

// Gebruik
var emailField = new FormField("email");
var passwordField = new FormField("password");

var validator = new FormValidator();
validator.AddRule("email", v => string.IsNullOrEmpty(v) ? "Email is required" : null);
validator.AddRule("email", v => !v.Contains('@') ? "Invalid email format" : null);
validator.AddRule("password", v => string.IsNullOrEmpty(v) ? "Password is required" : null);
validator.AddRule("password", v => v.Length < 8 ? "Password must be at least 8 characters" : null);

var display = new ValidationDisplay();

// Wire up observers
emailField.ValueChanged += validator.OnFieldChanged;
passwordField.ValueChanged += validator.OnFieldChanged;
validator.ValidationChanged += display.OnValidationChanged;

// Simulate user input
emailField.Value = "invalid";      // ? email: Invalid email format
emailField.Value = "test@test.com"; // ? email: Valid
passwordField.Value = "123";        // ? password: Password must be at least 8 characters
passwordField.Value = "securepassword123"; // ? password: Valid
```

## Voordelen
- ? Loose coupling tussen subject en observers
- ? Open/Closed: nieuwe observers zonder subject te wijzigen
- ? Runtime toevoegen/verwijderen van observers
- ? Broadcast communicatie

## Nadelen
- ? Observers worden in willekeurige volgorde genotificeerd
- ? Memory leaks als observers niet correct unsubscribed worden
- ? Kan cascading updates veroorzaken
- ? Performance bij veel observers

## C# Built-in Support

| Mechanisme | Gebruik |
|------------|---------|
| `event` keyword | Standaard observer pattern |
| `IObservable<T>` | Push-based notifications |
| `INotifyPropertyChanged` | WPF/MVVM data binding |
| `Action<T>` delegate | Eenvoudige callbacks |

## Gerelateerde Patterns
- **Mediator**: Centraliseert communicatie (vs directe subject-observer)
- **Singleton**: Subject is vaak een Singleton
- **Command**: Commands kunnen observers notificeren na executie
