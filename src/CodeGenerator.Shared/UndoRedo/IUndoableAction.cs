namespace CodeGenerator.Shared.UndoRedo
{
    /// <summary>
    /// Represents an undoable action that can be executed, undone and redone.
    /// This is the base interface for the Command pattern used in the Undo/Redo system.
    /// </summary>
    public interface IUndoableAction
    {
        /// <summary>
        /// A human-readable description of this action (e.g., "Changed Name to 'Customer'")
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Execute the action
        /// </summary>
        void Execute();

        /// <summary>
        /// Undo the action, restoring the previous state
        /// </summary>
        void Undo();

        /// <summary>
        /// Redo the action after it was undone
        /// </summary>
        void Redo();
    }
}
