# MVC (Model-View-Controller)

## Overzicht

MVC is een architectural pattern dat een applicatie verdeelt in drie componenten: **Model**, **View**, en **Controller**. Het werd oorspronkelijk ontwikkeld in 1979 voor Smalltalk en is nu de standaard voor web applicaties.

## Structuur

```
???????????????????????????????????????????????????????????????
?                         USER                                 ?
???????????????????????????????????????????????????????????????
                    ?                       ?
                    ? User Action           ? Display
                    ?                       ?
???????????????????????????????????????????????????????????????
?                        VIEW                                  ?
?  • Presenteert data aan gebruiker                           ?
?  • Stuurt user actions naar Controller                      ?
?  • Observeert Model voor updates (klassiek)                 ?
???????????????????????????????????????????????????????????????
                    ?                       ?
                    ? User Input            ? Updated View
                    ?                       ?
?????????????????????????????    ??????????????????????????????
?       CONTROLLER          ?    ?          MODEL             ?
?                           ?    ?                            ?
?  • Verwerkt user input    ????>?  • Business logic          ?
?  • Selecteert View        ?    ?  • Data & state            ?
?  • Coördineert flow       ?    ?  • Validatie regels        ?
?                           ?<????  • Database operaties      ?
?????????????????????????????    ??????????????????????????????
```

## Componenten

### Model
- Bevat de **business logic** en **data**
- Onafhankelijk van UI
- Notificeert observers bij state changes (klassiek MVC)
- Kan meerdere Models bevatten (domain models, view models)

### View
- **Presenteert** data aan de gebruiker
- Ontvangt data van Controller of observeert Model
- Bevat **geen business logic**
- Kan meerdere Views zijn voor hetzelfde Model

### Controller
- Ontvangt en verwerkt **user input**
- Selecteert welke View te tonen
- **Coördineert** tussen Model en View
- Bevat geen business logic (delegeert naar Model)

## ASP.NET Core MVC Implementatie

### Project Structuur

```
MyApp/
??? Controllers/
?   ??? HomeController.cs
?   ??? ProductController.cs
?   ??? OrderController.cs
??? Models/
?   ??? Product.cs
?   ??? Order.cs
?   ??? ViewModels/
?       ??? ProductViewModel.cs
?       ??? OrderViewModel.cs
??? Views/
?   ??? Home/
?   ?   ??? Index.cshtml
?   ??? Product/
?   ?   ??? Index.cshtml
?   ?   ??? Details.cshtml
?   ?   ??? Create.cshtml
?   ??? Shared/
?       ??? _Layout.cshtml
??? Services/
    ??? IProductService.cs
    ??? ProductService.cs
```

### Model

```csharp
// Domain Model
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    
    public bool IsInStock => StockQuantity > 0;
    
    public void DecreaseStock(int quantity)
    {
        if (quantity > StockQuantity)
            throw new InvalidOperationException("Insufficient stock");
        StockQuantity -= quantity;
    }
}

// ViewModel - specifiek voor View
public class ProductViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [Range(0.01, 10000)]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }
    
    [Required]
    [Range(0, 1000)]
    public int StockQuantity { get; set; }
    
    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }
    
    public SelectList? Categories { get; set; }
    
    // Computed property for view
    public string StockStatus => StockQuantity > 10 ? "In Stock" : 
                                  StockQuantity > 0 ? "Low Stock" : "Out of Stock";
}

// List ViewModel
public class ProductListViewModel
{
    public List<ProductViewModel> Products { get; set; } = new();
    public string? SearchTerm { get; set; }
    public int? CategoryFilter { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
```

### Controller

