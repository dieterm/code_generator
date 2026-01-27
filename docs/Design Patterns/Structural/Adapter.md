# Adapter Pattern

## Intentie
Converteert de interface van een class naar een andere interface die clients verwachten. Adapter laat classes samenwerken die anders niet zouden kunnen vanwege **incompatibele interfaces**.

## Wanneer gebruiken?
- Wanneer je een bestaande class wilt gebruiken met een incompatibele interface
- Wanneer je third-party libraries wilt integreren
- Wanneer je legacy code wilt wrappen
- Wanneer je meerdere subclasses wilt creëren met gemeenschappelijke functionaliteit

## Structuur

```
??????????????      ???????????????      ?????????????????
?   Client   ??????>?   Target    ?      ?   Adaptee     ?
??????????????      ?  Interface  ?      ?????????????????
                    ???????????????      ? +SpecificReq()?
                           ?             ?????????????????
                           ?                     ?
                    ???????????????              ?
                    ?   Adapter   ????????????????
                    ???????????????
                    ? +Request()  ?
                    ???????????????
```

## Implementatie in C#

### Object Adapter (Compositie - Aanbevolen)

```csharp
// Target interface - wat de client verwacht
public interface ILogger
{
    void Log(string message);
    void LogError(string message, Exception exception);
    void LogWarning(string message);
}

// Adaptee - bestaande class met andere interface
public class LegacyLogger
{
    public void WriteToFile(string content)
    {
        Console.WriteLine($"[FILE] {content}");
    }

    public void WriteError(string error, string stackTrace)
    {
        Console.WriteLine($"[FILE ERROR] {error}");
        Console.WriteLine($"[STACK] {stackTrace}");
    }
}

// Adapter - maakt LegacyLogger bruikbaar via ILogger interface
public class LegacyLoggerAdapter : ILogger
{
    private readonly LegacyLogger _legacyLogger;

    public LegacyLoggerAdapter(LegacyLogger legacyLogger)
    {
        _legacyLogger = legacyLogger;
    }

    public void Log(string message)
    {
        _legacyLogger.WriteToFile($"[INFO] {DateTime.Now}: {message}");
    }

    public void LogError(string message, Exception exception)
    {
        _legacyLogger.WriteError(message, exception.StackTrace ?? "No stack trace");
    }

    public void LogWarning(string message)
    {
        _legacyLogger.WriteToFile($"[WARN] {DateTime.Now}: {message}");
    }
}

// Client code - werkt alleen met ILogger
public class OrderService
{
    private readonly ILogger _logger;

    public OrderService(ILogger logger)
    {
        _logger = logger;
    }

    public void ProcessOrder(int orderId)
    {
        _logger.Log($"Processing order {orderId}");
        // Business logic...
        _logger.Log($"Order {orderId} processed successfully");
    }
}

// Gebruik
var legacyLogger = new LegacyLogger();
ILogger logger = new LegacyLoggerAdapter(legacyLogger);
var service = new OrderService(logger);
service.ProcessOrder(123);
```

### Class Adapter (Inheritance)

```csharp
// Adaptee
public class XmlDataReader
{
    public string ReadXml(string path)
    {
        return $"<data><item>Content from {path}</item></data>";
    }
}

// Target interface
public interface IDataReader
{
    string Read(string source);
    Dictionary<string, object> ReadAsDictionary(string source);
}

// Class Adapter - erft van Adaptee en implementeert Target
public class XmlDataReaderAdapter : XmlDataReader, IDataReader
{
    public string Read(string source)
    {
        return ReadXml(source);
    }

    public Dictionary<string, object> ReadAsDictionary(string source)
    {
        var xml = ReadXml(source);
        // Parse XML to dictionary (simplified)
        return new Dictionary<string, object>
        {
            ["source"] = source,
            ["content"] = xml
        };
    }
}
```

### Praktisch Voorbeeld: Payment Gateway Adapter

