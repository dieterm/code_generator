# MVVM (Model-View-ViewModel)

## Overzicht

MVVM is een architectural pattern ontwikkeld door Microsoft voor WPF. Het maakt gebruik van **data binding** om View en ViewModel te synchroniseren, waardoor de View declaratief kan worden gedefinieerd.

## Structuur

```
???????????????????????????????????????????????????????????????
?                         USER                                 ?
???????????????????????????????????????????????????????????????
                    ?                       ?
                    ? User Interaction      ? Display
                    ?                       ?
???????????????????????????????????????????????????????????????
?                        VIEW                                  ?
?  • XAML/Razor declaratieve UI                               ?
?  • Data bindings naar ViewModel                             ?
?  • Commands voor user actions                               ?
?  • Geen code-behind (ideaal)                                ?
???????????????????????????????????????????????????????????????
                    ?                       ?
                    ? Data Binding          ? INotifyPropertyChanged
                    ? Commands              ? 
                    ?                       ?
?????????????????????????????????????????????????????????????
?                     VIEWMODEL                              ?
?  • Observable properties                                   ?
?  • Commands voor actions                                   ?
?  • Presentation logic                                      ?
?  • Converteert Model data voor View                        ?
?????????????????????????????????????????????????????????????
                    ?                       ?
                    ? Requests              ? Data
                    ?                       ?
?????????????????????????????????????????????????????????????
?                        MODEL                               ?
?  • Business logic                                          ?
?  • Domain entities                                         ?
?  • Services                                                ?
?????????????????????????????????????????????????????????????
```

## Componenten

### Model
- **Business logic** en **domain entities**
- Onafhankelijk van UI
- Services, repositories, domain models

### View
- **Declaratieve UI** (XAML, Razor, HTML)
- Bindt aan ViewModel properties
- Geen business logic
- Minimale code-behind

### ViewModel
- **Presentation logic**
- Implementeert `INotifyPropertyChanged`
- Exposed **observable properties** en **commands**
- Converteert Model data voor View

## WPF/MAUI Implementatie

### Project Structuur

```
MyApp/
??? Models/
?   ??? Product.cs
?   ??? Category.cs
??? ViewModels/
?   ??? Base/
?   ?   ??? ViewModelBase.cs
?   ?   ??? RelayCommand.cs
?   ??? ProductListViewModel.cs
?   ??? ProductEditViewModel.cs
??? Views/
?   ??? ProductListView.xaml
?   ??? ProductEditView.xaml
??? Services/
?   ??? IProductService.cs
?   ??? ProductService.cs
?   ??? INavigationService.cs
?   ??? NavigationService.cs
??? App.xaml
```

### Base Classes

```csharp
// ViewModelBase.cs
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

// RelayCommand.cs
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => _execute(parameter);
}

// AsyncRelayCommand.cs
public class AsyncRelayCommand : ICommand
{
    private readonly Func<object?, Task> _execute;
    private readonly Func<object?, bool>? _canExecute;
    private bool _isExecuting;

    public AsyncRelayCommand(Func<object?, Task> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) => !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);

    public async void Execute(object? parameter)
    {
        if (_isExecuting) return;

        _isExecuting = true;
        try
        {
            await _execute(parameter);
        }
        finally
        {
            _isExecuting = false;
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
```

### ViewModel

