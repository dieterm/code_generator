# Specification Pattern

## Intentie
Encapsuleer **business rules en selectiecriteria** in herbruikbare objecten die kunnen worden **gecombineerd** om complexe queries en validaties te bouwen.

## Wanneer gebruiken?
- Complexe selectiecriteria voor queries
- Business rule validatie
- Filtering van collecties
- Dynamisch bouwen van queries
- Herbruikbare selectielogica
- Domain-Driven Design repositories

## Structuur

```
???????????????????????
?  ISpecification<T>  ?
???????????????????????
? +IsSatisfiedBy(T)   ?
? +ToExpression()     ?
? +And(spec)          ?
? +Or(spec)           ?
? +Not()              ?
???????????????????????
          ?
          ?
    ??????????????????????????????????????????
    ?           ?             ?              ?
??????????  ??????????  ????????????  ???????????????
?Concrete?  ?   And  ?  ?    Or    ?  ?     Not     ?
?  Spec  ?  ?  Spec  ?  ?   Spec   ?  ?    Spec     ?
??????????  ??????????  ????????????  ???????????????
```

## Implementatie in C#

### Basis Interface

```csharp
using System;
using System.Linq.Expressions;

/// <summary>
/// Base specification interface
/// </summary>
public interface ISpecification<T>
{
    /// <summary>
    /// Check if entity satisfies the specification
    /// </summary>
    bool IsSatisfiedBy(T entity);
    
    /// <summary>
    /// Convert specification to LINQ expression for database queries
    /// </summary>
    Expression<Func<T, bool>> ToExpression();
}
```

### Composite Specifications

```csharp
/// <summary>
/// AND specification - combines two specifications
/// </summary>
public class AndSpecification<T> : ISpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);
    }

    public Expression<Func<T, bool>> ToExpression()
    {
        var leftExpr = _left.ToExpression();
        var rightExpr = _right.ToExpression();
        
        var parameter = Expression.Parameter(typeof(T));
        var combined = Expression.AndAlso(
            Expression.Invoke(leftExpr, parameter),
            Expression.Invoke(rightExpr, parameter)
        );
        
        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}

/// <summary>
/// OR specification - either specification can be satisfied
/// </summary>
public class OrSpecification<T> : ISpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
    }

    public bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) || _right.IsSatisfiedBy(entity);
    }

    public Expression<Func<T, bool>> ToExpression()
    {
        var leftExpr = _left.ToExpression();
        var rightExpr = _right.ToExpression();
        
        var parameter = Expression.Parameter(typeof(T));
        var combined = Expression.OrElse(
            Expression.Invoke(leftExpr, parameter),
            Expression.Invoke(rightExpr, parameter)
        );
        
        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}

/// <summary>
/// NOT specification - negates a specification
/// </summary>
public class NotSpecification<T> : ISpecification<T>
{
    private readonly ISpecification<T> _spec;

    public NotSpecification(ISpecification<T> spec)
    {
        _spec = spec ?? throw new ArgumentNullException(nameof(spec));
    }

    public bool IsSatisfiedBy(T entity)
    {
        return !_spec.IsSatisfiedBy(entity);
    }

    public Expression<Func<T, bool>> ToExpression()
    {
        var expr = _spec.ToExpression();
        var parameter = Expression.Parameter(typeof(T));
        var notExpr = Expression.Not(Expression.Invoke(expr, parameter));
        
        return Expression.Lambda<Func<T, bool>>(notExpr, parameter);
    }
}
```

### Extension Methods voor Fluent Syntax

```csharp
public static class SpecificationExtensions
{
    public static ISpecification<T> And<T>(
        this ISpecification<T> left, 
        ISpecification<T> right)
    {
        return new AndSpecification<T>(left, right);
    }

    public static ISpecification<T> Or<T>(
        this ISpecification<T> left, 
        ISpecification<T> right)
    {
        return new OrSpecification<T>(left, right);
    }

    public static ISpecification<T> Not<T>(this ISpecification<T> spec)
    {
        return new NotSpecification<T>(spec);
    }
}
```

### Voorbeeld: Customer Specifications

