# Clean Architecture

## Overzicht

Clean Architecture, geïntroduceerd door Robert C. Martin (Uncle Bob) in 2012, combineert de beste ideeën van Hexagonal, Onion en andere architecturen. De kern is de **Dependency Rule**: dependencies mogen alleen naar binnen wijzen.

## Structuur

```
???????????????????????????????????????????????????????????????????????????????
?                        Frameworks & Drivers                                  ?
?  (Web, UI, Database, External Services, Devices)                            ?
?                                                                              ?
?    ???????????????????????????????????????????????????????????????????????  ?
?    ?                      Interface Adapters                              ?  ?
?    ?  (Controllers, Presenters, Gateways, Repositories)                  ?  ?
?    ?                                                                      ?  ?
?    ?    ???????????????????????????????????????????????????????????????  ?  ?
?    ?    ?                    Application Business Rules               ?  ?  ?
?    ?    ?  (Use Cases / Interactors)                                  ?  ?  ?
?    ?    ?                                                             ?  ?  ?
?    ?    ?    ???????????????????????????????????????????????????????  ?  ?  ?
?    ?    ?    ?              Enterprise Business Rules              ?  ?  ?  ?
?    ?    ?    ?  (Entities)                                         ?  ?  ?  ?
?    ?    ?    ?                                                     ?  ?  ?  ?
?    ?    ?    ?  • Domain Entities                                  ?  ?  ?  ?
?    ?    ?    ?  • Value Objects                                    ?  ?  ?  ?
?    ?    ?    ?  • Domain Events                                    ?  ?  ?  ?
?    ?    ?    ?  • Business Rules                                   ?  ?  ?  ?
?    ?    ?    ?                                                     ?  ?  ?  ?
?    ?    ?    ???????????????????????????????????????????????????????  ?  ?  ?
?    ?    ?                                                             ?  ?  ?
?    ?    ???????????????????????????????????????????????????????????????  ?  ?
?    ?                                                                      ?  ?
?    ???????????????????????????????????????????????????????????????????????  ?
?                                                                              ?
???????????????????????????????????????????????????????????????????????????????

                    ? ? ? DEPENDENCIES POINT INWARD ? ? ?
```

## The Dependency Rule

> **Source code dependencies must point only inward, toward higher-level policies.**

```
????????????????     ????????????????     ????????????????     ????????????????
?  Frameworks  ? ??> ?   Adapters   ? ??> ?  Use Cases   ? ??> ?   Entities   ?
????????????????     ????????????????     ????????????????     ????????????????

• Entities weten NIETS over Use Cases
• Use Cases weten NIETS over Adapters
• Adapters weten NIETS over Frameworks
```

## Lagen

| Laag | Verantwoordelijkheid | Bevat |
|------|---------------------|-------|
| **Entities** | Enterprise Business Rules | Domain Entities, Value Objects, Domain Services |
| **Use Cases** | Application Business Rules | Interactors, Input/Output Ports, DTOs |
| **Interface Adapters** | Data conversie | Controllers, Presenters, Gateways, Repositories |
| **Frameworks & Drivers** | Details | Database, Web Framework, UI, External APIs |

## Project Structuur

