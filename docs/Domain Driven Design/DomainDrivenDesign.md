# Domain-Driven Design (DDD) Concepten

## DDD Terminologie Overzicht

### Building Blocks (Tactische Patterns)

#### **Entity**
Een object dat gedefinieerd wordt door zijn **identiteit** in plaats van zijn attributen. De identiteit blijft constant, zelfs als de attributen wijzigen.

**Kenmerken:**
- Heeft een unieke identifier (Id)
- Mutable (kan wijzigen over tijd)
- Gelijkheid op basis van Id
- Heeft een levenscyclus

**Voorbeeld:** `Customer`, `Order`, `Product`, `User`

#### **Value Object**
Een object zonder conceptuele identiteit, gedefinieerd door zijn **attributen**. Twee Value Objects met dezelfde waarden zijn identiek.

**Kenmerken:**
- Geen unieke identifier
- Immutable (onveranderlijk)
- Gelijkheid op basis van alle eigenschappen
- Kan gedeeld worden tussen entities

**Voorbeeld:** `Money`, `Address`, `DateRange`, `EmailAddress`, `Coordinate`

#### **Aggregate**
Een cluster van **Entity** en **Value Objects** die samen een consistentie-grens vormen. Externe objecten mogen alleen de Aggregate Root refereren.

**Kenmerken:**
- Consistentie-grens voor transacties
- Bevat één of meerdere entities
- Encapsuleert business rules
- Wordt als één eenheid opgeslagen en opgehaald

**Voorbeeld:** `Order` aggregate met `OrderLine` entities en `ShippingAddress` value object

#### **Aggregate Root**
De primaire **Entity** binnen een Aggregate die de toegang tot alle andere objecten in de aggregate controleert.

**Kenmerken:**
- Enige entry point voor de aggregate
- Verantwoordelijk voor invariant validatie
- Heeft een globaal unieke Id
- Controleert levenscyclus van child entities

**Voorbeeld:** `Order` is de root van de Order aggregate (bevat `OrderLines`, `Payment`, etc.)

```csharp
public class Order // Aggregate Root
{
    private List<OrderLine> _orderLines = new();
    
    public IReadOnlyCollection<OrderLine> OrderLines => _orderLines.AsReadOnly();
    
    // Encapsulatie: wijzigingen alleen via methods
    public void AddOrderLine(Product product, int quantity)
    {
        // Business rules enforcement
        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be positive");
            
        _orderLines.Add(new OrderLine(product, quantity));
    }
}
```

#### **Domain Service**
Een service die **domain logic** bevat die niet natuurlijk bij een Entity of Value Object hoort. Stateless operatie die vaak meerdere aggregates coördineert.

**Wanneer gebruiken:**
- Operatie betreft meerdere aggregates
- Business logic past niet bij één specifiek object
- Complexe berekeningen of transformaties

**Voorbeeld:** `PricingService`, `TransferMoneyService`, `OrderFulfillmentService`

```csharp
public class MoneyTransferService // Domain Service
{
    public void Transfer(BankAccount from, BankAccount to, Money amount)
    {
        // Coördineert twee aggregates
        from.Withdraw(amount);
        to.Deposit(amount);
    }
}
```

#### **Domain Event**
Een gebeurtenis die iets **betekenisvols** in het domein beschrijft wat **al gebeurd is**. Immutable en benoemd in verleden tijd.

**Kenmerken:**
- Immutable (eenmaal gebeurd, altijd gebeurd)
- Verleden tijd benaming
- Bevat relevante data van het moment
- Kan gebruikt worden voor communicatie tussen aggregates

**Voorbeeld:** `OrderPlaced`, `PaymentReceived`, `ProductShipped`, `CustomerRegistered`

```csharp
public record OrderPlacedEvent : IDomainEvent
{
    public Guid OrderId { get; init; }
    public DateTime OccurredOn { get; init; }
    public decimal TotalAmount { get; init; }
    public Guid CustomerId { get; init; }
}
```

#### **Repository**
Een interface die **collectie-achtige toegang** biedt tot Aggregate Roots. Abstraheert de data persistence laag.

**Kenmerken:**
- Één repository per Aggregate Root
- Collection-style interface (`Add`, `Remove`, `FindById`)
- Abstraheert database details
- Geen business logic

**Voorbeeld:** `IOrderRepository`, `ICustomerRepository`

```csharp
public interface IOrderRepository
{
    Order? FindById(Guid orderId);
    IEnumerable<Order> FindByCustomerId(Guid customerId);
    void Add(Order order);
    void Remove(Order order);
    void SaveChanges();
}
```

