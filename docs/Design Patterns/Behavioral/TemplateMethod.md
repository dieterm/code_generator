# Template Method Pattern

## Intentie
Definieert het **skelet van een algoritme** in een operatie, waarbij sommige stappen worden uitgesteld naar subclasses. Template Method laat subclasses bepaalde stappen van een algoritme herdefiniëren zonder de structuur te veranderen.

## Wanneer gebruiken?
- Wanneer je een algoritme hebt met vaste stappen maar variabele implementaties
- Wanneer je code duplicatie tussen vergelijkbare algoritmes wilt voorkomen
- Wanneer je een framework bouwt dat uitgebreid moet worden
- Data processing pipelines, document generators

## Structuur

```
???????????????????????????????
?      AbstractClass          ?
???????????????????????????????
? + TemplateMethod()          ? ??> Calls Step1, Step2, Step3 in order
? + Step1()                   ? ??> Concrete implementation
? # Step2() {abstract}        ? ??> Must be overridden
? # Step3() {virtual}         ? ??> Can be overridden (hook)
???????????????????????????????
              ?
       ???????????????
       ?             ?
??????????????? ???????????????
? ConcreteA   ? ? ConcreteB   ?
??????????????? ???????????????
? # Step2()   ? ? # Step2()   ?
? # Step3()   ? ?             ?
??????????????? ???????????????
```

## Implementatie in C#

### Basis Implementatie: Data Mining

```csharp
// Abstract class met template method
public abstract class DataMiner
{
    // Template method - defines the algorithm skeleton
    public void Mine(string path)
    {
        var file = OpenFile(path);
        var rawData = ExtractData(file);
        var data = ParseData(rawData);
        var analysis = AnalyzeData(data);
        SendReport(analysis);
        CloseFile(file);
    }

    // Concrete step
    private void SendReport(AnalysisResult analysis)
    {
        Console.WriteLine($"Sending report: {analysis.Summary}");
    }

    // Abstract steps - must be implemented by subclasses
    protected abstract object OpenFile(string path);
    protected abstract string ExtractData(object file);
    protected abstract Data ParseData(string rawData);
    protected abstract void CloseFile(object file);

    // Hook - can be overridden, has default implementation
    protected virtual AnalysisResult AnalyzeData(Data data)
    {
        return new AnalysisResult { Summary = $"Analyzed {data.Records.Count} records" };
    }
}

public class Data
{
    public List<Dictionary<string, string>> Records { get; set; } = new();
}

public class AnalysisResult
{
    public string Summary { get; set; }
}

// Concrete implementation for CSV
public class CsvDataMiner : DataMiner
{
    protected override object OpenFile(string path)
    {
        Console.WriteLine($"Opening CSV file: {path}");
        return File.OpenRead(path);
    }

    protected override string ExtractData(object file)
    {
        using var reader = new StreamReader((FileStream)file);
        return reader.ReadToEnd();
    }

    protected override Data ParseData(string rawData)
    {
        var data = new Data();
        var lines = rawData.Split('\n');
        var headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');
            var record = new Dictionary<string, string>();
            for (int j = 0; j < headers.Length; j++)
            {
                record[headers[j]] = values[j];
            }
            data.Records.Add(record);
        }
        return data;
    }

    protected override void CloseFile(object file)
    {
        ((FileStream)file).Dispose();
        Console.WriteLine("CSV file closed");
    }
}

// Concrete implementation for PDF
public class PdfDataMiner : DataMiner
{
    protected override object OpenFile(string path)
    {
        Console.WriteLine($"Opening PDF file: {path}");
        return new PdfDocument(path);
    }

    protected override string ExtractData(object file)
    {
        var pdf = (PdfDocument)file;
        return pdf.ExtractText();
    }

    protected override Data ParseData(string rawData)
    {
        // PDF parsing logic
        var data = new Data();
        // ... parse PDF text
        return data;
    }

    protected override void CloseFile(object file)
    {
        ((PdfDocument)file).Dispose();
        Console.WriteLine("PDF file closed");
    }

    // Override hook method
    protected override AnalysisResult AnalyzeData(Data data)
    {
        var baseResult = base.AnalyzeData(data);
        baseResult.Summary += " (PDF format detected)";
        return baseResult;
    }
}

// Gebruik
DataMiner csvMiner = new CsvDataMiner();
csvMiner.Mine("data.csv");

DataMiner pdfMiner = new PdfDataMiner();
pdfMiner.Mine("report.pdf");
```

### Praktisch Voorbeeld: Document Generator

