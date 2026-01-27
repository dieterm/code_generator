# Memento Pattern

## Intentie
Capture en externaliseer de **interne state van een object** zonder encapsulation te schenden, zodat het object later naar deze state kan worden **hersteld**.

## Wanneer gebruiken?
- Undo/Redo functionaliteit
- Snapshots van object state
- Transactional rollback
- Game save/load states

## Structuur

```
???????????????????     ???????????????????
?   Originator    ?     ?    Memento      ?
???????????????????     ???????????????????
? -state          ?????>? -state          ?
? +CreateMemento()?     ? +GetState()     ?
? +Restore()      ?     ???????????????????
???????????????????             ?
                                ?
                    ?????????????????????????
                    ?     Caretaker         ?
                    ?????????????????????????
                    ? -mementos: Stack      ?
                    ? +Save()               ?
                    ? +Undo()               ?
                    ?????????????????????????
```

## Implementatie in C#

### Basis Implementatie: Text Editor

```csharp
// Memento - stores state
public class EditorMemento
{
    public string Content { get; }
    public int CursorPosition { get; }
    public DateTime CreatedAt { get; }

    public EditorMemento(string content, int cursorPosition)
    {
        Content = content;
        CursorPosition = cursorPosition;
        CreatedAt = DateTime.Now;
    }
}

// Originator - the object whose state we save
public class TextEditor
{
    public string Content { get; private set; } = string.Empty;
    public int CursorPosition { get; private set; }

    public void Type(string text)
    {
        Content = Content.Insert(CursorPosition, text);
        CursorPosition += text.Length;
    }

    public void Delete(int count)
    {
        if (CursorPosition >= count)
        {
            Content = Content.Remove(CursorPosition - count, count);
            CursorPosition -= count;
        }
    }

    public void MoveCursor(int position)
    {
        CursorPosition = Math.Clamp(position, 0, Content.Length);
    }

    // Create memento
    public EditorMemento Save()
    {
        return new EditorMemento(Content, CursorPosition);
    }

    // Restore from memento
    public void Restore(EditorMemento memento)
    {
        Content = memento.Content;
        CursorPosition = memento.CursorPosition;
    }

    public override string ToString()
    {
        var marker = new string(' ', CursorPosition) + "|";
        return $"Content: \"{Content}\"\nCursor:   {marker}";
    }
}

// Caretaker - manages mementos
public class EditorHistory
{
    private readonly Stack<EditorMemento> _undoStack = new();
    private readonly Stack<EditorMemento> _redoStack = new();
    private readonly TextEditor _editor;

    public EditorHistory(TextEditor editor)
    {
        _editor = editor;
    }

    public void Save()
    {
        _undoStack.Push(_editor.Save());
        _redoStack.Clear(); // Clear redo after new change
    }

    public void Undo()
    {
        if (_undoStack.Count > 0)
        {
            _redoStack.Push(_editor.Save()); // Save current for redo
            var memento = _undoStack.Pop();
            _editor.Restore(memento);
            Console.WriteLine($"Undo to state from {memento.CreatedAt:HH:mm:ss}");
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
            _undoStack.Push(_editor.Save()); // Save current for undo
            var memento = _redoStack.Pop();
            _editor.Restore(memento);
            Console.WriteLine($"Redo to state from {memento.CreatedAt:HH:mm:ss}");
        }
        else
        {
            Console.WriteLine("Nothing to redo");
        }
    }

    public void ShowHistory()
    {
        Console.WriteLine($"Undo stack: {_undoStack.Count} states");
        Console.WriteLine($"Redo stack: {_redoStack.Count} states");
    }
}

// Gebruik
var editor = new TextEditor();
var history = new EditorHistory(editor);

history.Save(); // Initial state
editor.Type("Hello");
Console.WriteLine(editor);

history.Save();
editor.Type(" World");
Console.WriteLine(editor);

history.Save();
editor.Type("!");
Console.WriteLine(editor);

Console.WriteLine("\n--- Undo ---");
history.Undo();
Console.WriteLine(editor);

Console.WriteLine("\n--- Undo ---");
history.Undo();
Console.WriteLine(editor);

Console.WriteLine("\n--- Redo ---");
history.Redo();
Console.WriteLine(editor);
```

### Game State Memento

