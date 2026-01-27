# Builder Pattern

## Intentie
Scheidt de constructie van een complex object van zijn representatie, zodat hetzelfde constructieproces **verschillende representaties** kan creëren.

## Wanneer gebruiken?
- Wanneer een object veel optionele parameters heeft
- Wanneer je "telescoping constructor" anti-pattern wilt vermijden
- Wanneer het creatie proces meerdere stappen bevat
- Wanneer je verschillende representaties van hetzelfde object wilt bouwen

## Structuur

```
???????????????      ???????????????
?  Director   ??????>?   Builder   ?
???????????????      ???????????????
                            ?
                    ?????????????????
                    ?               ?
             ??????????????? ???????????????
             ?ConcreteBuilder? ?ConcreteBuilder?
             ????????????????? ?????????????????
```

## Implementatie in C#

### Fluent Builder (Meest Gebruikt)

```csharp
public class Email
{
    public string From { get; set; }
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public bool IsHtml { get; set; }
    public List<string> Cc { get; set; } = new();
    public List<string> Bcc { get; set; } = new();
    public List<Attachment> Attachments { get; set; } = new();
}

public class Attachment
{
    public string FileName { get; set; }
    public byte[] Content { get; set; }
}

public class EmailBuilder
{
    private readonly Email _email = new();

    public EmailBuilder From(string from)
    {
        _email.From = from;
        return this;
    }

    public EmailBuilder To(string to)
    {
        _email.To = to;
        return this;
    }

    public EmailBuilder Subject(string subject)
    {
        _email.Subject = subject;
        return this;
    }

    public EmailBuilder Body(string body, bool isHtml = false)
    {
        _email.Body = body;
        _email.IsHtml = isHtml;
        return this;
    }

    public EmailBuilder AddCc(string cc)
    {
        _email.Cc.Add(cc);
        return this;
    }

    public EmailBuilder AddBcc(string bcc)
    {
        _email.Bcc.Add(bcc);
        return this;
    }

    public EmailBuilder AddAttachment(string fileName, byte[] content)
    {
        _email.Attachments.Add(new Attachment 
        { 
            FileName = fileName, 
            Content = content 
        });
        return this;
    }

    public Email Build()
    {
        // Validatie
        if (string.IsNullOrEmpty(_email.From))
            throw new InvalidOperationException("From is required");
        if (string.IsNullOrEmpty(_email.To))
            throw new InvalidOperationException("To is required");

        return _email;
    }
}

// Gebruik
var email = new EmailBuilder()
    .From("sender@example.com")
    .To("recipient@example.com")
    .Subject("Hello World")
    .Body("<h1>Welcome!</h1>", isHtml: true)
    .AddCc("manager@example.com")
    .AddAttachment("report.pdf", pdfBytes)
    .Build();
```

### Immutable Builder met Record

```csharp
public record Person
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public int Age { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public Address? Address { get; init; }
}

public record Address
{
    public string Street { get; init; }
    public string City { get; init; }
    public string PostalCode { get; init; }
    public string Country { get; init; }
}

public class PersonBuilder
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private int _age;
    private string? _email;
    private string? _phone;
    private Address? _address;

    public PersonBuilder WithName(string firstName, string lastName)
    {
        _firstName = firstName;
        _lastName = lastName;
        return this;
    }

    public PersonBuilder WithAge(int age)
    {
        _age = age;
        return this;
    }

    public PersonBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public PersonBuilder WithPhone(string phone)
    {
        _phone = phone;
        return this;
    }

    public PersonBuilder WithAddress(Action<AddressBuilder> configure)
    {
        var addressBuilder = new AddressBuilder();
        configure(addressBuilder);
        _address = addressBuilder.Build();
        return this;
    }

    public Person Build()
    {
        return new Person
        {
            FirstName = _firstName,
            LastName = _lastName,
            Age = _age,
            Email = _email,
            Phone = _phone,
            Address = _address
        };
    }
}

public class AddressBuilder
{
    private string _street = string.Empty;
    private string _city = string.Empty;
    private string _postalCode = string.Empty;
    private string _country = string.Empty;

    public AddressBuilder Street(string street)
    {
        _street = street;
        return this;
    }

    public AddressBuilder City(string city)
    {
        _city = city;
        return this;
    }

    public AddressBuilder PostalCode(string postalCode)
    {
        _postalCode = postalCode;
        return this;
    }

    public AddressBuilder Country(string country)
    {
        _country = country;
        return this;
    }

    public Address Build()
    {
        return new Address
        {
            Street = _street,
            City = _city,
            PostalCode = _postalCode,
            Country = _country
        };
    }
}

// Gebruik
var person = new PersonBuilder()
    .WithName("John", "Doe")
    .WithAge(30)
    .WithEmail("john@example.com")
    .WithAddress(a => a
        .Street("123 Main St")
        .City("Amsterdam")
        .PostalCode("1000 AA")
        .Country("Netherlands"))
    .Build();
```

