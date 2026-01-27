# Composite Pattern

## Intentie
Stelt objecten samen in **boomstructuren** om deel-geheel hiërarchieën te representeren. Composite laat clients individuele objecten en composities van objecten **uniform** behandelen.

## Wanneer gebruiken?
- Wanneer je boomstructuren wilt representeren
- Wanneer clients composities en individuele objecten uniform moeten behandelen
- File systems, menu structures, organizational hierarchies
- UI component trees

## Structuur

```
???????????????????
?   Component     ?
???????????????????
? +Operation()    ?
? +Add()          ?
? +Remove()       ?
? +GetChild()     ?
???????????????????
        ?
   ???????????
   ?         ?
????????  ???????????????
? Leaf ?  ?  Composite  ?????
????????  ???????????????   ?
          ? -children   ?????
          ? +Operation()?
          ? +Add()      ?
          ???????????????
```

## Implementatie in C#

### Basis Implementatie: File System

```csharp
// Component
public abstract class FileSystemItem
{
    public string Name { get; protected set; }
    public abstract long Size { get; }
    
    protected FileSystemItem(string name)
    {
        Name = name;
    }

    public abstract void Display(int indent = 0);
    
    protected string GetIndent(int indent) => new string(' ', indent * 2);
}

// Leaf
public class File : FileSystemItem
{
    private readonly long _size;

    public File(string name, long size) : base(name)
    {
        _size = size;
    }

    public override long Size => _size;

    public override void Display(int indent = 0)
    {
        Console.WriteLine($"{GetIndent(indent)}?? {Name} ({Size:N0} bytes)");
    }
}

// Composite
public class Directory : FileSystemItem
{
    private readonly List<FileSystemItem> _children = new();

    public Directory(string name) : base(name) { }

    public override long Size => _children.Sum(c => c.Size);

    public IReadOnlyList<FileSystemItem> Children => _children.AsReadOnly();

    public void Add(FileSystemItem item)
    {
        _children.Add(item);
    }

    public void Remove(FileSystemItem item)
    {
        _children.Remove(item);
    }

    public override void Display(int indent = 0)
    {
        Console.WriteLine($"{GetIndent(indent)}?? {Name}/ ({Size:N0} bytes)");
        foreach (var child in _children)
        {
            child.Display(indent + 1);
        }
    }
}

// Gebruik
var root = new Directory("root");

var documents = new Directory("Documents");
documents.Add(new File("resume.docx", 25600));
documents.Add(new File("report.pdf", 102400));

var photos = new Directory("Photos");
var vacation = new Directory("Vacation");
vacation.Add(new File("beach.jpg", 2048000));
vacation.Add(new File("sunset.jpg", 1536000));
photos.Add(vacation);
photos.Add(new File("profile.png", 512000));

root.Add(documents);
root.Add(photos);
root.Add(new File("readme.txt", 1024));

root.Display();
Console.WriteLine($"\nTotal size: {root.Size:N0} bytes");
```

### Praktisch Voorbeeld: Menu System

