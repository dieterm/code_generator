# MVP (Model-View-Presenter)

## Overzicht

MVP is een architectural pattern waarin de **Presenter** als intermediair fungeert tussen Model en View. De View is "dom" en delegeert alle logica naar de Presenter. Dit maakt de View volledig testbaar.

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
?  • Passief - bevat geen logica                              ?
?  • Implementeert IView interface                            ?
?  • Stuurt events naar Presenter                             ?
?  • Presenter update View properties                         ?
???????????????????????????????????????????????????????????????
                    ?                       ?
                    ? User events           ? Update View
                    ?                       ?
?????????????????????????????????????????????????????????????
?                      PRESENTER                             ?
?  • Bevat presentation logic                                ?
?  • Reageert op View events                                 ?
?  • Haalt data op uit Model                                 ?
?  • Update View expliciet                                   ?
?????????????????????????????????????????????????????????????
                    ?                       ?
                    ? Request data          ? Data
                    ?                       ?
?????????????????????????????????????????????????????????????
?                        MODEL                               ?
?  • Business logic                                          ?
?  • Data access                                             ?
?  • Validatie                                               ?
?????????????????????????????????????????????????????????????
```

## MVP Varianten

### Passive View (Aanbevolen)
- View is volledig passief
- Presenter doet **alle** UI updates
- Maximale testbaarheid
- Meer code in Presenter

### Supervising Controller
- View heeft basic data binding
- Presenter handelt complexe logica
- Minder code, minder testbaar

## Componenten

### Model
- Bevat **business logic** en **data**
- Onafhankelijk van UI
- Kan services, repositories bevatten

### View (IView)
- **Passieve** weergave van data
- Implementeert een **interface**
- Stuurt user events door naar Presenter
- Bevat **geen business/presentation logic**

### Presenter
- Bevat **presentation logic**
- Houdt referentie naar View (via interface)
- Houdt referentie naar Model/Services
- **Update View properties expliciet**

## WinForms Implementatie

### Project Structuur

```
MyApp/
??? Models/
?   ??? Product.cs
?   ??? Category.cs
??? Views/
?   ??? Interfaces/
?   ?   ??? IProductListView.cs
?   ?   ??? IProductEditView.cs
?   ??? Forms/
?       ??? ProductListForm.cs
?       ??? ProductEditForm.cs
??? Presenters/
?   ??? ProductListPresenter.cs
?   ??? ProductEditPresenter.cs
??? Services/
?   ??? IProductService.cs
?   ??? ProductService.cs
??? Program.cs
```

### View Interface

```csharp
public interface IProductListView
{
    // Properties voor data binding
    IEnumerable<ProductViewModel> Products { set; }
    ProductViewModel? SelectedProduct { get; }
    string SearchText { get; }
    bool IsLoading { set; }
    string StatusMessage { set; }
    
    // Events voor user actions
    event EventHandler? LoadRequested;
    event EventHandler? SearchRequested;
    event EventHandler? AddRequested;
    event EventHandler? EditRequested;
    event EventHandler? DeleteRequested;
    event EventHandler? RefreshRequested;
    
    // Methodes voor UI updates
    void ShowError(string message);
    void ShowSuccess(string message);
    bool ConfirmDelete(string productName);
    void Close();
}

public interface IProductEditView
{
    // Properties
    int ProductId { get; set; }
    string ProductName { get; set; }
    string Description { get; set; }
    decimal Price { get; set; }
    int StockQuantity { get; set; }
    int SelectedCategoryId { get; set; }
    IEnumerable<CategoryViewModel> Categories { set; }
    bool IsNewProduct { get; set; }
    
    // Validation
    Dictionary<string, string> ValidationErrors { set; }
    
    // Events
    event EventHandler? SaveRequested;
    event EventHandler? CancelRequested;
    event EventHandler? LoadRequested;
    
    // Methods
    void ShowError(string message);
    void Close();
}
```

### Presenter

```csharp
public class ProductListPresenter
{
    private readonly IProductListView _view;
    private readonly IProductService _productService;
    
    public ProductListPresenter(IProductListView view, IProductService productService)
    {
        _view = view;
        _productService = productService;
        
        // Subscribe to view events
        _view.LoadRequested += OnLoadRequested;
        _view.SearchRequested += OnSearchRequested;
        _view.AddRequested += OnAddRequested;
        _view.EditRequested += OnEditRequested;
        _view.DeleteRequested += OnDeleteRequested;
        _view.RefreshRequested += OnRefreshRequested;
    }

    private async void OnLoadRequested(object? sender, EventArgs e)
    {
        await LoadProductsAsync();
    }

    private async void OnSearchRequested(object? sender, EventArgs e)
    {
        await LoadProductsAsync(_view.SearchText);
    }

