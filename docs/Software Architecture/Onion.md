# Onion Architecture

## Overzicht

Onion Architecture, geïntroduceerd door Jeffrey Palermo in 2008, plaatst het **Domain Model in het centrum** van de applicatie. Alle dependencies wijzen naar binnen, waardoor de core volledig onafhankelijk is van externe concerns zoals databases en UI.

## Structuur

```
                    ???????????????????????????????????????
                    ?         Infrastructure              ?
                    ?   (UI, Database, External APIs)     ?
                    ?                                     ?
                    ?    ?????????????????????????????    ?
                    ?    ?      Application          ?    ?
                    ?    ?   (Services, DTOs)        ?    ?
                    ?    ?                           ?    ?
                    ?    ?    ???????????????????    ?    ?
                    ?    ?    ?  Domain Services?    ?    ?
                    ?    ?    ?                 ?    ?    ?
                    ?    ?    ?  ?????????????  ?    ?    ?
                    ?    ?    ?  ?  Domain   ?  ?    ?    ?
                    ?    ?    ?  ?  Model    ?  ?    ?    ?
                    ?    ?    ?  ? (Entities)?  ?    ?    ?
                    ?    ?    ?  ?????????????  ?    ?    ?
                    ?    ?    ?                 ?    ?    ?
                    ?    ?    ???????????????????    ?    ?
                    ?    ?                           ?    ?
                    ?    ?????????????????????????????    ?
                    ?                                     ?
                    ???????????????????????????????????????
                    
                    Dependencies wijzen naar BINNEN ?
```

## Lagen

| Laag | Verantwoordelijkheid | Voorbeelden |
|------|---------------------|-------------|
| **Domain Model** | Entities, Value Objects, Enums | `Product`, `Money`, `Email` |
| **Domain Services** | Domain logic die niet in één entity past | `PricingService`, `ShippingCalculator` |
| **Application Services** | Use cases, orchestratie | `OrderService`, `ProductService` |
| **Infrastructure** | Externe concerns | Database, UI, APIs, Logging |

## Project Structuur

```
Solution/
??? src/
?   ??? MyApp.Domain/                    # Centrum - geen dependencies
?   ?   ??? Entities/
?   ?   ?   ??? Product.cs
?   ?   ?   ??? Order.cs
?   ?   ?   ??? Customer.cs
?   ?   ??? ValueObjects/
?   ?   ?   ??? Money.cs
?   ?   ?   ??? Email.cs
?   ?   ?   ??? Address.cs
?   ?   ??? Enums/
?   ?   ?   ??? OrderStatus.cs
?   ?   ??? Events/
?   ?   ?   ??? OrderCreatedEvent.cs
?   ?   ??? Exceptions/
?   ?   ?   ??? DomainException.cs
?   ?   ??? Services/
?   ?       ??? IShippingCalculator.cs
?   ?
?   ??? MyApp.Application/               # Use Cases
?   ?   ??? Interfaces/
?   ?   ?   ??? IProductRepository.cs
?   ?   ?   ??? IOrderRepository.cs
?   ?   ?   ??? IEmailService.cs
?   ?   ??? Services/
?   ?   ?   ??? ProductService.cs
?   ?   ?   ??? OrderService.cs
?   ?   ??? DTOs/
?   ?   ?   ??? ProductDto.cs
?   ?   ?   ??? OrderDto.cs
?   ?   ??? Mappers/
?   ?       ??? ProductMapper.cs
?   ?
?   ??? MyApp.Infrastructure/            # External Concerns
?   ?   ??? Persistence/
?   ?   ?   ??? AppDbContext.cs
?   ?   ?   ??? ProductRepository.cs
?   ?   ?   ??? OrderRepository.cs
?   ?   ??? ExternalServices/
?   ?   ?   ??? EmailService.cs
?   ?   ?   ??? PaymentGateway.cs
?   ?   ??? DependencyInjection.cs
?   ?
?   ??? MyApp.API/                       # Entry Point
?       ??? Controllers/
?       ??? Middleware/
?       ??? Program.cs
?
??? tests/
    ??? MyApp.Domain.Tests/
    ??? MyApp.Application.Tests/
    ??? MyApp.API.Tests/
```

## Implementatie

### Domain Layer (Centrum)

