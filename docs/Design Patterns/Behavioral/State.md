# State Pattern

## Intentie
Laat een object zijn **gedrag wijzigen** wanneer zijn interne state verandert. Het object lijkt van class te veranderen.

## Wanneer gebruiken?
- Wanneer object gedrag afhangt van state
- Wanneer je grote conditional statements hebt op basis van state
- State machines implementeren
- Workflow engines

## Structuur

```
???????????????????       ???????????????????
?    Context      ???????>?     State       ?
???????????????????       ???????????????????
? -state          ?       ? +Handle()       ?
? +Request()      ?       ???????????????????
? +SetState()     ?               ?
???????????????????        ???????????????
                           ?      ?      ?
                    ??????????? ???????? ??????????
                    ?StateA   ? ?StateB? ? StateC ?
                    ??????????? ???????? ??????????
```

## Implementatie in C#

### Basis Implementatie: Order State Machine

```csharp
// State interface
public interface IOrderState
{
    void Confirm(Order order);
    void Ship(Order order);
    void Deliver(Order order);
    void Cancel(Order order);
    string Name { get; }
}

// Context
public class Order
{
    private IOrderState _state;

    public Guid Id { get; } = Guid.NewGuid();
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public string CurrentState => _state.Name;

    public Order(string customerName, decimal totalAmount)
    {
        CustomerName = customerName;
        TotalAmount = totalAmount;
        _state = new PendingState();
    }

    internal void TransitionTo(IOrderState newState)
    {
        Console.WriteLine($"Order {Id}: {_state.Name} -> {newState.Name}");
        _state = newState;
    }

    public void Confirm() => _state.Confirm(this);
    public void Ship() => _state.Ship(this);
    public void Deliver() => _state.Deliver(this);
    public void Cancel() => _state.Cancel(this);
}

// Concrete States
public class PendingState : IOrderState
{
    public string Name => "Pending";

    public void Confirm(Order order)
    {
        Console.WriteLine("Order confirmed! Processing payment...");
        order.TransitionTo(new ConfirmedState());
    }

    public void Ship(Order order)
    {
        Console.WriteLine("Cannot ship: Order must be confirmed first.");
    }

    public void Deliver(Order order)
    {
        Console.WriteLine("Cannot deliver: Order must be shipped first.");
    }

    public void Cancel(Order order)
    {
        Console.WriteLine("Order cancelled.");
        order.CancelledAt = DateTime.UtcNow;
        order.TransitionTo(new CancelledState());
    }
}

public class ConfirmedState : IOrderState
{
    public string Name => "Confirmed";

    public void Confirm(Order order)
    {
        Console.WriteLine("Order is already confirmed.");
    }

    public void Ship(Order order)
    {
        Console.WriteLine("Order shipped!");
        order.ShippedAt = DateTime.UtcNow;
        order.TransitionTo(new ShippedState());
    }

    public void Deliver(Order order)
    {
        Console.WriteLine("Cannot deliver: Order must be shipped first.");
    }

    public void Cancel(Order order)
    {
        Console.WriteLine("Order cancelled. Initiating refund...");
        order.CancelledAt = DateTime.UtcNow;
        order.TransitionTo(new CancelledState());
    }
}

public class ShippedState : IOrderState
{
    public string Name => "Shipped";

    public void Confirm(Order order)
    {
        Console.WriteLine("Order is already confirmed and shipped.");
    }

    public void Ship(Order order)
    {
        Console.WriteLine("Order is already shipped.");
    }

    public void Deliver(Order order)
    {
        Console.WriteLine("Order delivered successfully!");
        order.DeliveredAt = DateTime.UtcNow;
        order.TransitionTo(new DeliveredState());
    }

    public void Cancel(Order order)
    {
        Console.WriteLine("Cannot cancel: Order is already shipped. Please return after delivery.");
    }
}

public class DeliveredState : IOrderState
{
    public string Name => "Delivered";

    public void Confirm(Order order) => 
        Console.WriteLine("Order is already delivered.");

    public void Ship(Order order) => 
        Console.WriteLine("Order is already delivered.");

    public void Deliver(Order order) => 
        Console.WriteLine("Order is already delivered.");

    public void Cancel(Order order) => 
        Console.WriteLine("Cannot cancel delivered order. Please initiate a return.");
}

public class CancelledState : IOrderState
{
    public string Name => "Cancelled";

    public void Confirm(Order order) => 
        Console.WriteLine("Cannot confirm: Order is cancelled.");

    public void Ship(Order order) => 
        Console.WriteLine("Cannot ship: Order is cancelled.");

    public void Deliver(Order order) => 
        Console.WriteLine("Cannot deliver: Order is cancelled.");

    public void Cancel(Order order) => 
        Console.WriteLine("Order is already cancelled.");
}

// Gebruik
var order = new Order("John Doe", 99.99m);

order.Ship();     // Cannot ship: Order must be confirmed first.
order.Confirm();  // Order confirmed! Processing payment...
order.Cancel();   // Order cancelled. Initiating refund...
order.Confirm();  // Cannot confirm: Order is cancelled.

// Valid flow
var order2 = new Order("Jane Doe", 149.99m);
order2.Confirm(); // Pending -> Confirmed
order2.Ship();    // Confirmed -> Shipped
order2.Deliver(); // Shipped -> Delivered
```