#### **Factory**
Een object dat verantwoordelijk is voor het **creëren van complexe objecten** en aggregates, waarbij alle invarianten worden gegarandeerd.

**Wanneer gebruiken:**
- Complex object creatie
- Meerdere stappen nodig
- Invariants moeten gegarandeerd worden vanaf creatie

**Voorbeeld:** `OrderFactory`, `CustomerFactory`

```csharp
public class OrderFactory
{
    public Order CreateOrder(Customer customer, ShippingAddress address)
    {
        // Complexe creatie logica
        var order = new Order(Guid.NewGuid(), customer.Id);
        order.SetShippingAddress(address);
        order.ApplyCustomerDiscount(customer.DiscountPercentage);
        return order;
    }
}
```

#### **Specification**
Een pattern voor het **encapsuleren van business rules** voor selectie/validatie. Herbruikbare predicate logica.

**Wanneer gebruiken:**
- Complexe query logica
- Herbruikbare business rules
- Combineerbare voorwaarden

**Voorbeeld:**

```csharp
public class CustomerIsEligibleForPremiumSpecification : ISpecification<Customer>
{
    public bool IsSatisfiedBy(Customer customer)
    {
        return customer.TotalOrderValue > 10000 
            && customer.AccountAge > TimeSpan.FromYears(1);
    }
}

// Gebruik
var spec = new CustomerIsEligibleForPremiumSpecification();
if (spec.IsSatisfiedBy(customer))
{
    customer.UpgradeToPremium();
}
```

### Architectural Patterns (Strategische Patterns)

#### **Bounded Context**
Een **conceptuele grens** waarin een specifiek domain model geldig is. Binnen deze grens heeft elk concept een specifieke betekenis.

**Kenmerken:**
- Eigen ubiquitous language
- Expliciete grenzen
- Eigen data model
- Mogelijk eigen database

**Voorbeeld:** 
- **Sales Context**: `Customer` = koper met order history
- **Support Context**: `Customer` = gebruiker met support tickets
- **Billing Context**: `Customer` = account met betalingsinfo

#### **Context Map**
Een visualisatie van de **relaties tussen verschillende Bounded Contexts** en hoe ze met elkaar communiceren.

**Relatie types:**
- **Shared Kernel**: Gedeelde code tussen contexts
- **Customer-Supplier**: Upstream/downstream relatie
- **Conformist**: Downstream volgt upstream zonder invloed
- **Anticorruption Layer**: Translatie laag tussen contexts
- **Published Language**: Gestandaardiseerde communicatie format

#### **Ubiquitous Language**
Een **gedeelde taal** tussen developers en domain experts die gebruikt wordt in code, conversaties en documentatie.

**Kenmerken:**
- Gebruikt in code (class names, method names)
- Gebruikt in gesprekken met stakeholders
- Geëvolueerd door team collaboration
- Specifiek voor Bounded Context

**Voorbeeld:**
```csharp
// Ubiquitous Language in code
public class ShoppingCart
{
    public void AddItem(Product product, int quantity) { }
    public void RemoveItem(ProductId productId) { }
    public void Checkout() { }  // Niet "Submit" of "Process"
}
```

#### **Anticorruption Layer (ACL)**
Een **isolatie laag** die voorkomt dat een extern systeem of context je eigen domain model vervuilt.

**Wanneer gebruiken:**
- Integratie met legacy systemen
- Externe APIs met eigen model
- Voorkomen van directe dependencies

**Voorbeeld:**

```csharp
// External API model
public class ExternalCustomerDto
{
    public int CustNo { get; set; }
    public string CustName { get; set; }
}

// Anticorruption Layer
public class CustomerAdapter
{
    public Customer ToDomainModel(ExternalCustomerDto dto)
    {
        return new Customer(
            id: Guid.NewGuid(),
            name: new CustomerName(dto.CustName),
            externalReference: dto.CustNo.ToString()
        );
    }
}
```

### Layered Architecture Patterns

#### **Application Service**
Orkestreert **use cases** en coördineert domain objecten. Bevat geen business logic, alleen workflow coördinatie.

**Verantwoordelijkheden:**
- Transaction management
- Security/authorization
- Use case orchestration
- DTO mapping

```csharp
public class PlaceOrderService // Application Service
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    
    public void PlaceOrder(PlaceOrderCommand command)
    {
        // Orchestratie, geen business logic
        var customer = _customerRepository.FindById(command.CustomerId);
        var order = Order.Create(customer, command.Items);
        _orderRepository.Add(order);
    }
}
```

#### **Infrastructure Service**
Technische services voor **cross-cutting concerns** zoals logging, email, file storage.

