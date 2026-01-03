using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Schema;

namespace CodeGenerator.Core.Events;

/// <summary>
/// Base class for all generator events
/// </summary>
public abstract class GeneratorEventArgs : EventArgs
{
    /// <summary>
    /// The domain schema being processed
    /// </summary>
    public DomainSchema Schema { get; }

    /// <summary>
    /// The parsed domain context
    /// </summary>
    public DomainContext Context { get; }

    /// <summary>
    /// Timestamp when the event was raised
    /// </summary>
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    protected GeneratorEventArgs(DomainSchema schema, DomainContext context)
    {
        Schema = schema ?? throw new ArgumentNullException(nameof(schema));
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }
}
