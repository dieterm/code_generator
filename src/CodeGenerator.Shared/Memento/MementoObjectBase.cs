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

        protected void FixListOfObject<T>(string name)
        {
            if (Properties.ContainsKey(name))
            {
                if (Properties[name] is not List<T>)
                {
                    var oldValue = Properties[name];
                    var isCorrectType = oldValue is IEnumerable<T>;
                    if (!isCorrectType)
                    {
                        var isObjectType = oldValue is IEnumerable<object>;
                        if (isObjectType)
                        {
                            try
                            {
                                Properties[name] = ((IEnumerable<object>)oldValue).Cast<T>().ToList();
                            }
                            catch (InvalidCastException)
                            {
                                var firstChildObject = ((IEnumerable<object>)oldValue).FirstOrDefault();
                                if(firstChildObject is IDictionary<string, object> dict)
                                {
                                    foreach(var item in (IEnumerable<object>)oldValue)
                                    {
                                        if(item is IDictionary<string, object> itemDict)
                                        {
                                            var instance = Activator.CreateInstance<T>();
                                            foreach(var kvp in itemDict)
                                            {
                                                var propInfo = typeof(T).GetProperties().FirstOrDefault(p => string.Equals(p.Name, kvp.Key, StringComparison.OrdinalIgnoreCase));
                                                if(propInfo != null && propInfo.CanWrite)
                                                {
                                                    propInfo.SetValue(instance, kvp.Value);
                                                }
                                            }
                                            if(Properties[name] == null || Properties[name] is List<object>)
                                                Properties[name] = new List<T>();
                                            ((List<T>)Properties[name]).Add(instance);
                                        }
                                    }
                                }

                            }
                            
                        }
                        else
                        {
                            throw new ArgumentException($"Property '{name}' is not a list of the expected type.");
                        }
                    }
                }
            }
            else
            {
                Properties[name] = new List<T>();
            }
        }

        public bool IsStateChanged { get; private set; } = false;

        public virtual void RestoreState(IMementoState state)
        {
            Properties.Clear();
            foreach (var kvp in state.Properties)
            {
                Properties[kvp.Key] = kvp.Value;
            }
            ResetIsStateChangedFlag();
        }

        public virtual IMementoState CaptureState()
        {
            TState state = new TState();
            state.TypeName = this.GetType().AssemblyQualifiedName!;
            foreach (var kvp in Properties)
            {
                state.Properties[kvp.Key] = kvp.Value;
            }
            
            return state;
        }

        public void ResetIsStateChangedFlag()
        {
            IsStateChanged = false;
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