**Voorbeelden:** `IEmailService`, `ILoggingService`, `IFileStorageService`

### Andere Belangrijke Concepten

#### **Invariant**
Een **business rule** die altijd waar moet zijn voor een Aggregate. Gehandhaafd door de Aggregate Root.

**Voorbeeld:** "Een order moet minimaal 1 order line hebben"

```csharp
public class Order
{
    public void RemoveOrderLine(Guid lineId)
    {
        if (_orderLines.Count == 1)
            throw new InvalidOperationException("Order must have at least one line");
            
        _orderLines.RemoveAll(l => l.Id == lineId);
    }
}
```

#### **Side-Effect-Free Function**
Een methode die alleen **informatie teruggeeft** zonder state te wijzigen. Voorkomt verrassingen en maakt code voorspelbaar.

```csharp
public class Order
{
    // Side-effect-free
    public decimal CalculateTotal() => _orderLines.Sum(l => l.Total);
    
    // Met side-effect (vermijden in query methods)
    public decimal GetTotalAndNotify() 
    {
        var total = CalculateTotal();
        _eventBus.Publish(new TotalCalculated(total)); // Side effect!
        return total;
    }
}
```

#### **Intention-Revealing Interface**
Methods en classes hebben **namen die hun bedoeling** duidelijk maken zonder implementatie details.

```csharp
// Goed: intentie is duidelijk
customer.Upgrade ToPremium();

// Slecht: implementatie detail
customer.SetPremiumFlag(true);
```

#### **Closure of Operations**
Operaties die argumenten en return value van **hetzelfde type** hebben, waardoor ze composable zijn.

```csharp
public record Money
{
    public Money Add(Money other) => // Closure: Money + Money = Money
        new Money { Amount = Amount + other.Amount, Currency = Currency };
        
    public Money Multiply(decimal factor) => // Geen closure: Money * decimal = Money
        new Money { Amount = Amount * factor, Currency = Currency };
}
```

## Samenvatting Categorieën

| Categorie | Concepten |
|-----------|-----------|
| **Building Blocks** | Entity, Value Object, Aggregate, Aggregate Root, Domain Service, Domain Event, Repository, Factory, Specification |
| **Strategic Design** | Bounded Context, Context Map, Ubiquitous Language, Anticorruption Layer |
| **Layering** | Application Service, Domain Service, Infrastructure Service |
| **Patterns** | Specification, Factory, Repository, Domain Event |
| **Principes** | Invariant, Side-Effect-Free Function, Intention-Revealing Interface, Closure of Operations |

---

## Value Object vs Entity

### Entity
- ? **Heeft een unieke identiteit** (Id)
- De identiteit blijft behouden over de tijd, zelfs als attributen wijzigen
- Gelijkheid wordt bepaald door de **Id**
- Voorbeeld: `Customer`, `Order`, `Product`

```csharp
public class Customer // Entity
{
    public Guid Id { get; set; } // ? Heeft Id
    public string Name { get; set; }
    public Address Address { get; set; // Value Object
}
```

### Value Object
- ? **Geen unieke identiteit** (geen Id)
- Wordt gedefinieerd door zijn **attributen/waarden**
- Gelijkheid wordt bepaald door **alle eigenschappen**
- **Immutable** (onveranderlijk)
- Voorbeeld: `Money`, `Address`, `DateRange`, `EmailAddress`

```csharp
public class Address // Value Object
{
    // ? Geen Id property
    public string Street { get; init; }
    public string City { get; init; }
    public string PostalCode { get; init; }
    
    // Gelijkheid op basis van waarden
    public override bool Equals(object obj) 
    {
        if (obj is not Address other) return false;
        return Street == other.Street && 
               City == other.City && 
               PostalCode == other.PostalCode;
    }
}
```

### Waarom geen Id voor Value Objects?

1. **Conceptueel verschil**: Value Objects representeren **concepten zonder levenscyclus** - ze zijn wat ze zijn door hun waarden
2. **Uitwisselbaarheid**: Twee Value Objects met dezelfde waarden zijn **identiek** en volledig uitwisselbaar
3. **Immutability**: In plaats van een Value Object te wijzigen, maak je een **nieuwe instantie**

### Praktisch voorbeeld

```csharp
// Entity - heeft Id
public class BankAccount
{
    public Guid Id { get; set; } // ? Entity heeft Id
    public Money Balance { get; set; }
    public AccountHolder Owner { get; set; }
}

// Value Object - geen Id
public record Money // ? Geen Id
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }
    
    // Twee Money objecten met dezelfde waarden zijn identiek
    public Money Add(Money other) => 
        new Money { Amount = Amount + other.Amount, Currency = Currency };
}
```