### Document Workflow State Machine

```csharp
// State interface met async support
public interface IDocumentState
{
    Task SubmitAsync(Document doc);
    Task ApproveAsync(Document doc, string approver);
    Task RejectAsync(Document doc, string reason);
    Task PublishAsync(Document doc);
    Task ArchiveAsync(Document doc);
    string Name { get; }
    IEnumerable<string> AllowedActions { get; }
}

public class Document
{
    private IDocumentState _state;
    private readonly List<string> _history = new();

    public Guid Id { get; } = Guid.NewGuid();
    public string Title { get; set; }
    public string Content { get; set; }
    public string Author { get; set; }
    public string? ApprovedBy { get; set; }
    public string? RejectionReason { get; set; }
    public IReadOnlyList<string> History => _history.AsReadOnly();

    public string CurrentState => _state.Name;
    public IEnumerable<string> AllowedActions => _state.AllowedActions;

    public Document(string title, string content, string author)
    {
        Title = title;
        Content = content;
        Author = author;
        _state = new DraftState();
        AddHistory("Document created");
    }

    internal void TransitionTo(IDocumentState newState, string action)
    {
        AddHistory($"{action}: {_state.Name} -> {newState.Name}");
        _state = newState;
    }

    internal void AddHistory(string entry)
    {
        _history.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm}] {entry}");
    }

    public Task SubmitAsync() => _state.SubmitAsync(this);
    public Task ApproveAsync(string approver) => _state.ApproveAsync(this, approver);
    public Task RejectAsync(string reason) => _state.RejectAsync(this, reason);
    public Task PublishAsync() => _state.PublishAsync(this);
    public Task ArchiveAsync() => _state.ArchiveAsync(this);
}

// Concrete States
public class DraftState : IDocumentState
{
    public string Name => "Draft";
    public IEnumerable<string> AllowedActions => new[] { "Submit" };

    public Task SubmitAsync(Document doc)
    {
        doc.TransitionTo(new UnderReviewState(), "Submitted for review");
        return Task.CompletedTask;
    }

    public Task ApproveAsync(Document doc, string approver)
    {
        throw new InvalidOperationException("Cannot approve draft. Submit first.");
    }

    public Task RejectAsync(Document doc, string reason)
    {
        throw new InvalidOperationException("Cannot reject draft.");
    }

    public Task PublishAsync(Document doc)
    {
        throw new InvalidOperationException("Cannot publish draft. Get approval first.");
    }

    public Task ArchiveAsync(Document doc)
    {
        doc.TransitionTo(new ArchivedState(), "Archived without publishing");
        return Task.CompletedTask;
    }
}

public class UnderReviewState : IDocumentState
{
    public string Name => "Under Review";
    public IEnumerable<string> AllowedActions => new[] { "Approve", "Reject" };

    public Task SubmitAsync(Document doc)
    {
        throw new InvalidOperationException("Document is already submitted.");
    }

    public Task ApproveAsync(Document doc, string approver)
    {
        doc.ApprovedBy = approver;
        doc.AddHistory($"Approved by {approver}");
        doc.TransitionTo(new ApprovedState(), "Approved");
        return Task.CompletedTask;
    }

    public Task RejectAsync(Document doc, string reason)
    {
        doc.RejectionReason = reason;
        doc.AddHistory($"Rejected: {reason}");
        doc.TransitionTo(new DraftState(), "Rejected - back to draft");
        return Task.CompletedTask;
    }

    public Task PublishAsync(Document doc)
    {
        throw new InvalidOperationException("Cannot publish. Approval required.");
    }

    public Task ArchiveAsync(Document doc)
    {
        throw new InvalidOperationException("Cannot archive during review.");
    }
}

public class ApprovedState : IDocumentState
{
    public string Name => "Approved";
    public IEnumerable<string> AllowedActions => new[] { "Publish", "Archive" };

    public Task SubmitAsync(Document doc)
    {
        throw new InvalidOperationException("Document is already approved.");
    }

    public Task ApproveAsync(Document doc, string approver)
    {
        throw new InvalidOperationException("Document is already approved.");
    }

    public Task RejectAsync(Document doc, string reason)
    {
        doc.RejectionReason = reason;
        doc.ApprovedBy = null;
        doc.TransitionTo(new DraftState(), "Revoked - back to draft");
        return Task.CompletedTask;
    }

    public Task PublishAsync(Document doc)
    {
        doc.TransitionTo(new PublishedState(), "Published");
        return Task.CompletedTask;
    }

    public Task ArchiveAsync(Document doc)
    {
        doc.TransitionTo(new ArchivedState(), "Archived without publishing");
        return Task.CompletedTask;
    }
}

public class PublishedState : IDocumentState
{
    public string Name => "Published";
    public IEnumerable<string> AllowedActions => new[] { "Archive" };

    public Task SubmitAsync(Document doc) => 
        throw new InvalidOperationException("Cannot submit published document.");

    public Task ApproveAsync(Document doc, string approver) => 
        throw new InvalidOperationException("Already published.");

    public Task RejectAsync(Document doc, string reason) => 
        throw new InvalidOperationException("Cannot reject published document.");

    public Task PublishAsync(Document doc) => 
        throw new InvalidOperationException("Already published.");

    public Task ArchiveAsync(Document doc)
    {
        doc.TransitionTo(new ArchivedState(), "Archived");
        return Task.CompletedTask;
    }
}

public class ArchivedState : IDocumentState
{
    public string Name => "Archived";
    public IEnumerable<string> AllowedActions => Array.Empty<string>();

    public Task SubmitAsync(Document doc) => 
        throw new InvalidOperationException("Document is archived.");

    public Task ApproveAsync(Document doc, string approver) => 
        throw new InvalidOperationException("Document is archived.");

    public Task RejectAsync(Document doc, string reason) => 
        throw new InvalidOperationException("Document is archived.");

    public Task PublishAsync(Document doc) => 
        throw new InvalidOperationException("Document is archived.");

    public Task ArchiveAsync(Document doc) => 
        throw new InvalidOperationException("Document is already archived.");
}

// Gebruik
var doc = new Document("Annual Report", "Content here...", "John");

Console.WriteLine($"State: {doc.CurrentState}");
Console.WriteLine($"Allowed: {string.Join(", ", doc.AllowedActions)}");

await doc.SubmitAsync();
await doc.ApproveAsync("Manager");
await doc.PublishAsync();

Console.WriteLine("\n--- History ---");
foreach (var entry in doc.History)
{
    Console.WriteLine(entry);
}
```

