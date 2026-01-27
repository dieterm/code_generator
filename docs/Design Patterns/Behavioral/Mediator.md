# Mediator Pattern

## Intentie
Definieert een object dat encapsuleert hoe een set objecten met elkaar **communiceren**. Mediator bevordert loose coupling door te voorkomen dat objecten direct naar elkaar verwijzen.

## Wanneer gebruiken?
- Wanneer objecten op complexe manieren met elkaar communiceren
- Wanneer je coupling tussen componenten wilt reduceren
- CQRS (Command Query Responsibility Segregation)
- Request/Response pipelines
- Chat systemen, air traffic control

## Structuur

```
?????????????????       ???????????????????
?   Colleague   ???????>?    Mediator     ?
?????????????????       ???????????????????
? +Send()       ?       ? +Notify()       ?
? +Receive()    ?       ???????????????????
?????????????????               ?
       ?                        ?
       ?                ?????????????????
???????????????        ?ConcreteMediator?
?ColleagueA   ????????>?????????????????
?ColleagueB   ?        ?-colleagues    ?
???????????????        ?????????????????
```

## Implementatie in C#

### Basis Implementatie: Chat Room

```csharp
// Mediator interface
public interface IChatRoom
{
    void Register(User user);
    void Send(string message, User sender);
    void SendPrivate(string message, User sender, User recipient);
}

// Colleague base
public abstract class User
{
    protected IChatRoom _chatRoom;
    public string Name { get; }

    protected User(string name)
    {
        Name = name;
    }

    public void SetChatRoom(IChatRoom chatRoom)
    {
        _chatRoom = chatRoom;
    }

    public abstract void Send(string message);
    public abstract void SendPrivate(string message, User recipient);
    public abstract void Receive(string message, User sender);
}

// Concrete Mediator
public class ChatRoom : IChatRoom
{
    private readonly List<User> _users = new();

    public void Register(User user)
    {
        _users.Add(user);
        user.SetChatRoom(this);
        Console.WriteLine($"[{user.Name} joined the chat]");
    }

    public void Send(string message, User sender)
    {
        foreach (var user in _users)
        {
            if (user != sender)
            {
                user.Receive(message, sender);
            }
        }
    }

    public void SendPrivate(string message, User sender, User recipient)
    {
        recipient.Receive($"[Private] {message}", sender);
    }
}

// Concrete Colleagues
public class ChatUser : User
{
    public ChatUser(string name) : base(name) { }

    public override void Send(string message)
    {
        Console.WriteLine($"{Name} sends: {message}");
        _chatRoom.Send(message, this);
    }

    public override void SendPrivate(string message, User recipient)
    {
        Console.WriteLine($"{Name} (private to {recipient.Name}): {message}");
        _chatRoom.SendPrivate(message, this, recipient);
    }

    public override void Receive(string message, User sender)
    {
        Console.WriteLine($"{Name} received from {sender.Name}: {message}");
    }
}

// Gebruik
var chatRoom = new ChatRoom();

var alice = new ChatUser("Alice");
var bob = new ChatUser("Bob");
var charlie = new ChatUser("Charlie");

chatRoom.Register(alice);
chatRoom.Register(bob);
chatRoom.Register(charlie);

alice.Send("Hello everyone!");
bob.SendPrivate("Hi Alice, how are you?", alice);
charlie.Send("Hey team!");
```

### MediatR Pattern (CQRS)

```csharp
// Install: dotnet add package MediatR

// Request (Query)
public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;

// Response
public record UserDto(Guid Id, string Name, string Email);

// Handler
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _repository;

    public GetUserByIdQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (user == null)
            throw new NotFoundException($"User {request.Id} not found");

        return new UserDto(user.Id, user.Name, user.Email);
    }
}

// Command
public record CreateUserCommand(string Name, string Email, string Password) : IRequest<Guid>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IUserRepository repository, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password)
        };

        await _repository.AddAsync(user, cancellationToken);
        return user.Id;
    }
}

// Notification (Event)
public record UserCreatedNotification(Guid UserId, string Email) : INotification;

public class SendWelcomeEmailHandler : INotificationHandler<UserCreatedNotification>
{
    private readonly IEmailService _emailService;

    public SendWelcomeEmailHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        await _emailService.SendAsync(notification.Email, "Welcome!", "Thanks for joining!");
    }
}

public class AddToNewsletterHandler : INotificationHandler<UserCreatedNotification>
{
    private readonly INewsletterService _newsletterService;

    public AddToNewsletterHandler(INewsletterService newsletterService)
    {
        _newsletterService = newsletterService;
    }

    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        await _newsletterService.SubscribeAsync(notification.Email);
    }
}

// Usage in Controller
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> Get(Guid id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateUserCommand command)
    {
        var userId = await _mediator.Send(command);
        
        // Publish notification (multiple handlers can respond)
        await _mediator.Publish(new UserCreatedNotification(userId, command.Email));
        
        return CreatedAtAction(nameof(Get), new { id = userId }, userId);
    }
}

// DI Registration
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
```

### Pipeline Behaviors (Cross-cutting Concerns)

```csharp
// Logging Behavior
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("Handling {RequestName}: {@Request}", requestName, request);
        
        var response = await next();
        
        _logger.LogInformation("Handled {RequestName}", requestName);
        
        return response;
    }
}

// Validation Behavior
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}

// Performance Behavior
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _timer;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _timer = new Stopwatch();
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();
        
        var response = await next();
        
        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500) // Log slow requests
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogWarning(
                "Long Running Request: {Name} ({ElapsedMilliseconds}ms) {@Request}",
                requestName, elapsedMilliseconds, request);
        }

        return response;
    }
}

// Registration
services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
});
```

### Custom Mediator Implementation

```csharp
public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default);
    Task Publish<TNotification>(TNotification notification, CancellationToken ct = default)
        where TNotification : INotification;
}

public interface IRequest<TResponse> { }
public interface INotification { }

public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

public interface INotificationHandler<TNotification> where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request, 
        CancellationToken ct = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        
        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"Handler for {requestType.Name} not found");

        var method = handlerType.GetMethod("Handle");
        var result = await (Task<TResponse>)method!.Invoke(handler, new object[] { request, ct })!;
        
        return result;
    }

    public async Task Publish<TNotification>(
        TNotification notification, 
        CancellationToken ct = default) where TNotification : INotification
    {
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(typeof(TNotification));
        var handlers = _serviceProvider.GetServices(handlerType);

        var tasks = handlers.Select(handler =>
        {
            var method = handlerType.GetMethod("Handle");
            return (Task)method!.Invoke(handler, new object[] { notification, ct })!;
        });

        await Task.WhenAll(tasks);
    }
}
```

## Voordelen
- ? Reduces coupling tussen componenten
- ? Centraliseert communicatie logic
- ? Makkelijker te testen (mock mediator)
- ? Single Responsibility: handlers doen één ding

## Nadelen
- ? Mediator kan een "god object" worden
- ? Indirectie maakt debugging lastiger
- ? Kan overkill zijn voor simpele scenarios

## Mediator vs Observer

| Aspect | Mediator | Observer |
|--------|----------|----------|
| **Communicatie** | Via centraal punt | Direct subject->observer |
| **Coupling** | Colleagues kennen alleen mediator | Observers kennen subject |
| **Complexiteit** | Beter voor complexe interacties | Beter voor simpele notifications |

## Gerelateerde Patterns
- **Observer**: Alternatief voor publish/subscribe
- **Facade**: Beide vereenvoudigen, maar Facade is unidirectioneel
- **Command**: Requests kunnen als commands worden geïmplementeerd
