# Proxy Pattern

## Intentie
Biedt een **plaatsvervanger of placeholder** voor een ander object om toegang tot dat object te controleren.

## Wanneer gebruiken?
- **Lazy initialization** (Virtual Proxy)
- **Access control** (Protection Proxy)
- **Caching** (Caching Proxy)
- **Logging/Auditing** (Logging Proxy)
- **Remote service calls** (Remote Proxy)

## Structuur

```
???????????????????
?    Subject      ?
???????????????????
? +Request()      ?
???????????????????
        ?
   ???????????
   ?         ?
?????????? ??????????????
?RealSubj? ?   Proxy    ?????
?????????? ??????????????   ?
           ?-realSubject?????
           ?+Request()  ?
           ??????????????
```

## Implementatie in C#

### Virtual Proxy (Lazy Loading)

```csharp
public interface IImage
{
    void Display();
    int Width { get; }
    int Height { get; }
}

// Real Subject - expensive to create
public class HighResolutionImage : IImage
{
    private readonly string _filename;
    private byte[] _imageData;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public HighResolutionImage(string filename)
    {
        _filename = filename;
        LoadFromDisk(); // Expensive operation
    }

    private void LoadFromDisk()
    {
        Console.WriteLine($"Loading image from disk: {_filename}");
        Thread.Sleep(2000); // Simulate slow loading
        
        // Simulate loading image data
        _imageData = new byte[1024 * 1024]; // 1MB
        Width = 1920;
        Height = 1080;
        
        Console.WriteLine($"Image loaded: {Width}x{Height}");
    }

    public void Display()
    {
        Console.WriteLine($"Displaying image: {_filename} ({Width}x{Height})");
    }
}

// Proxy - lazy loads the real image
public class ImageProxy : IImage
{
    private readonly string _filename;
    private HighResolutionImage? _realImage;
    private int? _cachedWidth;
    private int? _cachedHeight;

    public ImageProxy(string filename)
    {
        _filename = filename;
        // Only load metadata, not full image
        LoadMetadata();
    }

    private void LoadMetadata()
    {
        // Quick metadata read without loading full image
        Console.WriteLine($"Loading metadata for: {_filename}");
        _cachedWidth = 1920;
        _cachedHeight = 1080;
    }

    public int Width => _cachedWidth ?? RealImage.Width;
    public int Height => _cachedHeight ?? RealImage.Height;

    private HighResolutionImage RealImage
    {
        get
        {
            if (_realImage == null)
            {
                Console.WriteLine("Creating real image on first access...");
                _realImage = new HighResolutionImage(_filename);
            }
            return _realImage;
        }
    }

    public void Display()
    {
        RealImage.Display();
    }
}

// Gebruik
Console.WriteLine("Creating image proxies (fast)...");
var images = new List<IImage>
{
    new ImageProxy("photo1.jpg"),
    new ImageProxy("photo2.jpg"),
    new ImageProxy("photo3.jpg")
};

Console.WriteLine($"\nImage count: {images.Count}");
Console.WriteLine($"First image dimensions: {images[0].Width}x{images[0].Height}");

Console.WriteLine("\nDisplaying first image (triggers load)...");
images[0].Display();

Console.WriteLine("\nOther images still not loaded!");
```

### Protection Proxy (Access Control)

