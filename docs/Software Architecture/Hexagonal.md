# Hexagonal Architecture (Ports & Adapters)

## Overzicht

Hexagonal Architecture, ontwikkeld door Alistair Cockburn in 2005, isoleert de **applicatie kern** van externe systemen via **Ports** (interfaces) en **Adapters** (implementaties). Dit maakt de applicatie onafhankelijk van databases, UI, en externe services.

## Structuur

```
                              External Systems
                    ???????????????????????????????????????
                    ?                                     ?
        ?????????????????????????       ???????????????????????????????
        ?    Driving Adapters   ?       ?     Driven Adapters         ?
        ?    (Primary)          ?       ?     (Secondary)             ?
        ?                       ?       ?                             ?
        ?  • REST Controller    ?       ?  • Database Repository      ?
        ?  • GraphQL            ?       ?  • Message Queue            ?
        ?  • CLI                ?       ?  • Email Service            ?
        ?  • gRPC               ?       ?  • External API             ?
        ?????????????????????????       ???????????????????????????????
                    ?                                     ?
                    ? uses                       implements?
                    ?                                     ?
        ?????????????????????????       ???????????????????????????????
        ?    Driving Ports      ?       ?      Driven Ports           ?
        ?    (Input)            ?       ?      (Output)               ?
        ?                       ?       ?                             ?
        ?  • IOrderService      ?       ?  • IOrderRepository         ?
        ?  • IProductService    ?       ?  • IEmailSender             ?
        ?                       ?       ?  • IPaymentGateway          ?
        ?????????????????????????       ???????????????????????????????
                    ?                                     ?
                    ???????????????????????????????????????
                                   ?
                                   ?
                    ???????????????????????????????????????
                    ?                                     ?
                    ?         APPLICATION CORE            ?
                    ?                                     ?
                    ?    ?????????????????????????????    ?
                    ?    ?      Domain Model         ?    ?
                    ?    ?   Entities, Value Objects ?    ?
                    ?    ?      Domain Services      ?    ?
                    ?    ?????????????????????????????    ?
                    ?                                     ?
                    ?    ?????????????????????????????    ?
                    ?    ?    Application Services   ?    ?
                    ?    ?       Use Cases           ?    ?
                    ?    ?????????????????????????????    ?
                    ?                                     ?
                    ???????????????????????????????????????
```

## Kernconcepten

### Ports
**Interfaces** die de grenzen van de applicatie definiëren:

| Type | Richting | Voorbeeld |
|------|----------|-----------|
| **Driving Port** (Primary) | Inkomend | `IOrderService`, `IProductService` |
| **Driven Port** (Secondary) | Uitgaand | `IOrderRepository`, `IEmailSender` |

### Adapters
**Implementaties** die de ports verbinden met externe systemen:

| Type | Functie | Voorbeeld |
|------|---------|-----------|
| **Driving Adapter** | Ontvangt requests | REST Controller, CLI, Queue Consumer |
| **Driven Adapter** | Voert externe acties uit | EF Repository, SMTP Client, HTTP Client |

## Project Structuur

```
Solution/
??? src/
?   ??? MyApp.Domain/                           # Domain Model
?   ?   ??? Entities/
?   ?   ??? ValueObjects/
?   ?   ??? Events/
?   ?   ??? Services/
?   ?
?   ??? MyApp.Application/                      # Application Core + Ports
?   ?   ??? Ports/
?   ?   ?   ??? Driving/                        # Input Ports (Interfaces)
?   ?   ?   ?   ??? IOrderService.cs
?   ?   ?   ?   ??? IProductService.cs
?   ?   ?   ??? Driven/                         # Output Ports (Interfaces)
?   ?   ?       ??? IOrderRepository.cs
?   ?   ?       ??? IProductRepository.cs
?   ?   ?       ??? IEmailSender.cs
?   ?   ?       ??? IPaymentGateway.cs
?   ?   ??? Services/                           # Use Cases (implementeert Driving Ports)
?   ?   ?   ??? OrderService.cs
?   ?   ?   ??? ProductService.cs
?   ?   ??? DTOs/
?   ?
?   ??? MyApp.Adapters.Persistence/             # Driven Adapter: Database
?   ?   ??? DbContext/
?   ?   ??? Repositories/
?   ?   ??? Configurations/
?   ?
?   ??? MyApp.Adapters.Email/                   # Driven Adapter: Email
?   ?   ??? SmtpEmailSender.cs
?   ?
?   ??? MyApp.Adapters.Payment/                 # Driven Adapter: Payment
?   ?   ??? StripePaymentGateway.cs
?   ?
?   ??? MyApp.Adapters.Web/                     # Driving Adapter: REST API
?   ?   ??? Controllers/
?   ?   ??? Program.cs
?   ?
?   ??? MyApp.Adapters.CLI/                     # Driving Adapter: CLI
?       ??? Program.cs
?
??? tests/
    ??? MyApp.Domain.Tests/
    ??? MyApp.Application.Tests/
    ??? MyApp.Adapters.Web.Tests/
```

