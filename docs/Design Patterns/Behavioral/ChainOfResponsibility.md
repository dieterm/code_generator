# Chain of Responsibility Pattern

## Intentie
Vermijdt koppeling van de zender van een verzoek aan zijn ontvanger door **meerdere objecten de kans te geven** het verzoek af te handelen. Keten de ontvangende objecten en geef het verzoek door totdat een object het afhandelt.

## Wanneer gebruiken?
- Wanneer meerdere objecten een verzoek kunnen afhandelen
- Wanneer je de handler niet van tevoren weet
- Wanneer de set handlers dynamisch moet zijn
- Middleware pipelines, event handling, approval workflows

## Structuur

```
?????????????????????
?     Handler       ?
?????????????????????
? -successor        ?????????
? +HandleRequest()  ?       ?
?????????????????????       ?
        ?                   ?
   ??????????????????       ?
   ?         ?      ?       ?
???????? ???????? ????????  ?
? H1   ? ? H2   ? ? H3   ????
???????? ???????? ????????
```

## Implementatie in C#

### Basis Implementatie: Support Tickets

```csharp
// Request
public class SupportTicket
{
    public string Id { get; } = Guid.NewGuid().ToString()[..8];
    public string Description { get; set; }
    public TicketPriority Priority { get; set; }
    public TicketCategory Category { get; set; }
    public string? Response { get; set; }
    public bool IsHandled { get; set; }
}

public enum TicketPriority { Low, Medium, High, Critical }
public enum TicketCategory { General, Technical, Billing, Security }

// Abstract Handler
public abstract class SupportHandler
{
    protected SupportHandler? Successor;

    public void SetSuccessor(SupportHandler successor)
    {
        Successor = successor;
    }

    public abstract void HandleTicket(SupportTicket ticket);

    protected void PassToSuccessor(SupportTicket ticket)
    {
        if (Successor != null)
        {
            Successor.HandleTicket(ticket);
        }
        else
        {
            Console.WriteLine($"Ticket {ticket.Id}: No handler available, escalating to manager.");
            ticket.Response = "Escalated to management";
        }
    }
}

// Concrete Handlers
public class FrontDeskSupport : SupportHandler
{
    public override void HandleTicket(SupportTicket ticket)
    {
        if (ticket.Priority == TicketPriority.Low && ticket.Category == TicketCategory.General)
        {
            Console.WriteLine($"[Front Desk] Handling ticket {ticket.Id}: {ticket.Description}");
            ticket.Response = "Handled by Front Desk";
            ticket.IsHandled = true;
        }
        else
        {
            Console.WriteLine($"[Front Desk] Escalating ticket {ticket.Id}");
            PassToSuccessor(ticket);
        }
    }
}

public class TechnicalSupport : SupportHandler
{
    public override void HandleTicket(SupportTicket ticket)
    {
        if (ticket.Category == TicketCategory.Technical && 
            ticket.Priority <= TicketPriority.Medium)
        {
            Console.WriteLine($"[Technical Support] Handling ticket {ticket.Id}: {ticket.Description}");
            ticket.Response = "Handled by Technical Support";
            ticket.IsHandled = true;
        }
        else
        {
            Console.WriteLine($"[Technical Support] Escalating ticket {ticket.Id}");
            PassToSuccessor(ticket);
        }
    }
}

public class SeniorEngineer : SupportHandler
{
    public override void HandleTicket(SupportTicket ticket)
    {
        if (ticket.Category == TicketCategory.Technical || 
            ticket.Category == TicketCategory.Security)
        {
            Console.WriteLine($"[Senior Engineer] Handling ticket {ticket.Id}: {ticket.Description}");
            ticket.Response = "Handled by Senior Engineer";
            ticket.IsHandled = true;
        }
        else
        {
            Console.WriteLine($"[Senior Engineer] Escalating ticket {ticket.Id}");
            PassToSuccessor(ticket);
        }
    }
}

public class Manager : SupportHandler
{
    public override void HandleTicket(SupportTicket ticket)
    {
        Console.WriteLine($"[Manager] Handling ticket {ticket.Id}: {ticket.Description}");
        ticket.Response = $"Handled by Manager (Priority: {ticket.Priority})";
        ticket.IsHandled = true;
    }
}

// Gebruik
var frontDesk = new FrontDeskSupport();
var techSupport = new TechnicalSupport();
var seniorEngineer = new SeniorEngineer();
var manager = new Manager();

// Build the chain
frontDesk.SetSuccessor(techSupport);
techSupport.SetSuccessor(seniorEngineer);
seniorEngineer.SetSuccessor(manager);

// Test tickets
var tickets = new[]
{
    new SupportTicket { Description = "How do I reset my password?", Priority = TicketPriority.Low, Category = TicketCategory.General },
    new SupportTicket { Description = "Application crashes on startup", Priority = TicketPriority.Medium, Category = TicketCategory.Technical },
    new SupportTicket { Description = "Security breach detected", Priority = TicketPriority.Critical, Category = TicketCategory.Security },
    new SupportTicket { Description = "Invoice incorrect", Priority = TicketPriority.High, Category = TicketCategory.Billing }
};

foreach (var ticket in tickets)
{
    Console.WriteLine($"\n--- Processing Ticket: {ticket.Description} ---");
    frontDesk.HandleTicket(ticket);
    Console.WriteLine($"Result: {ticket.Response}");
}
```

