using System.ComponentModel;

namespace CodeGenerator.Shared.UndoRedo
{
    /// <summary>
    /// Manages undo and redo stacks for undoable actions.
    /// This is the central manager that tracks all changes and provides Undo/Redo functionality.
    /// </summary>
    public class UndoRedoManager : INotifyPropertyChanged
    {
        private readonly Stack<IUndoableAction> _undoStack = new();
        private readonly Stack<IUndoableAction> _redoStack = new();
        private readonly int _maxHistorySize;
        private bool _isPerformingUndoRedo;
        private bool _isBatchRecording;
        private List<IUndoableAction>? _batchActions;
        private string? _batchDescription;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<UndoRedoEventArgs>? ActionExecuted;
        public event EventHandler<UndoRedoEventArgs>? ActionUndone;
        public event EventHandler<UndoRedoEventArgs>? ActionRedone;
        public event EventHandler? HistoryChanged;

        /// <summary>
        /// Creates a new UndoRedoManager
        /// </summary>
        /// <param name="maxHistorySize">Maximum number of actions to keep in history. 0 means unlimited.</param>
        public UndoRedoManager(int maxHistorySize = 100)
        {
            _maxHistorySize = maxHistorySize;
        }

        /// <summary>
        /// Whether an undo operation can be performed
        /// </summary>
        public bool CanUndo => _undoStack.Count > 0;

        /// <summary>
        /// Whether a redo operation can be performed
        /// </summary>
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>
        /// Number of actions in the undo stack
        /// </summary>
        public int UndoCount => _undoStack.Count;

        /// <summary>
        /// Number of actions in the redo stack
        /// </summary>
        public int RedoCount => _redoStack.Count;

        /// <summary>
        /// Whether the manager is currently performing an undo or redo operation.
        /// Use this to suppress recording of changes that occur during undo/redo.
        /// </summary>
        public bool IsPerformingUndoRedo => _isPerformingUndoRedo;

        /// <summary>
        /// Whether a batch operation is currently being recorded
        /// </summary>
        public bool IsBatchRecording => _isBatchRecording;

        /// <summary>
        /// Description of the next action to undo, or null if nothing to undo
        /// </summary>
        public string? UndoDescription => _undoStack.Count > 0 ? _undoStack.Peek().Description : null;

        /// <summary>
        /// Description of the next action to redo, or null if nothing to redo
        /// </summary>
        public string? RedoDescription => _redoStack.Count > 0 ? _redoStack.Peek().Description : null;

        /// <summary>
        /// Execute an action and add it to the undo stack.
        /// Clears the redo stack since new changes invalidate the redo history.
        /// </summary>
        /// <param name="action">The action to execute and track</param>
        public void ExecuteAction(IUndoableAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            // Don't record actions triggered by undo/redo
            if (_isPerformingUndoRedo) return;

            if (_isBatchRecording)
            {
                _batchActions!.Add(action);
                return;
            }

            PushToUndoStack(action);
            _redoStack.Clear();

            RaiseHistoryChanged();
            ActionExecuted?.Invoke(this, new UndoRedoEventArgs(action));
        }

        /// <summary>
        /// Record an already-executed action without re-executing it.
        /// Use this when the change has already been applied (e.g., from data binding)
        /// and you just want to record it for undo purposes.
        /// </summary>
        /// <param name="action">The action to record</param>
        public void RecordAction(IUndoableAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            // Don't record actions triggered by undo/redo
            if (_isPerformingUndoRedo) return;

            if (_isBatchRecording)
            {
                _batchActions!.Add(action);
                return;
            }

            PushToUndoStack(action);
            _redoStack.Clear();

            RaiseHistoryChanged();
        }

        /// <summary>
        /// Undo the last action
        /// </summary>
        public void Undo()
        {
            if (!CanUndo) return;

            _isPerformingUndoRedo = true;
            try
            {
                var action = _undoStack.Pop();
                action.Undo();
                _redoStack.Push(action);

                RaiseHistoryChanged();
                ActionUndone?.Invoke(this, new UndoRedoEventArgs(action));
            }
            finally
            {
                _isPerformingUndoRedo = false;
            }
        }