```
Solution/
??? src/
?   ??? MyApp.Domain/                           # Entities (innermost)
?   ?   ??? Entities/
?   ?   ?   ??? Product.cs
?   ?   ?   ??? Order.cs
?   ?   ?   ??? Customer.cs
?   ?   ??? ValueObjects/
?   ?   ?   ??? Money.cs
?   ?   ?   ??? Email.cs
?   ?   ?   ??? Address.cs
?   ?   ??? Enums/
?   ?   ??? Events/
?   ?   ?   ??? OrderCreatedEvent.cs
?   ?   ??? Exceptions/
?   ?   ?   ??? DomainException.cs
?   ?   ??? Common/
?   ?       ??? Entity.cs
?   ?       ??? ValueObject.cs
?   ?
?   ??? MyApp.Application/                      # Use Cases
?   ?   ??? Common/
?   ?   ?   ??? Interfaces/
?   ?   ?   ?   ??? IRepository.cs
?   ?   ?   ?   ??? IUnitOfWork.cs
?   ?   ?   ?   ??? IDateTimeProvider.cs
?   ?   ?   ??? Behaviors/
?   ?   ?   ?   ??? ValidationBehavior.cs
?   ?   ?   ?   ??? LoggingBehavior.cs
?   ?   ?   ??? Exceptions/
?   ?   ?       ??? ValidationException.cs
?   ?   ?
?   ?   ??? Products/
?   ?   ?   ??? Commands/
?   ?   ?   ?   ??? CreateProduct/
?   ?   ?   ?   ?   ??? CreateProductCommand.cs
?   ?   ?   ?   ?   ??? CreateProductCommandHandler.cs
?   ?   ?   ?   ?   ??? CreateProductCommandValidator.cs
?   ?   ?   ?   ??? UpdateProduct/
?   ?   ?   ?       ??? ...
?   ?   ?   ??? Queries/
?   ?   ?   ?   ??? GetProducts/
?   ?   ?   ?   ?   ??? GetProductsQuery.cs
?   ?   ?   ?   ?   ??? GetProductsQueryHandler.cs
?   ?   ?   ?   ?   ??? ProductDto.cs
?   ?   ?   ?   ??? GetProductById/
?   ?   ?   ?       ??? ...
?   ?   ?   ??? EventHandlers/
?   ?   ?       ??? ProductCreatedEventHandler.cs
?   ?   ?
?   ?   ??? Orders/
?   ?       ??? Commands/
?   ?       ??? Queries/
?   ?       ??? EventHandlers/
?   ?
?   ??? MyApp.Infrastructure/                   # Interface Adapters + Frameworks
?   ?   ??? Persistence/
?   ?   ?   ??? AppDbContext.cs
?   ?   ?   ??? Repositories/
?   ?   ?   ?   ??? ProductRepository.cs
?   ?   ?   ?   ??? OrderRepository.cs
?   ?   ?   ??? Configurations/
?   ?   ?   ??? Migrations/
?   ?   ??? Services/
?   ?   ?   ??? DateTimeProvider.cs
?   ?   ?   ??? EmailService.cs
?   ?   ??? DependencyInjection.cs
?   ?
?   ??? MyApp.API/                              # Frameworks & Drivers
?       ??? Controllers/
?       ?   ??? ProductsController.cs
?       ?   ??? OrdersController.cs
?       ??? Middleware/
?       ?   ??? ExceptionHandlingMiddleware.cs
?       ??? Program.cs
?
??? tests/
    ??? MyApp.Domain.Tests/
    ??? MyApp.Application.Tests/
    ??? MyApp.Infrastructure.Tests/
    ??? MyApp.API.Tests/
```

## Implementatie met CQRS + MediatR

### Domain Layer (Entities)