```csharp
// MyApp.Domain/Entities/Product.cs
namespace MyApp.Domain.Entities;

public class Product
{
    public ProductId Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }
    public int StockQuantity { get; private set; }
    public CategoryId CategoryId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Private constructor voor EF
    private Product() { }

    public Product(string name, string description, Money price, int stockQuantity, CategoryId categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required");
        if (name.Length < 3)
            throw new DomainException("Product name must be at least 3 characters");
        if (stockQuantity < 0)
            throw new DomainException("Stock quantity cannot be negative");

        Id = ProductId.New();
        Name = name;
        Description = description ?? string.Empty;
        Price = price ?? throw new DomainException("Price is required");
        StockQuantity = stockQuantity;
        CategoryId = categoryId;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string description, Money price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required");

        Name = name;
        Description = description ?? string.Empty;
        Price = price ?? throw new DomainException("Price is required");
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive");
        
        StockQuantity += quantity;
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive");
        if (quantity > StockQuantity)
            throw new DomainException($"Cannot remove {quantity} items. Only {StockQuantity} in stock");
        
        StockQuantity -= quantity;
    }

    public bool IsInStock => StockQuantity > 0;
    public bool IsLowStock => StockQuantity > 0 && StockQuantity <= 10;
}

// MyApp.Domain/Entities/ProductId.cs (Strongly Typed ID)
namespace MyApp.Domain.Entities;

public record ProductId
{
    public Guid Value { get; }

    private ProductId(Guid value) => Value = value;

    public static ProductId New() => new(Guid.NewGuid());
    public static ProductId From(Guid value) => new(value);
    
    public override string ToString() => Value.ToString();
}
```

### Value Objects

```csharp
// MyApp.Domain/ValueObjects/Money.cs
namespace MyApp.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative");
        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required");

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Create(decimal amount, string currency = "EUR") 
        => new(amount, currency);

    public static Money Zero(string currency = "EUR") 
        => new(0, currency);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
        => new(Amount * factor, Currency);

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot operate on different currencies: {Currency} and {other.Currency}");
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}

// MyApp.Domain/ValueObjects/Email.cs
namespace MyApp.Domain.ValueObjects;

public record Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required");

        email = email.Trim().ToLowerInvariant();

        if (!IsValidEmail(email))
            throw new DomainException("Invalid email format");

        return new Email(email);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public override string ToString() => Value;
}

// MyApp.Domain/ValueObjects/Address.cs
namespace MyApp.Domain.ValueObjects;

public record Address
{
    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; }

    private Address(string street, string city, string postalCode, string country)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }

    public static Address Create(string street, string city, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Street is required");
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City is required");
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new DomainException("Postal code is required");
        if (string.IsNullOrWhiteSpace(country))
            throw new DomainException("Country is required");

        return new Address(street.Trim(), city.Trim(), postalCode.Trim(), country.Trim());
    }

    public override string ToString() => $"{Street}, {PostalCode} {City}, {Country}";
}
```

### Domain Services

```csharp
// MyApp.Domain/Services/IShippingCalculator.cs
namespace MyApp.Domain.Services;

public interface IShippingCalculator
{
    Money CalculateShippingCost(Address destination, decimal totalWeight);
}

// MyApp.Domain/Services/ShippingCalculator.cs
namespace MyApp.Domain.Services;

public class ShippingCalculator : IShippingCalculator
{
    public Money CalculateShippingCost(Address destination, decimal totalWeight)
    {
        // Domain logic voor shipping berekening
        var baseRate = destination.Country switch
        {
            "NL" => 5.00m,
            "BE" or "DE" => 8.50m,
            _ => 15.00m
        };

        var weightRate = totalWeight * 0.50m;
        
        return Money.Create(baseRate + weightRate);
    }
}
```

### Domain Exceptions

```csharp
// MyApp.Domain/Exceptions/DomainException.cs
namespace MyApp.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object id)
        : base($"{entityName} with id '{id}' was not found") { }
}
```

### Application Layer

```csharp
// MyApp.Application/Interfaces/IProductRepository.cs
namespace MyApp.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(ProductId id, CancellationToken ct = default);
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Product>> GetByCategoryAsync(CategoryId categoryId, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    Task UpdateAsync(Product product, CancellationToken ct = default);
    Task DeleteAsync(ProductId id, CancellationToken ct = default);
}

// MyApp.Application/Interfaces/IUnitOfWork.cs
namespace MyApp.Application.Interfaces;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    ICategoryRepository Categories { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

// MyApp.Application/DTOs/ProductDto.cs
namespace MyApp.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    string StockStatus,
    Guid CategoryId,
    string? CategoryName
);

public record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    Guid CategoryId
);

public record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    string Currency
);

// MyApp.Application/Services/ProductService.cs
namespace MyApp.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(ProductId.From(id), ct);
        return product is null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        var products = await _unitOfWork.Products.GetAllAsync(ct);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        var price = Money.Create(request.Price, request.Currency);
        var categoryId = CategoryId.From(request.CategoryId);

        var product = new Product(
            request.Name,
            request.Description,
            price,
            request.StockQuantity,
            categoryId
        );

        await _unitOfWork.Products.AddAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Product created: {ProductId}", product.Id);
        
        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(ProductId.From(id), ct)
            ?? throw new EntityNotFoundException(nameof(Product), id);

        var price = Money.Create(request.Price, request.Currency);
        product.UpdateDetails(request.Name, request.Description, price);

        await _unitOfWork.Products.UpdateAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Product updated: {ProductId}", product.Id);
        
        return MapToDto(product);
    }

    public async Task AddStockAsync(Guid id, int quantity, CancellationToken ct = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(ProductId.From(id), ct)
            ?? throw new EntityNotFoundException(nameof(Product), id);

        product.AddStock(quantity);
        
        await _unitOfWork.Products.UpdateAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _unitOfWork.Products.DeleteAsync(ProductId.From(id), ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        _logger.LogInformation("Product deleted: {ProductId}", id);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto(
            product.Id.Value,
            product.Name,
            product.Description,
            product.Price.Amount,
            product.Price.Currency,
            product.StockQuantity,
            product.IsLowStock ? "Low Stock" : product.IsInStock ? "In Stock" : "Out of Stock",
            product.CategoryId.Value,
            null // Would be populated via join/include
        );
    }
}
```