## Implementatie

### Domain Layer

```csharp
// MyApp.Domain/Entities/Order.cs
namespace MyApp.Domain.Entities;

public class Order
{
    private readonly List<OrderLine> _lines = new();

    public OrderId Id { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();
    public OrderStatus Status { get; private set; }
    public Money Total { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }

    private Order() { } // EF

    public static Order Create(CustomerId customerId)
    {
        return new Order
        {
            Id = OrderId.New(),
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            Total = Money.Zero(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AddLine(ProductId productId, string productName, Money unitPrice, int quantity)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Cannot modify a non-pending order");

        if (quantity <= 0)
            throw new DomainException("Quantity must be positive");

        var existingLine = _lines.FirstOrDefault(l => l.ProductId == productId);
        if (existingLine != null)
        {
            existingLine.IncreaseQuantity(quantity);
        }
        else
        {
            _lines.Add(new OrderLine(productId, productName, unitPrice, quantity));
        }

        RecalculateTotal();
    }

    public void RemoveLine(ProductId productId)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Cannot modify a non-pending order");

        var line = _lines.FirstOrDefault(l => l.ProductId == productId);
        if (line != null)
        {
            _lines.Remove(line);
            RecalculateTotal();
        }
    }

    public void Submit()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be submitted");

        if (!_lines.Any())
            throw new DomainException("Cannot submit an empty order");

        Status = OrderStatus.Submitted;
    }

    public void MarkAsPaid(string transactionId)
    {
        if (Status != OrderStatus.Submitted)
            throw new DomainException("Only submitted orders can be marked as paid");

        Status = OrderStatus.Paid;
        PaidAt = DateTime.UtcNow;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Paid)
            throw new DomainException("Only paid orders can be shipped");

        Status = OrderStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped)
            throw new DomainException("Cannot cancel a shipped order");

        Status = OrderStatus.Cancelled;
    }

    private void RecalculateTotal()
    {
        Total = _lines.Aggregate(Money.Zero(), (sum, line) => sum.Add(line.LineTotal));
    }
}

public class OrderLine
{
    public ProductId ProductId { get; private set; }
    public string ProductName { get; private set; }
    public Money UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public Money LineTotal => UnitPrice.Multiply(Quantity);

    private OrderLine() { } // EF

    public OrderLine(ProductId productId, string productName, Money unitPrice, int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new DomainException("Amount must be positive");
        Quantity += amount;
    }
}

public enum OrderStatus
{
    Pending,
    Submitted,
    Paid,
    Shipped,
    Delivered,
    Cancelled
}
```

### Application Layer - Ports