```csharp
// MyApp.Domain/Common/Entity.cs
namespace MyApp.Domain.Common;

public abstract class Entity<TId> where TId : notnull
{
    public TId Id { get; protected set; } = default!;
    
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;
        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => Id.GetHashCode();
}

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

// MyApp.Domain/Entities/Product.cs
namespace MyApp.Domain.Entities;

public class Product : Entity<ProductId>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Money Price { get; private set; } = null!;
    public int StockQuantity { get; private set; }
    public CategoryId CategoryId { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Product() { } // EF

    public static Product Create(
        string name,
        string description,
        Money price,
        int stockQuantity,
        CategoryId categoryId)
    {
        var product = new Product
        {
            Id = ProductId.CreateUnique(),
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name)),
            Description = description ?? string.Empty,
            Price = Guard.Against.Null(price, nameof(price)),
            StockQuantity = Guard.Against.Negative(stockQuantity, nameof(stockQuantity)),
            CategoryId = Guard.Against.Null(categoryId, nameof(categoryId)),
            CreatedAt = DateTime.UtcNow
        };

        product.AddDomainEvent(new ProductCreatedEvent(product.Id, product.Name));
        
        return product;
    }

    public void Update(string name, string description, Money price)
    {
        Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Description = description ?? string.Empty;
        Price = Guard.Against.Null(price, nameof(price));
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProductUpdatedEvent(Id, Name));
    }

    public void AddStock(int quantity)
    {
        Guard.Against.NegativeOrZero(quantity, nameof(quantity));
        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveStock(int quantity)
    {
        Guard.Against.NegativeOrZero(quantity, nameof(quantity));
        
        if (quantity > StockQuantity)
            throw new DomainException($"Cannot remove {quantity} items. Only {StockQuantity} in stock.");
        
        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;

        if (StockQuantity == 0)
        {
            AddDomainEvent(new ProductOutOfStockEvent(Id));
        }
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}

// MyApp.Domain/Events/ProductCreatedEvent.cs
namespace MyApp.Domain.Events;

public record ProductCreatedEvent(ProductId ProductId, string ProductName) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ProductUpdatedEvent(ProductId ProductId, string ProductName) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record ProductOutOfStockEvent(ProductId ProductId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
```

### Application Layer (Use Cases)

```csharp
// MyApp.Application/Common/Interfaces/IRepository.cs
namespace MyApp.Application.Common.Interfaces;

public interface IRepository<T, TId> where T : Entity<TId> where TId : notnull
{
    Task<T?> GetByIdAsync(TId id, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
}

public interface IProductRepository : IRepository<Product, ProductId>
{
    Task<IEnumerable<Product>> GetByCategoryAsync(CategoryId categoryId, CancellationToken ct = default);
    Task<IEnumerable<Product>> SearchAsync(string searchTerm, CancellationToken ct = default);
    Task<bool> ExistsAsync(ProductId id, CancellationToken ct = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

// MyApp.Application/Products/Commands/CreateProduct/CreateProductCommand.cs
namespace MyApp.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    Guid CategoryId
) : IRequest<Guid>;

// MyApp.Application/Products/Commands/CreateProduct/CreateProductCommandValidator.cs
namespace MyApp.Application.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be 3 characters");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required");
    }
}

// MyApp.Application/Products/Commands/CreateProduct/CreateProductCommandHandler.cs
namespace MyApp.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Verify category exists
        var categoryId = CategoryId.Create(request.CategoryId);
        var categoryExists = await _categoryRepository.ExistsAsync(categoryId, cancellationToken);
        
        if (!categoryExists)
        {
            throw new NotFoundException(nameof(Category), request.CategoryId);
        }

        // Create domain entity
        var price = Money.Create(request.Price, request.Currency);
        var product = Product.Create(
            request.Name,
            request.Description,
            price,
            request.StockQuantity,
            categoryId
        );

        // Persist
        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product created: {ProductId} - {ProductName}", product.Id, product.Name);

        return product.Id.Value;
    }
}

// MyApp.Application/Products/Queries/GetProducts/GetProductsQuery.cs
namespace MyApp.Application.Products.Queries.GetProducts;

public record GetProductsQuery(
    string? SearchTerm = null,
    Guid? CategoryId = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<PaginatedList<ProductDto>>;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    string StockStatus,
    Guid CategoryId,
    string? CategoryName,
    bool IsActive,
    DateTime CreatedAt
);

// MyApp.Application/Products/Queries/GetProducts/GetProductsQueryHandler.cs
namespace MyApp.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedList<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<ProductDto>> Handle(
        GetProductsQuery request, 
        CancellationToken cancellationToken)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p => 
                p.Name.Contains(request.SearchTerm) || 
                p.Description.Contains(request.SearchTerm));
        }

        if (request.CategoryId.HasValue)
        {
            var categoryId = CategoryId.Create(request.CategoryId.Value);
            query = query.Where(p => p.CategoryId == categoryId);
        }

        // Project to DTO and paginate
        var products = await query
            .OrderBy(p => p.Name)
            .Select(p => new ProductDto(
                p.Id.Value,
                p.Name,
                p.Description,
                p.Price.Amount,
                p.Price.Currency,
                p.StockQuantity,
                p.StockQuantity > 10 ? "In Stock" : p.StockQuantity > 0 ? "Low Stock" : "Out of Stock",
                p.CategoryId.Value,
                p.Category != null ? p.Category.Name : null,
                p.IsActive,
                p.CreatedAt
            ))
            .ToPaginatedListAsync(request.Page, request.PageSize, cancellationToken);

        return products;
    }
}

// MyApp.Application/Products/EventHandlers/ProductCreatedEventHandler.cs
namespace MyApp.Application.Products.EventHandlers;

public class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedEventHandler> _logger;
    private readonly IEmailService _emailService;

    public ProductCreatedEventHandler(
        ILogger<ProductCreatedEventHandler> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Domain Event: Product created - {ProductId}: {ProductName}", 
            notification.ProductId, 
            notification.ProductName);

        // Example: Send notification to inventory team
        await _emailService.SendProductCreatedNotificationAsync(
            notification.ProductId.Value,
            notification.ProductName,
            cancellationToken);
    }
}
```

