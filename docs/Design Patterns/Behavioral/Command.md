# Command Pattern

## Intentie
Encapsuleert een **verzoek als een object**, waardoor je clients kunt parametriseren met verschillende verzoeken, verzoeken in een queue kunt plaatsen, loggen, en **ongedaan maken** ondersteunt.

## Wanneer gebruiken?
- Undo/Redo functionaliteit
- Queue of schedule operaties
- Transactional operations
- Logging van operaties
- Macro recording
- Remote procedure calls

## Structuur

```
???????????????      ???????????????????
?   Invoker   ??????>?    Command      ?
???????????????      ???????????????????
                     ? +Execute()      ?
                     ? +Undo()         ?
                     ???????????????????
                             ?
                     ?????????????????
                     ?               ?
              ??????????????? ???????????????
              ?ConcreteCmd1 ? ?ConcreteCmd2 ?
              ??????????????? ???????????????
              ? -receiver   ? ? -receiver   ?
              ??????????????? ???????????????
                     ?               ?
                     ?               ?
              ???????????????????????????
              ?       Receiver          ?
              ???????????????????????????
```

## Implementatie in C#

### Basis Implementatie met Undo

```csharp
// Command interface
public interface ICommand
{
    void Execute();
    void Undo();
    string Description { get; }
}

// Receiver
public class TextDocument
{
    private readonly StringBuilder _content = new();

    public string Content => _content.ToString();

    public void InsertText(int position, string text)
    {
        _content.Insert(position, text);
        Console.WriteLine($"Inserted '{text}' at position {position}");
    }

    public void DeleteText(int position, int length)
    {
        var deleted = _content.ToString(position, length);
        _content.Remove(position, length);
        Console.WriteLine($"Deleted '{deleted}' from position {position}");
    }

    public void ReplaceText(int position, int length, string newText)
    {
        _content.Remove(position, length);
        _content.Insert(position, newText);
    }
}

// Concrete Commands
public class InsertTextCommand : ICommand
{
    private readonly TextDocument _document;
    private readonly int _position;
    private readonly string _text;

    public InsertTextCommand(TextDocument document, int position, string text)
    {
        _document = document;
        _position = position;
        _text = text;
    }

    public string Description => $"Insert '{_text}' at {_position}";

    public void Execute()
    {
        _document.InsertText(_position, _text);
    }

    public void Undo()
    {
        _document.DeleteText(_position, _text.Length);
    }
}

public class DeleteTextCommand : ICommand
{
    private readonly TextDocument _document;
    private readonly int _position;
    private readonly int _length;
    private string _deletedText = string.Empty;

    public DeleteTextCommand(TextDocument document, int position, int length)
    {
        _document = document;
        _position = position;
        _length = length;
    }

    public string Description => $"Delete {_length} chars from {_position}";

    public void Execute()
    {
        // Save deleted text for undo
        _deletedText = _document.Content.Substring(_position, _length);
        _document.DeleteText(_position, _length);
    }

    public void Undo()
    {
        _document.InsertText(_position, _deletedText);
    }
}

// Invoker with Undo/Redo support
public class TextEditor
{
    private readonly TextDocument _document = new();
    private readonly Stack<ICommand> _undoStack = new();
    private readonly Stack<ICommand> _redoStack = new();

    public string Content => _document.Content;

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear(); // Clear redo stack after new command
    }

    public void Undo()
    {
        if (_undoStack.Count > 0)
        {
            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);
            Console.WriteLine($"Undone: {command.Description}");
        }
        else
        {
            Console.WriteLine("Nothing to undo");
        }
    }

    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            var command = _redoStack.Pop();
            command.Execute();
            _undoStack.Push(command);
            Console.WriteLine($"Redone: {command.Description}");
        }
        else
        {
            Console.WriteLine("Nothing to redo");
        }
    }

    public void ShowHistory()
    {
        Console.WriteLine("\n--- Command History ---");
        foreach (var cmd in _undoStack.Reverse())
        {
            Console.WriteLine($"  • {cmd.Description}");
        }
    }
}

// Gebruik
var editor = new TextEditor();

editor.ExecuteCommand(new InsertTextCommand(editor._document, 0, "Hello"));
editor.ExecuteCommand(new InsertTextCommand(editor._document, 5, " World"));
editor.ExecuteCommand(new InsertTextCommand(editor._document, 11, "!"));

Console.WriteLine($"Content: {editor.Content}"); // "Hello World!"

editor.Undo(); // Remove "!"
Console.WriteLine($"Content: {editor.Content}"); // "Hello World"

editor.Undo(); // Remove " World"
Console.WriteLine($"Content: {editor.Content}"); // "Hello"

editor.Redo(); // Add " World" back
Console.WriteLine($"Content: {editor.Content}"); // "Hello World"
```