### Infrastructure Layer

```csharp
// MyApp.Infrastructure/Persistence/AppDbContext.cs
namespace MyApp.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}

// MyApp.Infrastructure/Persistence/Configurations/ProductConfiguration.cs
namespace MyApp.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => ProductId.From(value));

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        // Value Object: Money
        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(m => m.Amount)
                .HasColumnName("Price")
                .HasPrecision(18, 2);
            price.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3);
        });

        builder.Property(p => p.CategoryId)
            .HasConversion(
                id => id.Value,
                value => CategoryId.From(value));
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
        return await _context.Products.OrderBy(p => p.Name).ToListAsync(ct);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(CategoryId categoryId, CancellationToken ct = default)
    {
        return await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        await _context.Products.AddAsync(product, ct);
    }

    public Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Update(product);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(ProductId id, CancellationToken ct = default)
    {
        var product = await GetByIdAsync(id, ct);
        if (product != null)
        {
            _context.Products.Remove(product);
        }
    }
}

// MyApp.Infrastructure/Persistence/UnitOfWork.cs
namespace MyApp.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }
    public ICategoryRepository Categories { get; }

    public UnitOfWork(
        AppDbContext context,
        IProductRepository products,
        IOrderRepository orders,
        ICategoryRepository categories)
    {
        _context = context;
        Products = products;
        Orders = orders;
        Categories = categories;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
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
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // External Services
        services.AddScoped<IEmailService, SmtpEmailService>();

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
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(CancellationToken ct)
    {
        var products = await _productService.GetAllAsync(ct);
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken ct)
    {
        var product = await _productService.GetByIdAsync(id, ct);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(
        [FromBody] CreateProductRequest request, 
        CancellationToken ct)
    {
        var product = await _productService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Update(
        Guid id, 
        [FromBody] UpdateProductRequest request, 
        CancellationToken ct)
    {
        var product = await _productService.UpdateAsync(id, request, ct);
        return Ok(product);
    }

    [HttpPost("{id:guid}/stock")]
    public async Task<IActionResult> AddStock(
        Guid id, 
        [FromBody] AddStockRequest request, 
        CancellationToken ct)
    {
        await _productService.AddStockAsync(id, request.Quantity, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _productService.DeleteAsync(id, ct);
        return NoContent();
    }
}

// MyApp.API/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application Layer
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Infrastructure Layer
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Project References

```xml
<!-- MyApp.Domain.csproj - NO external dependencies -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>

<!-- MyApp.Application.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\MyApp.Domain\MyApp.Domain.csproj" />
  </ItemGroup>
</Project>

<!-- MyApp.Infrastructure.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <ProjectReference Include="..\MyApp.Application\MyApp.Application.csproj" />
  </ItemGroup>
</Project>

<!-- MyApp.API.csproj -->
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

## Dependency Rule

```
                        ???????????
                        ?   API   ?
                        ???????????
                             ? references
                             ?
        ????????????????????????????????????????
        ?                                      ?
        ?                                      ?
?????????????????                    ???????????????????
? Infrastructure?                    ?   Application   ?
?????????????????                    ???????????????????
        ? implements                          ? references
        ?                                     ?
        ?                            ???????????????????
        ????????????????????????????>?     Domain      ?
              references             ???????????????????

Domain kent NIEMAND - is volledig onafhankelijk
Application definieert interfaces - kent alleen Domain
Infrastructure implementeert interfaces - kent Application & Domain
```

## Voordelen

- ? **Domain centraal**: Business logic is onafhankelijk
- ? **Testbaarheid**: Domain en Application testbaar zonder infrastructure
- ? **Flexibiliteit**: Infrastructure makkelijk te vervangen
- ? **DDD-friendly**: Ondersteunt Domain-Driven Design
- ? **Maintainability**: Duidelijke grenzen tussen lagen

## Nadelen

- ? **Complexiteit**: Meer projecten en interfaces
- ? **Learning curve**: Vereist begrip van DDD concepten
- ? **Overhead**: Voor simpele CRUD apps overkill
- ? **Mapping**: Veel mapping tussen lagen

## Wanneer Onion Gebruiken?

| Scenario | Geschikt? |
|----------|-----------|
| Complex business domein | ? Ja |
| Domain-Driven Design | ? Ja |
| Lange levensduur applicatie | ? Ja |
| Team ervaren met DDD | ? Ja |
| Simpele CRUD applicatie | ? Nee, gebruik N-Tier |
| MVP / Prototype | ? Nee |