    private async Task LoadProductsAsync(string? searchTerm = null)
    {
        try
        {
            _view.IsLoading = true;
            _view.StatusMessage = "Loading products...";
            
            var products = await _productService.GetAllAsync(searchTerm);
            
            _view.Products = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategoryName = p.Category?.Name ?? "Unknown"
            });
            
            _view.StatusMessage = $"Loaded {products.Count()} products";
        }
        catch (Exception ex)
        {
            _view.ShowError($"Error loading products: {ex.Message}");
            _view.StatusMessage = "Error loading products";
        }
        finally
        {
            _view.IsLoading = false;
        }
    }

    private void OnAddRequested(object? sender, EventArgs e)
    {
        var editView = new ProductEditForm();
        var presenter = new ProductEditPresenter(editView, _productService, null);
        
        editView.ShowDialog();
        
        // Refresh list after dialog closes
        _ = LoadProductsAsync();
    }

    private void OnEditRequested(object? sender, EventArgs e)
    {
        var selectedProduct = _view.SelectedProduct;
        if (selectedProduct == null)
        {
            _view.ShowError("Please select a product to edit.");
            return;
        }

        var editView = new ProductEditForm();
        var presenter = new ProductEditPresenter(editView, _productService, selectedProduct.Id);
        
        editView.ShowDialog();
        
        // Refresh list after dialog closes
        _ = LoadProductsAsync();
    }

    private async void OnDeleteRequested(object? sender, EventArgs e)
    {
        var selectedProduct = _view.SelectedProduct;
        if (selectedProduct == null)
        {
            _view.ShowError("Please select a product to delete.");
            return;
        }

        if (!_view.ConfirmDelete(selectedProduct.Name))
        {
            return;
        }

        try
        {
            _view.IsLoading = true;
            await _productService.DeleteAsync(selectedProduct.Id);
            _view.ShowSuccess("Product deleted successfully!");
            await LoadProductsAsync();
        }
        catch (Exception ex)
        {
            _view.ShowError($"Error deleting product: {ex.Message}");
        }
        finally
        {
            _view.IsLoading = false;
        }
    }

    private async void OnRefreshRequested(object? sender, EventArgs e)
    {
        await LoadProductsAsync();
    }
}

public class ProductEditPresenter
{
    private readonly IProductEditView _view;
    private readonly IProductService _productService;
    private readonly int? _productId;

    public ProductEditPresenter(IProductEditView view, IProductService productService, int? productId)
    {
        _view = view;
        _productService = productService;
        _productId = productId;
        
        _view.IsNewProduct = !productId.HasValue;
        
        _view.LoadRequested += OnLoadRequested;
        _view.SaveRequested += OnSaveRequested;
        _view.CancelRequested += OnCancelRequested;
    }

    private async void OnLoadRequested(object? sender, EventArgs e)
    {
        try
        {
            // Load categories
            var categories = await _productService.GetCategoriesAsync();
            _view.Categories = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name
            });

            // Load product if editing
            if (_productId.HasValue)
            {
                var product = await _productService.GetByIdAsync(_productId.Value);
                if (product != null)
                {
                    _view.ProductId = product.Id;
                    _view.ProductName = product.Name;
                    _view.Description = product.Description;
                    _view.Price = product.Price;
                    _view.StockQuantity = product.StockQuantity;
                    _view.SelectedCategoryId = product.CategoryId;
                }
            }
        }
        catch (Exception ex)
        {
            _view.ShowError($"Error loading data: {ex.Message}");
        }
    }

    private async void OnSaveRequested(object? sender, EventArgs e)
    {
        // Validate
        var errors = Validate();
        if (errors.Any())
        {
            _view.ValidationErrors = errors;
            return;
        }

        try
        {
            var product = new Product
            {
                Id = _view.ProductId,
                Name = _view.ProductName,
                Description = _view.Description,
                Price = _view.Price,
                StockQuantity = _view.StockQuantity,
                CategoryId = _view.SelectedCategoryId
            };

            if (_view.IsNewProduct)
            {
                await _productService.CreateAsync(product);
            }
            else
            {
                await _productService.UpdateAsync(product);
            }

            _view.Close();
        }
        catch (Exception ex)
        {
            _view.ShowError($"Error saving product: {ex.Message}");
        }
    }

    private void OnCancelRequested(object? sender, EventArgs e)
    {
        _view.Close();
    }

    private Dictionary<string, string> Validate()
    {
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(_view.ProductName))
        {
            errors["ProductName"] = "Name is required";
        }
        else if (_view.ProductName.Length < 3)
        {
            errors["ProductName"] = "Name must be at least 3 characters";
        }

        if (_view.Price <= 0)
        {
            errors["Price"] = "Price must be greater than 0";
        }

        if (_view.StockQuantity < 0)
        {
            errors["StockQuantity"] = "Stock cannot be negative";
        }

        if (_view.SelectedCategoryId <= 0)
        {
            errors["Category"] = "Please select a category";
        }

        return errors;
    }
}
```

### View Implementation (WinForms)

```csharp
public partial class ProductListForm : Form, IProductListView
{
    private List<ProductViewModel> _products = new();
    