### Director Pattern

```csharp
// Product
public class House
{
    public string Foundation { get; set; }
    public string Structure { get; set; }
    public string Roof { get; set; }
    public string Interior { get; set; }
    public bool HasGarage { get; set; }
    public bool HasGarden { get; set; }
    public bool HasPool { get; set; }

    public override string ToString()
    {
        return $"House: {Foundation}, {Structure}, {Roof}, {Interior}, " +
               $"Garage: {HasGarage}, Garden: {HasGarden}, Pool: {HasPool}";
    }
}

// Builder interface
public interface IHouseBuilder
{
    IHouseBuilder BuildFoundation();
    IHouseBuilder BuildStructure();
    IHouseBuilder BuildRoof();
    IHouseBuilder BuildInterior();
    IHouseBuilder AddGarage();
    IHouseBuilder AddGarden();
    IHouseBuilder AddPool();
    House Build();
}

// Concrete Builders
public class WoodenHouseBuilder : IHouseBuilder
{
    private readonly House _house = new();

    public IHouseBuilder BuildFoundation()
    {
        _house.Foundation = "Wooden Piles";
        return this;
    }

    public IHouseBuilder BuildStructure()
    {
        _house.Structure = "Wooden Frame";
        return this;
    }

    public IHouseBuilder BuildRoof()
    {
        _house.Roof = "Wooden Shingles";
        return this;
    }

    public IHouseBuilder BuildInterior()
    {
        _house.Interior = "Wooden Panels";
        return this;
    }

    public IHouseBuilder AddGarage()
    {
        _house.HasGarage = true;
        return this;
    }

    public IHouseBuilder AddGarden()
    {
        _house.HasGarden = true;
        return this;
    }

    public IHouseBuilder AddPool()
    {
        _house.HasPool = true;
        return this;
    }

    public House Build() => _house;
}

public class BrickHouseBuilder : IHouseBuilder
{
    private readonly House _house = new();

    public IHouseBuilder BuildFoundation()
    {
        _house.Foundation = "Concrete Foundation";
        return this;
    }

    public IHouseBuilder BuildStructure()
    {
        _house.Structure = "Brick Walls";
        return this;
    }

    public IHouseBuilder BuildRoof()
    {
        _house.Roof = "Clay Tiles";
        return this;
    }

    public IHouseBuilder BuildInterior()
    {
        _house.Interior = "Plaster and Paint";
        return this;
    }

    public IHouseBuilder AddGarage()
    {
        _house.HasGarage = true;
        return this;
    }

    public IHouseBuilder AddGarden()
    {
        _house.HasGarden = true;
        return this;
    }

    public IHouseBuilder AddPool()
    {
        _house.HasPool = true;
        return this;
    }

    public House Build() => _house;
}

// Director
public class HouseDirector
{
    public House ConstructSimpleHouse(IHouseBuilder builder)
    {
        return builder
            .BuildFoundation()
            .BuildStructure()
            .BuildRoof()
            .BuildInterior()
            .Build();
    }

    public House ConstructLuxuryHouse(IHouseBuilder builder)
    {
        return builder
            .BuildFoundation()
            .BuildStructure()
            .BuildRoof()
            .BuildInterior()
            .AddGarage()
            .AddGarden()
            .AddPool()
            .Build();
    }
}

// Gebruik
var director = new HouseDirector();

var simpleWoodenHouse = director.ConstructSimpleHouse(new WoodenHouseBuilder());
var luxuryBrickHouse = director.ConstructLuxuryHouse(new BrickHouseBuilder());

Console.WriteLine(simpleWoodenHouse);
Console.WriteLine(luxuryBrickHouse);
```

