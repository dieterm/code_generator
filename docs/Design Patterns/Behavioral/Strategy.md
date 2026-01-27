# Strategy Pattern

## Intentie
Definieert een familie van algoritmes, encapsuleert elk ervan, en maakt ze **uitwisselbaar**. Strategy laat het algoritme variëren onafhankelijk van clients die het gebruiken.

## Wanneer gebruiken?
- Wanneer je meerdere algoritmes hebt voor een specifieke taak
- Wanneer je algoritme selectie at runtime wilt maken
- Wanneer je veel conditional statements hebt voor algoritme keuze
- Wanneer je algoritmes wilt isoleren van de code die ze gebruikt

## Structuur

```
?????????????????       ???????????????????
?    Context    ???????>?    Strategy     ?
?????????????????       ???????????????????
? -strategy     ?       ? +Execute()      ?
? +SetStrategy()?       ???????????????????
? +DoSomething()?               ?
?????????????????        ???????????????
                         ?      ?      ?
                   ????????? ??????? ?????????
                   ?StratA ? ?StrB ? ? StratC?
                   ????????? ??????? ?????????
```

## Implementatie in C#

### Basis Implementatie

```csharp
// Strategy interface
public interface IPaymentStrategy
{
    bool Pay(decimal amount);
    string Name { get; }
}

// Concrete Strategies
public class CreditCardPayment : IPaymentStrategy
{
    private readonly string _cardNumber;
    private readonly string _cvv;
    private readonly string _expiryDate;

    public CreditCardPayment(string cardNumber, string cvv, string expiryDate)
    {
        _cardNumber = cardNumber;
        _cvv = cvv;
        _expiryDate = expiryDate;
    }

    public string Name => "Credit Card";

    public bool Pay(decimal amount)
    {
        Console.WriteLine($"Paying €{amount} with Credit Card ending in {_cardNumber[^4..]}");
        // Process credit card payment
        return true;
    }
}

public class PayPalPayment : IPaymentStrategy
{
    private readonly string _email;

    public PayPalPayment(string email)
    {
        _email = email;
    }

    public string Name => "PayPal";

    public bool Pay(decimal amount)
    {
        Console.WriteLine($"Paying €{amount} via PayPal ({_email})");
        // Process PayPal payment
        return true;
    }
}

public class BankTransferPayment : IPaymentStrategy
{
    private readonly string _iban;

    public BankTransferPayment(string iban)
    {
        _iban = iban;
    }

    public string Name => "Bank Transfer";

    public bool Pay(decimal amount)
    {
        Console.WriteLine($"Paying €{amount} via Bank Transfer to {_iban}");
        // Process bank transfer
        return true;
    }
}

public class CryptoPayment : IPaymentStrategy
{
    private readonly string _walletAddress;
    private readonly string _currency;

    public CryptoPayment(string walletAddress, string currency = "BTC")
    {
        _walletAddress = walletAddress;
        _currency = currency;
    }

    public string Name => $"Crypto ({_currency})";

    public bool Pay(decimal amount)
    {
        Console.WriteLine($"Paying €{amount} in {_currency} to wallet {_walletAddress}");
        return true;
    }
}

// Context
public class ShoppingCart
{
    private readonly List<(string Product, decimal Price)> _items = new();
    private IPaymentStrategy? _paymentStrategy;

    public void AddItem(string product, decimal price)
    {
        _items.Add((product, price));
    }

    public void SetPaymentStrategy(IPaymentStrategy strategy)
    {
        _paymentStrategy = strategy;
    }

    public decimal GetTotal() => _items.Sum(i => i.Price);

    public bool Checkout()
    {
        if (_paymentStrategy == null)
            throw new InvalidOperationException("Payment strategy not set");

        var total = GetTotal();
        Console.WriteLine($"Total: €{total}");
        Console.WriteLine($"Using payment method: {_paymentStrategy.Name}");
        
        return _paymentStrategy.Pay(total);
    }
}

// Gebruik
var cart = new ShoppingCart();
cart.AddItem("Laptop", 999.99m);
cart.AddItem("Mouse", 29.99m);

// Runtime strategy selectie
cart.SetPaymentStrategy(new CreditCardPayment("1234567890123456", "123", "12/25"));
cart.Checkout();

// Wissel naar andere strategy
cart.SetPaymentStrategy(new PayPalPayment("user@example.com"));
cart.Checkout();
```

### Met Dependency Injection

