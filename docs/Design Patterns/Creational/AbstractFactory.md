# Abstract Factory Pattern

## Intentie
Biedt een interface voor het creëren van **families van gerelateerde objecten** zonder hun concrete classes te specificeren.

## Wanneer gebruiken?
- Wanneer je systeem onafhankelijk moet zijn van hoe producten worden gecreëerd
- Wanneer je werkt met families van gerelateerde producten
- Wanneer je wilt garanderen dat producten uit dezelfde familie samen worden gebruikt
- Cross-platform UI components, database providers, etc.

## Structuur

```
????????????????????????
?   AbstractFactory    ?
????????????????????????
? +CreateProductA()    ?
? +CreateProductB()    ?
????????????????????????
          ?
    ?????????????
    ?           ?
?????????   ?????????
?Factory?   ?Factory?
?   1   ?   ?   2   ?
?????????   ?????????
```

## Implementatie in C#

### Basis Implementatie: UI Themes

```csharp
// Abstract Products
public interface IButton
{
    void Render();
    void OnClick();
}

public interface ITextBox
{
    void Render();
    string GetText();
}

public interface ICheckBox
{
    void Render();
    bool IsChecked();
}

// Concrete Products - Light Theme
public class LightButton : IButton
{
    public void Render() => Console.WriteLine("Rendering light-themed button");
    public void OnClick() => Console.WriteLine("Light button clicked");
}

public class LightTextBox : ITextBox
{
    public void Render() => Console.WriteLine("Rendering light-themed textbox");
    public string GetText() => "Light text";
}

public class LightCheckBox : ICheckBox
{
    public void Render() => Console.WriteLine("Rendering light-themed checkbox");
    public bool IsChecked() => true;
}

// Concrete Products - Dark Theme
public class DarkButton : IButton
{
    public void Render() => Console.WriteLine("Rendering dark-themed button");
    public void OnClick() => Console.WriteLine("Dark button clicked");
}

public class DarkTextBox : ITextBox
{
    public void Render() => Console.WriteLine("Rendering dark-themed textbox");
    public string GetText() => "Dark text";
}

public class DarkCheckBox : ICheckBox
{
    public void Render() => Console.WriteLine("Rendering dark-themed checkbox");
    public bool IsChecked() => false;
}

// Abstract Factory
public interface IUIFactory
{
    IButton CreateButton();
    ITextBox CreateTextBox();
    ICheckBox CreateCheckBox();
}

// Concrete Factories
public class LightThemeFactory : IUIFactory
{
    public IButton CreateButton() => new LightButton();
    public ITextBox CreateTextBox() => new LightTextBox();
    public ICheckBox CreateCheckBox() => new LightCheckBox();
}

public class DarkThemeFactory : IUIFactory
{
    public IButton CreateButton() => new DarkButton();
    public ITextBox CreateTextBox() => new DarkTextBox();
    public ICheckBox CreateCheckBox() => new DarkCheckBox();
}

// Client code
public class Application
{
    private readonly IButton _button;
    private readonly ITextBox _textBox;
    private readonly ICheckBox _checkBox;

    public Application(IUIFactory factory)
    {
        _button = factory.CreateButton();
        _textBox = factory.CreateTextBox();
        _checkBox = factory.CreateCheckBox();
    }

    public void Render()
    {
        _button.Render();
        _textBox.Render();
        _checkBox.Render();
    }
}

// Gebruik
IUIFactory factory = new DarkThemeFactory();
var app = new Application(factory);
app.Render();
```

### Praktisch Voorbeeld: Cross-Platform Database Access

```csharp
// Abstract Products
public interface IConnection
{
    void Open();
    void Close();
}

public interface ICommand
{
    void Execute(string sql);
}

public interface IDataReader
{
    bool Read();
    object GetValue(int index);
}

// SQL Server Products
public class SqlServerConnection : IConnection
{
    public void Open() => Console.WriteLine("Opening SQL Server connection");
    public void Close() => Console.WriteLine("Closing SQL Server connection");
}

public class SqlServerCommand : ICommand
{
    public void Execute(string sql) => Console.WriteLine($"SQL Server executing: {sql}");
}

public class SqlServerDataReader : IDataReader
{
    public bool Read() => true;
    public object GetValue(int index) => $"SqlServer_Value_{index}";
}

// PostgreSQL Products
public class PostgreSqlConnection : IConnection
{
    public void Open() => Console.WriteLine("Opening PostgreSQL connection");
    public void Close() => Console.WriteLine("Closing PostgreSQL connection");
}

public class PostgreSqlCommand : ICommand
{
    public void Execute(string sql) => Console.WriteLine($"PostgreSQL executing: {sql}");
}

public class PostgreSqlDataReader : IDataReader
{
    public bool Read() => true;
    public object GetValue(int index) => $"PostgreSQL_Value_{index}";
}

// Abstract Factory
public interface IDatabaseFactory
{
    IConnection CreateConnection();
    ICommand CreateCommand();
    IDataReader CreateDataReader();
}

// Concrete Factories
public class SqlServerFactory : IDatabaseFactory
{
    public IConnection CreateConnection() => new SqlServerConnection();
    public ICommand CreateCommand() => new SqlServerCommand();
    public IDataReader CreateDataReader() => new SqlServerDataReader();
}

public class PostgreSqlFactory : IDatabaseFactory
{
    public IConnection CreateConnection() => new PostgreSqlConnection();
    public ICommand CreateCommand() => new PostgreSqlCommand();
    public IDataReader CreateDataReader() => new PostgreSqlDataReader();
}

// Repository using the factory
public class UserRepository
{
    private readonly IDatabaseFactory _dbFactory;

    public UserRepository(IDatabaseFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public void GetUsers()
    {
        var connection = _dbFactory.CreateConnection();
        var command = _dbFactory.CreateCommand();
        var reader = _dbFactory.CreateDataReader();

        connection.Open();
        command.Execute("SELECT * FROM Users");
        
        while (reader.Read())
        {
            Console.WriteLine(reader.GetValue(0));
        }
        
        connection.Close();
    }
}

// Factory Provider based on configuration
public static class DatabaseFactoryProvider
{
    public static IDatabaseFactory GetFactory(string provider)
    {
        return provider.ToLower() switch
        {
            "sqlserver" => new SqlServerFactory(),
            "postgresql" => new PostgreSqlFactory(),
            _ => throw new ArgumentException($"Unknown provider: {provider}")
        };
    }
}

// Gebruik
var factory = DatabaseFactoryProvider.GetFactory("postgresql");
var repository = new UserRepository(factory);
repository.GetUsers();
```