```csharp
// Target interface - our unified payment interface
public interface IPaymentProcessor
{
    PaymentResult Process(decimal amount, string currency, CardDetails card);
    bool Refund(string transactionId, decimal amount);
    PaymentStatus GetStatus(string transactionId);
}

public record CardDetails(string Number, string Expiry, string Cvv, string HolderName);
public record PaymentResult(bool Success, string TransactionId, string Message);
public enum PaymentStatus { Pending, Completed, Failed, Refunded }

// Adaptee 1: Stripe-like API
public class StripeClient
{
    public StripeCharge CreateCharge(int amountCents, string curr, StripeCardToken token)
    {
        Console.WriteLine($"Stripe: Charging {amountCents} cents in {curr}");
        return new StripeCharge { Id = $"ch_{Guid.NewGuid():N}", Status = "succeeded" };
    }

    public StripeRefund CreateRefund(string chargeId, int amountCents)
    {
        Console.WriteLine($"Stripe: Refunding {amountCents} cents for charge {chargeId}");
        return new StripeRefund { Id = $"re_{Guid.NewGuid():N}", Status = "succeeded" };
    }

    public StripeCardToken CreateToken(string number, string exp, string cvc)
    {
        return new StripeCardToken { Id = $"tok_{Guid.NewGuid():N}" };
    }
}

public class StripeCharge { public string Id { get; set; } public string Status { get; set; } }
public class StripeRefund { public string Id { get; set; } public string Status { get; set; } }
public class StripeCardToken { public string Id { get; set; } }

// Adaptee 2: PayPal-like API
public class PayPalClient
{
    public PayPalPayment CreatePayment(PayPalAmount amount, PayPalCreditCard card)
    {
        Console.WriteLine($"PayPal: Creating payment of {amount.Value} {amount.Currency}");
        return new PayPalPayment 
        { 
            PaymentId = $"PAY-{Guid.NewGuid():N}", 
            State = "approved" 
        };
    }

    public PayPalRefund RefundPayment(string paymentId, PayPalAmount amount)
    {
        Console.WriteLine($"PayPal: Refunding payment {paymentId}");
        return new PayPalRefund { RefundId = $"REF-{Guid.NewGuid():N}", State = "completed" };
    }
}

public class PayPalAmount { public string Value { get; set; } public string Currency { get; set; } }
public class PayPalCreditCard { public string Number { get; set; } public string ExpireMonth { get; set; } public string ExpireYear { get; set; } public string Cvv2 { get; set; } }
public class PayPalPayment { public string PaymentId { get; set; } public string State { get; set; } }
public class PayPalRefund { public string RefundId { get; set; } public string State { get; set; } }

// Adapter for Stripe
public class StripePaymentAdapter : IPaymentProcessor
{
    private readonly StripeClient _stripe;

    public StripePaymentAdapter(StripeClient stripe)
    {
        _stripe = stripe;
    }

    public PaymentResult Process(decimal amount, string currency, CardDetails card)
    {
        var token = _stripe.CreateToken(card.Number, card.Expiry, card.Cvv);
        var amountCents = (int)(amount * 100);
        
        var charge = _stripe.CreateCharge(amountCents, currency.ToLower(), token);
        
        return new PaymentResult(
            Success: charge.Status == "succeeded",
            TransactionId: charge.Id,
            Message: charge.Status
        );
    }

    public bool Refund(string transactionId, decimal amount)
    {
        var amountCents = (int)(amount * 100);
        var refund = _stripe.CreateRefund(transactionId, amountCents);
        return refund.Status == "succeeded";
    }

    public PaymentStatus GetStatus(string transactionId)
    {
        // Simplified
        return PaymentStatus.Completed;
    }
}

// Adapter for PayPal
public class PayPalPaymentAdapter : IPaymentProcessor
{
    private readonly PayPalClient _paypal;

    public PayPalPaymentAdapter(PayPalClient paypal)
    {
        _paypal = paypal;
    }

    public PaymentResult Process(decimal amount, string currency, CardDetails card)
    {
        var ppAmount = new PayPalAmount 
        { 
            Value = amount.ToString("F2"), 
            Currency = currency.ToUpper() 
        };
        
        var parts = card.Expiry.Split('/');
        var ppCard = new PayPalCreditCard 
        { 
            Number = card.Number,
            ExpireMonth = parts[0],
            ExpireYear = "20" + parts[1],
            Cvv2 = card.Cvv
        };

        var payment = _paypal.CreatePayment(ppAmount, ppCard);
        
        return new PaymentResult(
            Success: payment.State == "approved",
            TransactionId: payment.PaymentId,
            Message: payment.State
        );
    }

    public bool Refund(string transactionId, decimal amount)
    {
        var ppAmount = new PayPalAmount { Value = amount.ToString("F2") };
        var refund = _paypal.RefundPayment(transactionId, ppAmount);
        return refund.State == "completed";
    }

    public PaymentStatus GetStatus(string transactionId)
    {
        // Simplified
        return PaymentStatus.Completed;
    }
}

// Client code - payment provider agnostic
public class CheckoutService
{
    private readonly IPaymentProcessor _paymentProcessor;

    public CheckoutService(IPaymentProcessor paymentProcessor)
    {
        _paymentProcessor = paymentProcessor;
    }

    public bool Checkout(decimal total, CardDetails card)
    {
        var result = _paymentProcessor.Process(total, "EUR", card);
        
        if (result.Success)
        {
            Console.WriteLine($"Payment successful: {result.TransactionId}");
        }
        
        return result.Success;
    }
}

// Gebruik met DI
services.AddSingleton<StripeClient>();
services.AddSingleton<IPaymentProcessor, StripePaymentAdapter>();
services.AddScoped<CheckoutService>();

// Of runtime switchen
IPaymentProcessor GetProcessor(string provider)
{
    return provider switch
    {
        "stripe" => new StripePaymentAdapter(new StripeClient()),
        "paypal" => new PayPalPaymentAdapter(new PayPalClient()),
        _ => throw new ArgumentException($"Unknown provider: {provider}")
    };
}
```