## Value Objects in Database Context

### Probleem: Value Object in aparte tabel

Als je een Value Object in een **aparte tabel** opslaat en individueel moet kunnen updaten, dan is het conceptueel een **Entity** en moet het een **Id** hebben.

**Voorbeeld:**
```csharp
// WorldSubRegion is een Entity (niet een Value Object)
public class WorldSubRegion
{
    public Guid Id { get; set; } // ? Heeft Id voor database tracking
    public Guid WorldRegionId { get; set; } // Foreign key
    public string Name { get; set; }
    public string Code { get; set; }
}

public class WorldRegion
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<WorldSubRegion> SubRegions { get; set; // One-to-Many relatie
}
```

**Database:**
```sql
-- WorldSubRegion heeft eigen Id
UPDATE WorldSubRegion 
SET Name = 'NewName' 
WHERE Id = @SubRegionId; -- ? Kan specifieke rij updaten
```

### Oplossing 1: Echte Value Objects (Owned Entities in EF Core)

#### Optie A: Inline opslag (JSON/XML kolom)

```csharp
public class WorldRegion
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    // Value Objects als JSON in de WorldRegion tabel
    public List<WorldSubRegionValue> SubRegions { get; set; }
}

public record WorldSubRegionValue // Value Object (geen Id)
{
    public string Name { get; init; }
    public string Code { get; init; }
}
```

**Database (EF Core):**
```csharp
modelBuilder.Entity<WorldRegion>()
    .OwnsMany(r => r.SubRegions, sub =>
    {
        // Opgeslagen als JSON in WorldRegion tabel
        sub.ToJson();
    });
```

Bij wijziging **vervang je alle subregio's**:
```csharp
// Hele collectie vervangen
region.SubRegions = region.SubRegions
    .Select(s => s.Name == oldName 
        ? s with { Name = newName } // Nieuwe instance
        : s)
    .ToList();
```

#### Optie B: Shadow Property (EF Core genereert hidden Id)

EF Core kan automatisch een **verborgen Id** genereren voor owned entities:

```csharp
modelBuilder.Entity<WorldRegion>()
    .OwnsMany(r => r.SubRegions, sub =>
    {
        sub.ToTable("WorldSubRegion"); // Aparte tabel
        // EF Core genereert automatisch een hidden Id kolom
        sub.Property<int>("WorldSubRegionId"); // Shadow property
    });
```

**Belangrijk**: De applicatie ziet dit Id niet - het is puur voor database tracking.

### Oplossing 2: Aggregate Root patroon

WorldRegion is de **Aggregate Root**, WorldSubRegion is een **child entity** binnen de aggregate:

```csharp
public class WorldRegion // Aggregate Root
{
    public Guid Id { get; set; }
    private List<WorldSubRegion> _subRegions = new();
    
    // Encapsulatie: wijzigingen via methods
    public IReadOnlyCollection<WorldSubRegion> SubRegions => _subRegions.AsReadOnly();
    
    public void RenameSubRegion(Guid subRegionId, string newName)
    {
        var subRegion = _subRegions.FirstOrDefault(s => s.Id == subRegionId);
        if (subRegion == null) 
            throw new InvalidOperationException("SubRegion not found");
        
        subRegion.Name = newName; // ? Id maakt dit mogelijk
    }
}

public class WorldSubRegion // Child Entity (niet Value Object)
{
    public Guid Id { get; set; } // ? Heeft Id
    internal string Name { get; set; } // Internal: alleen via WorldRegion wijzigbaar
}
```

### Regel van duim

- **Aparte tabel + individueel updaten** = Entity (met Id)
- **Onderdeel van parent + vervangen als geheel** = Value Object (zonder Id)

## Enum in Domain-Driven Design

In Domain-Driven Design (DDD) kan een **enum** op verschillende manieren worden vertaald, afhankelijk van de complexiteit en het gebruik:

### 1. Simple Value Object (meest voorkomend)

Een enum is typisch een **Value Object** - het heeft geen identiteit en wordt gedefinieerd door zijn waarde.

```csharp
// Gewone C# enum
public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}

// In DDD termen: Dit is een Value Object
public class Order // Entity
{
    public Guid Id { get; set; }
    public OrderStatus Status { get; set; } // Value Object
}
```

### 2. Enumeration Class Pattern (rijkere domain logic)

Wanneer je **meer gedrag** nodig hebt dan alleen waarden, gebruik je het **Enumeration Class pattern**:

