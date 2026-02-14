using System.Text;

namespace CodeGenerator.Domain.Mermaid;

/// <summary>
/// Lightweight builder for Mermaid graph diagrams.
/// Supports graph direction, nodes with optional labels, links between nodes, and subgraphs.
/// </summary>
public class MermaidGraphBuilder
{
    private readonly string _direction;
    private readonly List<string> _lines = new();
    private int _indent = 1;

    /// <summary>
    /// Create a new Mermaid graph builder.
    /// </summary>
    /// <param name="direction">Graph direction: TB (top-bottom), BT, LR (left-right), RL</param>
    public MermaidGraphBuilder(string direction = "LR")
    {
        _direction = direction;
    }

    /// <summary>
    /// Add a node with an optional display label.
    /// </summary>
    /// <param name="id">Node identifier (no spaces)</param>
    /// <param name="label">Optional display label. If null, the id is used.</param>
    /// <param name="shape">Node shape: box (default), round, stadium, cylinder, circle, diamond</param>
    public MermaidGraphBuilder Node(string id, string? label = null, string shape = "box")
    {
        if (label == null)
        {
            _lines.Add(Indent($"{id}"));
        }
        else
        {
            var (open, close) = GetShapeBrackets(shape);
            _lines.Add(Indent($"{id}{open}\"{Escape(label)}\"{close}"));
        }
        return this;
    }

    /// <summary>
    /// Add a directed link (arrow) between two nodes.
    /// </summary>
    /// <param name="from">Source node id</param>
    /// <param name="to">Target node id</param>
    /// <param name="label">Optional link label</param>
    public MermaidGraphBuilder Link(string from, string to, string? label = null)
    {
        if (label == null)
            _lines.Add(Indent($"{from} --> {to}"));
        else
            _lines.Add(Indent($"{from} -->|\"{Escape(label)}\"|{to}"));
        return this;
    }

    /// <summary>
    /// Add a dotted link between two nodes.
    /// </summary>
    public MermaidGraphBuilder DottedLink(string from, string to, string? label = null)
    {
        if (label == null)
            _lines.Add(Indent($"{from} -.-> {to}"));
        else
            _lines.Add(Indent($"{from} -.->|\"{Escape(label)}\"|{to}"));
        return this;
    }

    /// <summary>
    /// Start a subgraph section.
    /// </summary>
    /// <param name="id">Subgraph identifier</param>
    /// <param name="title">Display title for the subgraph</param>
    public MermaidGraphBuilder SubgraphStart(string id, string? title = null)
    {
        _lines.Add(Indent($"subgraph {id}[\"{Escape(title ?? id)}\"]"));
        _indent++;
        return this;
    }

    /// <summary>
    /// End the current subgraph section.
    /// </summary>
    public MermaidGraphBuilder SubgraphEnd()
    {
        _indent = Math.Max(1, _indent - 1);
        _lines.Add(Indent("end"));
        return this;
    }

    /// <summary>
    /// Add a raw Mermaid line (for advanced syntax not covered by helper methods).
    /// </summary>
    public MermaidGraphBuilder Raw(string line)
    {
        _lines.Add(Indent(line));
        return this;
    }

    /// <summary>
    /// Add a style class definition.
    /// </summary>
    /// <param name="className">CSS class name</param>
    /// <param name="style">CSS style string, e.g. "fill:#f9f,stroke:#333"</param>
    public MermaidGraphBuilder ClassDef(string className, string style)
    {
        _lines.Add(Indent($"classDef {className} {style}"));
        return this;
    }

    /// <summary>
    /// Apply a style class to one or more nodes.
    /// </summary>
    public MermaidGraphBuilder ApplyClass(string className, params string[] nodeIds)
    {
        _lines.Add(Indent($"class {string.Join(",", nodeIds)} {className}"));
        return this;
    }

    /// <summary>
    /// Build the complete Mermaid diagram string.
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"graph {_direction}");
        foreach (var line in _lines)
        {
            sb.AppendLine(line);
        }
        return sb.ToString();
    }

    private string Indent(string text)
    {
        return new string(' ', _indent * 4) + text;
    }

    private static string Escape(string text)
    {
        return text.Replace("\"", "#quot;");
    }

    private static (string open, string close) GetShapeBrackets(string shape) => shape switch
    {
        "round" => ("(", ")"),
        "stadium" => ("([", "])"),
        "cylinder" => ("[(", ")]"),
        "circle" => ("((", "))"),
        "diamond" => ("{", "}"),
        "hexagon" => ("{{", "}}"),
        _ => ("[", "]") // box (default)
    };
}
