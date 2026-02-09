using CodeGenerator.Shared.UndoRedo;

namespace CodeGenerator.Shared.Operations
{
    /// <summary>
    /// Central executor for all operations.
    /// Handles validation, execution, undo/redo recording, and provides
    /// a registry for operation discovery (e.g. by Copilot).
    /// </summary>
    public class OperationExecutor
    {
        private readonly UndoRedoManager _undoRedoManager;
        private readonly List<IOperation> _registeredOperations = new();

        public OperationExecutor(UndoRedoManager undoRedoManager)
        {
            _undoRedoManager = undoRedoManager;
        }

        /// <summary>
        /// Register an operation for discovery (e.g. by Copilot bridge).
        /// </summary>
        public void Register(IOperation operation)
        {
            _registeredOperations.Add(operation);
        }

        /// <summary>
        /// Execute an operation with the given parameters.
        /// On success, records the operation in the UndoRedoManager.
        /// Called from application code (controllers, context menus) and Copilot bridge.
        /// </summary>
        public OperationResult Execute<TParams>(IOperation<TParams> operation, TParams parameters)
            where TParams : class, new()
        {
            var result = operation.Execute(parameters);

            if (result.Success)
            {
                var undoableAction = new OperationUndoableAction<TParams>(operation, parameters);
                _undoRedoManager.RecordAction(undoableAction);
            }

            return result;
        }

        /// <summary>
        /// Get all registered operations — used by Copilot bridge to enumerate available tools.
        /// </summary>
        public IReadOnlyList<IOperation> GetRegisteredOperations() => _registeredOperations.AsReadOnly();
    }
}