### Pipeline Behaviors

```csharp
// MyApp.Application/Common/Behaviors/ValidationBehavior.cs
namespace MyApp.Application.Common.Behaviors;

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

// MyApp.Application/Common/Behaviors/LoggingBehavior.cs
namespace MyApp.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("Handling {RequestName}", requestName);
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var response = await next();
            
            stopwatch.Stop();
            _logger.LogInformation(
                "Handled {RequestName} in {ElapsedMilliseconds}ms", 
                requestName, 
                stopwatch.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex, 
                "Error handling {RequestName} after {ElapsedMilliseconds}ms", 
                requestName, 
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
```

### Infrastructure Layer

```csharp
// MyApp.Infrastructure/Persistence/AppDbContext.cs
namespace MyApp.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext, IUnitOfWork
{
    private readonly IMediator _mediator;

    public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) 
        : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events before saving
        await DispatchDomainEventsAsync(cancellationToken);
        
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var entities = ChangeTracker
            .Entries<Entity<ProductId>>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}

// MyApp.Infrastructure/Persistence/Repositories/ProductRepository.cs
namespace MyApp.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(ProductId id, CancellationToken ct = default)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Products.ToListAsync(ct);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(CategoryId categoryId, CancellationToken ct = default)
    {
        return await _context.Products.Where(p => p.CategoryId == categoryId).ToListAsync(ct);
    }

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm, CancellationToken ct = default)
    {
        return await _context.Products
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .ToListAsync(ct);
    }

    public async Task AddAsync(Product entity, CancellationToken ct = default)
    {
        await _context.Products.AddAsync(entity, ct);
    }

    public void Update(Product entity)
    {
        _context.Products.Update(entity);
    }

    public void Remove(Product entity)
    {
        _context.Products.Remove(entity);
    }

    public async Task<bool> ExistsAsync(ProductId id, CancellationToken ct = default)
    {
        return await _context.Products.AnyAsync(p => p.Id == id, ct);
    }
}

// MyApp.Infrastructure/DependencyInjection.cs
namespace MyApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider => 
            provider.GetRequiredService<AppDbContext>());
        
        services.AddScoped<IUnitOfWork>(provider => 
            provider.GetRequiredService<AppDbContext>());

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        services.AddTransient<IEmailService, SmtpEmailService>();

        return services;
    }
}
```

### API Layer

