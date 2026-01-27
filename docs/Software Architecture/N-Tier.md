# N-Tier Architecture

## Overzicht

N-Tier (ook wel N-Layer) is de meest traditionele architectuur waarbij de applicatie wordt opgedeeld in **horizontale lagen**. Elke laag heeft een specifieke verantwoordelijkheid en communiceert alleen met aangrenzende lagen.

## Structuur

```
???????????????????????????????????????????????????????????????
?                    PRESENTATION LAYER                        ?
?  • Web UI (MVC, Razor Pages)                                ?
?  • API Controllers                                          ?
?  • View Models                                              ?
???????????????????????????????????????????????????????????????
                            ?
                            ?
???????????????????????????????????????????????????????????????
?                    BUSINESS LAYER                            ?
?  • Business Logic                                           ?
?  • Validation                                               ?
?  • Domain Services                                          ?
???????????????????????????????????????????????????????????????
                            ?
                            ?
???????????????????????????????????????????????????????????????
?                    DATA ACCESS LAYER                         ?
?  • Repositories                                             ?
?  • Entity Framework DbContext                               ?
?  • Database Queries                                         ?
???????????????????????????????????????????????????????????????
                            ?
                            ?
???????????????????????????????????????????????????????????????
?                       DATABASE                               ?
?  • SQL Server, PostgreSQL, etc.                             ?
???????????????????????????????????????????????????????????????
```

## Kenmerken

| Aspect | Beschrijving |
|--------|--------------|
| **Richting** | Dependencies stromen naar beneden |
| **Communicatie** | Alleen met aangrenzende lagen |
| **Complexiteit** | Laag tot medium |
| **Testbaarheid** | Medium (door tight coupling) |

## Project Structuur

```
Solution/
??? MyApp.Web/                      # Presentation Layer
?   ??? Controllers/
?   ??? Views/
?   ??? ViewModels/
?   ??? Program.cs
?
??? MyApp.Business/                 # Business Logic Layer
?   ??? Services/
?   ??? Validators/
?   ??? Interfaces/
?
??? MyApp.Data/                     # Data Access Layer
?   ??? Repositories/
?   ??? Context/
?   ??? Configurations/
?
??? MyApp.Entities/                 # Shared Entities
    ??? Product.cs
    ??? Order.cs
    ??? Customer.cs
```

## Implementatie

### Entities (Shared)

```csharp
// MyApp.Entities/Product.cs
namespace MyApp.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation property
    public Category? Category { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
```

### Data Access Layer

```csharp
// MyApp.Data/Context/AppDbContext.cs
namespace MyApp.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}

// MyApp.Data/Configurations/ProductConfiguration.cs
namespace MyApp.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.Price)
            .HasPrecision(18, 2);
        
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);
    }
}

// MyApp.Data/Repositories/IProductRepository.cs
namespace MyApp.Data.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

// MyApp.Data/Repositories/ProductRepository.cs
namespace MyApp.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }
}
```

### Business Layer

```csharp
// MyApp.Business/Interfaces/IProductService.cs
namespace MyApp.Business.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto);
    Task DeleteProductAsync(int id);
}

// MyApp.Business/DTOs/ProductDtos.cs
namespace MyApp.Business.DTOs;

public record ProductDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string CategoryName,
    bool IsInStock
);

public record CreateProductDto(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    int CategoryId
);

public record UpdateProductDto(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    int CategoryId
);

// MyApp.Business/Services/ProductService.cs
namespace MyApp.Business.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        // Validation
        ValidateProduct(dto.Name, dto.Price, dto.StockQuantity);

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            CategoryId = dto.CategoryId
        };

        var created = await _productRepository.AddAsync(product);
        _logger.LogInformation("Product {ProductId} created: {ProductName}", created.Id, created.Name);
        
        return MapToDto(created);
    }

    public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product with id {id} not found");

        // Validation
        ValidateProduct(dto.Name, dto.Price, dto.StockQuantity);

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;
        product.CategoryId = dto.CategoryId;

        await _productRepository.UpdateAsync(product);
        _logger.LogInformation("Product {ProductId} updated", id);
        
        return MapToDto(product);
    }

    public async Task DeleteProductAsync(int id)
    {
        if (!await _productRepository.ExistsAsync(id))
        {
            throw new KeyNotFoundException($"Product with id {id} not found");
        }

        await _productRepository.DeleteAsync(id);
        _logger.LogInformation("Product {ProductId} deleted", id);
    }

    private static void ValidateProduct(string name, decimal price, int stock)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add("Name is required");
        else if (name.Length < 3)
            errors.Add("Name must be at least 3 characters");

        if (price <= 0)
            errors.Add("Price must be greater than 0");

        if (stock < 0)
            errors.Add("Stock cannot be negative");

        if (errors.Any())
            throw new ValidationException(string.Join("; ", errors));
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.Category?.Name ?? "Unknown",
            product.StockQuantity > 0
        );
    }
}

// MyApp.Business/Exceptions/ValidationException.cs
namespace MyApp.Business.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
```