```csharp
public abstract class DocumentGenerator
{
    // Template method
    public string Generate(ReportData data)
    {
        var builder = new StringBuilder();
        
        builder.Append(CreateHeader(data.Title));
        builder.Append(CreateTableOfContents(data.Sections));
        
        foreach (var section in data.Sections)
        {
            builder.Append(CreateSection(section));
        }
        
        builder.Append(CreateFooter(data));
        
        return PostProcess(builder.ToString());
    }

    // Abstract methods - must implement
    protected abstract string CreateHeader(string title);
    protected abstract string CreateSection(Section section);
    protected abstract string CreateFooter(ReportData data);

    // Hook methods - optional override
    protected virtual string CreateTableOfContents(List<Section> sections)
    {
        return string.Empty; // Default: no TOC
    }

    protected virtual string PostProcess(string document)
    {
        return document; // Default: no post-processing
    }
}

public class ReportData
{
    public string Title { get; set; }
    public List<Section> Sections { get; set; } = new();
    public string Author { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
}

public class Section
{
    public string Title { get; set; }
    public string Content { get; set; }
}

// HTML Document Generator
public class HtmlDocumentGenerator : DocumentGenerator
{
    protected override string CreateHeader(string title)
    {
        return $@"<!DOCTYPE html>
<html>
<head>
    <title>{title}</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; }}
        h1 {{ color: #333; }}
        h2 {{ color: #666; }}
    </style>
</head>
<body>
<h1>{title}</h1>
";
    }

    protected override string CreateTableOfContents(List<Section> sections)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<nav><h2>Table of Contents</h2><ul>");
        foreach (var section in sections)
        {
            var anchor = section.Title.Replace(" ", "-").ToLower();
            sb.AppendLine($"<li><a href=\"#{anchor}\">{section.Title}</a></li>");
        }
        sb.AppendLine("</ul></nav><hr>");
        return sb.ToString();
    }

    protected override string CreateSection(Section section)
    {
        var anchor = section.Title.Replace(" ", "-").ToLower();
        return $@"<section id=""{anchor}"">
<h2>{section.Title}</h2>
<p>{section.Content}</p>
</section>
";
    }

    protected override string CreateFooter(ReportData data)
    {
        return $@"<footer>
<hr>
<p>Generated by {data.Author} on {data.GeneratedAt:yyyy-MM-dd}</p>
</footer>
</body>
</html>";
    }
}

// Markdown Document Generator
public class MarkdownDocumentGenerator : DocumentGenerator
{
    protected override string CreateHeader(string title)
    {
        return $"# {title}\n\n";
    }

    protected override string CreateTableOfContents(List<Section> sections)
    {
        var sb = new StringBuilder();
        sb.AppendLine("## Table of Contents\n");
        foreach (var section in sections)
        {
            var anchor = section.Title.Replace(" ", "-").ToLower();
            sb.AppendLine($"- [{section.Title}](#{anchor})");
        }
        sb.AppendLine("\n---\n");
        return sb.ToString();
    }

    protected override string CreateSection(Section section)
    {
        return $"## {section.Title}\n\n{section.Content}\n\n";
    }

    protected override string CreateFooter(ReportData data)
    {
        return $"\n---\n*Generated by {data.Author} on {data.GeneratedAt:yyyy-MM-dd}*\n";
    }
}

// PDF-style plain text (simplified)
public class PlainTextDocumentGenerator : DocumentGenerator
{
    protected override string CreateHeader(string title)
    {
        var line = new string('=', title.Length);
        return $"{line}\n{title}\n{line}\n\n";
    }

    protected override string CreateSection(Section section)
    {
        var line = new string('-', section.Title.Length);
        return $"{section.Title}\n{line}\n{section.Content}\n\n";
    }

    protected override string CreateFooter(ReportData data)
    {
        return $"\n\nGenerated: {data.GeneratedAt:yyyy-MM-dd}\nAuthor: {data.Author}";
    }
}

// Gebruik
var data = new ReportData
{
    Title = "Annual Report 2024",
    Author = "Finance Department",
    Sections = new List<Section>
    {
        new() { Title = "Executive Summary", Content = "This year was successful..." },
        new() { Title = "Financial Overview", Content = "Revenue increased by 15%..." },
        new() { Title = "Future Outlook", Content = "We expect continued growth..." }
    }
};

var htmlGenerator = new HtmlDocumentGenerator();
var htmlDoc = htmlGenerator.Generate(data);

var mdGenerator = new MarkdownDocumentGenerator();
var mdDoc = mdGenerator.Generate(data);

Console.WriteLine(htmlDoc);
Console.WriteLine(mdDoc);
```

### Met Dependency Injection