```csharp
// Strategy factory
public interface IPaymentStrategyFactory
{
    IPaymentStrategy Create(PaymentMethod method, PaymentDetails details);
}

public enum PaymentMethod
{
    CreditCard,
    PayPal,
    BankTransfer,
    Crypto
}

public record PaymentDetails(
    string? CardNumber = null,
    string? Cvv = null,
    string? ExpiryDate = null,
    string? Email = null,
    string? Iban = null,
    string? WalletAddress = null,
    string? CryptoCurrency = null
);

public class PaymentStrategyFactory : IPaymentStrategyFactory
{
    public IPaymentStrategy Create(PaymentMethod method, PaymentDetails details)
    {
        return method switch
        {
            PaymentMethod.CreditCard => new CreditCardPayment(
                details.CardNumber!, 
                details.Cvv!, 
                details.ExpiryDate!),
            PaymentMethod.PayPal => new PayPalPayment(details.Email!),
            PaymentMethod.BankTransfer => new BankTransferPayment(details.Iban!),
            PaymentMethod.Crypto => new CryptoPayment(
                details.WalletAddress!, 
                details.CryptoCurrency ?? "BTC"),
            _ => throw new ArgumentException($"Unknown payment method: {method}")
        };
    }
}

// DI Registration
services.AddSingleton<IPaymentStrategyFactory, PaymentStrategyFactory>();

// Usage in service
public class CheckoutService
{
    private readonly IPaymentStrategyFactory _paymentFactory;

    public CheckoutService(IPaymentStrategyFactory paymentFactory)
    {
        _paymentFactory = paymentFactory;
    }

    public bool ProcessPayment(PaymentMethod method, PaymentDetails details, decimal amount)
    {
        var strategy = _paymentFactory.Create(method, details);
        return strategy.Pay(amount);
    }
}
```

### Praktisch Voorbeeld: Sorting Strategies

```csharp
public interface ISortStrategy<T>
{
    IEnumerable<T> Sort(IEnumerable<T> items);
    string Name { get; }
}

public class QuickSortStrategy<T> : ISortStrategy<T> where T : IComparable<T>
{
    public string Name => "QuickSort";

    public IEnumerable<T> Sort(IEnumerable<T> items)
    {
        Console.WriteLine("Sorting with QuickSort...");
        var list = items.ToList();
        QuickSort(list, 0, list.Count - 1);
        return list;
    }

    private void QuickSort(List<T> list, int low, int high)
    {
        if (low < high)
        {
            int pi = Partition(list, low, high);
            QuickSort(list, low, pi - 1);
            QuickSort(list, pi + 1, high);
        }
    }

    private int Partition(List<T> list, int low, int high)
    {
        T pivot = list[high];
        int i = low - 1;
        
        for (int j = low; j < high; j++)
        {
            if (list[j].CompareTo(pivot) < 0)
            {
                i++;
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
        
        (list[i + 1], list[high]) = (list[high], list[i + 1]);
        return i + 1;
    }
}

public class MergeSortStrategy<T> : ISortStrategy<T> where T : IComparable<T>
{
    public string Name => "MergeSort";

    public IEnumerable<T> Sort(IEnumerable<T> items)
    {
        Console.WriteLine("Sorting with MergeSort...");
        return MergeSort(items.ToList());
    }

    private List<T> MergeSort(List<T> list)
    {
        if (list.Count <= 1) return list;

        int mid = list.Count / 2;
        var left = MergeSort(list.Take(mid).ToList());
        var right = MergeSort(list.Skip(mid).ToList());

        return Merge(left, right);
    }

    private List<T> Merge(List<T> left, List<T> right)
    {
        var result = new List<T>();
        int i = 0, j = 0;

        while (i < left.Count && j < right.Count)
        {
            if (left[i].CompareTo(right[j]) <= 0)
                result.Add(left[i++]);
            else
                result.Add(right[j++]);
        }

        result.AddRange(left.Skip(i));
        result.AddRange(right.Skip(j));
        return result;
    }
}

public class BubbleSortStrategy<T> : ISortStrategy<T> where T : IComparable<T>
{
    public string Name => "BubbleSort";

    public IEnumerable<T> Sort(IEnumerable<T> items)
    {
        Console.WriteLine("Sorting with BubbleSort...");
        var list = items.ToList();
        
        for (int i = 0; i < list.Count - 1; i++)
        {
            for (int j = 0; j < list.Count - i - 1; j++)
            {
                if (list[j].CompareTo(list[j + 1]) > 0)
                {
                    (list[j], list[j + 1]) = (list[j + 1], list[j]);
                }
            }
        }
        
        return list;
    }
}

// Context met automatische strategy selectie
public class Sorter<T> where T : IComparable<T>
{
    private ISortStrategy<T> _strategy;

    public Sorter(ISortStrategy<T>? strategy = null)
    {
        _strategy = strategy ?? new QuickSortStrategy<T>();
    }

    public void SetStrategy(ISortStrategy<T> strategy)
    {
        _strategy = strategy;
    }

    // Auto-select based on data size
    public void AutoSelectStrategy(int dataSize)
    {
        _strategy = dataSize switch
        {
            < 10 => new BubbleSortStrategy<T>(),      // Small data: simple is fine
            < 1000 => new QuickSortStrategy<T>(),     // Medium: quicksort
            _ => new MergeSortStrategy<T>()           // Large: stable mergesort
        };
        Console.WriteLine($"Auto-selected: {_strategy.Name} for {dataSize} items");
    }

    public IEnumerable<T> Sort(IEnumerable<T> items)
    {
        return _strategy.Sort(items);
    }
}

// Gebruik
var sorter = new Sorter<int>();

var smallData = new[] { 5, 2, 8, 1, 9 };
sorter.AutoSelectStrategy(smallData.Length);
var sortedSmall = sorter.Sort(smallData);

var largeData = Enumerable.Range(0, 10000).OrderBy(_ => Random.Shared.Next()).ToArray();
sorter.AutoSelectStrategy(largeData.Length);
var sortedLarge = sorter.Sort(largeData);
```

