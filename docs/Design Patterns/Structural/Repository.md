# Repository Pattern

## Intentie
Medieert tussen de domain/business layer en de data mapping layer door een **collectie-achtige interface** te bieden voor toegang tot domain objecten.

## Wanneer gebruiken?
- Abstractie van data persistence logic
- Centraliseren van query logic
- Unit testing met mock repositories
- Wisselen tussen data sources
- Domain-Driven Design implementaties

## Structuur

```
???????????????????????
?   Domain Layer      ?
?   ???????????????   ?
?   ?   Service   ?   ?
?   ???????????????   ?
?          ?          ?
?   ???????????????   ?
?   ? IRepository ?   ?
?   ???????????????   ?
???????????????????????
          ?
???????????????????????
?   Infrastructure    ?
?   ???????????????   ?
?   ?  Repository ?   ?
?   ?   (EF Core) ?   ?
?   ???????????????   ?
?          ?          ?
?   ???????????????   ?
?   ?  Database   ?   ?
?   ???????????????   ?
???????????????????????
```

## Implementatie in C#

### Generic Repository Interface

```csharp
public interface IRepository<T> where T : class
{
    // Query
    T? GetById(int id);
    T? GetById(Guid id);
    IEnumerable<T> GetAll();
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    
    // Commands
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    
    // Async variants
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
}
```

### Generic Repository Implementation (EF Core)

```csharp
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual T? GetById(int id) => _dbSet.Find(id);
    
    public virtual T? GetById(Guid id) => _dbSet.Find(id);

    public virtual IEnumerable<T> GetAll() => _dbSet.ToList();

    public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        => _dbSet.Where(predicate).ToList();

    public virtual void Add(T entity) => _dbSet.Add(entity);

    public virtual void AddRange(IEnumerable<T> entities) => _dbSet.AddRange(entities);

    public virtual void Update(T entity) => _dbSet.Update(entity);

    public virtual void Remove(T entity) => _dbSet.Remove(entity);

    public virtual void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);
}
```

### Specific Repository met Domain Logic

```csharp
// Domain Entity
public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderLine> Lines { get; set; } = new();
    public Customer Customer { get; set; }
}

public enum OrderStatus { Pending, Confirmed, Shipped, Delivered, Cancelled }

// Specific Repository Interface
public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetPendingOrdersAsync(CancellationToken ct = default);
    Task<IEnumerable<Order>> GetOrdersInDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<Order?> GetWithLinesAsync(Guid orderId, CancellationToken ct = default);
    Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to, CancellationToken ct = default);
}

// Implementation
public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context)
    {
    }

    private AppDbContext AppContext => (AppDbContext)_context;

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(
        Guid customerId, 
        CancellationToken ct = default)
    {
        return await _dbSet
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Order>> GetPendingOrdersAsync(CancellationToken ct = default)
    {
        return await _dbSet
            .Where(o => o.Status == OrderStatus.Pending)
            .Include(o => o.Customer)
            .OrderBy(o => o.OrderDate)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Order>> GetOrdersInDateRangeAsync(
        DateTime from, 
        DateTime to, 
        CancellationToken ct = default)
    {
        return await _dbSet
            .Where(o => o.OrderDate >= from && o.OrderDate <= to)
            .Include(o => o.Lines)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(ct);
    }

    public async Task<Order?> GetWithLinesAsync(Guid orderId, CancellationToken ct = default)
    {
        return await _dbSet
            .Include(o => o.Lines)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);
    }

    public async Task<decimal> GetTotalRevenueAsync(
        DateTime from, 
        DateTime to, 
        CancellationToken ct = default)
    {
        return await _dbSet
            .Where(o => o.OrderDate >= from && o.OrderDate <= to)
            .Where(o => o.Status != OrderStatus.Cancelled)
            .SumAsync(o => o.TotalAmount, ct);
    }
}
```

### Unit of Work Pattern

