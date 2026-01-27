# Visitor Pattern

## Intentie
Representeert een **operatie uit te voeren** op elementen van een object structuur. Visitor laat je een nieuwe operatie definiëren zonder de classes van de elementen te wijzigen.

## Wanneer gebruiken?
- Wanneer je veel ongerelateerde operaties wilt uitvoeren op een object structuur
- Wanneer de object structuur stabiel is maar operaties vaak wijzigen
- Wanneer je operaties wilt toevoegen zonder de element classes te wijzigen
- Compiler design, document processing, report generators

## Structuur

```
???????????????????        ???????????????????
?    Visitor      ?        ?    Element      ?
???????????????????        ???????????????????
? +VisitConcreteA ?        ? +Accept(visitor)?
? +VisitConcreteB ?        ???????????????????
???????????????????                ?
        ?                   ???????????????
        ?                   ?             ?
?????????????????    ????????????  ????????????
?ConcreteVisitor?    ?ElementA  ?  ?ElementB  ?
?????????????????    ????????????  ????????????
```

## Implementatie in C#

### Basis Implementatie: Document Export

```csharp
// Element interface
public interface IDocumentElement
{
    void Accept(IDocumentVisitor visitor);
}

// Concrete Elements
public class Paragraph : IDocumentElement
{
    public string Text { get; set; }

    public Paragraph(string text)
    {
        Text = text;
    }

    public void Accept(IDocumentVisitor visitor)
    {
        visitor.Visit(this);
    }
}

public class Heading : IDocumentElement
{
    public string Text { get; set; }
    public int Level { get; set; }

    public Heading(string text, int level)
    {
        Text = text;
        Level = level;
    }

    public void Accept(IDocumentVisitor visitor)
    {
        visitor.Visit(this);
    }
}

public class Image : IDocumentElement
{
    public string Src { get; set; }
    public string Alt { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public Image(string src, string alt, int width, int height)
    {
        Src = src;
        Alt = alt;
        Width = width;
        Height = height;
    }

    public void Accept(IDocumentVisitor visitor)
    {
        visitor.Visit(this);
    }
}

public class Table : IDocumentElement
{
    public List<List<string>> Rows { get; set; } = new();

    public void Accept(IDocumentVisitor visitor)
    {
        visitor.Visit(this);
    }
}

// Visitor interface
public interface IDocumentVisitor
{
    void Visit(Paragraph paragraph);
    void Visit(Heading heading);
    void Visit(Image image);
    void Visit(Table table);
}

// Concrete Visitors
public class HtmlExportVisitor : IDocumentVisitor
{
    private readonly StringBuilder _output = new();

    public string GetOutput() => _output.ToString();

    public void Visit(Paragraph paragraph)
    {
        _output.AppendLine($"<p>{paragraph.Text}</p>");
    }

    public void Visit(Heading heading)
    {
        _output.AppendLine($"<h{heading.Level}>{heading.Text}</h{heading.Level}>");
    }

    public void Visit(Image image)
    {
        _output.AppendLine($"<img src=\"{image.Src}\" alt=\"{image.Alt}\" width=\"{image.Width}\" height=\"{image.Height}\" />");
    }

    public void Visit(Table table)
    {
        _output.AppendLine("<table>");
        foreach (var row in table.Rows)
        {
            _output.AppendLine("  <tr>");
            foreach (var cell in row)
            {
                _output.AppendLine($"    <td>{cell}</td>");
            }
            _output.AppendLine("  </tr>");
        }
        _output.AppendLine("</table>");
    }
}

public class MarkdownExportVisitor : IDocumentVisitor
{
    private readonly StringBuilder _output = new();

    public string GetOutput() => _output.ToString();

    public void Visit(Paragraph paragraph)
    {
        _output.AppendLine(paragraph.Text);
        _output.AppendLine();
    }

    public void Visit(Heading heading)
    {
        var hashes = new string('#', heading.Level);
        _output.AppendLine($"{hashes} {heading.Text}");
        _output.AppendLine();
    }

    public void Visit(Image image)
    {
        _output.AppendLine($"![{image.Alt}]({image.Src})");
        _output.AppendLine();
    }

    public void Visit(Table table)
    {
        if (table.Rows.Count == 0) return;

        var header = table.Rows[0];
        _output.AppendLine("| " + string.Join(" | ", header) + " |");
        _output.AppendLine("| " + string.Join(" | ", header.Select(_ => "---")) + " |");

        foreach (var row in table.Rows.Skip(1))
        {
            _output.AppendLine("| " + string.Join(" | ", row) + " |");
        }
        _output.AppendLine();
    }
}

public class WordCountVisitor : IDocumentVisitor
{
    public int TotalWords { get; private set; }
    public int ParagraphCount { get; private set; }
    public int HeadingCount { get; private set; }
    public int ImageCount { get; private set; }
    public int TableCount { get; private set; }

    private int CountWords(string text)
    {
        return text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }

    public void Visit(Paragraph paragraph)
    {
        ParagraphCount++;
        TotalWords += CountWords(paragraph.Text);
    }

    public void Visit(Heading heading)
    {
        HeadingCount++;
        TotalWords += CountWords(heading.Text);
    }

    public void Visit(Image image)
    {
        ImageCount++;
    }

    public void Visit(Table table)
    {
        TableCount++;
        foreach (var row in table.Rows)
        {
            foreach (var cell in row)
            {
                TotalWords += CountWords(cell);
            }
        }
    }

    public void PrintStats()
    {
        Console.WriteLine($"Document Statistics:");
        Console.WriteLine($"  Total words: {TotalWords}");
        Console.WriteLine($"  Paragraphs: {ParagraphCount}");
        Console.WriteLine($"  Headings: {HeadingCount}");
        Console.WriteLine($"  Images: {ImageCount}");
        Console.WriteLine($"  Tables: {TableCount}");
    }
}

// Document (Object Structure)
public class Document
{
    private readonly List<IDocumentElement> _elements = new();

    public void Add(IDocumentElement element) => _elements.Add(element);

    public void Accept(IDocumentVisitor visitor)
    {
        foreach (var element in _elements)
        {
            element.Accept(visitor);
        }
    }
}

// Gebruik
var document = new Document();
document.Add(new Heading("Welcome to My Blog", 1));
document.Add(new Paragraph("This is an introduction to the Visitor pattern."));
document.Add(new Image("diagram.png", "Visitor Pattern Diagram", 600, 400));
document.Add(new Heading("How it Works", 2));
document.Add(new Paragraph("The visitor visits each element in the structure."));
document.Add(new Table
{
    Rows = new List<List<string>>
    {
        new() { "Pattern", "Category" },
        new() { "Visitor", "Behavioral" },
        new() { "Strategy", "Behavioral" }
    }
});

// Export to HTML
var htmlVisitor = new HtmlExportVisitor();
document.Accept(htmlVisitor);
Console.WriteLine("=== HTML Output ===");
Console.WriteLine(htmlVisitor.GetOutput());

// Export to Markdown
var mdVisitor = new MarkdownExportVisitor();
document.Accept(mdVisitor);
Console.WriteLine("=== Markdown Output ===");
Console.WriteLine(mdVisitor.GetOutput());

// Get statistics
var statsVisitor = new WordCountVisitor();
document.Accept(statsVisitor);
statsVisitor.PrintStats();
```