```csharp
public abstract class MenuComponent
{
    public string Name { get; }
    public string? Description { get; protected set; }

    protected MenuComponent(string name)
    {
        Name = name;
    }

    public virtual void Add(MenuComponent component)
        => throw new NotSupportedException();
    
    public virtual void Remove(MenuComponent component)
        => throw new NotSupportedException();
    
    public virtual MenuComponent GetChild(int index)
        => throw new NotSupportedException();

    public abstract void Print(int indent = 0);
    public abstract decimal GetPrice();
}

// Leaf: Individual menu item
public class MenuItem : MenuComponent
{
    public decimal Price { get; }
    public bool IsVegetarian { get; }

    public MenuItem(string name, string description, decimal price, bool isVegetarian = false)
        : base(name)
    {
        Description = description;
        Price = price;
        IsVegetarian = isVegetarian;
    }

    public override decimal GetPrice() => Price;

    public override void Print(int indent = 0)
    {
        var veg = IsVegetarian ? " ??" : "";
        Console.WriteLine($"{new string(' ', indent * 2)}{Name}{veg} - €{Price:F2}");
        Console.WriteLine($"{new string(' ', indent * 2)}  {Description}");
    }
}

// Composite: Menu containing items or sub-menus
public class Menu : MenuComponent
{
    private readonly List<MenuComponent> _components = new();

    public Menu(string name, string description) : base(name)
    {
        Description = description;
    }

    public override void Add(MenuComponent component) => _components.Add(component);
    public override void Remove(MenuComponent component) => _components.Remove(component);
    public override MenuComponent GetChild(int index) => _components[index];

    public override decimal GetPrice() => _components.Sum(c => c.GetPrice());

    public override void Print(int indent = 0)
    {
        Console.WriteLine($"\n{new string(' ', indent * 2)}=== {Name.ToUpper()} ===");
        Console.WriteLine($"{new string(' ', indent * 2)}{Description}");
        Console.WriteLine();

        foreach (var component in _components)
        {
            component.Print(indent + 1);
        }
    }
}

// Gebruik
var allMenus = new Menu("ALL MENUS", "Complete restaurant menu");

var breakfastMenu = new Menu("Breakfast", "Served 7am - 11am");
breakfastMenu.Add(new MenuItem("Pancakes", "Fluffy pancakes with maple syrup", 8.99m, true));
breakfastMenu.Add(new MenuItem("Eggs Benedict", "Poached eggs on English muffin", 12.99m));
breakfastMenu.Add(new MenuItem("Fruit Bowl", "Fresh seasonal fruits", 6.99m, true));

var lunchMenu = new Menu("Lunch", "Served 11am - 4pm");
lunchMenu.Add(new MenuItem("Caesar Salad", "Romaine lettuce, croutons, parmesan", 9.99m, true));
lunchMenu.Add(new MenuItem("Club Sandwich", "Triple decker with fries", 13.99m));

var dinnerMenu = new Menu("Dinner", "Served 4pm - 10pm");
var steakSubmenu = new Menu("Steaks", "Premium cuts");
steakSubmenu.Add(new MenuItem("Ribeye", "12oz prime ribeye", 34.99m));
steakSubmenu.Add(new MenuItem("Filet Mignon", "8oz tenderloin", 39.99m));
dinnerMenu.Add(steakSubmenu);
dinnerMenu.Add(new MenuItem("Salmon", "Atlantic salmon with vegetables", 24.99m));

allMenus.Add(breakfastMenu);
allMenus.Add(lunchMenu);
allMenus.Add(dinnerMenu);

allMenus.Print();
```

### Organization Hierarchy

```csharp
public abstract class OrganizationComponent
{
    public string Name { get; }
    public string Title { get; }
    public decimal Salary { get; protected set; }

    protected OrganizationComponent(string name, string title, decimal salary)
    {
        Name = name;
        Title = title;
        Salary = salary;
    }

    public abstract decimal GetTotalSalary();
    public abstract int GetEmployeeCount();
    public abstract void Print(int indent = 0);
    
    public virtual void Add(OrganizationComponent component) 
        => throw new NotSupportedException();
    public virtual void Remove(OrganizationComponent component) 
        => throw new NotSupportedException();
}

// Leaf: Individual employee
public class Employee : OrganizationComponent
{
    public Employee(string name, string title, decimal salary) 
        : base(name, title, salary) { }

    public override decimal GetTotalSalary() => Salary;
    public override int GetEmployeeCount() => 1;

    public override void Print(int indent = 0)
    {
        Console.WriteLine($"{new string(' ', indent * 2)}?? {Name} - {Title} (€{Salary:N0})");
    }
}

// Composite: Manager with team
public class Manager : OrganizationComponent
{
    private readonly List<OrganizationComponent> _team = new();

    public Manager(string name, string title, decimal salary) 
        : base(name, title, salary) { }

    public IReadOnlyList<OrganizationComponent> Team => _team.AsReadOnly();

    public override void Add(OrganizationComponent component) => _team.Add(component);
    public override void Remove(OrganizationComponent component) => _team.Remove(component);

    public override decimal GetTotalSalary() => Salary + _team.Sum(m => m.GetTotalSalary());
    public override int GetEmployeeCount() => 1 + _team.Sum(m => m.GetEmployeeCount());

    public override void Print(int indent = 0)
    {
        Console.WriteLine($"{new string(' ', indent * 2)}?? {Name} - {Title} (€{Salary:N0})");
        foreach (var member in _team)
        {
            member.Print(indent + 1);
        }
    }
}

// Gebruik
var ceo = new Manager("Alice", "CEO", 250000);

var cto = new Manager("Bob", "CTO", 180000);
var devManager = new Manager("Charlie", "Dev Manager", 120000);
devManager.Add(new Employee("Dave", "Senior Developer", 80000));
devManager.Add(new Employee("Eve", "Developer", 60000));
devManager.Add(new Employee("Frank", "Junior Developer", 45000));
cto.Add(devManager);
cto.Add(new Employee("Grace", "DevOps Engineer", 75000));

var cfo = new Manager("Henry", "CFO", 170000);
cfo.Add(new Employee("Ivy", "Accountant", 55000));
cfo.Add(new Employee("Jack", "Financial Analyst", 65000));

ceo.Add(cto);
ceo.Add(cfo);

ceo.Print();
Console.WriteLine($"\nTotal employees: {ceo.GetEmployeeCount()}");
Console.WriteLine($"Total salary budget: €{ceo.GetTotalSalary():N0}");
```