```csharp
// MyApp.Application/Ports/Driving/IOrderService.cs (Input Port)
namespace MyApp.Application.Ports.Driving;

/// <summary>
/// Driving Port: Defines what the application CAN DO
/// Implemented by Application Services
/// Used by Driving Adapters (Controllers, CLI, etc.)
/// </summary>
public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(Guid customerId, CancellationToken ct = default);
    Task<OrderDto?> GetOrderAsync(Guid orderId, CancellationToken ct = default);
    Task<IEnumerable<OrderDto>> GetCustomerOrdersAsync(Guid customerId, CancellationToken ct = default);
    Task AddOrderLineAsync(Guid orderId, AddOrderLineRequest request, CancellationToken ct = default);
    Task RemoveOrderLineAsync(Guid orderId, Guid productId, CancellationToken ct = default);
    Task SubmitOrderAsync(Guid orderId, CancellationToken ct = default);
    Task ProcessPaymentAsync(Guid orderId, PaymentRequest request, CancellationToken ct = default);
    Task CancelOrderAsync(Guid orderId, CancellationToken ct = default);
}

// MyApp.Application/Ports/Driven/IOrderRepository.cs (Output Port)
namespace MyApp.Application.Ports.Driven;

/// <summary>
/// Driven Port: Defines what the application NEEDS
/// Implemented by Driven Adapters (Persistence, etc.)
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(OrderId id, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    Task UpdateAsync(Order order, CancellationToken ct = default);
    Task<bool> ExistsAsync(OrderId id, CancellationToken ct = default);
}

// MyApp.Application/Ports/Driven/IPaymentGateway.cs (Output Port)
namespace MyApp.Application.Ports.Driven;

/// <summary>
/// Driven Port: External payment service
/// </summary>
public interface IPaymentGateway
{
    Task<PaymentResult> ProcessPaymentAsync(
        Money amount, 
        PaymentMethod paymentMethod, 
        CancellationToken ct = default);
    
    Task<RefundResult> RefundPaymentAsync(
        string transactionId, 
        Money amount, 
        CancellationToken ct = default);
}

public record PaymentResult(bool Success, string? TransactionId, string? ErrorMessage);
public record RefundResult(bool Success, string? RefundId, string? ErrorMessage);

// MyApp.Application/Ports/Driven/IEmailSender.cs (Output Port)
namespace MyApp.Application.Ports.Driven;

/// <summary>
/// Driven Port: Email service
/// </summary>
public interface IEmailSender
{
    Task SendOrderConfirmationAsync(Email to, OrderDto order, CancellationToken ct = default);
    Task SendShippingNotificationAsync(Email to, OrderDto order, string trackingNumber, CancellationToken ct = default);
    Task SendOrderCancelledAsync(Email to, OrderDto order, CancellationToken ct = default);
}
```

### Application Layer - Services (Use Cases)