## Two-Way Adapter

```csharp
// Beide interfaces adapteren naar elkaar
public interface IModernCache
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan expiry);
}

public interface ILegacyCache
{
    object Get(string key);
    void Set(string key, object value, int ttlSeconds);
}

public class TwoWayCacheAdapter : IModernCache, ILegacyCache
{
    private readonly Dictionary<string, (object Value, DateTime Expiry)> _cache = new();

    // IModernCache implementation
    public Task<T?> GetAsync<T>(string key)
    {
        if (_cache.TryGetValue(key, out var entry) && entry.Expiry > DateTime.UtcNow)
        {
            return Task.FromResult((T?)entry.Value);
        }
        return Task.FromResult(default(T?));
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiry)
    {
        _cache[key] = (value!, DateTime.UtcNow + expiry);
        return Task.CompletedTask;
    }

    // ILegacyCache implementation
    public object Get(string key)
    {
        if (_cache.TryGetValue(key, out var entry) && entry.Expiry > DateTime.UtcNow)
        {
            return entry.Value;
        }
        return null!;
    }

    public void Set(string key, object value, int ttlSeconds)
    {
        _cache[key] = (value, DateTime.UtcNow.AddSeconds(ttlSeconds));
    }
}
```

## Voordelen
- ? Single Responsibility: interface conversie gescheiden van business logic
- ? Open/Closed: nieuwe adapters toevoegen zonder bestaande code te wijzigen
- ? Loose coupling tussen client en concrete implementaties
- ? Maakt legacy code herbruikbaar

## Nadelen
- ? Verhoogt complexiteit door extra classes
- ? Soms is het eenvoudiger om de Adaptee aan te passen

## Adapter vs Facade vs Decorator

| Pattern | Doel | Interface |
|---------|------|-----------|
| **Adapter** | Interface compatibiliteit | Converteert naar andere interface |
| **Facade** | Vereenvoudiging | Nieuwe, eenvoudigere interface |
| **Decorator** | Functionaliteit toevoegen | Zelfde interface |

## Gerelateerde Patterns
- **Bridge**: Scheidt interface van implementatie (design-time)
- **Decorator**: Wraps object met zelfde interface
- **Proxy**: Zelfde interface, controle over toegang