### Middleware Pattern (ASP.NET Core Style)

```csharp
public delegate Task RequestDelegate(HttpContext context);

public interface IMiddleware
{
    Task InvokeAsync(HttpContext context, RequestDelegate next);
}

public class HttpContext
{
    public Dictionary<string, string> Request { get; } = new();
    public Dictionary<string, string> Response { get; } = new();
    public Dictionary<string, object> Items { get; } = new();
    public bool IsAuthenticated { get; set; }
    public string? User { get; set; }
}

// Middleware implementations
public class LoggingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Console.WriteLine($"[Logging] Request started: {context.Request.GetValueOrDefault("path", "/")}");
        var stopwatch = Stopwatch.StartNew();
        
        await next(context);
        
        stopwatch.Stop();
        Console.WriteLine($"[Logging] Request completed in {stopwatch.ElapsedMilliseconds}ms");
    }
}

public class AuthenticationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var authHeader = context.Request.GetValueOrDefault("Authorization", "");
        
        if (authHeader.StartsWith("Bearer "))
        {
            context.IsAuthenticated = true;
            context.User = "AuthenticatedUser";
            Console.WriteLine("[Auth] User authenticated");
        }
        else
        {
            Console.WriteLine("[Auth] No authentication token found");
        }
        
        await next(context);
    }
}

public class AuthorizationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var path = context.Request.GetValueOrDefault("path", "/");
        
        if (path.StartsWith("/admin") && !context.IsAuthenticated)
        {
            Console.WriteLine("[Authorization] Access denied - not authenticated");
            context.Response["status"] = "401";
            context.Response["body"] = "Unauthorized";
            return; // Short-circuit the pipeline
        }
        
        Console.WriteLine("[Authorization] Access granted");
        await next(context);
    }
}

public class ExceptionHandlerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Exception] Caught: {ex.Message}");
            context.Response["status"] = "500";
            context.Response["body"] = "Internal Server Error";
        }
    }
}

// Pipeline builder
public class PipelineBuilder
{
    private readonly List<IMiddleware> _middlewares = new();

    public PipelineBuilder Use(IMiddleware middleware)
    {
        _middlewares.Add(middleware);
        return this;
    }

    public RequestDelegate Build(RequestDelegate finalHandler)
    {
        var pipeline = finalHandler;

        // Build pipeline in reverse order
        for (int i = _middlewares.Count - 1; i >= 0; i--)
        {
            var middleware = _middlewares[i];
            var next = pipeline;
            pipeline = context => middleware.InvokeAsync(context, next);
        }

        return pipeline;
    }
}

// Gebruik
var builder = new PipelineBuilder();

builder
    .Use(new ExceptionHandlerMiddleware())
    .Use(new LoggingMiddleware())
    .Use(new AuthenticationMiddleware())
    .Use(new AuthorizationMiddleware());

var pipeline = builder.Build(async context =>
{
    Console.WriteLine("[Handler] Processing request");
    context.Response["status"] = "200";
    context.Response["body"] = "Hello, World!";
});

// Test requests
var context1 = new HttpContext();
context1.Request["path"] = "/api/data";
context1.Request["Authorization"] = "Bearer token123";
await pipeline(context1);
Console.WriteLine($"Response: {context1.Response["status"]} - {context1.Response["body"]}");

var context2 = new HttpContext();
context2.Request["path"] = "/admin/settings";
await pipeline(context2); // Will be rejected
Console.WriteLine($"Response: {context2.Response["status"]} - {context2.Response["body"]}");
```