### Presentation Layer (Web API)

```csharp
// MyApp.Web/Controllers/ProductsController.cs
namespace MyApp.Web.Controllers;

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
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        return Ok(products);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        try
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto dto)
    {
        try
        {
            var product = await _productService.UpdateProductAsync(id, dto);
            return Ok(product);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
```

### Presentation Layer (MVC)

```csharp
// MyApp.Web/Controllers/ProductController.cs (MVC)
namespace MyApp.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();
        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound();

        return View(product);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = new SelectList(
            await _categoryService.GetAllAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(
                await _categoryService.GetAllAsync(), "Id", "Name");
            return View(dto);
        }

        try
        {
            await _productService.CreateProductAsync(dto);
            TempData["Success"] = "Product created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.Categories = new SelectList(
                await _categoryService.GetAllAsync(), "Id", "Name");
            return View(dto);
        }
    }
}
```

### Dependency Injection Setup

```csharp
// MyApp.Web/Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Data Layer
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// Business Layer
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

### Project References

```xml
<!-- MyApp.Web.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\MyApp.Business\MyApp.Business.csproj" />
    <ProjectReference Include="..\MyApp.Data\MyApp.Data.csproj" />
  </ItemGroup>
</Project>

<!-- MyApp.Business.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\MyApp.Data\MyApp.Data.csproj" />
    <ProjectReference Include="..\MyApp.Entities\MyApp.Entities.csproj" />
  </ItemGroup>
</Project>

<!-- MyApp.Data.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <ProjectReference Include="..\MyApp.Entities\MyApp.Entities.csproj" />
  </ItemGroup>
</Project>
```

## Voordelen

- ? **Eenvoudig te begrijpen**: Duidelijke, logische structuur
- ? **Snel op te zetten**: Weinig overhead
- ? **Geschikt voor kleine/medium projecten**
- ? **Team-friendly**: Makkelijk te leren
- ? **Tooling support**: Veel scaffolding tools

## Nadelen

- ? **Tight coupling**: Lagen zijn sterk gekoppeld
- ? **Database-centric**: Database structuur beïnvloedt alle lagen
- ? **Moeilijk te testen**: Business layer afhankelijk van Data layer
- ? **Beperkte flexibiliteit**: Moeilijk om infrastructuur te wisselen
- ? **Anemic domain model**: Business logic vaak verspreid

## Anti-Patterns in N-Tier

### ? Leaky Abstraction
```csharp
// FOUT: Repository retourneert IQueryable
public interface IProductRepository
{
    IQueryable<Product> GetAll(); // Lekt EF details naar Business Layer
}

// GOED: Repository retourneert concrete collectie
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
}
```

### ? Anemic Domain Model
```csharp
// FOUT: Entity zonder gedrag
public class Order
{
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; }
}

// OrderService doet alle logic
public class OrderService
{
    public void ApproveOrder(Order order) => order.Status = OrderStatus.Approved;
    public void CalculateTotal(Order order) => order.Total = order.Lines.Sum(l => l.Price);
}

// BETER: Domain logic in Entity
public class Order
{
    public decimal Total { get; private set; }
    public OrderStatus Status { get; private set; }
    
    public void Approve()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be approved");
        Status = OrderStatus.Approved;
    }
    
    public void CalculateTotal()
    {
        Total = Lines.Sum(l => l.Price * l.Quantity);
    }
}
```

## Wanneer N-Tier Gebruiken?

| Scenario | Geschikt? |
|----------|-----------|
| MVP / Proof of Concept | ? Ja |
| Kleine CRUD applicaties | ? Ja |
| Deadline-gedreven projecten | ? Ja |
| Complex domein met veel business rules | ? Overweeg Clean/Onion |
| Veel externe integraties | ? Overweeg Hexagonal |
| Microservices | ? Overweeg Clean |

## Evolutie naar Clean Architecture

N-Tier kan geleidelijk evolueren naar Clean Architecture:

```
Stap 1: Inverteer dependencies
???????????????
?     Web     ? ???> Business ???> Data
???????????????
        ?
Stap 2: Introduceer interfaces in Business Layer
???????????????
?     Web     ? ???> Business <??? Data (implements interfaces)
???????????????
        ?
Stap 3: Scheid Domain van Application
???????????????
?     Web     ? ???> Application ???> Domain
???????????????           ?
                    Infrastructure
```

## Gerelateerde Patterns

- [Clean Architecture](CleanArchitecture.md) - Evolutie van N-Tier
- [Onion Architecture](Onion.md) - Domain-centric alternatief
- [Repository Pattern](../Design%20Patterns/Creational/Repository.md) - Data access abstractie