```csharp
// DDD Enumeration - rijker dan gewone enum
public abstract class OrderStatus : Enumeration
{
    public static OrderStatus Pending = new PendingStatus();
    public static OrderStatus Confirmed = new ConfirmedStatus();
    public static OrderStatus Shipped = new ShippedStatus();
    public static OrderStatus Delivered = new DeliveredStatus();
    public static OrderStatus Cancelled = new CancelledStatus();

    protected OrderStatus(int id, string name) : base(id, name) { }

    // Domain logic in de enumeration
    public abstract bool CanTransitionTo(OrderStatus newStatus);
    public abstract decimal ShippingCostMultiplier { get; }

    private class PendingStatus : OrderStatus
    {
        public PendingStatus() : base(1, nameof(Pending)) { }
        
        public override bool CanTransitionTo(OrderStatus newStatus) 
            => newStatus == Confirmed || newStatus == Cancelled;
        
        public override decimal ShippingCostMultiplier => 1.0m;
    }

    private class ConfirmedStatus : OrderStatus
    {
        public ConfirmedStatus() : base(2, nameof(Confirmed)) { }
        
        public override bool CanTransitionTo(OrderStatus newStatus) 
            => newStatus == Shipped || newStatus == Cancelled;
        
        public override decimal ShippingCostMultiplier => 1.0m;
    }

    // ... andere statussen
}

// Base class
public abstract class Enumeration : IComparable
{
    public int Id { get; }
    public string Name { get; }

    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString() => Name;
    
    public override bool Equals(object obj) 
        => obj is Enumeration other && Id == other.Id;
    
    public override int GetHashCode() => Id.GetHashCode();

    public static IEnumerable<T> GetAll<T>() where T : Enumeration 
        => typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();

    public int CompareTo(object obj) 
        => Id.CompareTo(((Enumeration)obj).Id);
}
```

**Gebruik:**

```csharp
public class Order
{
    public OrderStatus Status { get; private set; }

    public void Ship()
    {
        if (!Status.CanTransitionTo(OrderStatus.Shipped))
            throw new InvalidOperationException(
                $"Cannot ship order in status {Status}");
        
        Status = OrderStatus.Shipped;
    }

    public decimal CalculateShippingCost(decimal basePrice)
        => basePrice * Status.ShippingCostMultiplier;
}
```

### 3. State Pattern (complexe business logic)

Voor complexe state machines gebruik je het **State Pattern**:

```csharp
// Abstract state
public abstract class OrderState
{
    public abstract void Confirm(Order order);
    public abstract void Ship(Order order);
    public abstract void Cancel(Order order);
}

// Concrete states
public class PendingState : OrderState
{
    public override void Confirm(Order order)
    {
        order.TransitionTo(new ConfirmedState());
    }

    public override void Ship(Order order)
    {
        throw new InvalidOperationException("Cannot ship pending order");
    }

    public override void Cancel(Order order)
    {
        order.TransitionTo(new CancelledState());
    }
}

// Entity met state
public class Order
{
    public OrderState State { get; private set; }

    public void Confirm() => State.Confirm(this);
    public void Ship() => State.Ship(this);
    
    internal void TransitionTo(OrderState newState)
    {
        State = newState;
    }
}
```

### 4. Database Mapping Overwegingen

#### Simple Enum (database int/string)
```csharp
// EF Core mapping
modelBuilder.Entity<Order>()
    .Property(o => o.Status)
    .HasConversion<string>(); // Of <int>
```

#### Enumeration Class (database int/string + conversie)
```csharp
modelBuilder.Entity<Order>()
    .Property(o => o.Status)
    .HasConversion(
        v => v.Id, // Naar database: int
        v => OrderStatus.FromId(v) // Uit database: reconstruct
    );

// Helper method in Enumeration base class
public static T FromId<T>(int id) where T : Enumeration
    => GetAll<T>().FirstOrDefault(e => e.Id == id)
       ?? throw new InvalidOperationException($"Unknown {typeof(T).Name} id: {id}");
```

### Wanneer welke aanpak?

| Scenario | Aanpak | Voorbeeld |
|----------|--------|-----------|
| **Simple waarden, geen logica** | Gewone C# enum | `PaymentMethod`, `Gender` |
| **Waarden + wat logica/properties** | Enumeration Class | `OrderStatus`, `Priority` |
| **Complexe state transitions** | State Pattern | Workflow engines, order processing |
| **Externe integratie (APIs)** | Enum met string conversie | API response codes |

### Samenvatting in DDD termen

- **Enum = Value Object** (geen identiteit, immutable)
- **Enumeration Class = Smart Value Object** (met gedrag)
- **State Pattern = Strategy/State behavioral pattern** (complexe transities)
