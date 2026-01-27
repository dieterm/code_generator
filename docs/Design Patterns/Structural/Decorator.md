# Decorator Pattern

## Intentie
Voegt **dynamisch extra verantwoordelijkheden** toe aan een object. Decorators bieden een flexibel alternatief voor subclassing om functionaliteit uit te breiden.

## Wanneer gebruiken?
- Wanneer je gedrag wilt toevoegen aan individuele objecten zonder andere objecten te beïnvloeden
- Wanneer uitbreiding via subclassing onpraktisch is
- Wanneer je functionaliteit wilt toevoegen/verwijderen at runtime
- Cross-cutting concerns: logging, caching, validatie

## Structuur

```
?????????????????
?   Component   ?
?????????????????
? +Operation()  ?
?????????????????
        ?
   ???????????
   ?         ?
????????  ???????????????
?Concr.?  ?  Decorator  ????????
?Comp. ?  ???????????????      ?
????????  ? -component  ????????
          ? +Operation()?
          ???????????????
                 ?
          ???????????????
          ?             ?
     ???????????  ?????????????
     ?DecorA   ?  ?  DecorB   ?
     ???????????  ?????????????
```

## Implementatie in C#

### Basis Implementatie

```csharp
// Component interface
public interface ICoffee
{
    string GetDescription();
    decimal GetCost();
}

// Concrete Component
public class SimpleCoffee : ICoffee
{
    public string GetDescription() => "Simple Coffee";
    public decimal GetCost() => 2.00m;
}

// Base Decorator
public abstract class CoffeeDecorator : ICoffee
{
    protected readonly ICoffee _coffee;

    protected CoffeeDecorator(ICoffee coffee)
    {
        _coffee = coffee;
    }

    public virtual string GetDescription() => _coffee.GetDescription();
    public virtual decimal GetCost() => _coffee.GetCost();
}

// Concrete Decorators
public class MilkDecorator : CoffeeDecorator
{
    public MilkDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription() => $"{_coffee.GetDescription()}, Milk";
    public override decimal GetCost() => _coffee.GetCost() + 0.50m;
}

public class SugarDecorator : CoffeeDecorator
{
    public SugarDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription() => $"{_coffee.GetDescription()}, Sugar";
    public override decimal GetCost() => _coffee.GetCost() + 0.20m;
}

public class WhippedCreamDecorator : CoffeeDecorator
{
    public WhippedCreamDecorator(ICoffee coffee) : base(coffee) { }

    public override string GetDescription() => $"{_coffee.GetDescription()}, Whipped Cream";
    public override decimal GetCost() => _coffee.GetCost() + 0.70m;
}

// Gebruik - decorators kunnen gestapeld worden!
ICoffee coffee = new SimpleCoffee();
coffee = new MilkDecorator(coffee);
coffee = new SugarDecorator(coffee);
coffee = new WhippedCreamDecorator(coffee);

Console.WriteLine(coffee.GetDescription()); // Simple Coffee, Milk, Sugar, Whipped Cream
Console.WriteLine($"Cost: €{coffee.GetCost()}"); // Cost: €3.40
```

### Praktisch Voorbeeld: Stream Decorators (Built-in .NET)

```csharp
// .NET's Stream is een perfect voorbeeld van Decorator pattern
using var fileStream = new FileStream("data.txt", FileMode.Create);
using var bufferedStream = new BufferedStream(fileStream); // Decorator
using var gzipStream = new GZipStream(bufferedStream, CompressionMode.Compress); // Decorator
using var writer = new StreamWriter(gzipStream); // Decorator

writer.WriteLine("Hello, Decorated World!");
```

### Praktisch Voorbeeld: Repository met Caching & Logging