```csharp
// MyApp.Application/Services/OrderService.cs
namespace MyApp.Application.Services;

/// <summary>
/// Application Service: Implements Driving Port
/// Orchestrates use cases using Domain and Driven Ports
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        IPaymentGateway paymentGateway,
        IEmailSender emailSender,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _paymentGateway = paymentGateway;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<OrderDto> CreateOrderAsync(Guid customerId, CancellationToken ct = default)
    {
        var customer = await _customerRepository.GetByIdAsync(CustomerId.From(customerId), ct)
            ?? throw new EntityNotFoundException(nameof(Customer), customerId);

        var order = Order.Create(customer.Id);
        
        await _orderRepository.AddAsync(order, ct);
        
        _logger.LogInformation("Order {OrderId} created for customer {CustomerId}", order.Id, customerId);
        
        return MapToDto(order);
    }

    public async Task AddOrderLineAsync(Guid orderId, AddOrderLineRequest request, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(OrderId.From(orderId), ct)
            ?? throw new EntityNotFoundException(nameof(Order), orderId);

        var product = await _productRepository.GetByIdAsync(ProductId.From(request.ProductId), ct)
            ?? throw new EntityNotFoundException(nameof(Product), request.ProductId);

        // Domain logic: Product checks stock
        if (!product.HasStock(request.Quantity))
            throw new DomainException($"Insufficient stock for product {product.Name}");

        // Domain logic: Order adds line
        order.AddLine(product.Id, product.Name, product.Price, request.Quantity);
        
        await _orderRepository.UpdateAsync(order, ct);
    }

    public async Task SubmitOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(OrderId.From(orderId), ct)
            ?? throw new EntityNotFoundException(nameof(Order), orderId);

        // Reserve stock for all products
        foreach (var line in order.Lines)
        {
            var product = await _productRepository.GetByIdAsync(line.ProductId, ct);
            product?.ReserveStock(line.Quantity);
            if (product != null)
                await _productRepository.UpdateAsync(product, ct);
        }

        // Domain logic: Order submits itself
        order.Submit();
        
        await _orderRepository.UpdateAsync(order, ct);
        
        _logger.LogInformation("Order {OrderId} submitted", orderId);
    }

    public async Task ProcessPaymentAsync(Guid orderId, PaymentRequest request, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(OrderId.From(orderId), ct)
            ?? throw new EntityNotFoundException(nameof(Order), orderId);

        var customer = await _customerRepository.GetByIdAsync(order.CustomerId, ct);

        // Use Driven Port: Payment Gateway
        var paymentResult = await _paymentGateway.ProcessPaymentAsync(
            order.Total,
            request.PaymentMethod,
            ct);

        if (!paymentResult.Success)
        {
            _logger.LogWarning("Payment failed for order {OrderId}: {Error}", orderId, paymentResult.ErrorMessage);
            throw new PaymentException(paymentResult.ErrorMessage ?? "Payment failed");
        }

        // Domain logic: Order marks as paid
        order.MarkAsPaid(paymentResult.TransactionId!);
        
        await _orderRepository.UpdateAsync(order, ct);

        // Use Driven Port: Email Sender
        if (customer != null)
        {
            await _emailSender.SendOrderConfirmationAsync(customer.Email, MapToDto(order), ct);
        }
        
        _logger.LogInformation("Order {OrderId} paid with transaction {TransactionId}", 
            orderId, paymentResult.TransactionId);
    }

    public async Task CancelOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(OrderId.From(orderId), ct)
            ?? throw new EntityNotFoundException(nameof(Order), orderId);

        // Release reserved stock
        foreach (var line in order.Lines)
        {
            var product = await _productRepository.GetByIdAsync(line.ProductId, ct);
            product?.ReleaseStock(line.Quantity);
            if (product != null)
                await _productRepository.UpdateAsync(product, ct);
        }

        // Domain logic
        order.Cancel();
        
        await _orderRepository.UpdateAsync(order, ct);
        
        var customer = await _customerRepository.GetByIdAsync(order.CustomerId, ct);
        if (customer != null)
        {
            await _emailSender.SendOrderCancelledAsync(customer.Email, MapToDto(order), ct);
        }
        
        _logger.LogInformation("Order {OrderId} cancelled", orderId);
    }

    public async Task<OrderDto?> GetOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(OrderId.From(orderId), ct);
        return order is null ? null : MapToDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetCustomerOrdersAsync(Guid customerId, CancellationToken ct = default)
    {
        var orders = await _orderRepository.GetByCustomerIdAsync(CustomerId.From(customerId), ct);
        return orders.Select(MapToDto);
    }

    public async Task RemoveOrderLineAsync(Guid orderId, Guid productId, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(OrderId.From(orderId), ct)
            ?? throw new EntityNotFoundException(nameof(Order), orderId);

        order.RemoveLine(ProductId.From(productId));
        
        await _orderRepository.UpdateAsync(order, ct);
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto(
            order.Id.Value,
            order.CustomerId.Value,
            order.Status.ToString(),
            order.Total.Amount,
            order.Total.Currency,
            order.Lines.Select(l => new OrderLineDto(
                l.ProductId.Value,
                l.ProductName,
                l.UnitPrice.Amount,
                l.Quantity,
                l.LineTotal.Amount
            )).ToList(),
            order.CreatedAt,
            order.PaidAt,
            order.ShippedAt
        );
    }
}
```

### Driven Adapters