```csharp
// Domain entity
public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public CustomerStatus Status { get; set; }
    public DateTime RegisteredDate { get; set; }
    public string Region { get; set; }
    public decimal LifetimeValue { get; set; }
    public List<Order> Orders { get; set; } = new();
    public bool IsDeleted { get; set; }
}

public enum CustomerStatus
{
    New,
    Active,
    Inactive,
    Suspended
}

// Concrete specifications
public class CustomerIsActiveSpecification : ISpecification<Customer>
{
    public bool IsSatisfiedBy(Customer customer)
    {
        return customer.Status == CustomerStatus.Active && !customer.IsDeleted;
    }

    public Expression<Func<Customer, bool>> ToExpression()
    {
        return customer => customer.Status == CustomerStatus.Active 
                        && !customer.IsDeleted;
    }
}

public class CustomerInRegionSpecification : ISpecification<Customer>
{
    private readonly string _region;

    public CustomerInRegionSpecification(string region)
    {
        _region = region ?? throw new ArgumentNullException(nameof(region));
    }

    public bool IsSatisfiedBy(Customer customer)
    {
        return customer.Region.Equals(_region, StringComparison.OrdinalIgnoreCase);
    }

    public Expression<Func<Customer, bool>> ToExpression()
    {
        return customer => customer.Region.ToLower() == _region.ToLower();
    }
}

public class CustomerHasOrdersSpecification : ISpecification<Customer>
{
    public bool IsSatisfiedBy(Customer customer)
    {
        return customer.Orders.Any();
    }

    public Expression<Func<Customer, bool>> ToExpression()
    {
        return customer => customer.Orders.Any();
    }
}

public class CustomerLifetimeValueSpecification : ISpecification<Customer>
{
    private readonly decimal _minValue;

    public CustomerLifetimeValueSpecification(decimal minValue)
    {
        _minValue = minValue;
    }

    public bool IsSatisfiedBy(Customer customer)
    {
        return customer.LifetimeValue >= _minValue;
    }

    public Expression<Func<Customer, bool>> ToExpression()
    {
        return customer => customer.LifetimeValue >= _minValue;
    }
}

public class CustomerRegisteredAfterSpecification : ISpecification<Customer>
{
    private readonly DateTime _date;

    public CustomerRegisteredAfterSpecification(DateTime date)
    {
        _date = date;
    }

    public bool IsSatisfiedBy(Customer customer)
    {
        return customer.RegisteredDate >= _date;
    }

    public Expression<Func<Customer, bool>> ToExpression()
    {
        return customer => customer.RegisteredDate >= _date;
    }
}
```

### Repository met Specification Support

```csharp
public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> FindAsync(ISpecification<Customer> specification);
    Task<Customer?> FindOneAsync(ISpecification<Customer> specification);
    Task<int> CountAsync(ISpecification<Customer> specification);
    Task<bool> AnyAsync(ISpecification<Customer> specification);
}

public class CustomerRepository : ICustomerRepository
{
    private readonly DbContext _context;

    public CustomerRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> FindAsync(ISpecification<Customer> specification)
    {
        return await _context.Set<Customer>()
            .Where(specification.ToExpression())
            .ToListAsync();
    }

    public async Task<Customer?> FindOneAsync(ISpecification<Customer> specification)
    {
        return await _context.Set<Customer>()
            .Where(specification.ToExpression())
            .FirstOrDefaultAsync();
    }

    public async Task<int> CountAsync(ISpecification<Customer> specification)
    {
        return await _context.Set<Customer>()
            .Where(specification.ToExpression())
            .CountAsync();
    }

    public async Task<bool> AnyAsync(ISpecification<Customer> specification)
    {
        return await _context.Set<Customer>()
            .Where(specification.ToExpression())
            .AnyAsync();
    }
}
```

### Gebruik in Application Service

