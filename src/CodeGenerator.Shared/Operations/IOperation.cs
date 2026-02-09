namespace CodeGenerator.Shared.Operations
{
    /// <summary>
    /// Non-generic marker interface for operation discovery via DI container.
    /// All operations implement this so they can be enumerated with GetServices&lt;IOperation&gt;().
    /// </summary>
    public interface IOperation
    {
        /// <summary>Unique operation ID (e.g. "AddScope")</summary>
        string OperationId { get; }

        /// <summary>Human-readable name for UI display and undo history</summary>
        string DisplayName { get; }

        /// <summary>Detailed description for Copilot tool registration</summary>
        string Description { get; }

        /// <summary>The Type of the TParams POCO for this operation</summary>
        Type ParameterType { get; }
    }

    /// <summary>
    /// A typed operation that can be:
    /// <list type="bullet">
    /// <item>Executed from application code (controllers, context menus)</item>
    /// <item>Registered as a Copilot AI tool (parameter metadata via [Description] attributes on TParams)</item>
    /// <item>Tracked by the UndoRedoManager (via OperationExecutor)</item>
    /// </list>
    /// </summary>
    /// <typeparam name="TParams">
    /// A POCO class describing the operation's input parameters.
    /// Use [Description] and [Required] attributes on properties to provide
    /// metadata for both Copilot and validation.
    /// </typeparam>
    public interface IOperation<TParams> : IOperation where TParams : class, new()
    {
        /// <summary>
        /// Validate whether this operation can execute with the given parameters.
        /// Returns null if valid, or an error message if not.
        /// </summary>
        string? Validate(TParams parameters);

        /// <summary>Execute the operation. Returns a result message.</summary>
        OperationResult Execute(TParams parameters);

        /// <summary>Undo the last execution, restoring the previous state.</summary>
        void Undo(TParams parameters);

        /// <summary>Redo the operation after it was undone.</summary>
        void Redo(TParams parameters);
    }
}
