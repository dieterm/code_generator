# Iterator Pattern

## Intentie
Biedt een manier om **sequentieel toegang** te krijgen tot elementen van een aggregate object zonder de onderliggende representatie bloot te stellen.

## Wanneer gebruiken?
- Wanneer je door een collectie wilt lopen zonder de interne structuur te kennen
- Wanneer je meerdere traversal methoden wilt ondersteunen
- Wanneer je een uniforme interface wilt voor verschillende collecties

## Implementatie in C#

C# heeft ingebouwde ondersteuning voor het Iterator pattern via `IEnumerable<T>` en `IEnumerator<T>`.

### Built-in Iterator

```csharp
public class BookCollection : IEnumerable<Book>
{
    private readonly List<Book> _books = new();

    public void Add(Book book) => _books.Add(book);

    public IEnumerator<Book> GetEnumerator() => _books.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
}

// Gebruik
var collection = new BookCollection();
collection.Add(new Book { Title = "Design Patterns", Author = "GoF" });
collection.Add(new Book { Title = "Clean Code", Author = "Robert Martin" });

foreach (var book in collection)
{
    Console.WriteLine($"{book.Title} by {book.Author}");
}
```

### Custom Iterator met yield return

```csharp
public class TreeNode<T>
{
    public T Value { get; set; }
    public TreeNode<T>? Left { get; set; }
    public TreeNode<T>? Right { get; set; }

    public TreeNode(T value) => Value = value;

    // In-order traversal
    public IEnumerable<T> InOrder()
    {
        if (Left != null)
            foreach (var item in Left.InOrder())
                yield return item;

        yield return Value;

        if (Right != null)
            foreach (var item in Right.InOrder())
                yield return item;
    }

    // Pre-order traversal
    public IEnumerable<T> PreOrder()
    {
        yield return Value;

        if (Left != null)
            foreach (var item in Left.PreOrder())
                yield return item;

        if (Right != null)
            foreach (var item in Right.PreOrder())
                yield return item;
    }

    // Level-order (BFS)
    public IEnumerable<T> LevelOrder()
    {
        var queue = new Queue<TreeNode<T>>();
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            yield return node.Value;

            if (node.Left != null) queue.Enqueue(node.Left);
            if (node.Right != null) queue.Enqueue(node.Right);
        }
    }
}

// Gebruik
var root = new TreeNode<int>(4)
{
    Left = new TreeNode<int>(2)
    {
        Left = new TreeNode<int>(1),
        Right = new TreeNode<int>(3)
    },
    Right = new TreeNode<int>(6)
    {
        Left = new TreeNode<int>(5),
        Right = new TreeNode<int>(7)
    }
};

Console.WriteLine("In-order: " + string.Join(", ", root.InOrder()));     // 1,2,3,4,5,6,7
Console.WriteLine("Pre-order: " + string.Join(", ", root.PreOrder()));   // 4,2,1,3,6,5,7
Console.WriteLine("Level-order: " + string.Join(", ", root.LevelOrder())); // 4,2,6,1,3,5,7
```

### Paginated Iterator

```csharp
public class PaginatedList<T> : IEnumerable<T>
{
    private readonly IQueryable<T> _source;
    private readonly int _pageSize;

    public PaginatedList(IQueryable<T> source, int pageSize = 10)
    {
        _source = source;
        _pageSize = pageSize;
    }

    public IEnumerator<T> GetEnumerator()
    {
        int page = 0;
        while (true)
        {
            var items = _source
                .Skip(page * _pageSize)
                .Take(_pageSize)
                .ToList();

            if (items.Count == 0)
                yield break;

            foreach (var item in items)
                yield return item;

            page++;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

// Gebruik - automatisch pagineren door grote dataset
var allUsers = new PaginatedList<User>(dbContext.Users, pageSize: 100);

foreach (var user in allUsers)
{
    // Processed in batches of 100
    ProcessUser(user);
}
```

## Voordelen
- ? Uniforme interface voor traversal
- ? Single Responsibility: traversal logic gescheiden
- ? Meerdere traversal methoden mogelijk
- ? Lazy evaluation met yield return

## Modern C# Alternatieven

```csharp
// LINQ queries
var filtered = collection.Where(x => x.IsActive).OrderBy(x => x.Name);

// Async iterators
public async IAsyncEnumerable<T> GetAllAsync([EnumeratorCancellation] CancellationToken ct = default)
{
    await foreach (var item in source.WithCancellation(ct))
    {
        yield return item;
    }
}
```

## Gerelateerde Patterns
- **Composite**: Iterator kan composite structuren traverseren
- **Visitor**: Kan samen met Iterator worden gebruikt