### Expression Visitor (Compiler/Interpreter)

```csharp
public interface IExpression
{
    T Accept<T>(IExpressionVisitor<T> visitor);
}

public interface IExpressionVisitor<T>
{
    T Visit(NumberExpression expr);
    T Visit(BinaryExpression expr);
    T Visit(UnaryExpression expr);
    T Visit(VariableExpression expr);
}

// Expression types
public class NumberExpression : IExpression
{
    public double Value { get; }
    public NumberExpression(double value) => Value = value;
    public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
}

public class VariableExpression : IExpression
{
    public string Name { get; }
    public VariableExpression(string name) => Name = name;
    public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
}

public class BinaryExpression : IExpression
{
    public IExpression Left { get; }
    public IExpression Right { get; }
    public string Operator { get; }

    public BinaryExpression(IExpression left, string op, IExpression right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }

    public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
}

public class UnaryExpression : IExpression
{
    public string Operator { get; }
    public IExpression Operand { get; }

    public UnaryExpression(string op, IExpression operand)
    {
        Operator = op;
        Operand = operand;
    }

    public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
}

// Evaluation visitor
public class EvaluatorVisitor : IExpressionVisitor<double>
{
    private readonly Dictionary<string, double> _variables;

    public EvaluatorVisitor(Dictionary<string, double> variables = null)
    {
        _variables = variables ?? new Dictionary<string, double>();
    }

    public double Visit(NumberExpression expr) => expr.Value;

    public double Visit(VariableExpression expr)
    {
        if (_variables.TryGetValue(expr.Name, out var value))
            return value;
        throw new InvalidOperationException($"Unknown variable: {expr.Name}");
    }

    public double Visit(BinaryExpression expr)
    {
        var left = expr.Left.Accept(this);
        var right = expr.Right.Accept(this);

        return expr.Operator switch
        {
            "+" => left + right,
            "-" => left - right,
            "*" => left * right,
            "/" => left / right,
            "^" => Math.Pow(left, right),
            _ => throw new InvalidOperationException($"Unknown operator: {expr.Operator}")
        };
    }

    public double Visit(UnaryExpression expr)
    {
        var operand = expr.Operand.Accept(this);
        return expr.Operator switch
        {
            "-" => -operand,
            "sqrt" => Math.Sqrt(operand),
            _ => throw new InvalidOperationException($"Unknown operator: {expr.Operator}")
        };
    }
}

// Print visitor
public class PrintVisitor : IExpressionVisitor<string>
{
    public string Visit(NumberExpression expr) => expr.Value.ToString();
    public string Visit(VariableExpression expr) => expr.Name;

    public string Visit(BinaryExpression expr)
    {
        return $"({expr.Left.Accept(this)} {expr.Operator} {expr.Right.Accept(this)})";
    }

    public string Visit(UnaryExpression expr)
    {
        return $"{expr.Operator}({expr.Operand.Accept(this)})";
    }
}

// Gebruik
// Expression: (x + 5) * 2
var expression = new BinaryExpression(
    new BinaryExpression(
        new VariableExpression("x"),
        "+",
        new NumberExpression(5)
    ),
    "*",
    new NumberExpression(2)
);

var printer = new PrintVisitor();
Console.WriteLine($"Expression: {expression.Accept(printer)}");

var evaluator = new EvaluatorVisitor(new Dictionary<string, double> { ["x"] = 10 });
Console.WriteLine($"Result (x=10): {expression.Accept(evaluator)}");
```

## Voordelen
- ? Open/Closed: nieuwe operaties zonder element wijzigingen
- ? Single Responsibility: operatie logica gecentraliseerd
- ? Kan state accumuleren tijdens traversal
- ? Werkt goed met Composite structuren

## Nadelen
- ? Moeilijk om nieuwe element types toe te voegen
- ? Kan encapsulation breken (elements moeten details blootstellen)
- ? Complexer dan directe methods op elements

## Wanneer NIET gebruiken
- Wanneer element hiërarchie vaak verandert
- Wanneer elements weinig operaties nodig hebben
- Wanneer encapsulation cruciaal is

## Gerelateerde Patterns
- **Composite**: Visitor traverseert composite structuren
- **Iterator**: Alternatief voor traversal
- **Interpreter**: Visitor kan expression trees evalueren
