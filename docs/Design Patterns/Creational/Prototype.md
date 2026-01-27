# Prototype Pattern

## Intentie
Specificeert het soort objecten dat gecreëerd moet worden met behulp van een prototype instance, en creëert nieuwe objecten door dit prototype te **klonen**.

## Wanneer gebruiken?
- Wanneer het creëren van een object duur is (database calls, file I/O)
- Wanneer je runtime wilt beslissen welke objecten te creëren
- Wanneer je onafhankelijk wilt zijn van concrete classes
- Wanneer je variaties van objecten wilt maken op basis van een basis configuratie

## Structuur

```
???????????????????
?   Prototype     ?
???????????????????
? + Clone()       ?
???????????????????
        ?
        ?
?????????????????
?               ?
?????????????????   ?????????????????
?ConcreteProto1 ?   ?ConcreteProto2 ?
?????????????????   ?????????????????
```

## Implementatie in C#

### Basis Implementatie met ICloneable

```csharp
public class Person : ICloneable
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Address Address { get; set; }

    // Shallow copy
    public object Clone()
    {
        return MemberwiseClone();
    }

    // Deep copy
    public Person DeepClone()
    {
        var clone = (Person)MemberwiseClone();
        clone.Address = new Address
        {
            Street = Address.Street,
            City = Address.City
        };
        return clone;
    }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}

// Gebruik
var original = new Person
{
    Name = "John",
    Age = 30,
    Address = new Address { Street = "Main St", City = "Amsterdam" }
};

// Shallow clone - Address is shared!
var shallowClone = (Person)original.Clone();
shallowClone.Name = "Jane";
shallowClone.Address.City = "Rotterdam"; // Wijzigt ook original!

// Deep clone - Address is independent
var deepClone = original.DeepClone();
deepClone.Address.City = "Utrecht"; // original blijft ongewijzigd
```

### Generic Prototype Interface

```csharp
public interface IPrototype<T>
{
    T Clone();
    T DeepClone();
}

public class Document : IPrototype<Document>
{
    public string Title { get; set; }
    public string Content { get; set; }
    public List<string> Authors { get; set; } = new();
    public DocumentSettings Settings { get; set; }

    public Document Clone()
    {
        return (Document)MemberwiseClone();
    }

    public Document DeepClone()
    {
        return new Document
        {
            Title = Title,
            Content = Content,
            Authors = new List<string>(Authors),
            Settings = Settings?.DeepClone()
        };
    }
}

public class DocumentSettings : IPrototype<DocumentSettings>
{
    public string FontFamily { get; set; }
    public int FontSize { get; set; }
    public bool IsBold { get; set; }

    public DocumentSettings Clone()
    {
        return (DocumentSettings)MemberwiseClone();
    }

    public DocumentSettings DeepClone()
    {
        return (DocumentSettings)MemberwiseClone(); // Alleen primitieven
    }
}
```

### Deep Clone met Serialization

```csharp
using System.Text.Json;

public static class CloneExtensions
{
    public static T DeepClone<T>(this T source) where T : class
    {
        if (source == null) return null;

        var json = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(json);
    }
}

// Gebruik
var original = new Person
{
    Name = "John",
    Age = 30,
    Address = new Address { Street = "Main St", City = "Amsterdam" }
};

var clone = original.DeepClone();
clone.Address.City = "Rotterdam"; // original blijft ongewijzigd
```

### Prototype Registry/Manager

```csharp
public interface IShape : IPrototype<IShape>
{
    void Draw();
}

public class Circle : IShape
{
    public int Radius { get; set; }
    public string Color { get; set; }

    public IShape Clone() => (Circle)MemberwiseClone();
    public IShape DeepClone() => (Circle)MemberwiseClone();

    public void Draw() => Console.WriteLine($"Circle: Radius={Radius}, Color={Color}");
}

public class Rectangle : IShape
{
    public int Width { get; set; }
    public int Height { get; set; }
    public string Color { get; set; }

    public IShape Clone() => (Rectangle)MemberwiseClone();
    public IShape DeepClone() => (Rectangle)MemberwiseClone();

    public void Draw() => Console.WriteLine($"Rectangle: {Width}x{Height}, Color={Color}");
}

// Prototype Registry
public class ShapeRegistry
{
    private readonly Dictionary<string, IShape> _prototypes = new();

    public void Register(string key, IShape prototype)
    {
        _prototypes[key] = prototype;
    }

    public IShape Create(string key)
    {
        if (!_prototypes.TryGetValue(key, out var prototype))
            throw new ArgumentException($"Unknown prototype: {key}");

        return prototype.DeepClone();
    }

    public bool Contains(string key) => _prototypes.ContainsKey(key);
}

// Gebruik
var registry = new ShapeRegistry();

// Register prototypes
registry.Register("red-circle", new Circle { Radius = 10, Color = "Red" });
registry.Register("blue-circle", new Circle { Radius = 20, Color = "Blue" });
registry.Register("green-rect", new Rectangle { Width = 100, Height = 50, Color = "Green" });

// Create clones
var circle1 = registry.Create("red-circle");
var circle2 = registry.Create("red-circle");
var rect = registry.Create("green-rect");

circle1.Draw();
circle2.Draw();
rect.Draw();
```

### Praktisch Voorbeeld: Document Templates

