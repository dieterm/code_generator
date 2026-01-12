using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Memento
{
    public abstract class MementoObjectBase<TState> : INotifyPropertyChanged, INotifyPropertyChanging, IMementoObject where TState : IMementoState, new()
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangingEventHandler? PropertyChanging;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// returns true if the event was canceled
        /// </summary>
        protected bool RaiseCancelablePropertyChangingEvent(string propertyName, object? oldValue, object? newValue)
        {
            var args = new CancelablePropertyChangingEventArgs(propertyName, oldValue, newValue);
            PropertyChanging?.Invoke(this, args);
            return args.Cancel;
        }

        public T? GetValue<T>(string name, T? defaultValue = default)
        {
            return Properties.ContainsKey(name) ? (T?)Properties[name] : defaultValue;
        }

        public bool SetValue<T>(string name, T? value)
        {
            var oldValue = Properties.ContainsKey(name) ? Properties[name] : default(T?);
            if (!Equals(oldValue, value))
            {
                if (RaiseCancelablePropertyChangingEvent(name, oldValue, value))
                {
                    return false;
                }

                Properties[name] = value;
                IsStateChanged = true;
                RaisePropertyChangedEvent(name);
                return true;
            }
            return false;
        }

        public Dictionary<string, object?> Properties { get; } = new();

        public bool IsStateChanged { get; private set; } = false;

        public virtual void RestoreState(IMementoState state)
        {
            Properties.Clear();
            foreach (var kvp in state.Properties)
            {
                Properties[kvp.Key] = kvp.Value;
            }
            IsStateChanged = false;
        }

        public virtual IMementoState CaptureState()
        {
            TState state = new TState();
            foreach (var kvp in Properties)
            {
                state.Properties[kvp.Key] = kvp.Value;
            }
            return state;
        }

        protected MementoObjectBase(IMementoState state)
        {
            RestoreState(state);
        }

        protected MementoObjectBase()
        {
        }
    }
}