```csharp
// MyApp.Adapters.Persistence/Repositories/OrderRepository.cs
namespace MyApp.Adapters.Persistence.Repositories;

/// <summary>
/// Driven Adapter: Implements the repository port
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(OrderId id, CancellationToken ct = default)
    {
        return await _context.Orders
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken ct = default)
    {
        return await _context.Orders
            .Include(o => o.Lines)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        await _context.Orders.AddAsync(order, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsAsync(OrderId id, CancellationToken ct = default)
    {
        return await _context.Orders.AnyAsync(o => o.Id == id, ct);
    }
}

// MyApp.Adapters.Payment/StripePaymentGateway.cs
namespace MyApp.Adapters.Payment;

/// <summary>
/// Driven Adapter: Implements payment gateway port using Stripe
/// </summary>
public class StripePaymentGateway : IPaymentGateway
{
    private readonly IStripeClient _stripeClient;
    private readonly ILogger<StripePaymentGateway> _logger;

    public StripePaymentGateway(IStripeClient stripeClient, ILogger<StripePaymentGateway> logger)
    {
        _stripeClient = stripeClient;
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(
        Money amount, 
        PaymentMethod paymentMethod, 
        CancellationToken ct = default)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount.Amount * 100), // Stripe uses cents
                Currency = amount.Currency.ToLower(),
                PaymentMethod = paymentMethod.Token,
                Confirm = true
            };

            var service = new PaymentIntentService(_stripeClient);
            var paymentIntent = await service.CreateAsync(options, cancellationToken: ct);

            if (paymentIntent.Status == "succeeded")
            {
                _logger.LogInformation("Payment succeeded: {PaymentId}", paymentIntent.Id);
                return new PaymentResult(true, paymentIntent.Id, null);
            }

            return new PaymentResult(false, null, $"Payment status: {paymentIntent.Status}");
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe payment failed");
            return new PaymentResult(false, null, ex.Message);
        }
    }

    public async Task<RefundResult> RefundPaymentAsync(
        string transactionId, 
        Money amount, 
        CancellationToken ct = default)
    {
        try
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = transactionId,
                Amount = (long)(amount.Amount * 100)
            };

            var service = new RefundService(_stripeClient);
            var refund = await service.CreateAsync(options, cancellationToken: ct);

            return new RefundResult(true, refund.Id, null);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe refund failed");
            return new RefundResult(false, null, ex.Message);
        }
    }
}

// MyApp.Adapters.Email/SmtpEmailSender.cs
namespace MyApp.Adapters.Email;

/// <summary>
/// Driven Adapter: Implements email sender port using SMTP
/// </summary>
public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpSettings> settings, ILogger<SmtpEmailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(Email to, OrderDto order, CancellationToken ct = default)
    {
        var subject = $"Order Confirmation - {order.Id}";
        var body = $"""
            Thank you for your order!
            
            Order ID: {order.Id}
            Total: {order.TotalAmount:C} {order.Currency}
            
            Items:
            {string.Join("\n", order.Lines.Select(l => $"- {l.ProductName} x{l.Quantity}: {l.LineTotal:C}"))}
            """;

        await SendEmailAsync(to.Value, subject, body, ct);
    }

    public async Task SendShippingNotificationAsync(Email to, OrderDto order, string trackingNumber, CancellationToken ct = default)
    {
        var subject = $"Your Order Has Shipped - {order.Id}";
        var body = $"""
            Great news! Your order has been shipped!
            
            Order ID: {order.Id}
            Tracking Number: {trackingNumber}
            """;

        await SendEmailAsync(to.Value, subject, body, ct);
    }

    public async Task SendOrderCancelledAsync(Email to, OrderDto order, CancellationToken ct = default)
    {
        var subject = $"Order Cancelled - {order.Id}";
        var body = $"Your order {order.Id} has been cancelled.";

        await SendEmailAsync(to.Value, subject, body, ct);
    }

    private async Task SendEmailAsync(string to, string subject, string body, CancellationToken ct)
    {
        using var client = new SmtpClient(_settings.Host, _settings.Port);
        client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
        client.EnableSsl = _settings.UseSsl;

        var message = new MailMessage(_settings.FromAddress, to, subject, body);
        
        await client.SendMailAsync(message, ct);
        
        _logger.LogInformation("Email sent to {To}: {Subject}", to, subject);
    }
}
```

### Driving Adapters

