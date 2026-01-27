# Dependency Injection Pattern

## Intentie
Een techniek waarbij een object zijn dependencies ontvangt van een externe bron in plaats van ze zelf te creëren. Dit bevordert **loose coupling** en **testbaarheid**.

## Wanneer gebruiken?
- Altijd in moderne applicaties!
- Wanneer je loose coupling wilt tussen componenten
- Wanneer je unit testing wilt vereenvoudigen
- Wanneer je dependencies wilt kunnen wisselen

## Soorten Dependency Injection

### 1. Constructor Injection (Aanbevolen)

```csharp
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}

public interface ILogger
{
    void Log(string message);
}

// Dependencies via constructor
public class OrderService
{
    private readonly IEmailService _emailService;
    private readonly ILogger _logger;
    private readonly IOrderRepository _repository;

    public OrderService(
        IEmailService emailService, 
        ILogger logger,
        IOrderRepository repository)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task PlaceOrderAsync(Order order)
    {
        _logger.Log($"Placing order {order.Id}");
        
        await _repository.AddAsync(order);
        await _emailService.SendAsync(order.CustomerEmail, "Order Confirmed", $"Order {order.Id}");
        
        _logger.Log($"Order {order.Id} placed successfully");
    }
}
```

### 2. Property Injection (Optionele dependencies)

```csharp
public class ReportGenerator
{
    // Required dependency via constructor
    private readonly IDataSource _dataSource;
    
    // Optional dependency via property
    public ILogger? Logger { get; set; }

    public ReportGenerator(IDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public Report Generate()
    {
        Logger?.Log("Generating report...");
        var data = _dataSource.GetData();
        // Generate report...
        return new Report(data);
    }
}
```

### 3. Method Injection (Per-call dependencies)

```csharp
public class DataProcessor
{
    public void Process(IDataSource dataSource, IValidator validator)
    {
        var data = dataSource.GetData();
        if (validator.Validate(data))
        {
            // Process data
        }
    }
}
```

## .NET Dependency Injection Container

### Service Registration

```csharp
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Transient: New instance elke keer
services.AddTransient<IEmailService, SmtpEmailService>();

// Scoped: Eén instance per scope (bijv. HTTP request)
services.AddScoped<IOrderRepository, OrderRepository>();

// Singleton: Eén instance voor hele applicatie
services.AddSingleton<ILogger, FileLogger>();

// Factory registration
services.AddTransient<IPaymentService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var gateway = config["PaymentGateway"];
    
    return gateway switch
    {
        "Stripe" => new StripePaymentService(config["Stripe:ApiKey"]),
        "PayPal" => new PayPalPaymentService(config["PayPal:ClientId"]),
        _ => throw new InvalidOperationException($"Unknown gateway: {gateway}")
    };
});

// Implementation type registration
services.AddTransient<OrderService>();

var serviceProvider = services.BuildServiceProvider();

// Resolve service
var orderService = serviceProvider.GetRequiredService<OrderService>();
```

### ASP.NET Core Registration

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IEmailService, SmtpEmailService>();

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Register HTTP clients
builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
{
    client.BaseAddress = new Uri("https://api.weather.com/");
});

// Register options
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Email"));

var app = builder.Build();
```

### Praktisch Voorbeeld: Complete Application

```csharp
// Interfaces
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface ITokenService
{
    string GenerateToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
}

// Implementations
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
        => await _context.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}

public class JwtTokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_settings.ExpirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        // Validation logic...
        return null;
    }
}

// Service using injected dependencies
public class AuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<AuthenticationService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        _logger.LogInformation("Registering user: {Email}", request.Email);

        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing != null)
        {
            return AuthResult.Fail("Email already registered");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = request.Name,
            PasswordHash = _passwordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        var token = _tokenService.GenerateToken(user);
        return AuthResult.Success(token);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("Login attempt: {Email}", request.Email);

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            return AuthResult.Fail("Invalid credentials");
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return AuthResult.Fail("Invalid credentials");
        }

        var token = _tokenService.GenerateToken(user);
        return AuthResult.Success(token);
    }
}

// Registration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddScoped<AuthenticationService>();
        
        return services;
    }
}
```

### Unit Testing met Mocks

```csharp
public class AuthenticationServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<ILogger<AuthenticationService>> _loggerMock;
    private readonly AuthenticationService _sut;

    public AuthenticationServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();
        _loggerMock = new Mock<ILogger<AuthenticationService>>();

        _sut = new AuthenticationService(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var user = new User 
        { 
            Id = Guid.NewGuid(), 
            Email = "test@test.com",
            PasswordHash = "hashed" 
        };
        
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("test@test.com"))
            .ReturnsAsync(user);
        
        _passwordHasherMock
            .Setup(x => x.Verify("password123", "hashed"))
            .Returns(true);
        
        _tokenServiceMock
            .Setup(x => x.GenerateToken(user))
            .Returns("jwt_token_here");

        // Act
        var result = await _sut.LoginAsync(new LoginRequest("test@test.com", "password123"));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("jwt_token_here", result.Token);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsFail()
    {
        // Arrange
        var user = new User { Email = "test@test.com", PasswordHash = "hashed" };
        
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("test@test.com"))
            .ReturnsAsync(user);
        
        _passwordHasherMock
            .Setup(x => x.Verify("wrongpassword", "hashed"))
            .Returns(false);

        // Act
        var result = await _sut.LoginAsync(new LoginRequest("test@test.com", "wrongpassword"));

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials", result.Error);
    }
}
```

## Service Lifetimes

| Lifetime | Beschrijving | Gebruik |
|----------|-------------|---------|
| **Transient** | Nieuwe instance elke keer | Lightweight, stateless services |
| **Scoped** | Eén instance per scope/request | DbContext, repositories |
| **Singleton** | Eén instance voor hele app | Caching, configuration |

### Lifetime Regels

```csharp
// ?? FOUT: Singleton met Scoped dependency
services.AddSingleton<ISingletonService, SingletonService>();
services.AddScoped<IScopedService, ScopedService>();

public class SingletonService : ISingletonService
{
    // ? Captive dependency problem!
    private readonly IScopedService _scopedService;
    
    public SingletonService(IScopedService scopedService)
    {
        _scopedService = scopedService; // Will use same scoped instance forever!
    }
}

// ? CORRECT: Gebruik IServiceProvider of factory
public class SingletonService : ISingletonService
{
    private readonly IServiceProvider _serviceProvider;
    
    public SingletonService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void DoWork()
    {
        using var scope = _serviceProvider.CreateScope();
        var scopedService = scope.ServiceProvider.GetRequiredService<IScopedService>();
        // Use scopedService...
    }
}
```

## Voordelen
- ? Loose coupling
- ? Makkelijker unit testing
- ? Flexible configuratie
- ? Single Responsibility
- ? Lifetime management

## Nadelen
- ? Meer abstractie layers
- ? Runtime errors bij missing registrations
- ? Constructor kan veel parameters krijgen

## Best Practices

1. **Prefer constructor injection** voor required dependencies
2. **Gebruik interfaces** voor abstractions
3. **Registreer in composition root** (Startup/Program.cs)
4. **Let op lifetime mismatches**
5. **Vermijd Service Locator pattern**

## Gerelateerde Patterns
- **Factory**: DI container is een soort factory
- **Strategy**: Dependencies kunnen strategies zijn
- **Decorator**: DI kan decorators registreren
