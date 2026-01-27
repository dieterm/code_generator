# Cross-Cutting Concerns

**Cross-cutting concerns** zijn aspecten van een applicatie die **meerdere lagen of modules doorkruisen** en niet netjes in één enkele module of laag passen. Ze "snijden dwars door" de normale architectuurlagen heen.

## Overzicht

```
???????????????????????????????????????????????????????????????????????????????
?                           APPLICATION LAYERS                                 ?
???????????????????????????????????????????????????????????????????????????????
? Presentation? Application ?   Domain    ?Infrastructure?     Database       ?
?             ?             ?             ?              ?                    ?
?  ?????????  ?  ?????????  ?  ?????????  ?  ?????????   ?                    ?
?  ? View  ?  ?  ?Service?  ?  ?Entity ?  ?  ? Repo  ?   ?                    ?
?  ?????????  ?  ?????????  ?  ?????????  ?  ?????????   ?                    ?
?             ?             ?             ?              ?                    ?
???????????????????????????????????????????????????????????????????????????????
?                                                                              ?
?  ??????????????????????????? LOGGING ???????????????????????????????????    ?
?  ??????????????????????????? SECURITY / AUTHENTICATION ?????????????????    ?
?  ??????????????????????????? EXCEPTION HANDLING ????????????????????????    ?
?  ??????????????????????????? CACHING ???????????????????????????????????    ?
?  ??????????????????????????? VALIDATION ????????????????????????????????    ?
?  ??????????????????????????? TRANSACTION MANAGEMENT ????????????????????    ?
?                                                                              ?
?                         CROSS-CUTTING CONCERNS                               ?
????????????????????????????????????????????????????????????????????????????????
```

## Categorieën

| Categorie | Concerns |
|-----------|----------|
| **Observability** | Logging, Tracing, Metrics, Health Checks, Auditing |
| **Security** | Authentication, Authorization, Encryption, CORS, Input Sanitization |
| **Resilience** | Error Handling, Retry, Circuit Breaker, Timeout |
| **Performance** | Caching, Compression, Rate Limiting, Connection Pooling |
| **Data** | Validation, Transactions, Mapping |
| **Infrastructure** | Configuration, DI, Messaging, Scheduling |
| **UX** | Localization, Globalization |

## Gedetailleerde Concerns

### 1. Logging & Monitoring

| Concern | Beschrijving | .NET Implementatie |
|---------|--------------|-------------------|
| **Logging** | Applicatie events vastleggen | `ILogger<T>`, Serilog, NLog |
| **Tracing** | Request flow volgen | OpenTelemetry, Application Insights |
| **Metrics** | Performance meten | Prometheus, App Insights |
| **Health Checks** | Systeem gezondheid | `IHealthCheck` |
| **Auditing** | Wie deed wat wanneer | Custom audit trail |

```csharp
// Logging via DI
public class ProductService
{
    private readonly ILogger<ProductService> _logger;

    public ProductService(ILogger<ProductService> logger)
    {
        _logger = logger;
    }

    public async Task<Product> CreateAsync(CreateProductRequest request)
    {
        _logger.LogInformation("Creating product: {ProductName}", request.Name);
        
        // Business logic...
        
        _logger.LogInformation("Product created: {ProductId}", product.Id);
        return product;
    }
}
```

### 2. Security

| Concern | Beschrijving | .NET Implementatie |
|---------|--------------|-------------------|
| **Authentication** | Wie is de gebruiker? | ASP.NET Identity, JWT, OAuth |
| **Authorization** | Wat mag de gebruiker? | `[Authorize]`, Policies, Claims |
| **Encryption** | Data versleuteling | Data Protection API |
| **Input Sanitization** | XSS/Injection voorkomen | Input validation, encoding |
| **CORS** | Cross-origin beveiliging | `UseCors()` |

```csharp
// Authorization via attribute
[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    [Authorize(Roles = "SuperAdmin")]
    public IActionResult DeleteUser(int id) 
    { 
        // Only SuperAdmin can access
    }
}

// Custom authorization handler
public class ProductOwnerHandler : AuthorizationHandler<ProductOwnerRequirement, Product>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProductOwnerRequirement requirement,
        Product resource)
    {
        var userId = context.User.FindFirst("UserId")?.Value;
        
        if (userId == resource.OwnerId.ToString())
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
```

### 3. Error Handling & Exception Management

