namespace CodeGenerator.Shared.UndoRedo
{
    /// <summary>
    /// A scope that automatically groups all recorded actions within it into a single composite action.
    /// Implements IDisposable for use with <c>using</c> statements.
    /// 
    /// <example>
    /// <code>
    /// using (var scope = new UndoRedoBatchScope(undoRedoManager, "Rename entity"))
    /// {
    ///     entity.Name = "NewName";
    ///     entity.Description = "Updated description";
    /// }
    /// // Both changes are now a single undoable action
    /// </code>
    /// </example>
    /// </summary>
    public class UndoRedoBatchScope : IDisposable
    {
        private readonly UndoRedoManager _manager;
        private bool _disposed;
        private bool _cancelled;

        /// <summary>
        /// Creates a new batch scope and begins recording
        /// </summary>
        /// <param name="manager">The UndoRedoManager to record to</param>
        /// <param name="description">Description for the composite action</param>
        public UndoRedoBatchScope(UndoRedoManager manager, string description)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _manager.BeginBatch(description);
        }

        /// <summary>
        /// Cancel the batch. All recorded actions will be discarded.
        /// </summary>
        public void Cancel()
        {
            if (!_disposed && !_cancelled)
            {
                _cancelled = true;
                _manager.CancelBatch();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (!_cancelled)
            {
                _manager.EndBatch();
            }
        }
    }
}