```csharp
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(
        IProductService productService,
        ICategoryService categoryService,
        ILogger<ProductController> logger)
    {
        _productService = productService;
        _categoryService = categoryService;
        _logger = logger;
    }

    // GET: /Product
    public async Task<IActionResult> Index(string? searchTerm, int? categoryId, int page = 1)
    {
        var products = await _productService.GetProductsAsync(searchTerm, categoryId, page, pageSize: 10);
        var totalCount = await _productService.GetTotalCountAsync(searchTerm, categoryId);

        var viewModel = new ProductListViewModel
        {
            Products = products.Select(MapToViewModel).ToList(),
            SearchTerm = searchTerm,
            CategoryFilter = categoryId,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalCount / 10.0)
        };

        return View(viewModel);
    }

    // GET: /Product/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        
        if (product == null)
        {
            return NotFound();
        }

        return View(MapToViewModel(product));
    }

    // GET: /Product/Create
    public async Task<IActionResult> Create()
    {
        var viewModel = new ProductViewModel
        {
            Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name")
        };
        
        return View(viewModel);
    }

    // POST: /Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            viewModel.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name");
            return View(viewModel);
        }

        try
        {
            var product = MapToEntity(viewModel);
            await _productService.CreateAsync(product);
            
            TempData["SuccessMessage"] = "Product created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            ModelState.AddModelError("", "An error occurred while creating the product.");
            viewModel.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name");
            return View(viewModel);
        }
    }

    // GET: /Product/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        
        if (product == null)
        {
            return NotFound();
        }

        var viewModel = MapToViewModel(product);
        viewModel.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name", product.CategoryId);
        
        return View(viewModel);
    }

    // POST: /Product/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            viewModel.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name");
            return View(viewModel);
        }

        try
        {
            var product = MapToEntity(viewModel);
            await _productService.UpdateAsync(product);
            
            TempData["SuccessMessage"] = "Product updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _productService.ExistsAsync(id))
            {
                return NotFound();
            }
            throw;
        }
    }

    // POST: /Product/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        TempData["SuccessMessage"] = "Product deleted successfully!";
        return RedirectToAction(nameof(Index));
    }

    // Helper methods
    private static ProductViewModel MapToViewModel(Product product)
    {
        return new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId
        };
    }

    private static Product MapToEntity(ProductViewModel viewModel)
    {
        return new Product
        {
            Id = viewModel.Id,
            Name = viewModel.Name,
            Description = viewModel.Description ?? string.Empty,
            Price = viewModel.Price,
            StockQuantity = viewModel.StockQuantity,
            CategoryId = viewModel.CategoryId
        };
    }
}
```

### View (Razor)

```html
@* Views/Product/Index.cshtml *@
@model ProductListViewModel

@{
    ViewData["Title"] = "Products";
}

<h1>Products</h1>

@if (TempData["SuccessMessage"] != null)
{
    <div class="alert alert-success alert-dismissible fade show">
        @TempData["SuccessMessage"]
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </div>
}

<div class="row mb-3">
    <div class="col-md-6">
        <form asp-action="Index" method="get" class="d-flex">
            <input type="text" name="searchTerm" value="@Model.SearchTerm" 
                   class="form-control me-2" placeholder="Search products..." />
            <button type="submit" class="btn btn-outline-primary">Search</button>
        </form>
    </div>
    <div class="col-md-6 text-end">
        <a asp-action="Create" class="btn btn-primary">
            <i class="bi bi-plus"></i> Add Product
        </a>
    </div>
</div>

<table class="table table-striped">
    <thead>
        <tr>
            <th>Name</th>
            <th>Price</th>
            <th>Stock</th>
            <th>Status</th>
            <th>Actions</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var product in Model.Products)
        {
            <tr>
                <td>
                    <a asp-action="Details" asp-route-id="@product.Id">
                        @product.Name
                    </a>
                </td>
                <td>@product.Price.ToString("C")</td>
                <td>@product.StockQuantity</td>
                <td>
                    <span class="badge @(product.StockQuantity > 10 ? "bg-success" : 
                                         product.StockQuantity > 0 ? "bg-warning" : "bg-danger")">
                        @product.StockStatus
                    </span>
                </td>
                <td>
                    <a asp-action="Edit" asp-route-id="@product.Id" class="btn btn-sm btn-outline-primary">
                        Edit
                    </a>
                    <form asp-action="Delete" asp-route-id="@product.Id" method="post" 
                          class="d-inline" onsubmit="return confirm('Are you sure?')">
                        @Html.AntiForgeryToken()
                        <button type="submit" class="btn btn-sm btn-outline-danger">Delete</button>
                    </form>
                </td>
            </tr>
        }
    </tbody>
</table>

@* Pagination *@
@if (Model.TotalPages > 1)
{
    <nav>
        <ul class="pagination">
            @for (int i = 1; i <= Model.TotalPages; i++)
            {
                <li class="page-item @(i == Model.CurrentPage ? "active" : "")">
                    <a class="page-link" asp-action="Index" 
                       asp-route-page="@i"
                       asp-route-searchTerm="@Model.SearchTerm"
                       asp-route-categoryId="@Model.CategoryFilter">
                        @i
                    </a>
                </li>
            }
        </ul>
    </nav>
}
```