```csharp
public class ReportTemplate : IPrototype<ReportTemplate>
{
    public string Name { get; set; }
    public PageSettings PageSettings { get; set; }
    public List<ReportSection> Sections { get; set; } = new();
    public Dictionary<string, string> Styles { get; set; } = new();

    public ReportTemplate Clone()
    {
        return (ReportTemplate)MemberwiseClone();
    }

    public ReportTemplate DeepClone()
    {
        return new ReportTemplate
        {
            Name = Name,
            PageSettings = PageSettings?.DeepClone(),
            Sections = Sections.Select(s => s.DeepClone()).ToList(),
            Styles = new Dictionary<string, string>(Styles)
        };
    }
}

public class PageSettings : IPrototype<PageSettings>
{
    public string Size { get; set; } = "A4";
    public string Orientation { get; set; } = "Portrait";
    public Margins Margins { get; set; } = new();

    public PageSettings Clone() => (PageSettings)MemberwiseClone();
    
    public PageSettings DeepClone()
    {
        return new PageSettings
        {
            Size = Size,
            Orientation = Orientation,
            Margins = Margins.DeepClone()
        };
    }
}

public class Margins : IPrototype<Margins>
{
    public int Top { get; set; } = 20;
    public int Bottom { get; set; } = 20;
    public int Left { get; set; } = 25;
    public int Right { get; set; } = 25;

    public Margins Clone() => (Margins)MemberwiseClone();
    public Margins DeepClone() => (Margins)MemberwiseClone();
}

public class ReportSection : IPrototype<ReportSection>
{
    public string Title { get; set; }
    public string Content { get; set; }
    public int Order { get; set; }

    public ReportSection Clone() => (ReportSection)MemberwiseClone();
    public ReportSection DeepClone() => (ReportSection)MemberwiseClone();
}

// Template Manager
public class ReportTemplateManager
{
    private readonly Dictionary<string, ReportTemplate> _templates = new();

    public ReportTemplateManager()
    {
        InitializeDefaultTemplates();
    }

    private void InitializeDefaultTemplates()
    {
        // Corporate template
        var corporate = new ReportTemplate
        {
            Name = "Corporate",
            PageSettings = new PageSettings
            {
                Size = "A4",
                Orientation = "Portrait",
                Margins = new Margins { Top = 30, Bottom = 30, Left = 25, Right = 25 }
            },
            Sections = new List<ReportSection>
            {
                new() { Title = "Executive Summary", Order = 1 },
                new() { Title = "Introduction", Order = 2 },
                new() { Title = "Analysis", Order = 3 },
                new() { Title = "Conclusion", Order = 4 }
            },
            Styles = new Dictionary<string, string>
            {
                ["HeaderFont"] = "Arial",
                ["BodyFont"] = "Calibri"
            }
        };
        _templates["corporate"] = corporate;

        // Simple template
        var simple = new ReportTemplate
        {
            Name = "Simple",
            PageSettings = new PageSettings(),
            Sections = new List<ReportSection>
            {
                new() { Title = "Content", Order = 1 }
            },
            Styles = new Dictionary<string, string>
            {
                ["HeaderFont"] = "Helvetica",
                ["BodyFont"] = "Helvetica"
            }
        };
        _templates["simple"] = simple;
    }

    public ReportTemplate CreateFromTemplate(string templateName)
    {
        if (!_templates.TryGetValue(templateName.ToLower(), out var template))
            throw new ArgumentException($"Unknown template: {templateName}");

        return template.DeepClone();
    }

    public void AddTemplate(string name, ReportTemplate template)
    {
        _templates[name.ToLower()] = template.DeepClone();
    }
}

// Gebruik
var manager = new ReportTemplateManager();

// Create reports from templates
var report1 = manager.CreateFromTemplate("corporate");
report1.Sections.Add(new ReportSection { Title = "Custom Section", Order = 5 });

var report2 = manager.CreateFromTemplate("corporate");
// report2 has original 4 sections, not 5

Console.WriteLine($"Report 1 sections: {report1.Sections.Count}"); // 5
Console.WriteLine($"Report 2 sections: {report2.Sections.Count}"); // 4
```

### Met Record Types (C# 9+)

```csharp
public record PersonRecord(string Name, int Age, AddressRecord Address);
public record AddressRecord(string Street, string City);

// Records have built-in value-based equality and with-expressions
var original = new PersonRecord("John", 30, new AddressRecord("Main St", "Amsterdam"));

// Clone with modifications using 'with' expression
var clone = original with { Name = "Jane" };
var cloneWithNewAddress = original with 
{ 
    Address = original.Address with { City = "Rotterdam" } 
};

Console.WriteLine(original);  // PersonRecord { Name = John, ... }
Console.WriteLine(clone);     // PersonRecord { Name = Jane, ... }
```

## Shallow vs Deep Clone

| Aspect | Shallow Clone | Deep Clone |
|--------|--------------|------------|
| **Primitieven** | Gekopieerd | Gekopieerd |
| **Reference types** | Shared reference | Nieuwe instantie |
| **Performance** | Sneller | Langzamer |
| **Gebruik** | Immutable nested objects | Mutable nested objects |

## Voordelen
- ? Vermijdt dure initialisatie
- ? Runtime configuratie van objecten
- ? Reduceert subclassing
- ? Dynamisch objecten toevoegen/verwijderen

## Nadelen
- ? Klonen van complexe objecten kan lastig zijn
- ? Circulaire referenties zijn problematisch
- ? Deep clone vereist extra implementatie

## Gerelateerde Patterns
- **Abstract Factory**: Kan prototypes gebruiken voor product creatie
- **Composite**: Vaak gecombineerd voor complexe structuren
- **Memento**: Gerelateerd voor state opslag
