namespace CodeGenerator.Shared.UndoRedo
{
    /// <summary>
    /// Represents adding or removing a child from a parent that can be undone and redone.
    /// </summary>
    public class ChildChangeAction : IUndoableAction
    {
        private readonly object _parent;
        private readonly object _child;
        private readonly ChildChangeType _changeType;
        private readonly Action<object, object> _addChild;
        private readonly Action<object, object> _removeChild;

        /// <summary>
        /// Creates a new child change action
        /// </summary>
        /// <param name="parent">The parent object</param>
        /// <param name="child">The child object that was added or removed</param>
        /// <param name="changeType">Whether the child was added or removed</param>
        /// <param name="addChild">Delegate to add a child to the parent</param>
        /// <param name="removeChild">Delegate to remove a child from the parent</param>
        public ChildChangeAction(
            object parent,
            object child,
            ChildChangeType changeType,
            Action<object, object> addChild,
            Action<object, object> removeChild)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _child = child ?? throw new ArgumentNullException(nameof(child));
            _changeType = changeType;
            _addChild = addChild ?? throw new ArgumentNullException(nameof(addChild));
            _removeChild = removeChild ?? throw new ArgumentNullException(nameof(removeChild));
        }

        public string Description => _changeType == ChildChangeType.Added
            ? $"Add child"
            : $"Remove child";

        public void Execute()
        {
            if (_changeType == ChildChangeType.Added)
                _addChild(_parent, _child);
            else
                _removeChild(_parent, _child);
        }

        public void Undo()
        {
            if (_changeType == ChildChangeType.Added)
                _removeChild(_parent, _child);
            else
                _addChild(_parent, _child);
        }

        public void Redo()
        {
            Execute();
        }
    }

    /// <summary>
    /// The type of child change
    /// </summary>
    public enum ChildChangeType
    {
        Added,
        Removed
    }
}
