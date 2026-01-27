# Facade Pattern

## Intentie
Biedt een **vereenvoudigde interface** naar een complex subsysteem. Facade definieert een higher-level interface die het subsysteem makkelijker te gebruiken maakt.

## Wanneer gebruiken?
- Wanneer je een eenvoudige interface wilt voor een complex subsysteem
- Wanneer je een subsysteem wilt ontkoppelen van clients
- Wanneer je layers wilt definiëren in je architectuur
- Wanneer je legacy code wilt wrappen

## Structuur

```
????????????????????????????????????????
?              Facade                  ?
????????????????????????????????????????
? + SimpleOperation()                  ?
? + AnotherOperation()                 ?
????????????????????????????????????????
           ?
           ?
???????????????????????????????????????????????????
?              Subsystem                          ?
?  ???????????  ???????????  ???????????         ?
?  ? Class A ?  ? Class B ?  ? Class C ?  ...    ?
?  ???????????  ???????????  ???????????         ?
???????????????????????????????????????????????????
```

## Implementatie in C#

### Basis Implementatie: Home Theater

```csharp
// Complex subsystem classes
public class Amplifier
{
    public void On() => Console.WriteLine("Amplifier on");
    public void Off() => Console.WriteLine("Amplifier off");
    public void SetVolume(int level) => Console.WriteLine($"Amplifier volume set to {level}");
    public void SetStereoSound() => Console.WriteLine("Amplifier stereo mode on");
    public void SetSurroundSound() => Console.WriteLine("Amplifier surround mode on");
}

public class DvdPlayer
{
    public void On() => Console.WriteLine("DVD Player on");
    public void Off() => Console.WriteLine("DVD Player off");
    public void Play(string movie) => Console.WriteLine($"Playing movie: {movie}");
    public void Stop() => Console.WriteLine("DVD Player stopped");
    public void Eject() => Console.WriteLine("DVD ejected");
}

public class Projector
{
    public void On() => Console.WriteLine("Projector on");
    public void Off() => Console.WriteLine("Projector off");
    public void WideScreenMode() => Console.WriteLine("Projector in widescreen mode");
}

public class TheaterLights
{
    public void On() => Console.WriteLine("Lights on");
    public void Off() => Console.WriteLine("Lights off");
    public void Dim(int level) => Console.WriteLine($"Lights dimmed to {level}%");
}

public class Screen
{
    public void Down() => Console.WriteLine("Screen going down");
    public void Up() => Console.WriteLine("Screen going up");
}

public class PopcornPopper
{
    public void On() => Console.WriteLine("Popcorn Popper on");
    public void Off() => Console.WriteLine("Popcorn Popper off");
    public void Pop() => Console.WriteLine("Popping popcorn!");
}

// Facade - simplifies the complex subsystem
public class HomeTheaterFacade
{
    private readonly Amplifier _amp;
    private readonly DvdPlayer _dvd;
    private readonly Projector _projector;
    private readonly TheaterLights _lights;
    private readonly Screen _screen;
    private readonly PopcornPopper _popper;

    public HomeTheaterFacade(
        Amplifier amp,
        DvdPlayer dvd,
        Projector projector,
        TheaterLights lights,
        Screen screen,
        PopcornPopper popper)
    {
        _amp = amp;
        _dvd = dvd;
        _projector = projector;
        _lights = lights;
        _screen = screen;
        _popper = popper;
    }

    // Simple method that orchestrates many subsystem calls
    public void WatchMovie(string movie)
    {
        Console.WriteLine("Get ready to watch a movie...");
        
        _popper.On();
        _popper.Pop();
        _lights.Dim(10);
        _screen.Down();
        _projector.On();
        _projector.WideScreenMode();
        _amp.On();
        _amp.SetSurroundSound();
        _amp.SetVolume(5);
        _dvd.On();
        _dvd.Play(movie);
        
        Console.WriteLine("Enjoy your movie!");
    }

    public void EndMovie()
    {
        Console.WriteLine("Shutting down movie theater...");
        
        _popper.Off();
        _lights.On();
        _screen.Up();
        _projector.Off();
        _amp.Off();
        _dvd.Stop();
        _dvd.Eject();
        _dvd.Off();
        
        Console.WriteLine("Movie theater is off");
    }

    public void ListenToMusic(string album)
    {
        Console.WriteLine("Get ready to listen to music...");
        
        _lights.Dim(50);
        _amp.On();
        _amp.SetStereoSound();
        _amp.SetVolume(4);
        
        Console.WriteLine($"Playing album: {album}");
    }
}

// Gebruik - client hoeft subsystem niet te kennen
var facade = new HomeTheaterFacade(
    new Amplifier(),
    new DvdPlayer(),
    new Projector(),
    new TheaterLights(),
    new Screen(),
    new PopcornPopper()
);

facade.WatchMovie("The Matrix");
// ... later
facade.EndMovie();
```