```csharp
public class CustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Customer>> GetActiveCustomersInRegionAsync(string region)
    {
        var specification = new CustomerIsActiveSpecification()
            .And(new CustomerInRegionSpecification(region));

        return await _repository.FindAsync(specification);
    }

    public async Task<IEnumerable<Customer>> GetHighValueCustomersAsync(decimal minValue)
    {
        var specification = new CustomerIsActiveSpecification()
            .And(new CustomerLifetimeValueSpecification(minValue))
            .And(new CustomerHasOrdersSpecification());

        return await _repository.FindAsync(specification);
    }

    public async Task<IEnumerable<Customer>> GetRecentActiveCustomersAsync(int daysAgo)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysAgo);
        
        var specification = new CustomerIsActiveSpecification()
            .And(new CustomerRegisteredAfterSpecification(cutoffDate));

        return await _repository.FindAsync(specification);
    }

    public async Task<IEnumerable<Customer>> GetCustomersForCampaignAsync(
        string region, 
        decimal minLifetimeValue)
    {
        // Complex business rule: Active customers in region with high value OR recent customers
        var highValueInRegion = new CustomerIsActiveSpecification()
            .And(new CustomerInRegionSpecification(region))
            .And(new CustomerLifetimeValueSpecification(minLifetimeValue));

        var recentCustomers = new CustomerRegisteredAfterSpecification(
            DateTime.UtcNow.AddMonths(-3));

        var specification = highValueInRegion.Or(recentCustomers);

        return await _repository.FindAsync(specification);
    }
}
```

### Specification Factory

```csharp
/// <summary>
/// Factory for creating common customer specifications
/// </summary>
public class CustomerSpecificationFactory
{
    public static ISpecification<Customer> CreateActiveCustomers()
    {
        return new CustomerIsActiveSpecification();
    }

    public static ISpecification<Customer> CreateHighValueCustomers(decimal minValue)
    {
        return new CustomerIsActiveSpecification()
            .And(new CustomerLifetimeValueSpecification(minValue));
    }

    public static ISpecification<Customer> CreateCustomersInRegion(string region)
    {
        return new CustomerIsActiveSpecification()
            .And(new CustomerInRegionSpecification(region));
    }

    public static ISpecification<Customer> CreateRecentCustomers(int daysAgo)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysAgo);
        return new CustomerRegisteredAfterSpecification(cutoffDate);
    }

    public static ISpecification<Customer> CreatePremiumCustomers(
        string region, 
        decimal minLifetimeValue)
    {
        return new CustomerIsActiveSpecification()
            .And(new CustomerInRegionSpecification(region))
            .And(new CustomerLifetimeValueSpecification(minLifetimeValue))
            .And(new CustomerHasOrdersSpecification());
    }

    public static ISpecification<Customer> CreateInactiveCustomersWithoutOrders()
    {
        return new CustomerIsActiveSpecification()
            .Not()
            .And(new CustomerHasOrdersSpecification().Not());
    }
}

// Gebruik
var spec = CustomerSpecificationFactory.CreatePremiumCustomers("Europe", 10000);
var customers = await _repository.FindAsync(spec);
```

### In-Memory Filtering

```csharp
public class CustomerValidator
{
    public bool ValidateCustomer(Customer customer, ISpecification<Customer> specification)
    {
        return specification.IsSatisfiedBy(customer);
    }

    public IEnumerable<Customer> FilterCustomers(
        IEnumerable<Customer> customers, 
        ISpecification<Customer> specification)
    {
        return customers.Where(c => specification.IsSatisfiedBy(c));
    }
}

// Gebruik
var customers = GetAllCustomers();
var specification = new CustomerIsActiveSpecification()
    .And(new CustomerInRegionSpecification("Europe"));

var filtered = customers.Where(c => specification.IsSatisfiedBy(c)).ToList();
```

### Generic Specification Base Class

```csharp
public abstract class Specification<T> : ISpecification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();

    public bool IsSatisfiedBy(T entity)
    {
        var predicate = ToExpression().Compile();
        return predicate(entity);
    }

    public ISpecification<T> And(ISpecification<T> specification)
    {
        return new AndSpecification<T>(this, specification);
    }

    public ISpecification<T> Or(ISpecification<T> specification)
    {
        return new OrSpecification<T>(this, specification);
    }

    public ISpecification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}

// Gebruik met base class
public class ActiveCustomerSpec : Specification<Customer>
{
    public override Expression<Func<Customer, bool>> ToExpression()
    {
        return customer => customer.Status == CustomerStatus.Active 
                        && !customer.IsDeleted;
    }
}
```

### Unit Tests