```csharp
public abstract class OrderProcessor
{
    protected readonly ILogger _logger;
    protected readonly INotificationService _notificationService;

    protected OrderProcessor(ILogger logger, INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    // Template method
    public async Task<OrderResult> ProcessAsync(Order order)
    {
        _logger.Log($"Processing order {order.Id}");

        // Validate
        var validationResult = await ValidateAsync(order);
        if (!validationResult.IsValid)
        {
            return OrderResult.Failed(validationResult.Errors);
        }

        // Calculate
        var total = await CalculateTotalAsync(order);
        order.TotalAmount = total;

        // Process payment
        var paymentResult = await ProcessPaymentAsync(order);
        if (!paymentResult.Success)
        {
            return OrderResult.Failed("Payment failed");
        }

        // Fulfill
        await FulfillOrderAsync(order);

        // Notify
        await _notificationService.SendOrderConfirmationAsync(order);

        _logger.Log($"Order {order.Id} completed");
        return OrderResult.Succeeded(order);
    }

    // Abstract - must implement
    protected abstract Task<ValidationResult> ValidateAsync(Order order);
    protected abstract Task<PaymentResult> ProcessPaymentAsync(Order order);
    protected abstract Task FulfillOrderAsync(Order order);

    // Virtual - can override
    protected virtual async Task<decimal> CalculateTotalAsync(Order order)
    {
        var subtotal = order.Items.Sum(i => i.Price * i.Quantity);
        var tax = subtotal * 0.21m; // 21% VAT
        return subtotal + tax;
    }
}

// Concrete: Digital order processor
public class DigitalOrderProcessor : OrderProcessor
{
    private readonly IEmailService _emailService;
    private readonly ILicenseGenerator _licenseGenerator;

    public DigitalOrderProcessor(
        ILogger logger,
        INotificationService notificationService,
        IEmailService emailService,
        ILicenseGenerator licenseGenerator)
        : base(logger, notificationService)
    {
        _emailService = emailService;
        _licenseGenerator = licenseGenerator;
    }

    protected override async Task<ValidationResult> ValidateAsync(Order order)
    {
        // Check if all items are digital
        if (order.Items.Any(i => !i.IsDigital))
        {
            return ValidationResult.Failed("Only digital items allowed");
        }
        return ValidationResult.Success();
    }

    protected override async Task<PaymentResult> ProcessPaymentAsync(Order order)
    {
        // Digital payment processing
        return new PaymentResult { Success = true };
    }

    protected override async Task FulfillOrderAsync(Order order)
    {
        // Generate licenses and send download links
        foreach (var item in order.Items)
        {
            var license = await _licenseGenerator.GenerateAsync(item.ProductId);
            await _emailService.SendDownloadLinkAsync(order.CustomerEmail, item, license);
        }
    }

    // No shipping for digital, so no tax on shipping
    protected override async Task<decimal> CalculateTotalAsync(Order order)
    {
        var subtotal = order.Items.Sum(i => i.Price * i.Quantity);
        var tax = subtotal * 0.21m;
        return subtotal + tax;
    }
}

// Concrete: Physical order processor
public class PhysicalOrderProcessor : OrderProcessor
{
    private readonly IInventoryService _inventoryService;
    private readonly IShippingService _shippingService;

    public PhysicalOrderProcessor(
        ILogger logger,
        INotificationService notificationService,
        IInventoryService inventoryService,
        IShippingService shippingService)
        : base(logger, notificationService)
    {
        _inventoryService = inventoryService;
        _shippingService = shippingService;
    }

    protected override async Task<ValidationResult> ValidateAsync(Order order)
    {
        // Check inventory
        foreach (var item in order.Items)
        {
            var inStock = await _inventoryService.CheckStockAsync(item.ProductId, item.Quantity);
            if (!inStock)
            {
                return ValidationResult.Failed($"Item {item.ProductId} out of stock");
            }
        }
        return ValidationResult.Success();
    }

    protected override async Task<PaymentResult> ProcessPaymentAsync(Order order)
    {
        // Physical payment processing with fraud check
        return new PaymentResult { Success = true };
    }

    protected override async Task FulfillOrderAsync(Order order)
    {
        // Reserve inventory and create shipment
        foreach (var item in order.Items)
        {
            await _inventoryService.ReserveAsync(item.ProductId, item.Quantity);
        }
        await _shippingService.CreateShipmentAsync(order);
    }

    // Add shipping cost
    protected override async Task<decimal> CalculateTotalAsync(Order order)
    {
        var baseTotal = await base.CalculateTotalAsync(order);
        var shippingCost = await _shippingService.CalculateCostAsync(order.ShippingAddress);
        return baseTotal + shippingCost;
    }
}
```

## Voordelen
- ? Code hergebruik: gemeenschappelijke code in base class
- ? Inverse of Control: parent class roept child methods aan
- ? Single point of control voor algoritme structuur
- ? Hook methods voor optionele uitbreidingen

## Nadelen
- ? Inheritance-based: minder flexibel dan composition
- ? Liskov Substitution Principle violations mogelijk
- ? Kan complex worden met veel abstracte/virtuele methods

## Template Method vs Strategy

| Aspect | Template Method | Strategy |
|--------|----------------|----------|
| **Mechanisme** | Inheritance | Composition |
| **Flexibiliteit** | Compile-time | Runtime |
| **Granulariteit** | Stappen van algoritme | Gehele algoritme |

## Gerelateerde Patterns
- **Strategy**: Alternatief met composition
- **Factory Method**: Template Method voor object creatie
- **Hook Method**: Optionele extensie punten