    public ProductListForm()
    {
        InitializeComponent();
        WireUpEvents();
    }

    #region IProductListView Implementation

    public IEnumerable<ProductViewModel> Products
    {
        set
        {
            _products = value.ToList();
            productBindingSource.DataSource = _products;
            dataGridView.DataSource = productBindingSource;
        }
    }

    public ProductViewModel? SelectedProduct
    {
        get
        {
            if (dataGridView.CurrentRow?.DataBoundItem is ProductViewModel product)
                return product;
            return null;
        }
    }

    public string SearchText => searchTextBox.Text;

    public bool IsLoading
    {
        set
        {
            progressBar.Visible = value;
            dataGridView.Enabled = !value;
            Cursor = value ? Cursors.WaitCursor : Cursors.Default;
        }
    }

    public string StatusMessage
    {
        set => statusLabel.Text = value;
    }

    public event EventHandler? LoadRequested;
    public event EventHandler? SearchRequested;
    public event EventHandler? AddRequested;
    public event EventHandler? EditRequested;
    public event EventHandler? DeleteRequested;
    public event EventHandler? RefreshRequested;

    public void ShowError(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public void ShowSuccess(string message)
    {
        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public bool ConfirmDelete(string productName)
    {
        var result = MessageBox.Show(
            $"Are you sure you want to delete '{productName}'?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
        
        return result == DialogResult.Yes;
    }

    #endregion

    #region Event Wiring

    private void WireUpEvents()
    {
        Load += (s, e) => LoadRequested?.Invoke(this, EventArgs.Empty);
        
        searchButton.Click += (s, e) => SearchRequested?.Invoke(this, EventArgs.Empty);
        searchTextBox.KeyPress += (s, e) =>
        {
            if (e.KeyChar == (char)Keys.Enter)
                SearchRequested?.Invoke(this, EventArgs.Empty);
        };
        
        addButton.Click += (s, e) => AddRequested?.Invoke(this, EventArgs.Empty);
        editButton.Click += (s, e) => EditRequested?.Invoke(this, EventArgs.Empty);
        deleteButton.Click += (s, e) => DeleteRequested?.Invoke(this, EventArgs.Empty);
        refreshButton.Click += (s, e) => RefreshRequested?.Invoke(this, EventArgs.Empty);
        
        dataGridView.CellDoubleClick += (s, e) => EditRequested?.Invoke(this, EventArgs.Empty);
    }

    #endregion
}

public partial class ProductEditForm : Form, IProductEditView
{
    public ProductEditForm()
    {
        InitializeComponent();
        WireUpEvents();
    }

    #region IProductEditView Implementation

    public int ProductId { get; set; }
    
    public string ProductName
    {
        get => nameTextBox.Text;
        set => nameTextBox.Text = value;
    }

    public string Description
    {
        get => descriptionTextBox.Text;
        set => descriptionTextBox.Text = value;
    }

    public decimal Price
    {
        get => decimal.TryParse(priceTextBox.Text, out var p) ? p : 0;
        set => priceTextBox.Text = value.ToString("F2");
    }

    public int StockQuantity
    {
        get => (int)stockNumericUpDown.Value;
        set => stockNumericUpDown.Value = value;
    }

    public int SelectedCategoryId
    {
        get => categoryComboBox.SelectedValue is int id ? id : 0;
        set => categoryComboBox.SelectedValue = value;
    }

    public IEnumerable<CategoryViewModel> Categories
    {
        set
        {
            categoryComboBox.DataSource = value.ToList();
            categoryComboBox.DisplayMember = "Name";
            categoryComboBox.ValueMember = "Id";
        }
    }

    public bool IsNewProduct
    {
        get => Text.Contains("New");
        set => Text = value ? "New Product" : "Edit Product";
    }

    public Dictionary<string, string> ValidationErrors
    {
        set
        {
            errorProvider.Clear();
            foreach (var (field, error) in value)
            {
                var control = field switch
                {
                    "ProductName" => nameTextBox,
                    "Price" => priceTextBox,
                    "StockQuantity" => (Control)stockNumericUpDown,
                    "Category" => categoryComboBox,
                    _ => null
                };
                
                if (control != null)
                {
                    errorProvider.SetError(control, error);
                }
            }
        }
    }

    public event EventHandler? SaveRequested;
    public event EventHandler? CancelRequested;
    public event EventHandler? LoadRequested;

    public void ShowError(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public new void Close()
    {
        DialogResult = DialogResult.OK;
        base.Close();
    }

    #endregion

    private void WireUpEvents()
    {
        Load += (s, e) => LoadRequested?.Invoke(this, EventArgs.Empty);
        saveButton.Click += (s, e) => SaveRequested?.Invoke(this, EventArgs.Empty);
        cancelButton.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);
    }
}
```

### Unit Testing

```csharp
public class ProductListPresenterTests
{
    private readonly Mock<IProductListView> _viewMock;
    private readonly Mock<IProductService> _serviceMock;
    private readonly ProductListPresenter _presenter;

    public ProductListPresenterTests()
    {
        _viewMock = new Mock<IProductListView>();
        _serviceMock = new Mock<IProductService>();
        _presenter = new ProductListPresenter(_viewMock.Object, _serviceMock.Object);
    }

    [Fact]
    public async Task Load_SetsProductsOnView()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Price = 10 },
            new Product { Id = 2, Name = "Product 2", Price = 20 }
        };
        
        _serviceMock.Setup(s => s.GetAllAsync(null))
            .ReturnsAsync(products);

        // Act
        _viewMock.Raise(v => v.LoadRequested += null, EventArgs.Empty);
        
        // Allow async operation to complete
        await Task.Delay(100);

        // Assert
        _viewMock.VerifySet(v => v.IsLoading = true, Times.Once);
        _viewMock.VerifySet(v => v.Products = It.IsAny<IEnumerable<ProductViewModel>>(), Times.Once);
        _viewMock.VerifySet(v => v.IsLoading = false, Times.Once);
    }