```csharp
// MyApp.Adapters.Web/Controllers/OrdersController.cs
namespace MyApp.Adapters.Web.Controllers;

/// <summary>
/// Driving Adapter: REST API that uses the OrderService (Driving Port)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken ct)
    {
        var order = await _orderService.CreateOrderAsync(request.CustomerId, ct);
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id, CancellationToken ct)
    {
        var order = await _orderService.GetOrderAsync(id, ct);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetCustomerOrders(
        Guid customerId, 
        CancellationToken ct)
    {
        var orders = await _orderService.GetCustomerOrdersAsync(customerId, ct);
        return Ok(orders);
    }

    [HttpPost("{id:guid}/lines")]
    public async Task<IActionResult> AddOrderLine(
        Guid id,
        [FromBody] AddOrderLineRequest request,
        CancellationToken ct)
    {
        await _orderService.AddOrderLineAsync(id, request, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}/lines/{productId:guid}")]
    public async Task<IActionResult> RemoveOrderLine(
        Guid id, 
        Guid productId, 
        CancellationToken ct)
    {
        await _orderService.RemoveOrderLineAsync(id, productId, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> SubmitOrder(Guid id, CancellationToken ct)
    {
        await _orderService.SubmitOrderAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/pay")]
    public async Task<IActionResult> ProcessPayment(
        Guid id,
        [FromBody] PaymentRequest request,
        CancellationToken ct)
    {
        await _orderService.ProcessPaymentAsync(id, request, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid id, CancellationToken ct)
    {
        await _orderService.CancelOrderAsync(id, ct);
        return NoContent();
    }
}

// MyApp.Adapters.CLI/Program.cs
namespace MyApp.Adapters.CLI;

/// <summary>
/// Driving Adapter: CLI that uses the same OrderService
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Same service registrations as Web
                services.AddApplication();
                services.AddInfrastructure(context.Configuration);
            })
            .Build();

        var orderService = host.Services.GetRequiredService<IOrderService>();

        Console.WriteLine("Order Management CLI");
        Console.WriteLine("1. Create Order");
        Console.WriteLine("2. View Order");
        Console.WriteLine("3. Cancel Order");
        
        // CLI logic using the same IOrderService
        var choice = Console.ReadLine();
        
        switch (choice)
        {
            case "1":
                Console.Write("Customer ID: ");
                var customerId = Guid.Parse(Console.ReadLine()!);
                var order = await orderService.CreateOrderAsync(customerId);
                Console.WriteLine($"Order created: {order.Id}");
                break;
            // ... more cases
        }
    }
}
```

### Dependency Registration

```csharp
// MyApp.Adapters.Web/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Application (Driving Ports implementation)
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();

// Infrastructure (Driven Ports implementation)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// External Services (Driven Adapters)
builder.Services.AddScoped<IPaymentGateway, StripePaymentGateway>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

var app = builder.Build();
```

## Testing

```csharp
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly Mock<IPaymentGateway> _paymentMock;
    private readonly Mock<IEmailSender> _emailMock;
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _orderRepoMock = new Mock<IOrderRepository>();
        _paymentMock = new Mock<IPaymentGateway>();
        _emailMock = new Mock<IEmailSender>();
        
        _sut = new OrderService(
            _orderRepoMock.Object,
            Mock.Of<IProductRepository>(),
            Mock.Of<ICustomerRepository>(),
            _paymentMock.Object,
            _emailMock.Object,
            Mock.Of<ILogger<OrderService>>()
        );
    }

    [Fact]
    public async Task ProcessPayment_WhenSuccessful_MarksOrderAsPaid()
    {
        // Arrange
        var order = Order.Create(CustomerId.From(Guid.NewGuid()));
        order.AddLine(ProductId.From(Guid.NewGuid()), "Test", Money.Create(10), 1);
        order.Submit();
        
        _orderRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<OrderId>(), default))
            .ReturnsAsync(order);
        
        _paymentMock.Setup(p => p.ProcessPaymentAsync(It.IsAny<Money>(), It.IsAny<PaymentMethod>(), default))
            .ReturnsAsync(new PaymentResult(true, "txn_123", null));

        // Act
        await _sut.ProcessPaymentAsync(order.Id.Value, new PaymentRequest("tok_visa"), default);

        // Assert
        Assert.Equal(OrderStatus.Paid, order.Status);
        _emailMock.Verify(e => e.SendOrderConfirmationAsync(It.IsAny<Email>(), It.IsAny<OrderDto>(), default), Times.Once);
    }
}
```

## Voordelen

- ? **Volledig geïsoleerd**: Core is onafhankelijk van alle externe systemen
- ? **Uitstekende testbaarheid**: Ports zijn makkelijk te mocken
- ? **Flexibel**: Adapters zijn uitwisselbaar
- ? **Meerdere entry points**: Zelfde core voor API, CLI, Queue, etc.
- ? **Duidelijke grenzen**: Ports definiëren expliciete contracts

## Nadelen

- ? **Veel abstracties**: Kan overkill zijn voor simpele apps
- ? **Meer projecten**: Meerdere adapter projecten
- ? **Mapping overhead**: Data transformatie tussen lagen

## Wanneer Hexagonal Gebruiken?

| Scenario | Geschikt? |
|----------|-----------|
| Veel externe integraties | ? Ja |
| Meerdere UI's (Web, CLI, Mobile) | ? Ja |
| Microservices | ? Ja |
| Simpele CRUD app | ? Nee |
| Prototype / MVP | ? Nee |