### Met Delegates (Modern C# Alternatief)

```csharp
// Strategy via Func<T>
public class DiscountCalculator
{
    private Func<decimal, decimal> _discountStrategy;

    public DiscountCalculator()
    {
        _discountStrategy = amount => amount; // No discount
    }

    public void SetDiscountStrategy(Func<decimal, decimal> strategy)
    {
        _discountStrategy = strategy;
    }

    public decimal CalculateFinalPrice(decimal originalPrice)
    {
        return _discountStrategy(originalPrice);
    }
}

// Predefined strategies
public static class DiscountStrategies
{
    public static Func<decimal, decimal> NoDiscount => amount => amount;
    public static Func<decimal, decimal> Percent10 => amount => amount * 0.90m;
    public static Func<decimal, decimal> Percent20 => amount => amount * 0.80m;
    public static Func<decimal, decimal> Fixed5Euro => amount => Math.Max(0, amount - 5);
    public static Func<decimal, decimal> BuyOneGetOneHalf => amount => amount * 0.75m;
    
    public static Func<decimal, decimal> CreatePercentDiscount(int percent) 
        => amount => amount * (100 - percent) / 100;
}

// Gebruik
var calculator = new DiscountCalculator();

calculator.SetDiscountStrategy(DiscountStrategies.Percent20);
Console.WriteLine(calculator.CalculateFinalPrice(100)); // 80

calculator.SetDiscountStrategy(DiscountStrategies.CreatePercentDiscount(15));
Console.WriteLine(calculator.CalculateFinalPrice(100)); // 85

// Lambda voor custom logic
calculator.SetDiscountStrategy(amount => 
    amount > 100 ? amount * 0.85m : amount * 0.95m);
Console.WriteLine(calculator.CalculateFinalPrice(150)); // 127.5
```

## Voordelen
- ? Open/Closed Principle: nieuwe strategies zonder bestaande code te wijzigen
- ? Algoritmes uitwisselbaar at runtime
- ? Isoleert algoritme implementatie details
- ? Elimineert conditional statements

## Nadelen
- ? Clients moeten strategies kennen om te kiezen
- ? Kan overkill zijn voor weinig algoritmes
- ? Verhoogt aantal objecten

## Strategy vs State

| Aspect | Strategy | State |
|--------|----------|-------|
| **Doel** | Algoritme kiezen | Gedrag op basis van state |
| **Wie kiest** | Client | Object zelf |
| **Wisselen** | Expliciet door client | Automatisch door state transitions |

## Modern C# Alternatieven

- **Func<T> delegates**: Voor eenvoudige strategies
- **Lambda expressions**: Voor inline strategies
- **Pattern matching**: Voor strategy selectie

## Gerelateerde Patterns
- **State**: Vergelijkbare structuur, ander doel
- **Command**: Encapsuleert een request
- **Template Method**: Definieert algoritme skelet, subclasses vullen in