```csharp
// Memento
public record GameMemento
{
    public int Level { get; init; }
    public int Health { get; init; }
    public int Score { get; init; }
    public (int X, int Y) Position { get; init; }
    public List<string> Inventory { get; init; }
    public DateTime SavedAt { get; init; }
}

// Originator
public class Game
{
    public int Level { get; private set; } = 1;
    public int Health { get; private set; } = 100;
    public int Score { get; private set; } = 0;
    public (int X, int Y) Position { get; private set; } = (0, 0);
    public List<string> Inventory { get; } = new();

    public void Play()
    {
        // Simulate gameplay
        Score += Random.Shared.Next(10, 100);
        Health -= Random.Shared.Next(0, 20);
        Position = (Position.X + Random.Shared.Next(-5, 5), Position.Y + Random.Shared.Next(-5, 5));
        
        if (Score > Level * 100)
            Level++;
    }

    public void PickupItem(string item)
    {
        Inventory.Add(item);
    }

    public void TakeDamage(int damage)
    {
        Health = Math.Max(0, Health - damage);
    }

    public GameMemento Save()
    {
        Console.WriteLine($"Saving game: Level {Level}, Score {Score}, Health {Health}");
        return new GameMemento
        {
            Level = Level,
            Health = Health,
            Score = Score,
            Position = Position,
            Inventory = new List<string>(Inventory),
            SavedAt = DateTime.Now
        };
    }

    public void Load(GameMemento memento)
    {
        Level = memento.Level;
        Health = memento.Health;
        Score = memento.Score;
        Position = memento.Position;
        Inventory.Clear();
        Inventory.AddRange(memento.Inventory);
        Console.WriteLine($"Loaded save from {memento.SavedAt:HH:mm:ss}");
    }

    public void ShowStatus()
    {
        Console.WriteLine($"Level: {Level} | Health: {Health} | Score: {Score} | Position: {Position}");
        Console.WriteLine($"Inventory: [{string.Join(", ", Inventory)}]");
    }
}

// Caretaker with multiple save slots
public class SaveManager
{
    private readonly Dictionary<string, GameMemento> _saves = new();
    private readonly Stack<GameMemento> _autosaves = new();
    private const int MaxAutosaves = 3;

    public void SaveToSlot(Game game, string slotName)
    {
        _saves[slotName] = game.Save();
        Console.WriteLine($"Saved to slot: {slotName}");
    }

    public void LoadFromSlot(Game game, string slotName)
    {
        if (_saves.TryGetValue(slotName, out var memento))
        {
            game.Load(memento);
        }
        else
        {
            Console.WriteLine($"No save found in slot: {slotName}");
        }
    }

    public void Autosave(Game game)
    {
        if (_autosaves.Count >= MaxAutosaves)
        {
            // Remove oldest autosave
            var temp = new Stack<GameMemento>();
            while (_autosaves.Count > 1)
                temp.Push(_autosaves.Pop());
            _autosaves.Clear();
            while (temp.Count > 0)
                _autosaves.Push(temp.Pop());
        }
        _autosaves.Push(game.Save());
        Console.WriteLine($"Autosaved ({_autosaves.Count}/{MaxAutosaves})");
    }

    public void LoadLastAutosave(Game game)
    {
        if (_autosaves.Count > 0)
        {
            game.Load(_autosaves.Pop());
        }
        else
        {
            Console.WriteLine("No autosaves available");
        }
    }

    public void ListSaves()
    {
        Console.WriteLine("Save slots:");
        foreach (var (name, memento) in _saves)
        {
            Console.WriteLine($"  {name}: Level {memento.Level}, Score {memento.Score} ({memento.SavedAt})");
        }
        Console.WriteLine($"Autosaves: {_autosaves.Count}");
    }
}

// Gebruik
var game = new Game();
var saveManager = new SaveManager();

game.Play();
game.PickupItem("Sword");
saveManager.Autosave(game);

game.Play();
game.Play();
game.PickupItem("Shield");
saveManager.SaveToSlot(game, "Slot1");

game.TakeDamage(80);
game.ShowStatus();

Console.WriteLine("\n--- Loading save ---");
saveManager.LoadFromSlot(game, "Slot1");
game.ShowStatus();
```

### With Serialization

```csharp
public static class MementoSerializer
{
    public static string Serialize<T>(T memento)
    {
        return JsonSerializer.Serialize(memento, new JsonSerializerOptions { WriteIndented = true });
    }

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }

    public static void SaveToFile<T>(T memento, string filePath)
    {
        var json = Serialize(memento);
        File.WriteAllText(filePath, json);
    }

    public static T? LoadFromFile<T>(string filePath)
    {
        if (!File.Exists(filePath))
            return default;
        
        var json = File.ReadAllText(filePath);
        return Deserialize<T>(json);
    }
}

// Gebruik
var memento = game.Save();
MementoSerializer.SaveToFile(memento, "savegame.json");

var loaded = MementoSerializer.LoadFromFile<GameMemento>("savegame.json");
if (loaded != null)
    game.Load(loaded);
```

## Voordelen
- ? Behoudt encapsulation
- ? Vereenvoudigt Originator
- ? Easy undo/redo implementatie
- ? Snapshots voor recovery

## Nadelen
- ? Kan veel geheugen gebruiken
- ? Caretaker moet mementos opruimen
- ? Sommige talen maken het moeilijk encapsulation te behouden

## Gerelateerde Patterns
- **Command**: Kan samen met Memento voor complete undo
- **Prototype**: Alternatief voor deep copy
- **Iterator**: Voor traversing memento history