```csharp
public class ProductListViewModel : ViewModelBase
{
    private readonly IProductService _productService;
    private readonly INavigationService _navigationService;
    
    private ObservableCollection<ProductItemViewModel> _products = new();
    private ProductItemViewModel? _selectedProduct;
    private string _searchText = string.Empty;
    private bool _isLoading;
    private string _statusMessage = string.Empty;

    public ProductListViewModel(IProductService productService, INavigationService navigationService)
    {
        _productService = productService;
        _navigationService = navigationService;
        
        // Initialize commands
        LoadCommand = new AsyncRelayCommand(async _ => await LoadProductsAsync());
        SearchCommand = new AsyncRelayCommand(async _ => await LoadProductsAsync());
        AddCommand = new RelayCommand(_ => NavigateToEdit(null));
        EditCommand = new RelayCommand(_ => NavigateToEdit(SelectedProduct?.Id), _ => SelectedProduct != null);
        DeleteCommand = new AsyncRelayCommand(async _ => await DeleteSelectedAsync(), _ => SelectedProduct != null);
        RefreshCommand = new AsyncRelayCommand(async _ => await LoadProductsAsync());
    }

    #region Properties

    public ObservableCollection<ProductItemViewModel> Products
    {
        get => _products;
        set => SetProperty(ref _products, value);
    }

    public ProductItemViewModel? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (SetProperty(ref _selectedProduct, value))
            {
                // Commands herevalueren
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    #endregion

    #region Commands

    public ICommand LoadCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand RefreshCommand { get; }

    #endregion

    #region Methods

    private async Task LoadProductsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading products...";

            var searchTerm = string.IsNullOrWhiteSpace(SearchText) ? null : SearchText;
            var products = await _productService.GetAllAsync(searchTerm);

            Products = new ObservableCollection<ProductItemViewModel>(
                products.Select(p => new ProductItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryName = p.Category?.Name ?? "Unknown"
                }));

            StatusMessage = $"Loaded {Products.Count} products";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void NavigateToEdit(int? productId)
    {
        _navigationService.NavigateTo<ProductEditViewModel>(productId);
    }

    private async Task DeleteSelectedAsync()
    {
        if (SelectedProduct == null) return;

        var confirmed = await _navigationService.ShowConfirmationAsync(
            "Delete Product",
            $"Are you sure you want to delete '{SelectedProduct.Name}'?");

        if (!confirmed) return;

        try
        {
            IsLoading = true;
            await _productService.DeleteAsync(SelectedProduct.Id);
            Products.Remove(SelectedProduct);
            StatusMessage = "Product deleted successfully";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deleting: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion
}

// ProductItemViewModel - voor items in de lijst
public class ProductItemViewModel : ViewModelBase
{
    private int _id;
    private string _name = string.Empty;
    private decimal _price;
    private int _stockQuantity;
    private string _categoryName = string.Empty;

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public decimal Price
    {
        get => _price;
        set => SetProperty(ref _price, value);
    }

    public int StockQuantity
    {
        get => _stockQuantity;
        set => SetProperty(ref _stockQuantity, value);
    }

    public string CategoryName
    {
        get => _categoryName;
        set => SetProperty(ref _categoryName, value);
    }

    // Computed properties
    public string FormattedPrice => Price.ToString("C");
    
    public string StockStatus => StockQuantity switch
    {
        > 10 => "In Stock",
        > 0 => "Low Stock",
        _ => "Out of Stock"
    };

    public string StockStatusColor => StockQuantity switch
    {
        > 10 => "Green",
        > 0 => "Orange",
        _ => "Red"
    };
}
```

### Edit ViewModel