### Met Dependency Injection

```csharp
// DI Registration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseFactory(
        this IServiceCollection services, 
        string provider)
    {
        IDatabaseFactory factory = provider.ToLower() switch
        {
            "sqlserver" => new SqlServerFactory(),
            "postgresql" => new PostgreSqlFactory(),
            _ => throw new ArgumentException($"Unknown provider: {provider}")
        };

        services.AddSingleton(factory);
        return services;
    }
}

// In Startup/Program.cs
var provider = configuration["Database:Provider"];
services.AddDatabaseFactory(provider);
services.AddScoped<UserRepository>();
```

## Praktisch Voorbeeld: Document Export

```csharp
// Abstract Products
public interface IDocumentWriter
{
    void WriteHeader(string title);
    void WriteParagraph(string text);
    void WriteTable(string[,] data);
    byte[] GetOutput();
}

public interface IStyleSheet
{
    string HeaderStyle { get; }
    string ParagraphStyle { get; }
    string TableStyle { get; }
}

// PDF Products
public class PdfWriter : IDocumentWriter
{
    public void WriteHeader(string title) => Console.WriteLine($"PDF Header: {title}");
    public void WriteParagraph(string text) => Console.WriteLine($"PDF Paragraph: {text}");
    public void WriteTable(string[,] data) => Console.WriteLine("PDF Table");
    public byte[] GetOutput() => new byte[] { 0x25, 0x50, 0x44, 0x46 }; // %PDF
}

public class PdfStyleSheet : IStyleSheet
{
    public string HeaderStyle => "PDF-Header-Bold-24pt";
    public string ParagraphStyle => "PDF-Body-12pt";
    public string TableStyle => "PDF-Table-Border";
}

// HTML Products
public class HtmlWriter : IDocumentWriter
{
    public void WriteHeader(string title) => Console.WriteLine($"<h1>{title}</h1>");
    public void WriteParagraph(string text) => Console.WriteLine($"<p>{text}</p>");
    public void WriteTable(string[,] data) => Console.WriteLine("<table>...</table>");
    public byte[] GetOutput() => System.Text.Encoding.UTF8.GetBytes("<html>...</html>");
}

public class HtmlStyleSheet : IStyleSheet
{
    public string HeaderStyle => "h1 { font-size: 24px; font-weight: bold; }";
    public string ParagraphStyle => "p { font-size: 12px; }";
    public string TableStyle => "table { border: 1px solid black; }";
}

// Abstract Factory
public interface IDocumentExportFactory
{
    IDocumentWriter CreateWriter();
    IStyleSheet CreateStyleSheet();
}

// Concrete Factories
public class PdfExportFactory : IDocumentExportFactory
{
    public IDocumentWriter CreateWriter() => new PdfWriter();
    public IStyleSheet CreateStyleSheet() => new PdfStyleSheet();
}

public class HtmlExportFactory : IDocumentExportFactory
{
    public IDocumentWriter CreateWriter() => new HtmlWriter();
    public IStyleSheet CreateStyleSheet() => new HtmlStyleSheet();
}

// Client
public class ReportGenerator
{
    private readonly IDocumentExportFactory _factory;

    public ReportGenerator(IDocumentExportFactory factory)
    {
        _factory = factory;
    }

    public byte[] GenerateReport(ReportData data)
    {
        var writer = _factory.CreateWriter();
        var styles = _factory.CreateStyleSheet();

        Console.WriteLine($"Using style: {styles.HeaderStyle}");
        
        writer.WriteHeader(data.Title);
        writer.WriteParagraph(data.Summary);
        writer.WriteTable(data.TableData);

        return writer.GetOutput();
    }
}

public record ReportData(string Title, string Summary, string[,] TableData);
```

## Voordelen
- ? Garandeert compatibiliteit tussen producten van dezelfde familie
- ? Vermijdt tight coupling tussen client en concrete products
- ? Single Responsibility: product creatie op één plek
- ? Open/Closed: nieuwe families toevoegen zonder bestaande code te wijzigen

## Nadelen
- ? Complexiteit: veel interfaces en classes
- ? Moeilijk om nieuwe product types toe te voegen (alle factories moeten aangepast)

## Verschil met Factory Method

| Aspect | Factory Method | Abstract Factory |
|--------|---------------|------------------|
| **Focus** | Eén product | Familie van producten |
| **Inheritance** | Subclassing | Object compositie |
| **Flexibiliteit** | Eén variatiepunt | Meerdere variatiepunten |

## Gerelateerde Patterns
- **Factory Method**: Abstract Factory gebruikt vaak Factory Methods
- **Singleton**: Concrete factories zijn vaak Singletons
- **Prototype**: Alternatief wanneer families dynamisch moeten zijn