### Generic Composite

```csharp
public interface IComponent<T> where T : IComponent<T>
{
    string Name { get; }
    IEnumerable<T> GetChildren();
    void Accept(IVisitor<T> visitor);
}

public interface IComposite<T> : IComponent<T> where T : IComponent<T>
{
    void Add(T component);
    void Remove(T component);
}

public interface IVisitor<T> where T : IComponent<T>
{
    void Visit(T component);
}

// Example: Task hierarchy
public abstract class TaskItem : IComponent<TaskItem>
{
    public string Name { get; }
    public abstract bool IsComplete { get; }
    public abstract TimeSpan EstimatedDuration { get; }

    protected TaskItem(string name)
    {
        Name = name;
    }

    public abstract IEnumerable<TaskItem> GetChildren();
    public abstract void Accept(IVisitor<TaskItem> visitor);
}

public class SimpleTask : TaskItem
{
    public override bool IsComplete { get; }
    public override TimeSpan EstimatedDuration { get; }

    public SimpleTask(string name, TimeSpan duration, bool isComplete = false) 
        : base(name)
    {
        EstimatedDuration = duration;
        IsComplete = isComplete;
    }

    public override IEnumerable<TaskItem> GetChildren() => Enumerable.Empty<TaskItem>();
    
    public override void Accept(IVisitor<TaskItem> visitor) => visitor.Visit(this);
}

public class TaskGroup : TaskItem, IComposite<TaskItem>
{
    private readonly List<TaskItem> _tasks = new();

    public TaskGroup(string name) : base(name) { }

    public override bool IsComplete => _tasks.All(t => t.IsComplete);
    public override TimeSpan EstimatedDuration => 
        _tasks.Aggregate(TimeSpan.Zero, (sum, t) => sum + t.EstimatedDuration);

    public void Add(TaskItem component) => _tasks.Add(component);
    public void Remove(TaskItem component) => _tasks.Remove(component);

    public override IEnumerable<TaskItem> GetChildren() => _tasks;

    public override void Accept(IVisitor<TaskItem> visitor)
    {
        visitor.Visit(this);
        foreach (var task in _tasks)
        {
            task.Accept(visitor);
        }
    }
}

// Visitor for printing
public class TaskPrinter : IVisitor<TaskItem>
{
    private int _indent = 0;

    public void Visit(TaskItem component)
    {
        var status = component.IsComplete ? "?" : "?";
        var duration = component.EstimatedDuration.TotalHours;
        Console.WriteLine($"{new string(' ', _indent * 2)}{status} {component.Name} ({duration:F1}h)");
        
        if (component is TaskGroup)
        {
            _indent++;
            // Children will be visited by TaskGroup.Accept
        }
    }
}
```

## Voordelen
- ? Uniforme behandeling van individuele en samengestelde objecten
- ? Open/Closed: nieuwe component types toevoegen is makkelijk
- ? Recursieve structuren eenvoudig te bouwen
- ? Client code blijft eenvoudig

## Nadelen
- ? Kan te generiek worden (moeilijk type-specifieke operaties)
- ? Design kan overcomplicated zijn voor simpele hiërarchieën

## Gerelateerde Patterns
- **Iterator**: Traverse composite structures
- **Visitor**: Operaties toevoegen aan composite
- **Decorator**: Voegt verantwoordelijkheden toe
- **Flyweight**: Leaf nodes kunnen gedeeld worden