```csharp
public class ProductEditViewModel : ViewModelBase
{
    private readonly IProductService _productService;
    private readonly INavigationService _navigationService;
    private readonly int? _productId;

    private string _name = string.Empty;
    private string _description = string.Empty;
    private decimal _price;
    private int _stockQuantity;
    private int _selectedCategoryId;
    private ObservableCollection<CategoryViewModel> _categories = new();
    private bool _isLoading;
    private Dictionary<string, string> _errors = new();

    public ProductEditViewModel(
        IProductService productService, 
        INavigationService navigationService,
        int? productId = null)
    {
        _productService = productService;
        _navigationService = navigationService;
        _productId = productId;

        LoadCommand = new AsyncRelayCommand(async _ => await LoadAsync());
        SaveCommand = new AsyncRelayCommand(async _ => await SaveAsync(), _ => CanSave());
        CancelCommand = new RelayCommand(_ => _navigationService.GoBack());
    }

    #region Properties

    public bool IsNewProduct => !_productId.HasValue;
    public string Title => IsNewProduct ? "New Product" : "Edit Product";

    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value))
            {
                ValidateProperty(nameof(Name));
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (SetProperty(ref _price, value))
            {
                ValidateProperty(nameof(Price));
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public int StockQuantity
    {
        get => _stockQuantity;
        set
        {
            if (SetProperty(ref _stockQuantity, value))
            {
                ValidateProperty(nameof(StockQuantity));
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public int SelectedCategoryId
    {
        get => _selectedCategoryId;
        set
        {
            if (SetProperty(ref _selectedCategoryId, value))
            {
                ValidateProperty(nameof(SelectedCategoryId));
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public ObservableCollection<CategoryViewModel> Categories
    {
        get => _categories;
        set => SetProperty(ref _categories, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool HasErrors => _errors.Any();

    public string? GetError(string propertyName) => 
        _errors.TryGetValue(propertyName, out var error) ? error : null;

    #endregion

    #region Commands

    public ICommand LoadCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    #endregion

    #region Methods

    private async Task LoadAsync()
    {
        try
        {
            IsLoading = true;

            // Load categories
            var categories = await _productService.GetCategoriesAsync();
            Categories = new ObservableCollection<CategoryViewModel>(
                categories.Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name }));

            // Load product if editing
            if (_productId.HasValue)
            {
                var product = await _productService.GetByIdAsync(_productId.Value);
                if (product != null)
                {
                    Name = product.Name;
                    Description = product.Description;
                    Price = product.Price;
                    StockQuantity = product.StockQuantity;
                    SelectedCategoryId = product.CategoryId;
                }
            }
        }
        catch (Exception ex)
        {
            await _navigationService.ShowErrorAsync("Error loading data", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SaveAsync()
    {
        if (!ValidateAll())
        {
            return;
        }

        try
        {
            IsLoading = true;

            var product = new Product
            {
                Id = _productId ?? 0,
                Name = Name,
                Description = Description,
                Price = Price,
                StockQuantity = StockQuantity,
                CategoryId = SelectedCategoryId
            };

            if (IsNewProduct)
            {
                await _productService.CreateAsync(product);
            }
            else
            {
                await _productService.UpdateAsync(product);
            }

            _navigationService.GoBack();
        }
        catch (Exception ex)
        {
            await _navigationService.ShowErrorAsync("Error saving product", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanSave() => !HasErrors && !string.IsNullOrWhiteSpace(Name);

    private void ValidateProperty(string propertyName)
    {
        _errors.Remove(propertyName);

        var error = propertyName switch
        {
            nameof(Name) when string.IsNullOrWhiteSpace(Name) => "Name is required",
            nameof(Name) when Name.Length < 3 => "Name must be at least 3 characters",
            nameof(Price) when Price <= 0 => "Price must be greater than 0",
            nameof(StockQuantity) when StockQuantity < 0 => "Stock cannot be negative",
            nameof(SelectedCategoryId) when SelectedCategoryId <= 0 => "Please select a category",
            _ => null
        };

        if (error != null)
        {
            _errors[propertyName] = error;
        }

        OnPropertyChanged(nameof(HasErrors));
    }

    private bool ValidateAll()
    {
        ValidateProperty(nameof(Name));
        ValidateProperty(nameof(Price));
        ValidateProperty(nameof(StockQuantity));
        ValidateProperty(nameof(SelectedCategoryId));
        return !HasErrors;
    }

    #endregion
}

public class CategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

### View (XAML)

```xml
<!-- ProductListView.xaml -->
<UserControl x:Class="MyApp.Views.ProductListView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:vm="clr-namespace:MyApp.ViewModels"
             mc:Ignorable="d"
             d:DataContext="{d:DesignInstance Type=vm:ProductListViewModel}">

    <UserControl.InputBindings>
        <KeyBinding Key="F5" Command="{Binding RefreshCommand}" />
        <KeyBinding Key="N" Modifiers="Ctrl" Command="{Binding AddCommand}" />
    </UserControl.InputBindings>

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>

        <!-- Toolbar -->
        <ToolBar Grid.Row="0">
            <TextBox Width="200" 
                     Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}"
                     ToolTip="Search products" />
            <Button Command="{Binding SearchCommand}" Content="?? Search" />
            <Separator />
            <Button Command="{Binding AddCommand}" Content="? Add" />
            <Button Command="{Binding EditCommand}" Content="?? Edit" />
            <Button Command="{Binding DeleteCommand}" Content="??? Delete" />
            <Separator />
            <Button Command="{Binding RefreshCommand}" Content="?? Refresh" />
        </ToolBar>

        <!-- Products Grid -->
        <DataGrid Grid.Row="1"
                  ItemsSource="{Binding Products}"
                  SelectedItem="{Binding SelectedProduct}"
                  AutoGenerateColumns="False"
                  IsReadOnly="True"
                  SelectionMode="Single">
            
            <DataGrid.Columns>
                <DataGridTextColumn Header="Name" Binding="{Binding Name}" Width="*" />
                <DataGridTextColumn Header="Price" Binding="{Binding FormattedPrice}" Width="100" />
                <DataGridTextColumn Header="Stock" Binding="{Binding StockQuantity}" Width="80" />
                <DataGridTemplateColumn Header="Status" Width="100">
                    <DataGridTemplateColumn.CellTemplate>
                        <DataTemplate>
                            <Border Background="{Binding StockStatusColor}" 
                                    CornerRadius="3" Padding="5,2">
                                <TextBlock Text="{Binding StockStatus}" 
                                           Foreground="White" 
                                           FontWeight="Bold" />
                            </Border>
                        </DataTemplate>
                    </DataGridTemplateColumn.CellTemplate>
                </DataGridTemplateColumn>
                <DataGridTextColumn Header="Category" Binding="{Binding CategoryName}" Width="120" />
            </DataGrid.Columns>

            <DataGrid.InputBindings>
                <MouseBinding MouseAction="LeftDoubleClick" Command="{Binding EditCommand}" />
            </DataGrid.InputBindings>
        </DataGrid>

        <!-- Status Bar -->
        <StatusBar Grid.Row="2">
            <StatusBarItem>
                <TextBlock Text="{Binding StatusMessage}" />
            </StatusBarItem>
            <StatusBarItem HorizontalAlignment="Right">
                <ProgressBar Width="100" Height="15"
                             IsIndeterminate="{Binding IsLoading}"
                             Visibility="{Binding IsLoading, Converter={StaticResource BoolToVisibility}}" />
            </StatusBarItem>
        </StatusBar>

        <!-- Loading Overlay -->
        <Border Grid.RowSpan="3" 
                Background="#80000000"
                Visibility="{Binding IsLoading, Converter={StaticResource BoolToVisibility}}">
            <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">
                <ProgressBar IsIndeterminate="True" Width="200" Height="20" />
                <TextBlock Text="Loading..." Foreground="White" Margin="0,10,0,0" />
            </StackPanel>
        </Border>
    </Grid>