```csharp
public interface IDocument
{
    string Content { get; }
    void Edit(string newContent);
    void Delete();
}

public class SensitiveDocument : IDocument
{
    public string Content { get; private set; }

    public SensitiveDocument(string content)
    {
        Content = content;
    }

    public void Edit(string newContent)
    {
        Content = newContent;
        Console.WriteLine("Document edited.");
    }

    public void Delete()
    {
        Content = string.Empty;
        Console.WriteLine("Document deleted.");
    }
}

public class User
{
    public string Username { get; }
    public UserRole Role { get; }

    public User(string username, UserRole role)
    {
        Username = username;
        Role = role;
    }
}

public enum UserRole
{
    Viewer,
    Editor,
    Admin
}

// Protection Proxy
public class DocumentProxy : IDocument
{
    private readonly SensitiveDocument _document;
    private readonly User _currentUser;

    public DocumentProxy(SensitiveDocument document, User currentUser)
    {
        _document = document;
        _currentUser = currentUser;
    }

    public string Content
    {
        get
        {
            LogAccess("Read");
            return _document.Content;
        }
    }

    public void Edit(string newContent)
    {
        if (_currentUser.Role < UserRole.Editor)
        {
            Console.WriteLine($"Access denied: {_currentUser.Username} cannot edit.");
            throw new UnauthorizedAccessException("Edit permission required.");
        }

        LogAccess("Edit");
        _document.Edit(newContent);
    }

    public void Delete()
    {
        if (_currentUser.Role < UserRole.Admin)
        {
            Console.WriteLine($"Access denied: {_currentUser.Username} cannot delete.");
            throw new UnauthorizedAccessException("Admin permission required.");
        }

        LogAccess("Delete");
        _document.Delete();
    }

    private void LogAccess(string action)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {_currentUser.Username} ({_currentUser.Role}): {action}");
    }
}

// Gebruik
var document = new SensitiveDocument("Confidential data...");

var viewer = new User("John", UserRole.Viewer);
var editor = new User("Jane", UserRole.Editor);
var admin = new User("Boss", UserRole.Admin);

var viewerProxy = new DocumentProxy(document, viewer);
var editorProxy = new DocumentProxy(document, editor);
var adminProxy = new DocumentProxy(document, admin);

// Viewer can read
Console.WriteLine(viewerProxy.Content);

// Viewer cannot edit
try { viewerProxy.Edit("Hacked!"); } 
catch (UnauthorizedAccessException) { }

// Editor can edit
editorProxy.Edit("Updated content");

// Editor cannot delete
try { editorProxy.Delete(); } 
catch (UnauthorizedAccessException) { }

// Admin can delete
adminProxy.Delete();
```

### Caching Proxy

```csharp
public interface IWeatherService
{
    Task<WeatherData> GetWeatherAsync(string city);
}

public record WeatherData(string City, double Temperature, string Condition, DateTime FetchedAt);

// Real service
public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherData> GetWeatherAsync(string city)
    {
        Console.WriteLine($"Fetching weather from API for {city}...");
        await Task.Delay(1000); // Simulate API call
        
        // Simulate API response
        return new WeatherData(city, 22.5, "Sunny", DateTime.UtcNow);
    }
}

// Caching Proxy
public class CachingWeatherProxy : IWeatherService
{
    private readonly IWeatherService _realService;
    private readonly Dictionary<string, (WeatherData Data, DateTime CachedAt)> _cache = new();
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public CachingWeatherProxy(IWeatherService realService)
    {
        _realService = realService;
    }

    public async Task<WeatherData> GetWeatherAsync(string city)
    {
        var normalizedCity = city.ToLower().Trim();

        // Check cache
        if (_cache.TryGetValue(normalizedCity, out var cached))
        {
            if (DateTime.UtcNow - cached.CachedAt < _cacheDuration)
            {
                Console.WriteLine($"Cache hit for {city}");
                return cached.Data;
            }
            Console.WriteLine($"Cache expired for {city}");
        }

        // Fetch from real service
        var data = await _realService.GetWeatherAsync(city);
        
        // Update cache
        _cache[normalizedCity] = (data, DateTime.UtcNow);
        Console.WriteLine($"Cached weather for {city}");

        return data;
    }
}

// Gebruik
IWeatherService weatherService = new CachingWeatherProxy(
    new WeatherService(new HttpClient()));

// First call - fetches from API
var weather1 = await weatherService.GetWeatherAsync("Amsterdam");

// Second call - from cache
var weather2 = await weatherService.GetWeatherAsync("Amsterdam");

// Different city - fetches from API
var weather3 = await weatherService.GetWeatherAsync("Rotterdam");
```

### Logging/Auditing Proxy

