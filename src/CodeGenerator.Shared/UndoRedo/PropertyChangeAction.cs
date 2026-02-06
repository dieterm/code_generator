namespace CodeGenerator.Shared.UndoRedo
{
    /// <summary>
    /// Represents a property value change that can be undone and redone.
    /// Captures the old and new value of a named property on a target object.
    /// </summary>
    public class PropertyChangeAction : IUndoableAction
    {
        private readonly object _target;
        private readonly string _propertyName;
        private readonly object? _oldValue;
        private readonly object? _newValue;
        private readonly Action<object, string, object?> _applyValue;

        /// <summary>
        /// Creates a new property change action
        /// </summary>
        /// <param name="target">The object that owns the property</param>
        /// <param name="propertyName">The name of the property that was changed</param>
        /// <param name="oldValue">The value before the change</param>
        /// <param name="newValue">The value after the change</param>
        /// <param name="applyValue">Delegate to apply a value to the target object's property</param>
        public PropertyChangeAction(
            object target,
            string propertyName,
            object? oldValue,
            object? newValue,
            Action<object, string, object?> applyValue)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            _oldValue = oldValue;
            _newValue = newValue;
            _applyValue = applyValue ?? throw new ArgumentNullException(nameof(applyValue));
        }

        public string Description => $"Change {_propertyName}";

        public void Execute()
        {
            _applyValue(_target, _propertyName, _newValue);
        }

        public void Undo()
        {
            _applyValue(_target, _propertyName, _oldValue);
        }

        public void Redo()
        {
            _applyValue(_target, _propertyName, _newValue);
        }
    }
}
