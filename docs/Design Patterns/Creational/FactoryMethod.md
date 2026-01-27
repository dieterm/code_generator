# Factory Method Pattern

## Intentie
Definieert een interface voor het creëren van objecten, maar laat **subclasses beslissen** welke class te instantiëren. Factory Method laat een class instantiatie uitstellen naar subclasses.

## Wanneer gebruiken?
- Wanneer je niet van tevoren weet welk exact type object je moet creëren
- Wanneer je de creatie logica wilt delegeren aan subclasses
- Wanneer je een framework bouwt dat uitgebreid moet kunnen worden

## Structuur

```
?????????????????????         ?????????????????????
?     Creator       ?         ?     Product       ?
?????????????????????         ?????????????????????
?                   ???????????                   ?
? + FactoryMethod() ?         ? + Operation()     ?
? + SomeOperation() ?         ?????????????????????
?????????????????????                   ?
         ?                              ?
         ?                    ?????????????????????
???????????????????          ?                   ?
? ConcreteCreator ?    ?????????????   ???????????????????
???????????????????    ?ConcreteA  ?   ?  ConcreteB      ?
? +FactoryMethod()?    ?????????????   ???????????????????
???????????????????
```

## Implementatie in C#

### Basis Implementatie

```csharp
// Product interface
public interface IDocument
{
    void Open();
    void Save();
    void Close();
}

// Concrete Products
public class WordDocument : IDocument
{
    public void Open() => Console.WriteLine("Opening Word document");
    public void Save() => Console.WriteLine("Saving Word document");
    public void Close() => Console.WriteLine("Closing Word document");
}

public class PdfDocument : IDocument
{
    public void Open() => Console.WriteLine("Opening PDF document");
    public void Save() => Console.WriteLine("Saving PDF document");
    public void Close() => Console.WriteLine("Closing PDF document");
}

public class ExcelDocument : IDocument
{
    public void Open() => Console.WriteLine("Opening Excel document");
    public void Save() => Console.WriteLine("Saving Excel document");
    public void Close() => Console.WriteLine("Closing Excel document");
}

// Creator (abstract)
public abstract class DocumentCreator
{
    // Factory Method
    public abstract IDocument CreateDocument();

    // Template method die de factory method gebruikt
    public void OpenDocument()
    {
        var doc = CreateDocument();
        doc.Open();
    }
}

// Concrete Creators
public class WordDocumentCreator : DocumentCreator
{
    public override IDocument CreateDocument() => new WordDocument();
}

public class PdfDocumentCreator : DocumentCreator
{
    public override IDocument CreateDocument() => new PdfDocument();
}

public class ExcelDocumentCreator : DocumentCreator
{
    public override IDocument CreateDocument() => new ExcelDocument();
}

// Gebruik
DocumentCreator creator = new WordDocumentCreator();
IDocument doc = creator.CreateDocument();
doc.Open();
```

### Parameterized Factory Method

```csharp
public enum DocumentType
{
    Word,
    Pdf,
    Excel
}

public class DocumentFactory
{
    public IDocument CreateDocument(DocumentType type)
    {
        return type switch
        {
            DocumentType.Word => new WordDocument(),
            DocumentType.Pdf => new PdfDocument(),
            DocumentType.Excel => new ExcelDocument(),
            _ => throw new ArgumentException($"Unknown document type: {type}")
        };
    }
}

// Gebruik
var factory = new DocumentFactory();
var doc = factory.CreateDocument(DocumentType.Pdf);
doc.Open();
```

### Met Dependency Injection

```csharp
// Factory interface
public interface IDocumentFactory
{
    IDocument Create(string extension);
}

// Factory implementation
public class DocumentFactory : IDocumentFactory
{
    private readonly IServiceProvider _serviceProvider;

    public DocumentFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IDocument Create(string extension)
    {
        return extension.ToLower() switch
        {
            ".docx" => _serviceProvider.GetRequiredService<WordDocument>(),
            ".pdf" => _serviceProvider.GetRequiredService<PdfDocument>(),
            ".xlsx" => _serviceProvider.GetRequiredService<ExcelDocument>(),
            _ => throw new ArgumentException($"Unsupported extension: {extension}")
        };
    }
}

// DI Registration
services.AddTransient<WordDocument>();
services.AddTransient<PdfDocument>();
services.AddTransient<ExcelDocument>();
services.AddSingleton<IDocumentFactory, DocumentFactory>();

// Gebruik
public class DocumentService
{
    private readonly IDocumentFactory _factory;

    public DocumentService(IDocumentFactory factory)
    {
        _factory = factory;
    }

    public void ProcessFile(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        var document = _factory.Create(extension);
        document.Open();
    }
}
```

## Praktisch Voorbeeld: Database Connections

```csharp
public interface IDbConnection
{
    void Connect();
    void Execute(string query);
    void Disconnect();
}

public class SqlServerConnection : IDbConnection
{
    private readonly string _connectionString;

    public SqlServerConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Connect() => Console.WriteLine($"Connecting to SQL Server: {_connectionString}");
    public void Execute(string query) => Console.WriteLine($"Executing on SQL Server: {query}");
    public void Disconnect() => Console.WriteLine("Disconnecting from SQL Server");
}

public class MySqlConnection : IDbConnection
{
    private readonly string _connectionString;

    public MySqlConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Connect() => Console.WriteLine($"Connecting to MySQL: {_connectionString}");
    public void Execute(string query) => Console.WriteLine($"Executing on MySQL: {query}");
    public void Disconnect() => Console.WriteLine("Disconnecting from MySQL");
}

public class PostgreSqlConnection : IDbConnection
{
    private readonly string _connectionString;

    public PostgreSqlConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Connect() => Console.WriteLine($"Connecting to PostgreSQL: {_connectionString}");
    public void Execute(string query) => Console.WriteLine($"Executing on PostgreSQL: {query}");
    public void Disconnect() => Console.WriteLine("Disconnecting from PostgreSQL");
}

// Factory
public interface IDbConnectionFactory
{
    IDbConnection Create(string provider, string connectionString);
}

public class DbConnectionFactory : IDbConnectionFactory
{
    public IDbConnection Create(string provider, string connectionString)
    {
        return provider.ToLower() switch
        {
            "sqlserver" => new SqlServerConnection(connectionString),
            "mysql" => new MySqlConnection(connectionString),
            "postgresql" => new PostgreSqlConnection(connectionString),
            _ => throw new ArgumentException($"Unknown provider: {provider}")
        };
    }
}

// Gebruik
var factory = new DbConnectionFactory();
var connection = factory.Create("postgresql", "Host=localhost;Database=mydb");
connection.Connect();
connection.Execute("SELECT * FROM users");
connection.Disconnect();
```

## Voordelen
- ? Single Responsibility Principle: creatie code op één plek
- ? Open/Closed Principle: nieuwe types toevoegen zonder bestaande code te wijzigen
- ? Loose coupling tussen creator en concrete products
- ? Makkelijker te testen

## Nadelen
- ? Kan leiden tot veel subclasses
- ? Code complexiteit neemt toe

## Gerelateerde Patterns
- **Abstract Factory**: Meerdere gerelateerde factories
- **Template Method**: Factory Method is vaak een stap in Template Method
- **Prototype**: Alternatief voor Factory wanneer subclassing te complex wordt