```html
@* Views/Product/Create.cshtml *@
@model ProductViewModel

@{
    ViewData["Title"] = "Create Product";
}

<h1>Create Product</h1>

<div class="row">
    <div class="col-md-6">
        <form asp-action="Create" method="post">
            @Html.AntiForgeryToken()
            
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            
            <div class="mb-3">
                <label asp-for="Name" class="form-label"></label>
                <input asp-for="Name" class="form-control" />
                <span asp-validation-for="Name" class="text-danger"></span>
            </div>
            
            <div class="mb-3">
                <label asp-for="Description" class="form-label"></label>
                <textarea asp-for="Description" class="form-control" rows="3"></textarea>
                <span asp-validation-for="Description" class="text-danger"></span>
            </div>
            
            <div class="mb-3">
                <label asp-for="Price" class="form-label"></label>
                <input asp-for="Price" class="form-control" type="number" step="0.01" />
                <span asp-validation-for="Price" class="text-danger"></span>
            </div>
            
            <div class="mb-3">
                <label asp-for="StockQuantity" class="form-label"></label>
                <input asp-for="StockQuantity" class="form-control" type="number" />
                <span asp-validation-for="StockQuantity" class="text-danger"></span>
            </div>
            
            <div class="mb-3">
                <label asp-for="CategoryId" class="form-label"></label>
                <select asp-for="CategoryId" asp-items="Model.Categories" class="form-select">
                    <option value="">-- Select Category --</option>
                </select>
                <span asp-validation-for="CategoryId" class="text-danger"></span>
            </div>
            
            <div class="mb-3">
                <button type="submit" class="btn btn-primary">Create</button>
                <a asp-action="Index" class="btn btn-secondary">Cancel</a>
            </div>
        </form>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

### Service Layer

```csharp
public interface IProductService
{
    Task<IEnumerable<Product>> GetProductsAsync(string? searchTerm, int? categoryId, int page, int pageSize);
    Task<int> GetTotalCountAsync(string? searchTerm, int? categoryId);
    Task<Product?> GetByIdAsync(int id);
    Task CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(
        string? searchTerm, int? categoryId, int page, int pageSize)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || 
                                     p.Description.Contains(searchTerm));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        return await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(string? searchTerm, int? categoryId)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        return await query.CountAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
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

## Request Flow

```
1. User clicks "Create Product"
   ?
   ?
2. Browser sends GET /Product/Create
   ?
   ?
3. ProductController.Create() (GET)
   ?  - Loads categories
   ?  - Creates empty ViewModel
   ?
   ?
4. Returns View with ViewModel
   ?
   ?
5. User fills form and submits
   ?
   ?
6. Browser sends POST /Product/Create with form data
   ?
   ?
7. ProductController.Create(viewModel) (POST)
   ?  - Model binding
   ?  - Validation (ModelState.IsValid)
   ?  - Calls ProductService.CreateAsync()
   ?
   ?
8. RedirectToAction(nameof(Index))
   ?
   ?
9. Browser redirects to /Product
```

## Voordelen

- ? **Separation of Concerns**: Duidelijke scheiding tussen lagen
- ? **Testbaarheid**: Controllers en Services makkelijk te unit testen
- ? **Parallelle ontwikkeling**: Teams kunnen aan verschillende lagen werken
- ? **Framework support**: Uitstekende ondersteuning in ASP.NET Core
- ? **SEO-friendly**: Server-side rendering

## Nadelen

- ? **Boilerplate**: Veel code voor simpele CRUD operaties
- ? **Tight coupling**: Views zijn gekoppeld aan specifieke ViewModels
- ? **State management**: Moeilijker voor complexe UI interacties

## Wanneer MVC Gebruiken?

- ? Web applicaties met server-side rendering
- ? Content-heavy websites
- ? SEO belangrijke applicaties
- ? Teams bekend met request/response model
- ? Niet ideaal voor: complexe real-time UI, desktop apps

## Gerelateerde Patterns

- **Repository Pattern**: Abstractie van data access
- **Service Layer**: Business logic scheiding
- **ViewModel**: Data shaping voor View