### Generic Builder met Validation

```csharp
public class ValidationResult
{
    public bool IsValid { get; set; } = true;
    public List<string> Errors { get; set; } = new();
}

public abstract class Builder<TProduct, TBuilder> 
    where TBuilder : Builder<TProduct, TBuilder>
{
    protected abstract TProduct CreateProduct();
    protected abstract ValidationResult Validate();

    public TProduct Build()
    {
        var result = Validate();
        if (!result.IsValid)
        {
            throw new InvalidOperationException(
                $"Validation failed: {string.Join(", ", result.Errors)}");
        }
        return CreateProduct();
    }

    protected TBuilder Self => (TBuilder)this;
}

public class OrderBuilder : Builder<Order, OrderBuilder>
{
    private Guid _customerId;
    private List<OrderLine> _lines = new();
    private Address? _shippingAddress;

    public OrderBuilder ForCustomer(Guid customerId)
    {
        _customerId = customerId;
        return Self;
    }

    public OrderBuilder AddLine(string product, int quantity, decimal price)
    {
        _lines.Add(new OrderLine(product, quantity, price));
        return Self;
    }

    public OrderBuilder ShipTo(Address address)
    {
        _shippingAddress = address;
        return Self;
    }

    protected override Order CreateProduct()
    {
        return new Order
        {
            CustomerId = _customerId,
            Lines = _lines,
            ShippingAddress = _shippingAddress!
        };
    }

    protected override ValidationResult Validate()
    {
        var result = new ValidationResult();
        
        if (_customerId == Guid.Empty)
            result.Errors.Add("Customer is required");
        
        if (!_lines.Any())
            result.Errors.Add("At least one order line is required");
        
        if (_shippingAddress == null)
            result.Errors.Add("Shipping address is required");

        result.IsValid = !result.Errors.Any();
        return result;
    }
}

public class Order
{
    public Guid CustomerId { get; init; }
    public List<OrderLine> Lines { get; init; } = new();
    public Address ShippingAddress { get; init; }
}

public record OrderLine(string Product, int Quantity, decimal Price);

// Gebruik
var order = new OrderBuilder()
    .ForCustomer(Guid.NewGuid())
    .AddLine("Product A", 2, 29.99m)
    .AddLine("Product B", 1, 49.99m)
    .ShipTo(new Address { Street = "Main St", City = "Amsterdam" })
    .Build();
```

## Voordelen
- ? Vermijdt telescoping constructors
- ? Stap-voor-stap constructie
- ? Dezelfde code kan verschillende representaties bouwen
- ? Single Responsibility: constructie code geïsoleerd
- ? Immutable objects mogelijk

## Nadelen
- ? Meer code nodig
- ? Kan overkill zijn voor eenvoudige objecten

## Modern C# Alternatieven

### Object Initializers
```csharp
var person = new Person
{
    FirstName = "John",
    LastName = "Doe",
    Age = 30
};
```

### Required Properties (C# 11+)
```csharp
public class Person
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public int Age { get; init; }
}
```

### Record met Positional Syntax
```csharp
public record Person(string FirstName, string LastName, int Age);
var person = new Person("John", "Doe", 30);
```

## Gerelateerde Patterns
- **Abstract Factory**: Builder focust op stapsgewijs bouwen, Factory op families van objecten
- **Composite**: Builder kan Composite structuren bouwen
- **Fluent Interface**: Vaak gebruikt samen met Builder