### Praktisch Voorbeeld: Order Processing Facade

```csharp
// Subsystem classes
public class InventoryService
{
    public bool CheckStock(string productId, int quantity)
    {
        Console.WriteLine($"Checking stock for {productId}");
        return true; // Simplified
    }

    public void ReserveStock(string productId, int quantity)
    {
        Console.WriteLine($"Reserved {quantity} of {productId}");
    }

    public void ReleaseStock(string productId, int quantity)
    {
        Console.WriteLine($"Released {quantity} of {productId}");
    }
}

public class PaymentService
{
    public bool ValidateCard(string cardNumber)
    {
        Console.WriteLine("Validating card...");
        return true;
    }

    public string ProcessPayment(decimal amount, string cardNumber)
    {
        Console.WriteLine($"Processing payment of €{amount}");
        return $"TXN-{Guid.NewGuid():N}";
    }

    public void Refund(string transactionId, decimal amount)
    {
        Console.WriteLine($"Refunding €{amount} for {transactionId}");
    }
}

public class ShippingService
{
    public decimal CalculateShippingCost(string address, decimal weight)
    {
        Console.WriteLine("Calculating shipping cost...");
        return 5.99m;
    }

    public string CreateShipment(string orderId, string address)
    {
        Console.WriteLine($"Creating shipment for order {orderId}");
        return $"SHIP-{Guid.NewGuid():N}";
    }

    public string GetTrackingUrl(string shipmentId)
    {
        return $"https://tracking.example.com/{shipmentId}";
    }
}

public class NotificationService
{
    public void SendOrderConfirmation(string email, string orderId)
    {
        Console.WriteLine($"Sending order confirmation to {email}");
    }

    public void SendShippingNotification(string email, string trackingUrl)
    {
        Console.WriteLine($"Sending shipping notification to {email}");
    }

    public void SendPaymentFailedNotification(string email)
    {
        Console.WriteLine($"Sending payment failed notification to {email}");
    }
}

public class FraudDetectionService
{
    public bool CheckForFraud(string customerId, decimal amount)
    {
        Console.WriteLine("Checking for fraud...");
        return false; // No fraud detected
    }
}

// DTOs
public class OrderRequest
{
    public string CustomerId { get; set; }
    public string Email { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public string ShippingAddress { get; set; }
    public string CardNumber { get; set; }
}

public class OrderItem
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class OrderResult
{
    public bool Success { get; set; }
    public string? OrderId { get; set; }
    public string? TrackingNumber { get; set; }
    public string? ErrorMessage { get; set; }
}

// Facade - hides complexity of order processing
public class OrderFacade
{
    private readonly InventoryService _inventory;
    private readonly PaymentService _payment;
    private readonly ShippingService _shipping;
    private readonly NotificationService _notification;
    private readonly FraudDetectionService _fraudDetection;

    public OrderFacade(
        InventoryService inventory,
        PaymentService payment,
        ShippingService shipping,
        NotificationService notification,
        FraudDetectionService fraudDetection)
    {
        _inventory = inventory;
        _payment = payment;
        _shipping = shipping;
        _notification = notification;
        _fraudDetection = fraudDetection;
    }

    public OrderResult PlaceOrder(OrderRequest request)
    {
        var orderId = $"ORD-{Guid.NewGuid():N}";
        Console.WriteLine($"Starting order process: {orderId}");

        try
        {
            // 1. Check inventory
            foreach (var item in request.Items)
            {
                if (!_inventory.CheckStock(item.ProductId, item.Quantity))
                {
                    return new OrderResult
                    {
                        Success = false,
                        ErrorMessage = $"Product {item.ProductId} is out of stock"
                    };
                }
            }

            // 2. Calculate total
            var subtotal = request.Items.Sum(i => i.Quantity * i.UnitPrice);
            var shippingCost = _shipping.CalculateShippingCost(request.ShippingAddress, 1.0m);
            var total = subtotal + shippingCost;

            // 3. Fraud check
            if (_fraudDetection.CheckForFraud(request.CustomerId, total))
            {
                return new OrderResult
                {
                    Success = false,
                    ErrorMessage = "Order flagged for fraud review"
                };
            }

            // 4. Validate payment method
            if (!_payment.ValidateCard(request.CardNumber))
            {
                _notification.SendPaymentFailedNotification(request.Email);
                return new OrderResult
                {
                    Success = false,
                    ErrorMessage = "Invalid payment method"
                };
            }

            // 5. Reserve inventory
            foreach (var item in request.Items)
            {
                _inventory.ReserveStock(item.ProductId, item.Quantity);
            }

            // 6. Process payment
            var transactionId = _payment.ProcessPayment(total, request.CardNumber);

            // 7. Create shipment
            var shipmentId = _shipping.CreateShipment(orderId, request.ShippingAddress);
            var trackingUrl = _shipping.GetTrackingUrl(shipmentId);

            // 8. Send notifications
            _notification.SendOrderConfirmation(request.Email, orderId);
            _notification.SendShippingNotification(request.Email, trackingUrl);

            Console.WriteLine($"Order {orderId} completed successfully!");

            return new OrderResult
            {
                Success = true,
                OrderId = orderId,
                TrackingNumber = shipmentId
            };
        }
        catch (Exception ex)
        {
            // Rollback: release reserved stock
            foreach (var item in request.Items)
            {
                _inventory.ReleaseStock(item.ProductId, item.Quantity);
            }

            return new OrderResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public void CancelOrder(string orderId)
    {
        Console.WriteLine($"Cancelling order: {orderId}");
        // Simplified cancellation logic
    }
}

// Client - clean and simple
var facade = new OrderFacade(
    new InventoryService(),
    new PaymentService(),
    new ShippingService(),
    new NotificationService(),
    new FraudDetectionService()
);

var result = facade.PlaceOrder(new OrderRequest
{
    CustomerId = "CUST-123",
    Email = "customer@example.com",
    Items = new List<OrderItem>
    {
        new() { ProductId = "PROD-1", Quantity = 2, UnitPrice = 29.99m },
        new() { ProductId = "PROD-2", Quantity = 1, UnitPrice = 49.99m }
    },
    ShippingAddress = "123 Main St, Amsterdam",
    CardNumber = "4111111111111111"
});

if (result.Success)
{
    Console.WriteLine($"Order placed: {result.OrderId}");
    Console.WriteLine($"Track at: {result.TrackingNumber}");
}
```

