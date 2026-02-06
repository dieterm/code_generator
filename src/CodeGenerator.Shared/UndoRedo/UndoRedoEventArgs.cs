namespace CodeGenerator.Shared.UndoRedo
{
    /// <summary>
    /// Event args for undo/redo events
    /// </summary>
    public class UndoRedoEventArgs : EventArgs
    {
        /// <summary>
        /// The action that was executed, undone or redone
        /// </summary>
        public IUndoableAction Action { get; }

        public UndoRedoEventArgs(IUndoableAction action)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }
    }
}