```csharp
public interface IUnitOfWork : IDisposable
{
    IOrderRepository Orders { get; }
    IRepository<Customer> Customers { get; }
    IRepository<Product> Products { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    private IOrderRepository? _orders;
    private IRepository<Customer>? _customers;
    private IRepository<Product>? _products;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IOrderRepository Orders => 
        _orders ??= new OrderRepository(_context);

    public IRepository<Customer> Customers => 
        _customers ??= new Repository<Customer>(_context);

    public IRepository<Product> Products => 
        _products ??= new Repository<Product>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

// Usage in Service
public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Order> CreateOrderAsync(Guid customerId, List<OrderLineDto> lines)
    {
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
                throw new NotFoundException("Customer not found");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };

            foreach (var lineDto in lines)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(lineDto.ProductId);
                order.Lines.Add(new OrderLine
                {
                    ProductId = lineDto.ProductId,
                    Quantity = lineDto.Quantity,
                    UnitPrice = product!.Price
                });
            }

            order.TotalAmount = order.Lines.Sum(l => l.Quantity * l.UnitPrice);

            _unitOfWork.Orders.Add(order);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return order;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
```

### Specification Pattern Integration

```csharp
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    int? Take { get; }
    int? Skip { get; }
}

public abstract class Specification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; private set; } = _ => true;
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public int? Take { get; private set; }
    public int? Skip { get; private set; }

    protected void AddCriteria(Expression<Func<T, bool>> criteria) => Criteria = criteria;
    protected void AddInclude(Expression<Func<T, object>> include) => Includes.Add(include);
    protected void AddInclude(string include) => IncludeStrings.Add(include);
    protected void ApplyOrderBy(Expression<Func<T, object>> orderBy) => OrderBy = orderBy;
    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderBy) => OrderByDescending = orderBy;
    protected void ApplyPaging(int skip, int take) { Skip = skip; Take = take; }
}

// Concrete Specification
public class PendingOrdersForCustomerSpec : Specification<Order>
{
    public PendingOrdersForCustomerSpec(Guid customerId)
    {
        AddCriteria(o => o.CustomerId == customerId && o.Status == OrderStatus.Pending);
        AddInclude(o => o.Lines);
        ApplyOrderByDescending(o => o.OrderDate);
    }
}

public class RecentOrdersSpec : Specification<Order>
{
    public RecentOrdersSpec(int days, int pageNumber, int pageSize)
    {
        var fromDate = DateTime.UtcNow.AddDays(-days);
        AddCriteria(o => o.OrderDate >= fromDate);
        AddInclude(o => o.Customer);
        ApplyOrderByDescending(o => o.OrderDate);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}

// Repository with Specification support
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> FindAsync(ISpecification<T> specification, CancellationToken ct = default);
    Task<T?> FindOneAsync(ISpecification<T> specification, CancellationToken ct = default);
    Task<int> CountAsync(ISpecification<T> specification, CancellationToken ct = default);
}

// Usage
var spec = new PendingOrdersForCustomerSpec(customerId);
var orders = await _orderRepository.FindAsync(spec);
```

### DI Registration

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IOrderRepository, OrderRepository>();
        
        return services;
    }
}

// In Program.cs
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddRepositories();
```

## Voordelen
- ? Abstractie van data access code
- ? Centraliseren van query logic
- ? Makkelijker unit testing met mocks
- ? Wisselen tussen data sources
- ? Separation of concerns

## Nadelen
- ? Kan een extra abstractie laag zijn over EF Core
- ? Generic repositories kunnen te beperkt zijn
- ? Kan leiden tot "leaky abstractions"

## Best Practices

1. **Gebruik specifieke repositories** voor complexe queries
2. **Combineer met Unit of Work** voor transacties
3. **Overweeg Specification Pattern** voor herbruikbare queries
4. **Vermijd exposing IQueryable** (leaky abstraction)
5. **In simpele applicaties**: overweeg EF Core direct te gebruiken

## Gerelateerde Patterns
- **Unit of Work**: Coördineert repositories in transactie
- **Specification**: Encapsuleert query criteria
- **Factory**: Voor complexe entity creatie