```csharp
[TestClass]
public class CustomerSpecificationTests
{
    [TestMethod]
    public void CustomerIsActive_ActiveCustomer_ReturnsTrue()
    {
        // Arrange
        var customer = new Customer 
        { 
            Status = CustomerStatus.Active, 
            IsDeleted = false 
        };
        var spec = new CustomerIsActiveSpecification();

        // Act
        var result = spec.IsSatisfiedBy(customer);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CustomerIsActive_DeletedCustomer_ReturnsFalse()
    {
        // Arrange
        var customer = new Customer 
        { 
            Status = CustomerStatus.Active, 
            IsDeleted = true 
        };
        var spec = new CustomerIsActiveSpecification();

        // Act
        var result = spec.IsSatisfiedBy(customer);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void CombinedSpecification_AndOperator_WorksCorrectly()
    {
        // Arrange
        var customer = new Customer 
        { 
            Status = CustomerStatus.Active,
            IsDeleted = false,
            Region = "Europe",
            LifetimeValue = 5000
        };

        var spec = new CustomerIsActiveSpecification()
            .And(new CustomerInRegionSpecification("Europe"))
            .And(new CustomerLifetimeValueSpecification(1000));

        // Act
        var result = spec.IsSatisfiedBy(customer);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CombinedSpecification_OrOperator_WorksCorrectly()
    {
        // Arrange
        var customer = new Customer 
        { 
            Status = CustomerStatus.Inactive,
            Region = "Europe"
        };

        var spec = new CustomerIsActiveSpecification()
            .Or(new CustomerInRegionSpecification("Europe"));

        // Act
        var result = spec.IsSatisfiedBy(customer);

        // Assert
        Assert.IsTrue(result); // True because in Europe
    }

    [TestMethod]
    public void NotSpecification_NegatesResult()
    {
        // Arrange
        var customer = new Customer 
        { 
            Status = CustomerStatus.Active,
            IsDeleted = false 
        };
        var spec = new CustomerIsActiveSpecification().Not();

        // Act
        var result = spec.IsSatisfiedBy(customer);

        // Assert
        Assert.IsFalse(result);
    }
}
```

## Voordelen
- ? Herbruikbare business rules
- ? Testbare selectielogica
- ? Fluent en leesbare syntax
- ? Werkt met zowel database queries als in-memory filtering
- ? Single Responsibility - elke spec heeft één duidelijke regel
- ? Open/Closed Principle - nieuwe specs zonder bestaande code te wijzigen
- ? Vermijdt query logic duplication

## Nadelen
- ? Kan complex worden met veel specifications
- ? Expression tree manipulation kan lastig zijn
- ? Overhead voor simpele queries
- ? Moeilijker te debuggen dan inline LINQ

## Wanneer NIET gebruiken
- Simple CRUD operaties zonder complexe business rules
- One-off queries die nooit hergebruikt worden
- Performance-critical queries die handmatig geoptimaliseerd moeten worden

## Best Practices
1. **Keep specifications atomic** - één spec = één business rule
2. **Use specification factories** voor veelgebruikte combinaties
3. **Test specifications in isolation**
4. **Combine specifications in services**, niet in repositories
5. **Use meaningful names** die de business rule beschrijven

## Gerelateerde Patterns
- **Repository**: Specifications worden vaak gebruikt met Repository pattern
- **Strategy**: Beide encapsuleren algoritmes/gedrag
- **Composite**: Specification gebruikt Composite voor AND/OR/NOT
- **Chain of Responsibility**: Vergelijkbaar in het combineren van logica
- **Query Object**: Alternatief pattern voor query building

## Gebruik in Domain-Driven Design
In DDD horen specifications in de **Domain Layer**:
- Encapsuleren domain business rules
- Kunnen worden gebruikt voor validatie én queries
- Maken repositories expressiever
- Domain logic blijft uit de infrastructure layer

```csharp
// Domain Layer
Domain/
??? Entities/
?   ??? Customer.cs
??? Specifications/
?   ??? ISpecification.cs
?   ??? CustomerIsActiveSpecification.cs
?   ??? CustomerInRegionSpecification.cs
?   ??? AndSpecification.cs
??? Repositories/
    ??? ICustomerRepository.cs

// Infrastructure Layer
Infrastructure/
??? Repositories/
    ??? CustomerRepository.cs  // Uses specifications from Domain
```