### Macro Commands (Composite Command)

```csharp
public class MacroCommand : ICommand
{
    private readonly List<ICommand> _commands = new();
    private readonly string _name;

    public MacroCommand(string name)
    {
        _name = name;
    }

    public string Description => $"Macro: {_name} ({_commands.Count} commands)";

    public void Add(ICommand command)
    {
        _commands.Add(command);
    }

    public void Execute()
    {
        Console.WriteLine($"Executing macro: {_name}");
        foreach (var command in _commands)
        {
            command.Execute();
        }
    }

    public void Undo()
    {
        Console.WriteLine($"Undoing macro: {_name}");
        foreach (var command in Enumerable.Reverse(_commands))
        {
            command.Undo();
        }
    }
}

// Gebruik
var document = new TextDocument();

var headerMacro = new MacroCommand("Add Header");
headerMacro.Add(new InsertTextCommand(document, 0, "=================\n"));
headerMacro.Add(new InsertTextCommand(document, 0, "   MY DOCUMENT   \n"));
headerMacro.Add(new InsertTextCommand(document, 0, "=================\n"));

headerMacro.Execute();
Console.WriteLine(document.Content);

headerMacro.Undo();
Console.WriteLine(document.Content);
```

### Praktisch Voorbeeld: Drawing Application

```csharp
public interface IDrawCommand
{
    void Execute();
    void Undo();
}

public class Canvas
{
    private readonly List<Shape> _shapes = new();

    public IReadOnlyList<Shape> Shapes => _shapes.AsReadOnly();

    public void AddShape(Shape shape)
    {
        _shapes.Add(shape);
        Console.WriteLine($"Added {shape}");
    }

    public void RemoveShape(Shape shape)
    {
        _shapes.Remove(shape);
        Console.WriteLine($"Removed {shape}");
    }

    public void MoveShape(Shape shape, int deltaX, int deltaY)
    {
        shape.X += deltaX;
        shape.Y += deltaY;
        Console.WriteLine($"Moved {shape} by ({deltaX}, {deltaY})");
    }

    public void ResizeShape(Shape shape, int newWidth, int newHeight)
    {
        shape.Width = newWidth;
        shape.Height = newHeight;
        Console.WriteLine($"Resized {shape}");
    }
}

public class Shape
{
    public string Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Color { get; set; }

    public override string ToString() => $"{Color} {Type} at ({X},{Y}) {Width}x{Height}";
}

public class AddShapeCommand : IDrawCommand
{
    private readonly Canvas _canvas;
    private readonly Shape _shape;

    public AddShapeCommand(Canvas canvas, Shape shape)
    {
        _canvas = canvas;
        _shape = shape;
    }

    public void Execute() => _canvas.AddShape(_shape);
    public void Undo() => _canvas.RemoveShape(_shape);
}

public class MoveShapeCommand : IDrawCommand
{
    private readonly Canvas _canvas;
    private readonly Shape _shape;
    private readonly int _deltaX;
    private readonly int _deltaY;

    public MoveShapeCommand(Canvas canvas, Shape shape, int deltaX, int deltaY)
    {
        _canvas = canvas;
        _shape = shape;
        _deltaX = deltaX;
        _deltaY = deltaY;
    }

    public void Execute() => _canvas.MoveShape(_shape, _deltaX, _deltaY);
    public void Undo() => _canvas.MoveShape(_shape, -_deltaX, -_deltaY);
}

public class ResizeShapeCommand : IDrawCommand
{
    private readonly Canvas _canvas;
    private readonly Shape _shape;
    private readonly int _newWidth;
    private readonly int _newHeight;
    private int _previousWidth;
    private int _previousHeight;

    public ResizeShapeCommand(Canvas canvas, Shape shape, int newWidth, int newHeight)
    {
        _canvas = canvas;
        _shape = shape;
        _newWidth = newWidth;
        _newHeight = newHeight;
    }

    public void Execute()
    {
        _previousWidth = _shape.Width;
        _previousHeight = _shape.Height;
        _canvas.ResizeShape(_shape, _newWidth, _newHeight);
    }

    public void Undo()
    {
        _canvas.ResizeShape(_shape, _previousWidth, _previousHeight);
    }
}

public class DrawingApplication
{
    private readonly Canvas _canvas = new();
    private readonly Stack<IDrawCommand> _history = new();
    private readonly Stack<IDrawCommand> _redoStack = new();

    public Canvas Canvas => _canvas;

    public void Execute(IDrawCommand command)
    {
        command.Execute();
        _history.Push(command);
        _redoStack.Clear();
    }

    public void Undo()
    {
        if (_history.TryPop(out var command))
        {
            command.Undo();
            _redoStack.Push(command);
        }
    }

    public void Redo()
    {
        if (_redoStack.TryPop(out var command))
        {
            command.Execute();
            _history.Push(command);
        }
    }
}

// Gebruik
var app = new DrawingApplication();

var rect = new Shape { Type = "Rectangle", X = 10, Y = 10, Width = 100, Height = 50, Color = "Blue" };
var circle = new Shape { Type = "Circle", X = 200, Y = 100, Width = 80, Height = 80, Color = "Red" };

app.Execute(new AddShapeCommand(app.Canvas, rect));
app.Execute(new AddShapeCommand(app.Canvas, circle));
app.Execute(new MoveShapeCommand(app.Canvas, rect, 50, 30));
app.Execute(new ResizeShapeCommand(app.Canvas, circle, 120, 120));

Console.WriteLine("\n--- Shapes on canvas ---");
foreach (var shape in app.Canvas.Shapes)
{
    Console.WriteLine($"  {shape}");
}

app.Undo(); // Undo resize
app.Undo(); // Undo move
app.Redo(); // Redo move
```