```csharp
public interface IPaymentGateway
{
    PaymentResult ProcessPayment(decimal amount, string cardNumber);
    void Refund(string transactionId, decimal amount);
}

public class PaymentGateway : IPaymentGateway
{
    public PaymentResult ProcessPayment(decimal amount, string cardNumber)
    {
        // Process payment
        return new PaymentResult(true, Guid.NewGuid().ToString());
    }

    public void Refund(string transactionId, decimal amount)
    {
        // Process refund
    }
}

public record PaymentResult(bool Success, string TransactionId);

// Logging Proxy
public class LoggingPaymentProxy : IPaymentGateway
{
    private readonly IPaymentGateway _realGateway;
    private readonly ILogger _logger;

    public LoggingPaymentProxy(IPaymentGateway realGateway, ILogger logger)
    {
        _realGateway = realGateway;
        _logger = logger;
    }

    public PaymentResult ProcessPayment(decimal amount, string cardNumber)
    {
        var maskedCard = $"****-****-****-{cardNumber[^4..]}";
        _logger.LogInformation("Payment initiated: Amount={Amount}, Card={Card}", amount, maskedCard);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = _realGateway.ProcessPayment(amount, cardNumber);
            stopwatch.Stop();

            _logger.LogInformation(
                "Payment completed: TransactionId={TransactionId}, Duration={Duration}ms",
                result.TransactionId, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, 
                "Payment failed: Amount={Amount}, Duration={Duration}ms",
                amount, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    public void Refund(string transactionId, decimal amount)
    {
        _logger.LogInformation("Refund initiated: TransactionId={TransactionId}, Amount={Amount}",
            transactionId, amount);

        _realGateway.Refund(transactionId, amount);

        _logger.LogInformation("Refund completed: TransactionId={TransactionId}", transactionId);
    }
}
```

### Remote Proxy

```csharp
public interface ICalculatorService
{
    int Add(int a, int b);
    int Multiply(int a, int b);
}

// Remote Proxy - calls remote service
public class CalculatorServiceProxy : ICalculatorService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public CalculatorServiceProxy(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }

    public int Add(int a, int b)
    {
        var response = _httpClient.GetAsync($"{_baseUrl}/add?a={a}&b={b}").Result;
        response.EnsureSuccessStatusCode();
        return int.Parse(response.Content.ReadAsStringAsync().Result);
    }

    public int Multiply(int a, int b)
    {
        var response = _httpClient.GetAsync($"{_baseUrl}/multiply?a={a}&b={b}").Result;
        response.EnsureSuccessStatusCode();
        return int.Parse(response.Content.ReadAsStringAsync().Result);
    }
}
```

### Generic Dynamic Proxy met DispatchProxy

```csharp
public class LoggingProxy<T> : DispatchProxy where T : class
{
    private T _target;
    private ILogger _logger;

    public static T Create(T target, ILogger logger)
    {
        var proxy = Create<T, LoggingProxy<T>>() as LoggingProxy<T>;
        proxy._target = target;
        proxy._logger = logger;
        return proxy as T;
    }

    protected override object Invoke(MethodInfo targetMethod, object[] args)
    {
        _logger.LogInformation("Calling {Method} with args: {@Args}",
            targetMethod.Name, args);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = targetMethod.Invoke(_target, args);
            stopwatch.Stop();

            _logger.LogInformation("{Method} returned {@Result} in {Duration}ms",
                targetMethod.Name, result, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (TargetInvocationException ex)
        {
            _logger.LogError(ex.InnerException, "{Method} threw exception", targetMethod.Name);
            throw ex.InnerException;
        }
    }
}

// Gebruik
ICalculator calculator = new Calculator();
ICalculator loggedCalculator = LoggingProxy<ICalculator>.Create(calculator, logger);

loggedCalculator.Add(5, 3); // Automatically logged
```

## Soorten Proxies

| Type | Doel | Voorbeeld |
|------|------|-----------|
| **Virtual** | Lazy initialization | Image loading |
| **Protection** | Access control | Document permissions |
| **Caching** | Cache results | API responses |
| **Logging** | Audit trail | Payment logging |
| **Remote** | Remote calls | Web service client |
| **Smart Reference** | Extra actions | Reference counting |

## Voordelen
- ? Controle over object lifecycle
- ? Open/Closed: extra functionaliteit zonder RealSubject te wijzigen
- ? Security: access control transparant
- ? Performance: lazy loading, caching

## Nadelen
- ? Extra indirectie
- ? Kan response time verhogen

## Proxy vs Decorator

| Aspect | Proxy | Decorator |
|--------|-------|-----------|
| **Doel** | Control access | Add behavior |
| **Lifecycle** | Manages object creation | Receives existing object |
| **Interface** | Zelfde interface | Zelfde interface |

## Gerelateerde Patterns
- **Decorator**: Voegt gedrag toe
- **Adapter**: Converteert interface
- **Facade**: Vereenvoudigt interface
