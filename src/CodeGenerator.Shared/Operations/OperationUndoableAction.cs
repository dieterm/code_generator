using CodeGenerator.Shared.UndoRedo;

namespace CodeGenerator.Shared.Operations
{
    /// <summary>
    /// Adapts an <see cref="IOperation{TParams}"/> into an <see cref="IUndoableAction"/>
    /// so it can be recorded by the <see cref="UndoRedoManager"/>.
    /// Created by <see cref="OperationExecutor"/> after a successful execution.
    /// </summary>
    public class OperationUndoableAction<TParams> : IUndoableAction where TParams : class, new()
    {
        private readonly IOperation<TParams> _operation;
        private readonly TParams _parameters;

        public OperationUndoableAction(IOperation<TParams> operation, TParams parameters)
        {
            _operation = operation;
            _parameters = parameters;
        }

        public string Description => _operation.DisplayName;

        public void Execute() => _operation.Execute(_parameters);

        public void Undo() => _operation.Undo(_parameters);

        public void Redo() => _operation.Redo(_parameters);
    }
}