### Command Queue / Scheduler

```csharp
public interface IAsyncCommand
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
    string Name { get; }
}

public class CommandQueue
{
    private readonly Queue<IAsyncCommand> _queue = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _isProcessing;

    public void Enqueue(IAsyncCommand command)
    {
        _queue.Enqueue(command);
        Console.WriteLine($"Queued: {command.Name}");
    }

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            _isProcessing = true;
            while (_queue.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                var command = _queue.Dequeue();
                Console.WriteLine($"Executing: {command.Name}");
                await command.ExecuteAsync(cancellationToken);
                Console.WriteLine($"Completed: {command.Name}");
            }
        }
        finally
        {
            _isProcessing = false;
            _semaphore.Release();
        }
    }
}

public class EmailCommand : IAsyncCommand
{
    private readonly string _to;
    private readonly string _subject;

    public EmailCommand(string to, string subject)
    {
        _to = to;
        _subject = subject;
    }

    public string Name => $"Send email to {_to}";

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(1000, cancellationToken); // Simulate sending
        Console.WriteLine($"  ? Email sent to {_to}: {_subject}");
    }
}

public class BackupCommand : IAsyncCommand
{
    private readonly string _database;

    public BackupCommand(string database)
    {
        _database = database;
    }

    public string Name => $"Backup {_database}";

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(2000, cancellationToken); // Simulate backup
        Console.WriteLine($"  ? Database {_database} backed up");
    }
}

// Gebruik
var queue = new CommandQueue();

queue.Enqueue(new EmailCommand("admin@example.com", "Daily Report"));
queue.Enqueue(new BackupCommand("ProductionDB"));
queue.Enqueue(new EmailCommand("team@example.com", "Backup Complete"));

await queue.ProcessAsync();
```

### Met Delegates (Simplified)

```csharp
public class SimpleCommand
{
    private readonly Action _execute;
    private readonly Action? _undo;

    public SimpleCommand(Action execute, Action? undo = null)
    {
        _execute = execute;
        _undo = undo;
    }

    public void Execute() => _execute();
    public void Undo() => _undo?.Invoke();
}

// Gebruik
var document = new TextDocument();
int pos = 0;

var commands = new List<SimpleCommand>
{
    new SimpleCommand(
        execute: () => { document.InsertText(pos, "Hello"); pos += 5; },
        undo: () => { pos -= 5; document.DeleteText(pos, 5); }
    ),
    new SimpleCommand(
        execute: () => { document.InsertText(pos, " World"); },
        undo: () => { document.DeleteText(pos, 6); }
    )
};

foreach (var cmd in commands)
{
    cmd.Execute();
}

Console.WriteLine(document.Content); // "Hello World"

foreach (var cmd in Enumerable.Reverse(commands))
{
    cmd.Undo();
}

Console.WriteLine(document.Content); // ""
```

## Voordelen
- ? Decoupling tussen invoker en receiver
- ? Easy undo/redo implementation
- ? Commands kunnen gequeued, gelogd, getimed worden
- ? Macro's / composite commands
- ? Transactional behavior

## Nadelen
- ? Verhoogt aantal classes
- ? Complexer voor simple operaties

## Command vs Strategy

| Aspect | Command | Strategy |
|--------|---------|----------|
| **Doel** | Encapsuleert een actie | Encapsuleert een algoritme |
| **Wanneer** | Deferred/queued execution | Immediate execution |
| **State** | Bevat data voor undo | Stateless |
| **Voorbeeld** | Undo, macro, queue | Sort algorithm, payment method |

## Gerelateerde Patterns
- **Memento**: Kan samen met Command voor undo
- **Composite**: Macro commands
- **Prototype**: Command cloning
- **Chain of Responsibility**: Commands kunnen door chain gaan