```csharp
// Component interface
public interface IUserRepository
{
    User? GetById(int id);
    IEnumerable<User> GetAll();
    void Save(User user);
    void Delete(int id);
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

// Concrete Component
public class UserRepository : IUserRepository
{
    private readonly Dictionary<int, User> _users = new();

    public User? GetById(int id)
    {
        _users.TryGetValue(id, out var user);
        return user;
    }

    public IEnumerable<User> GetAll() => _users.Values;

    public void Save(User user)
    {
        _users[user.Id] = user;
    }

    public void Delete(int id)
    {
        _users.Remove(id);
    }
}

// Logging Decorator
public class LoggingUserRepositoryDecorator : IUserRepository
{
    private readonly IUserRepository _inner;
    private readonly ILogger _logger;

    public LoggingUserRepositoryDecorator(IUserRepository inner, ILogger logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public User? GetById(int id)
    {
        _logger.Log($"Getting user by id: {id}");
        var result = _inner.GetById(id);
        _logger.Log($"Found user: {result?.Name ?? "null"}");
        return result;
    }

    public IEnumerable<User> GetAll()
    {
        _logger.Log("Getting all users");
        var result = _inner.GetAll().ToList();
        _logger.Log($"Found {result.Count} users");
        return result;
    }

    public void Save(User user)
    {
        _logger.Log($"Saving user: {user.Name}");
        _inner.Save(user);
        _logger.Log($"User saved with id: {user.Id}");
    }

    public void Delete(int id)
    {
        _logger.Log($"Deleting user: {id}");
        _inner.Delete(id);
        _logger.Log($"User deleted: {id}");
    }
}

// Caching Decorator
public class CachingUserRepositoryDecorator : IUserRepository
{
    private readonly IUserRepository _inner;
    private readonly Dictionary<int, User> _cache = new();
    private List<User>? _allUsersCache;
    private DateTime _allUsersCacheExpiry;

    public CachingUserRepositoryDecorator(IUserRepository inner)
    {
        _inner = inner;
    }

    public User? GetById(int id)
    {
        if (_cache.TryGetValue(id, out var user))
        {
            Console.WriteLine($"Cache hit for user {id}");
            return user;
        }

        Console.WriteLine($"Cache miss for user {id}");
        user = _inner.GetById(id);
        
        if (user != null)
        {
            _cache[id] = user;
        }
        
        return user;
    }

    public IEnumerable<User> GetAll()
    {
        if (_allUsersCache != null && DateTime.UtcNow < _allUsersCacheExpiry)
        {
            Console.WriteLine("Cache hit for all users");
            return _allUsersCache;
        }

        Console.WriteLine("Cache miss for all users");
        _allUsersCache = _inner.GetAll().ToList();
        _allUsersCacheExpiry = DateTime.UtcNow.AddMinutes(5);
        
        return _allUsersCache;
    }

    public void Save(User user)
    {
        _inner.Save(user);
        _cache[user.Id] = user;
        _allUsersCache = null; // Invalidate list cache
    }

    public void Delete(int id)
    {
        _inner.Delete(id);
        _cache.Remove(id);
        _allUsersCache = null;
    }
}

// Validation Decorator
public class ValidatingUserRepositoryDecorator : IUserRepository
{
    private readonly IUserRepository _inner;

    public ValidatingUserRepositoryDecorator(IUserRepository inner)
    {
        _inner = inner;
    }

    public User? GetById(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be positive", nameof(id));
        
        return _inner.GetById(id);
    }

    public IEnumerable<User> GetAll() => _inner.GetAll();

    public void Save(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(user.Name))
            throw new ArgumentException("Name is required", nameof(user));
        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ArgumentException("Email is required", nameof(user));
        if (!user.Email.Contains('@'))
            throw new ArgumentException("Email is invalid", nameof(user));

        _inner.Save(user);
    }

    public void Delete(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be positive", nameof(id));
        
        _inner.Delete(id);
    }
}

// Gebruik - stack decorators!
IUserRepository repository = new UserRepository();
repository = new ValidatingUserRepositoryDecorator(repository);
repository = new CachingUserRepositoryDecorator(repository);
repository = new LoggingUserRepositoryDecorator(repository, new ConsoleLogger());

// Elke call gaat door alle decorators
repository.Save(new User { Id = 1, Name = "John", Email = "john@example.com" });
var user = repository.GetById(1); // Logged, cached, validated
```

### Met Dependency Injection

```csharp
// DI Registration met Scrutor library
services.AddScoped<UserRepository>();
services.AddScoped<IUserRepository>(provider =>
{
    var repo = provider.GetRequiredService<UserRepository>();
    var logger = provider.GetRequiredService<ILogger>();
    
    IUserRepository decorated = new ValidatingUserRepositoryDecorator(repo);
    decorated = new CachingUserRepositoryDecorator(decorated);
    decorated = new LoggingUserRepositoryDecorator(decorated, logger);
    
    return decorated;
});

// Met Scrutor (NuGet package)
services.AddScoped<IUserRepository, UserRepository>();
services.Decorate<IUserRepository, ValidatingUserRepositoryDecorator>();
services.Decorate<IUserRepository, CachingUserRepositoryDecorator>();
services.Decorate<IUserRepository, LoggingUserRepositoryDecorator>();
```

### Generic Decorator Base

```csharp
public abstract class RepositoryDecorator<T> : IRepository<T> where T : class
{
    protected readonly IRepository<T> Inner;

    protected RepositoryDecorator(IRepository<T> inner)
    {
        Inner = inner;
    }

    public virtual T? GetById(int id) => Inner.GetById(id);
    public virtual IEnumerable<T> GetAll() => Inner.GetAll();
    public virtual void Add(T entity) => Inner.Add(entity);
    public virtual void Update(T entity) => Inner.Update(entity);
    public virtual void Delete(int id) => Inner.Delete(id);
}

// Concrete decorator hoeft alleen specifieke methods te overriden
public class LoggingRepositoryDecorator<T> : RepositoryDecorator<T> where T : class
{
    private readonly ILogger _logger;

    public LoggingRepositoryDecorator(IRepository<T> inner, ILogger logger) : base(inner)
    {
        _logger = logger;
    }

    public override T? GetById(int id)
    {
        _logger.Log($"Getting {typeof(T).Name} by id: {id}");
        return base.GetById(id);
    }

    // Andere methods gebruiken default gedrag van base class
}
```

## Voordelen
- ? Meer flexibiliteit dan inheritance
- ? Single Responsibility: elke decorator heeft één verantwoordelijkheid
- ? Open/Closed: nieuwe functionaliteit zonder bestaande code te wijzigen
- ? Combineerbaar: decorators kunnen gestapeld worden
- ? Runtime configureerbaar

## Nadelen
- ? Veel kleine objecten
- ? Volgorde van decorators kan belangrijk zijn
- ? Kan verwarrend zijn om te debuggen

## Decorator vs Inheritance

| Aspect | Decorator | Inheritance |
|--------|-----------|-------------|
| **Tijdstip** | Runtime | Compile-time |
| **Combinaties** | Vrij te combineren | Vast in hiërarchie |
| **Objecten** | Per instance | Per class |
| **Complexiteit** | Meer objecten | Meer classes |

## Gerelateerde Patterns
- **Adapter**: Andere interface, Decorator zelfde interface
- **Composite**: Decorator is een beperkte Composite met één child
- **Strategy**: Decorator wijzigt de "skin", Strategy wijzigt de "guts"
- **Proxy**: Zelfde interface, maar ander doel (access control)