```csharp
// MyApp.API/Controllers/ProductsController.cs
namespace MyApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<ProductDto>>> GetProducts(
        [FromQuery] GetProductsQuery query,
        CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id, CancellationToken ct)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateProduct(
        [FromBody] CreateProductCommand command,
        CancellationToken ct)
    {
        var productId = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetProduct), new { id = productId }, productId);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductCommand command,
        CancellationToken ct)
    {
        if (id != command.Id)
        {
            return BadRequest("Route ID must match command ID");
        }

        await _mediator.Send(command, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteProductCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/stock")]
    public async Task<IActionResult> AddStock(
        Guid id,
        [FromBody] AddStockCommand command,
        CancellationToken ct)
    {
        await _mediator.Send(command with { ProductId = id }, ct);
        return NoContent();
    }
}

// MyApp.API/Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(CreateProductCommandValidator).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
```

### Exception Handling

```csharp
// MyApp.API/Middleware/ExceptionHandlingMiddleware.cs
namespace MyApp.API.Middleware;

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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                new ProblemDetails
                {
                    Title = "Validation Error",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "One or more validation errors occurred.",
                    Extensions = { ["errors"] = validationEx.Errors }
                }
            ),
            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                new ProblemDetails
                {
                    Title = "Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = notFoundEx.Message
                }
            ),
            DomainException domainEx => (
                StatusCodes.Status400BadRequest,
                new ProblemDetails
                {
                    Title = "Domain Error",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = domainEx.Message
                }
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "An unexpected error occurred."
                }
            )
        };

        _logger.LogError(exception, "Error: {Message}", exception.Message);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(message);
    }
}
```

## Project References

```xml
<!-- Dependency Direction: Outward layers reference inward layers -->

<!-- MyApp.Domain.csproj - NO dependencies on other projects -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>

<!-- MyApp.Application.csproj - Only references Domain -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="MediatR" Version="12.2.0" />
    <PackageReference Include="FluentValidation" Version="11.9.0" />
    <ProjectReference Include="..\MyApp.Domain\MyApp.Domain.csproj" />
  </ItemGroup>
</Project>

<!-- MyApp.Infrastructure.csproj - References Application (and implicitly Domain) -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <ProjectReference Include="..\MyApp.Application\MyApp.Application.csproj" />
  </ItemGroup>
</Project>

<!-- MyApp.API.csproj - References Application and Infrastructure -->
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\MyApp.Application\MyApp.Application.csproj" />
    <ProjectReference Include="..\MyApp.Infrastructure\MyApp.Infrastructure.csproj" />
  </ItemGroup>
</Project>
```

## Voordelen

- ? **Framework onafhankelijk**: Core is niet gekoppeld aan frameworks
- ? **Testbaar**: Elke laag onafhankelijk testbaar
- ? **Schaalbaar**: Geschikt voor grote, complexe applicaties
- ? **CQRS ready**: Natuurlijke scheiding tussen reads en writes
- ? **Onderhoudbaar**: Duidelijke verantwoordelijkheden

## Nadelen

- ? **Complexiteit**: Veel projecten, interfaces, mappings
- ? **Overkill**: Voor simpele CRUD apps
- ? **Learning curve**: Vereist begrip van meerdere patterns
- ? **Boilerplate**: Veel code voor simpele operaties

## Wanneer Clean Architecture Gebruiken?

| Scenario | Geschikt? |
|----------|-----------|
| Enterprise applicatie | ? Ja |
| Lange levensduur (5+ jaar) | ? Ja |
| Groot development team | ? Ja |
| Complex business domein | ? Ja |
| Microservices | ? Ja |
| MVP / Prototype | ? Nee |
| Simpele CRUD | ? Nee |
| Kleine team (1-2 devs) | ?? Mogelijk overkill |

## Template / Starter

Je kunt de [Clean Architecture Solution Template](https://github.com/jasontaylordev/CleanArchitecture) van Jason Taylor gebruiken:

```bash
dotnet new install Clean.Architecture.Solution.Template
dotnet new ca-sln -n MyApp
```
