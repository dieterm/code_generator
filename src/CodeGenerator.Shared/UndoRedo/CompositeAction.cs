namespace CodeGenerator.Shared.UndoRedo
{
    /// <summary>
    /// Groups multiple undoable actions into a single atomic operation.
    /// When undone, all actions are undone in reverse order.
    /// When redone, all actions are re-applied in original order.
    /// </summary>
    public class CompositeAction : IUndoableAction
    {
        private readonly List<IUndoableAction> _actions;

        /// <summary>
        /// Creates a new composite action with a description
        /// </summary>
        /// <param name="description">A human-readable description of this composite action</param>
        /// <param name="actions">The actions that make up this composite action</param>
        public CompositeAction(string description, IEnumerable<IUndoableAction> actions)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
            _actions = new List<IUndoableAction>(actions ?? throw new ArgumentNullException(nameof(actions)));
        }

        /// <summary>
        /// Creates a new composite action with a description
        /// </summary>
        /// <param name="description">A human-readable description of this composite action</param>
        /// <param name="actions">The actions that make up this composite action</param>
        public CompositeAction(string description, params IUndoableAction[] actions)
            : this(description, (IEnumerable<IUndoableAction>)actions)
        {
        }

        public string Description { get; }

        /// <summary>
        /// The actions that make up this composite action (read-only)
        /// </summary>
        public IReadOnlyList<IUndoableAction> Actions => _actions.AsReadOnly();

        public void Execute()
        {
            foreach (var action in _actions)
            {
                action.Execute();
            }
        }

        public void Undo()
        {
            // Undo in reverse order
            for (var i = _actions.Count - 1; i >= 0; i--)
            {
                _actions[i].Undo();
            }
        }

        public void Redo()
        {
            foreach (var action in _actions)
            {
                action.Redo();
            }
        }
    }
}