</UserControl>
```

```xml
<!-- ProductEditView.xaml -->
<UserControl x:Class="MyApp.Views.ProductEditView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="clr-namespace:MyApp.ViewModels"
             d:DataContext="{d:DesignInstance Type=vm:ProductEditViewModel}">

    <Grid Margin="20">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>

        <!-- Header -->
        <TextBlock Grid.Row="0" 
                   Text="{Binding Title}" 
                   FontSize="24" 
                   FontWeight="Bold" 
                   Margin="0,0,0,20" />

        <!-- Form -->
        <ScrollViewer Grid.Row="1" VerticalScrollBarVisibility="Auto">
            <StackPanel MaxWidth="500">
                
                <!-- Name -->
                <Label Content="Name *" />
                <TextBox Text="{Binding Name, UpdateSourceTrigger=PropertyChanged, ValidatesOnDataErrors=True}" />
                <TextBlock Text="{Binding (Validation.Errors)[0].ErrorContent, RelativeSource={RelativeSource Self}}"
                           Foreground="Red" FontSize="11" Margin="0,2,0,10" />

                <!-- Description -->
                <Label Content="Description" />
                <TextBox Text="{Binding Description}" 
                         TextWrapping="Wrap" 
                         AcceptsReturn="True" 
                         Height="80" 
                         Margin="0,0,0,10" />

                <!-- Price -->
                <Label Content="Price *" />
                <TextBox Text="{Binding Price, UpdateSourceTrigger=PropertyChanged, StringFormat=F2}" />
                <TextBlock Foreground="Red" FontSize="11" Margin="0,2,0,10" />

                <!-- Stock -->
                <Label Content="Stock Quantity *" />
                <TextBox Text="{Binding StockQuantity, UpdateSourceTrigger=PropertyChanged}" />
                <TextBlock Foreground="Red" FontSize="11" Margin="0,2,0,10" />

                <!-- Category -->
                <Label Content="Category *" />
                <ComboBox ItemsSource="{Binding Categories}"
                          SelectedValue="{Binding SelectedCategoryId}"
                          SelectedValuePath="Id"
                          DisplayMemberPath="Name"
                          Margin="0,0,0,10" />

            </StackPanel>
        </ScrollViewer>

        <!-- Buttons -->
        <StackPanel Grid.Row="2" Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,20,0,0">
            <Button Content="Save" 
                    Command="{Binding SaveCommand}" 
                    Width="100" 
                    IsDefault="True" />
            <Button Content="Cancel" 
                    Command="{Binding CancelCommand}" 
                    Width="100" 
                    Margin="10,0,0,0"
                    IsCancel="True" />
        </StackPanel>
    </Grid>