### Validation Chain

```csharp
public abstract class Validator<T>
{
    protected Validator<T>? Next;

    public Validator<T> SetNext(Validator<T> next)
    {
        Next = next;
        return next;
    }

    public ValidationResult Validate(T input)
    {
        var result = DoValidate(input);
        
        if (!result.IsValid)
            return result;

        if (Next != null)
            return Next.Validate(input);

        return ValidationResult.Success();
    }

    protected abstract ValidationResult DoValidate(T input);
}

public class ValidationResult
{
    public bool IsValid { get; }
    public string? Error { get; }

    private ValidationResult(bool isValid, string? error = null)
    {
        IsValid = isValid;
        Error = error;
    }

    public static ValidationResult Success() => new(true);
    public static ValidationResult Fail(string error) => new(false, error);
}

// Concrete validators
public class EmailValidator : Validator<UserRegistration>
{
    protected override ValidationResult DoValidate(UserRegistration input)
    {
        if (string.IsNullOrWhiteSpace(input.Email))
            return ValidationResult.Fail("Email is required");
        
        if (!input.Email.Contains("@"))
            return ValidationResult.Fail("Email format is invalid");

        return ValidationResult.Success();
    }
}

public class PasswordValidator : Validator<UserRegistration>
{
    protected override ValidationResult DoValidate(UserRegistration input)
    {
        if (string.IsNullOrWhiteSpace(input.Password))
            return ValidationResult.Fail("Password is required");
        
        if (input.Password.Length < 8)
            return ValidationResult.Fail("Password must be at least 8 characters");
        
        if (!input.Password.Any(char.IsDigit))
            return ValidationResult.Fail("Password must contain at least one digit");

        return ValidationResult.Success();
    }
}

public class AgeValidator : Validator<UserRegistration>
{
    protected override ValidationResult DoValidate(UserRegistration input)
    {
        if (input.Age < 18)
            return ValidationResult.Fail("Must be at least 18 years old");

        return ValidationResult.Success();
    }
}

public class UserRegistration
{
    public string Email { get; set; }
    public string Password { get; set; }
    public int Age { get; set; }
}

// Gebruik
var emailValidator = new EmailValidator();
var passwordValidator = new PasswordValidator();
var ageValidator = new AgeValidator();

emailValidator
    .SetNext(passwordValidator)
    .SetNext(ageValidator);

var registration = new UserRegistration
{
    Email = "test@example.com",
    Password = "short",
    Age = 25
};

var result = emailValidator.Validate(registration);
Console.WriteLine(result.IsValid ? "Valid!" : $"Error: {result.Error}");
```

## Voordelen
- ? Loose coupling tussen sender en receivers
- ? Dynamisch handlers toevoegen/verwijderen
- ? Single Responsibility: elke handler doet één ding
- ? Open/Closed: nieuwe handlers zonder bestaande code te wijzigen

## Nadelen
- ? Geen garantie dat verzoek wordt afgehandeld
- ? Kan moeilijk te debuggen zijn
- ? Performance impact bij lange ketens

## Gerelateerde Patterns
- **Command**: Request als object
- **Composite**: Chain kan composite structuur zijn
- **Decorator**: Vergelijkbare structuur, ander doel