    [Fact]
    public void Delete_WithNoSelection_ShowsError()
    {
        // Arrange
        _viewMock.Setup(v => v.SelectedProduct).Returns((ProductViewModel?)null);

        // Act
        _viewMock.Raise(v => v.DeleteRequested += null, EventArgs.Empty);

        // Assert
        _viewMock.Verify(v => v.ShowError(It.IsAny<string>()), Times.Once);
        _serviceMock.Verify(s => s.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Delete_WithConfirmation_DeletesProduct()
    {
        // Arrange
        var product = new ProductViewModel { Id = 1, Name = "Test Product" };
        _viewMock.Setup(v => v.SelectedProduct).Returns(product);
        _viewMock.Setup(v => v.ConfirmDelete(It.IsAny<string>())).Returns(true);

        // Act
        _viewMock.Raise(v => v.DeleteRequested += null, EventArgs.Empty);
        
        await Task.Delay(100);

        // Assert
        _serviceMock.Verify(s => s.DeleteAsync(1), Times.Once);
        _viewMock.Verify(v => v.ShowSuccess(It.IsAny<string>()), Times.Once);
    }
}
```

### Dependency Injection Setup

```csharp
// Program.cs
static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var services = new ServiceCollection();
        ConfigureServices(services);
        
        using var serviceProvider = services.BuildServiceProvider();
        
        var mainForm = serviceProvider.GetRequiredService<ProductListForm>();
        var presenter = new ProductListPresenter(
            mainForm,
            serviceProvider.GetRequiredService<IProductService>()
        );
        
        Application.Run(mainForm);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("Default")));

        // Services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();

        // Forms
        services.AddTransient<ProductListForm>();
        services.AddTransient<ProductEditForm>();
    }
}
```

## Voordelen

- ? **Maximale testbaarheid**: View volledig mockbaar via interface
- ? **Duidelijke verantwoordelijkheden**: View is passief, Presenter bevat logica
- ? **Platform onafhankelijk**: Presenter kan hergebruikt worden
- ? **Geschikt voor legacy**: Werkt goed met WinForms, Web Forms

## Nadelen

- ? **Veel boilerplate**: Interfaces voor elke View
- ? **Tight coupling**: 1:1 relatie tussen View en Presenter
- ? **Geen data binding**: View updates zijn handmatig
- ? **Complexere code**: Vergeleken met MVVM

## Wanneer MVP Gebruiken?

- ? WinForms applicaties
- ? Legacy Web Forms
- ? Wanneer maximale testbaarheid vereist is
- ? Platforms zonder data binding
- ? Niet ideaal voor: WPF, MAUI (gebruik MVVM)

## MVP vs MVC vs MVVM

| Aspect | MVC | MVP | MVVM |
|--------|-----|-----|------|
| **View** | Actief | Passief | Declaratief |
| **Binding** | Nee | Nee | Ja |
| **Testbaarheid** | Medium | Hoog | Hoog |
| **Boilerplate** | Laag | Hoog | Medium |