        /// <summary>
        /// Undo multiple actions at once (up to and including the action at the given index).
        /// Index 0 = most recent action (top of stack).
        /// </summary>
        /// <param name="count">Number of actions to undo</param>
        public void Undo(int count)
        {
            if (count <= 0) return;
            count = Math.Min(count, _undoStack.Count);

            _isPerformingUndoRedo = true;
            try
            {
                for (var i = 0; i < count; i++)
                {
                    var action = _undoStack.Pop();
                    action.Undo();
                    _redoStack.Push(action);
                }

                RaiseHistoryChanged();
            }
            finally
            {
                _isPerformingUndoRedo = false;
            }
        }

        /// <summary>
        /// Redo the last undone action
        /// </summary>
        public void Redo()
        {
            if (!CanRedo) return;

            _isPerformingUndoRedo = true;
            try
            {
                var action = _redoStack.Pop();
                action.Redo();
                _undoStack.Push(action);

                RaiseHistoryChanged();
                ActionRedone?.Invoke(this, new UndoRedoEventArgs(action));
            }
            finally
            {
                _isPerformingUndoRedo = false;
            }
        }

        /// <summary>
        /// Redo multiple actions at once (up to and including the action at the given index).
        /// Index 0 = most recent undone action (top of stack).
        /// </summary>
        /// <param name="count">Number of actions to redo</param>
        public void Redo(int count)
        {
            if (count <= 0) return;
            count = Math.Min(count, _redoStack.Count);

            _isPerformingUndoRedo = true;
            try
            {
                for (var i = 0; i < count; i++)
                {
                    var action = _redoStack.Pop();
                    action.Redo();
                    _undoStack.Push(action);
                }

                RaiseHistoryChanged();
            }
            finally
            {
                _isPerformingUndoRedo = false;
            }
        }

        /// <summary>
        /// Start recording a batch of actions that will be grouped into a single composite action.
        /// Call <see cref="EndBatch"/> to finalize the batch.
        /// </summary>
        /// <param name="description">Description for the composite action</param>
        public void BeginBatch(string description)
        {
            if (_isBatchRecording)
                throw new InvalidOperationException("A batch operation is already in progress. Call EndBatch() first.");

            _isBatchRecording = true;
            _batchDescription = description;
            _batchActions = new List<IUndoableAction>();
        }

        /// <summary>
        /// End the current batch and push the composite action to the undo stack.
        /// If no actions were recorded during the batch, nothing is pushed.
        /// </summary>
        public void EndBatch()
        {
            if (!_isBatchRecording)
                throw new InvalidOperationException("No batch operation is in progress. Call BeginBatch() first.");

            _isBatchRecording = false;

            if (_batchActions != null && _batchActions.Count > 0)
            {
                var compositeAction = new CompositeAction(
                    _batchDescription ?? "Batch operation",
                    _batchActions);

                PushToUndoStack(compositeAction);
                _redoStack.Clear();

                RaiseHistoryChanged();
            }

            _batchActions = null;
            _batchDescription = null;
        }

        /// <summary>
        /// Cancel the current batch without recording any actions
        /// </summary>
        public void CancelBatch()
        {
            if (!_isBatchRecording)
                throw new InvalidOperationException("No batch operation is in progress.");

            _isBatchRecording = false;
            _batchActions = null;
            _batchDescription = null;
        }

        /// <summary>
        /// Clear all undo and redo history
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            RaiseHistoryChanged();
        }

        /// <summary>
        /// Get the descriptions of all actions in the undo stack (most recent first)
        /// </summary>
        public IEnumerable<string> GetUndoHistory()
        {
            return _undoStack.Select(a => a.Description);
        }

        /// <summary>
        /// Get the descriptions of all actions in the redo stack (most recent first)
        /// </summary>
        public IEnumerable<string> GetRedoHistory()
        {
            return _redoStack.Select(a => a.Description);
        }

        private void PushToUndoStack(IUndoableAction action)
        {
            _undoStack.Push(action);

            // Trim history if it exceeds the maximum size
            if (_maxHistorySize > 0 && _undoStack.Count > _maxHistorySize)
            {
                var items = _undoStack.ToArray();
                _undoStack.Clear();
                for (var i = items.Length - _maxHistorySize; i < items.Length; i++)
                {
                    _undoStack.Push(items[i]);
                }
            }
        }

        private void RaiseHistoryChanged()
        {
            HistoryChanged?.Invoke(this, EventArgs.Empty);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanUndo)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanRedo)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UndoCount)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RedoCount)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UndoDescription)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RedoDescription)));
        }
    }
}