### State Pattern met Enum (Simplified)

```csharp
public enum TrafficLightState
{
    Red,
    Yellow,
    Green
}

public class TrafficLight
{
    public TrafficLightState State { get; private set; } = TrafficLightState.Red;

    public void Next()
    {
        State = State switch
        {
            TrafficLightState.Red => TrafficLightState.Green,
            TrafficLightState.Green => TrafficLightState.Yellow,
            TrafficLightState.Yellow => TrafficLightState.Red,
            _ => throw new InvalidOperationException()
        };
    }

    public string GetInstruction() => State switch
    {
        TrafficLightState.Red => "STOP",
        TrafficLightState.Yellow => "CAUTION",
        TrafficLightState.Green => "GO",
        _ => "UNKNOWN"
    };

    public TimeSpan GetDuration() => State switch
    {
        TrafficLightState.Red => TimeSpan.FromSeconds(30),
        TrafficLightState.Yellow => TimeSpan.FromSeconds(5),
        TrafficLightState.Green => TimeSpan.FromSeconds(25),
        _ => TimeSpan.Zero
    };
}
```

## Voordelen
- ? Organiseert state-specific gedrag in aparte classes
- ? Elimineert grote conditional statements
- ? Maakt state transitions expliciet
- ? Open/Closed: nieuwe states toevoegen is makkelijk

## Nadelen
- ? Kan overkill zijn voor simpele state machines
- ? Verhoogt aantal classes
- ? States moeten context kennen voor transitions

## State vs Strategy

| Aspect | State | Strategy |
|--------|-------|----------|
| **Doel** | Gedrag op basis van interne state | Algoritme selectie |
| **Transitions** | States kunnen zichzelf vervangen | Client selecteert strategy |
| **Awareness** | States kennen andere states | Strategies kennen elkaar niet |

## Gerelateerde Patterns
- **Strategy**: Vergelijkbare structuur
- **Singleton**: States kunnen Singletons zijn (stateless)
- **Flyweight**: State objects kunnen gedeeld worden