</UserControl>
```

### Code-Behind (Minimaal)

```csharp
// ProductListView.xaml.cs
public partial class ProductListView : UserControl
{
    public ProductListView()
    {
        InitializeComponent();
        
        // Load data when control is loaded
        Loaded += (s, e) =>
        {
            if (DataContext is ProductListViewModel vm)
            {
                vm.LoadCommand.Execute(null);
            }
        };
    }
}
```

### Dependency Injection Setup (WPF)

```csharp
// App.xaml.cs
public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Database
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(context.Configuration.GetConnectionString("Default")));

                // Services
                services.AddScoped<IProductService, ProductService>();
                services.AddScoped<INavigationService, NavigationService>();

                // ViewModels
                services.AddTransient<ProductListViewModel>();
                services.AddTransient<ProductEditViewModel>();

                // Views
                services.AddTransient<MainWindow>();
                services.AddTransient<ProductListView>();
                services.AddTransient<ProductEditView>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
    }
}
```

## CommunityToolkit.Mvvm

Modern MVVM met minder boilerplate using source generators:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class ProductListViewModel : ObservableObject
{
    private readonly IProductService _productService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    private ProductItemViewModel? _selectedProduct;

    [ObservableProperty]
    private ObservableCollection<ProductItemViewModel> _products = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    public ProductListViewModel(IProductService productService)
    {
        _productService = productService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        try
        {
            IsLoading = true;
            var products = await _productService.GetAllAsync(SearchText);
            Products = new ObservableCollection<ProductItemViewModel>(
                products.Select(MapToViewModel));
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync() => await LoadAsync();

    [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
    private void Edit()
    {
        // Navigate to edit
    }

    [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
    private async Task DeleteAsync()
    {
        if (SelectedProduct == null) return;
        await _productService.DeleteAsync(SelectedProduct.Id);
        Products.Remove(SelectedProduct);
    }

    private bool CanEditOrDelete() => SelectedProduct != null;

    private static ProductItemViewModel MapToViewModel(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price
    };
}
```

## Unit Testing

```csharp
public class ProductListViewModelTests
{
    private readonly Mock<IProductService> _serviceMock;
    private readonly Mock<INavigationService> _navMock;
    private readonly ProductListViewModel _viewModel;

    public ProductListViewModelTests()
    {
        _serviceMock = new Mock<IProductService>();
        _navMock = new Mock<INavigationService>();
        _viewModel = new ProductListViewModel(_serviceMock.Object, _navMock.Object);
    }

    [Fact]
    public async Task LoadCommand_PopulatesProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Product 1" },
            new() { Id = 2, Name = "Product 2" }
        };
        _serviceMock.Setup(s => s.GetAllAsync(null)).ReturnsAsync(products);

        // Act
        _viewModel.LoadCommand.Execute(null);
        await Task.Delay(100); // Wait for async

        // Assert
        Assert.Equal(2, _viewModel.Products.Count);
    }

    [Fact]
    public void EditCommand_CanExecute_WhenProductSelected()
    {
        // Arrange
        _viewModel.SelectedProduct = null;
        Assert.False(_viewModel.EditCommand.CanExecute(null));

        // Act
        _viewModel.SelectedProduct = new ProductItemViewModel { Id = 1 };

        // Assert
        Assert.True(_viewModel.EditCommand.CanExecute(null));
    }

    [Fact]
    public async Task DeleteCommand_RemovesProductFromCollection()
    {
        // Arrange
        var product = new ProductItemViewModel { Id = 1, Name = "Test" };
        _viewModel.Products.Add(product);
        _viewModel.SelectedProduct = product;
        _navMock.Setup(n => n.ShowConfirmationAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        _viewModel.DeleteCommand.Execute(null);
        await Task.Delay(100);

        // Assert
        Assert.Empty(_viewModel.Products);
        _serviceMock.Verify(s => s.DeleteAsync(1), Times.Once);
    }
}
```

## Voordelen

- ? **Data Binding**: Automatische UI synchronisatie
- ? **Declaratieve UI**: XAML is leesbaar en designer-friendly
- ? **Testbaarheid**: ViewModel volledig testbaar
- ? **Separation of Concerns**: Duidelijke scheiding
- ? **Design-time support**: Preview in Visual Studio/Blend

## Nadelen

- ? **Learning curve**: Data binding concepten
- ? **Debugging**: Binding errors kunnen lastig zijn
- ? **Memory leaks**: Bij verkeerd gebruik van events
- ? **Overkill**: Voor simpele applicaties

## Wanneer MVVM Gebruiken?

- ? WPF applicaties
- ? .NET MAUI / Xamarin
- ? Blazor (met aanpassingen)
- ? UWP / WinUI
- ? Niet ideaal voor: Simpele console apps, Web APIs

## MVVM Frameworks

| Framework | Features |
|-----------|----------|
| **CommunityToolkit.Mvvm** | Source generators, lightweight |
| **Prism** | Navigation, Dialogs, Modules |
| **ReactiveUI** | Reactive programming |
| **Caliburn.Micro** | Convention-based binding |