### API Facade

```csharp
// Facade for external API
public interface IWeatherFacade
{
    Task<WeatherInfo> GetCurrentWeatherAsync(string city);
    Task<WeatherForecast> GetForecastAsync(string city, int days);
}

public class WeatherFacade : IWeatherFacade
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<WeatherFacade> _logger;

    public WeatherFacade(HttpClient httpClient, IConfiguration config, ILogger<WeatherFacade> logger)
    {
        _httpClient = httpClient;
        _apiKey = config["Weather:ApiKey"];
        _logger = logger;
    }

    public async Task<WeatherInfo> GetCurrentWeatherAsync(string city)
    {
        _logger.LogInformation("Getting weather for {City}", city);

        // Hides complex API details
        var response = await _httpClient.GetAsync(
            $"https://api.weather.com/v1/current?city={city}&key={_apiKey}");
        
        response.EnsureSuccessStatusCode();
        
        var apiResponse = await response.Content.ReadFromJsonAsync<WeatherApiResponse>();
        
        // Transform complex API response to simple domain model
        return new WeatherInfo
        {
            City = city,
            Temperature = apiResponse.Main.Temp,
            Description = apiResponse.Weather.FirstOrDefault()?.Description ?? "Unknown",
            Humidity = apiResponse.Main.Humidity
        };
    }

    public async Task<WeatherForecast> GetForecastAsync(string city, int days)
    {
        // Similar simplification of complex API
        throw new NotImplementedException();
    }
}

// Simple domain models
public record WeatherInfo(string City, double Temperature, string Description, int Humidity);
public record WeatherForecast(string City, List<DayForecast> Days);
public record DayForecast(DateTime Date, double High, double Low, string Description);

// Complex API response (hidden from clients)
internal class WeatherApiResponse
{
    public MainData Main { get; set; }
    public List<WeatherData> Weather { get; set; }
}
internal class MainData { public double Temp { get; set; } public int Humidity { get; set; } }
internal class WeatherData { public string Description { get; set; } }
```

## Voordelen
- ? Vereenvoudigt interface naar complex systeem
- ? Ontkoppelt client van subsystem
- ? Maakt subsystem makkelijker te gebruiken
- ? Kan als entry point dienen voor layered architecture

## Nadelen
- ? Facade kan een "god object" worden
- ? Kan flexibiliteit beperken voor power users

## Facade vs Adapter vs Mediator

| Pattern | Doel |
|---------|------|
| **Facade** | Vereenvoudigt interface naar subsystem |
| **Adapter** | Maakt incompatibele interfaces compatibel |
| **Mediator** | Centraliseert communicatie tussen objecten |

## Gerelateerde Patterns
- **Abstract Factory**: Kan samen met Facade gebruikt worden om subsystem objecten te creëren
- **Singleton**: Facades zijn vaak Singletons
- **Mediator**: Beide abstraheren complexiteit, maar Mediator focust op object communicatie