| Concern | Beschrijving | .NET Implementatie |
|---------|--------------|-------------------|
| **Global Exception Handling** | Centrale foutafhandeling | Middleware, Filters |
| **Error Logging** | Fouten vastleggen | Serilog, Seq |
| **User-Friendly Errors** | Foutmeldingen voor gebruiker | ProblemDetails |
| **Retry Logic** | Automatisch opnieuw proberen | Polly |
| **Circuit Breaker** | Systeem beschermen | Polly |

```csharp
// Global exception handling middleware
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = "An error occurred" });
        }
    }
}

// Polly retry policy
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(3, retryAttempt => 
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
```

### 4. Caching

| Concern | Beschrijving | .NET Implementatie |
|---------|--------------|-------------------|
| **In-Memory Cache** | Lokale cache | `IMemoryCache` |
| **Distributed Cache** | Gedeelde cache | `IDistributedCache`, Redis |
| **Output Caching** | Response caching | `[ResponseCache]` |
| **Query Caching** | Database query cache | EF Core Second Level Cache |

```csharp
public class CachedProductService : IProductService
{
    private readonly IProductService _inner;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedProductService> _logger;

    public CachedProductService(
        IProductService inner, 
        IMemoryCache cache,
        ILogger<CachedProductService> logger)
    {
        _inner = inner;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var cacheKey = $"product_{id}";
        
        if (_cache.TryGetValue(cacheKey, out Product? product))
        {
            _logger.LogDebug("Cache hit for {CacheKey}", cacheKey);
            return product;
        }

        _logger.LogDebug("Cache miss for {CacheKey}", cacheKey);
        product = await _inner.GetByIdAsync(id, ct);
        
        if (product != null)
        {
            var options = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                
            _cache.Set(cacheKey, product, options);
        }
        
        return product;
    }

    public async Task InvalidateCacheAsync(int productId)
    {
        _cache.Remove($"product_{productId}");
    }
}
```

### 5. Validation

| Concern | Beschrijving | .NET Implementatie |
|---------|--------------|-------------------|
| **Input Validation** | Request data valideren | FluentValidation, DataAnnotations |
| **Business Rule Validation** | Domain regels | Domain services, Specifications |
| **Model State Validation** | MVC model validatie | `ModelState.IsValid` |

```csharp
// FluentValidation
public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator(IProductRepository productRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters")
            .MustAsync(async (name, ct) => !await productRepository.ExistsByNameAsync(name, ct))
            .WithMessage("Product with this name already exists");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required");
    }
}

// Pipeline behavior voor automatische validatie (MediatR)
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
```

### 6. Transaction Management

| Concern | Beschrijving | .NET Implementatie |
|---------|--------------|-------------------|
| **Database Transactions** | ACID compliance | `DbContext.SaveChanges()` |
| **Distributed Transactions** | Meerdere systemen | Saga pattern, Outbox pattern |
| **Unit of Work** | Atomic operations | `IUnitOfWork` |

```csharp
// Unit of Work pattern
public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext context, IProductRepository products, IOrderRepository orders)
    {
        _context = context;
        Products = products;
        Orders = orders;
    }

    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
        await _transaction?.CommitAsync(ct)!;
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        await _transaction?.RollbackAsync(ct)!;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
```

### 7. Configuration & Settings

| Concern | Beschrijving | .NET Implementatie |
|---------|--------------|-------------------|
| **App Settings** | Configuratie waarden | `IConfiguration`, `IOptions<T>` |
| **Environment Config** | Per-omgeving settings | appsettings.{env}.json |
| **Secrets Management** | Gevoelige data | User Secrets, Key Vault |
| **Feature Flags** | Feature toggles | Microsoft.FeatureManagement |

```csharp
// Strongly typed configuration
public class EmailSettings
{
    public const string SectionName = "Email";
    
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = true;
}

// Registration in Program.cs
services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));

// Usage via IOptions
public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        _logger.LogInformation("Sending email to {To} via {SmtpHost}", to, _settings.SmtpHost);
        // Send email...
    }
}

// Feature flags
public class ProductController : Controller
{
    private readonly IFeatureManager _featureManager;

    public async Task<IActionResult> Index()
    {
        if (await _featureManager.IsEnabledAsync("NewProductUI"))
        {
            return View("IndexV2");
        }
        return View();
    }
}
```

### 8. Localization & Internationalization

| Concern | Beschrijving | .NET Implementatie |
|---------|--------------|-------------------|
| **String Localization** | Vertaalde teksten | `IStringLocalizer<T>` |
| **Culture Settings** | Datum/nummer formaat | `CultureInfo` |
| **Resource Files** | Vertalingen opslaan | .resx files |

```csharp
// Setup in Program.cs
services.AddLocalization(options => options.ResourcesPath = "Resources");

services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "nl-NL", "en-US", "de-DE" };
    options.SetDefaultCulture("nl-NL")
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

// Usage
public class ProductController : Controller
{
    private readonly IStringLocalizer<ProductController> _localizer;

    public ProductController(IStringLocalizer<ProductController> localizer)
    {
        _localizer = localizer;
    }

    public IActionResult Create()
    {
        ViewData["Title"] = _localizer["CreateProduct"];
        ViewData["SaveButton"] = _localizer["Save"];
        return View();
    }

    [HttpPost]
    public IActionResult Create(ProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = _localizer["ValidationFailed"];
            return View(model);
        }
        
        TempData["Success"] = _localizer["ProductCreated"];
        return RedirectToAction(nameof(Index));
    }
}
```

### 9. Performance & Optimization

| Concern | Beschrijving | .NET Implementatie |
|---------|--------------|-------------------|
| **Response Compression** | Data compressie | `UseResponseCompression()` |
| **Rate Limiting** | Request throttling | `UseRateLimiter()` |
| **Connection Pooling** | Database connecties | EF Core, Dapper |
| **Async Operations** | Non-blocking I/O | `async/await` |

```csharp
// Rate limiting in .NET 8
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100;
        opt.QueueLimit = 10;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.AddSlidingWindowLimiter("sliding", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.SegmentsPerWindow = 6;
        opt.PermitLimit = 100;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Too many requests", token);
    };
});

// Usage
[EnableRateLimiting("fixed")]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [DisableRateLimiting] // Override for specific endpoint
    [HttpGet("health")]
    public IActionResult Health() => Ok();
}
```

### 10. Dependency Injection & IoC

| Concern | Beschrijving | .NET Implementatie |
|---------|--------------|-------------------|
| **Service Registration** | Dependencies registreren | `IServiceCollection` |
| **Lifetime Management** | Singleton, Scoped, Transient | `AddSingleton`, `AddScoped` |
| **Factory Patterns** | Dynamische creatie | `IServiceProvider` |

```csharp
// Service registration met extension methods
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Scoped - per request
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        
        // Singleton - één instance voor hele applicatie
        services.AddSingleton<ICacheService, MemoryCacheService>();
        
        // Transient - nieuwe instance per injectie
        services.AddTransient<IEmailService, SmtpEmailService>();
        
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

// In Program.cs
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);
```

### 11. Messaging & Events

| Concern | Beschrijving | .NET Implementatie |
|---------|--------------|-------------------|
| **Domain Events** | Interne events | MediatR `INotification` |
| **Integration Events** | Externe events | RabbitMQ, Azure Service Bus |
| **Event Sourcing** | Event log | EventStore, Marten |

```csharp
// Domain event
public record OrderCreatedEvent(Guid OrderId, Guid CustomerId, decimal TotalAmount) : INotification;

// Event handler
public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(IEmailService emailService, ILogger<OrderCreatedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order created: {OrderId}", notification.OrderId);
        
        await _emailService.SendOrderConfirmationAsync(
            notification.OrderId, 
            notification.CustomerId);
    }
}

// Publishing events
public class OrderService
{
    private readonly IMediator _mediator;

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = new Order(request.CustomerId, request.Items);
        
        // Save order...
        
        // Publish domain event
        await _mediator.Publish(new OrderCreatedEvent(order.Id, order.CustomerId, order.Total));
        
        return order;
    }
}
```

### 12. Timing & Scheduling

| Concern | Beschrijving | .NET Implementatie |
|---------|--------------|-------------------|
| **Background Jobs** | Async verwerking | Hangfire, Quartz.NET |
| **Scheduled Tasks** | Geplande taken | `IHostedService`, Cron jobs |
| **Time Provider** | Testbare tijd | `TimeProvider`, `IDateTimeProvider` |

```csharp
// Abstracted time for testability (.NET 8)
public class OrderService
{
    private readonly TimeProvider _timeProvider;

    public OrderService(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public Order CreateOrder()
    {
        return new Order
        {
            CreatedAt = _timeProvider.GetUtcNow().DateTime
        };
    }
}

// Registration
services.AddSingleton(TimeProvider.System);

// In tests
var fakeTime = new FakeTimeProvider(new DateTimeOffset(2024, 1, 15, 10, 0, 0, TimeSpan.Zero));
var service = new OrderService(fakeTime);

// Background service
public class OrderCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderCleanupService> _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Running order cleanup job");
            
            using var scope = _scopeFactory.CreateScope();
            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
            
            await orderService.CleanupExpiredOrdersAsync(stoppingToken);
            
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
```

## Implementatie Technieken

### 1. Middleware Pipeline (ASP.NET Core)

```csharp
// Program.cs - Middleware volgorde is belangrijk!
var app = builder.Build();

// Cross-cutting concerns als middleware
app.UseMiddleware<RequestLoggingMiddleware>();      // Logging
app.UseMiddleware<ExceptionHandlingMiddleware>();   // Error handling
app.UseMiddleware<CorrelationIdMiddleware>();       // Tracing

app.UseResponseCompression();                        // Performance
app.UseRateLimiter();                               // Rate limiting

app.UseAuthentication();                            // Security
app.UseAuthorization();                             // Security

app.UseMiddleware<PerformanceMiddleware>();         // Metrics

app.MapControllers();
```

### 2. Pipeline Behaviors (MediatR)

```csharp
// Registration
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

// Logging behavior
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("Handling {RequestName}: {@Request}", requestName, request);
        
        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();
        
        _logger.LogInformation(
            "Handled {RequestName} in {ElapsedMs}ms", 
            requestName, 
            stopwatch.ElapsedMilliseconds);
        
        return response;
    }
}
```

### 3. Decorator Pattern

```csharp
// Base service
services.AddScoped<ProductService>();

// Wrap with decorators
services.AddScoped<IProductService>(sp =>
{
    var inner = sp.GetRequiredService<ProductService>();
    var cache = sp.GetRequiredService<IMemoryCache>();
    var logger = sp.GetRequiredService<ILogger<CachedProductService>>();
    
    return new CachedProductService(inner, cache, logger);
});

// Of met Scrutor library
services.AddScoped<IProductService, ProductService>();
services.Decorate<IProductService, CachedProductService>();
services.Decorate<IProductService, LoggingProductService>();
```

### 4. AOP met Interceptors

```csharp
// Met Castle.DynamicProxy of vergelijkbaar
public class LoggingInterceptor : IInterceptor
{
    private readonly ILogger _logger;

    public void Intercept(IInvocation invocation)
    {
        _logger.LogInformation("Calling {Method}", invocation.Method.Name);
        
        invocation.Proceed();
        
        _logger.LogInformation("Called {Method}", invocation.Method.Name);
    }
}
```

## CodeGenerator Project Mapping

In jouw CodeGenerator workspace zijn de volgende cross-cutting concerns herkenbaar:

| Concern | Project(s) | Beschrijving |
|---------|-----------|--------------|
| **Configuration/Settings** | `CodeGenerator.Core.Settings`, `CodeGenerator.Core.Settings.Views` | Applicatie configuratie |
| **Messaging/Events** | `CodeGenerator.Core.MessageBus` | Event-based communicatie |
| **Domain Model** | `CodeGenerator.Domain`, `CodeGenerator.Core.DomainSchema` | Gedeeld domein model |
| **Shared Utilities** | `CodeGenerator.Shared` | Gedeelde hulpfuncties |
| **UI Components** | `CodeGenerator.UserControls`, `CodeGenerator.UserControls.Views` | Herbruikbare UI elementen |
| **Templates** | `CodeGenerator.Core.Templates` | Template management |
| **Artifacts** | `CodeGenerator.Core.Artifacts` | Output artifact handling |

## Best Practices

### ? Do's

1. **Centraliseer cross-cutting concerns** - Één plek voor logging, error handling, etc.
2. **Gebruik abstracties** - Interfaces voor testbaarheid
3. **Configureer via DI** - Niet hardcoden
4. **Houd concerns gescheiden** - Eén verantwoordelijkheid per component
5. **Documenteer de pipeline** - Middleware/behavior volgorde is belangrijk

### ? Don'ts

1. **Verspreid logging calls overal** - Gebruik pipeline behaviors
2. **Hardcode configuratie** - Gebruik IOptions/IConfiguration
3. **Mix concerns** - Caching in business logic
4. **Negeer volgorde** - Middleware volgorde maakt uit
5. **Dupliceer code** - Maak herbruikbare components

## Zie Ook

- [Clean Architecture](../Software%20Architecture/CleanArchitecture.md) - Waar cross-cutting concerns passen
- [Microkernel Architecture](../Architectural%20Patterns/Microkernel.md) - Plugin-based extensibility
- [Design Patterns](../Design%20Patterns/README.md) - Decorator, Strategy voor implementatie
