using CodeGenerator.Shared.Memento;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.ObjectBase
{
    public abstract class ObjectBase : IObjectBase
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangingEventHandler? PropertyChanging;

        protected ObjectBase()
        {
            Properties = new ReadOnlyDictionary<string, object?>(_properties);
        }

        protected virtual void RaisePropertyChangedEvent(string propertyName)
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
            return _properties.ContainsKey(name) ? (T?)_properties[name] : defaultValue;
        }

        public bool SetValue<T>(string name, T? value)
        {
            var oldValue = _properties.ContainsKey(name) ? _properties[name] : default(T?);
            if (!Equals(oldValue, value))
            {
                if (RaiseCancelablePropertyChangingEvent(name, oldValue, value))
                {
                    return false;
                }

                _properties[name] = value;

                RaisePropertyChangedEvent(name);
                return true;
            }
            return false;
        }

        public Dictionary<string, object?> _properties { get; } = new();
        public IReadOnlyDictionary<string, object?> Properties { get; }

        protected void FixListOfObject<T>(string name)
        {
            if (_properties.ContainsKey(name))
            {
                if (_properties[name] is not List<T>)
                {
                    var oldValue = _properties[name];
                    var isCorrectType = oldValue is IEnumerable<T>;
                    if (!isCorrectType)
                    {
                        var isObjectType = oldValue is IEnumerable<object>;
                        if (isObjectType)
                        {
                            try
                            {
                                _properties[name] = ((IEnumerable<object>)oldValue).Cast<T>().ToList();
                            }
                            catch (InvalidCastException)
                            {
                                var firstChildObject = ((IEnumerable<object>)oldValue).FirstOrDefault();
                                if (firstChildObject is IDictionary<string, object> dict)
                                {
                                    foreach (var item in (IEnumerable<object>)oldValue)
                                    {
                                        if (item is IDictionary<string, object> itemDict)
                                        {
                                            var instance = Activator.CreateInstance<T>();
                                            foreach (var kvp in itemDict)
                                            {
                                                var propInfo = typeof(T).GetProperties().FirstOrDefault(p => string.Equals(p.Name, kvp.Key, StringComparison.OrdinalIgnoreCase));
                                                if (propInfo != null && propInfo.CanWrite)
                                                {
                                                    propInfo.SetValue(instance, kvp.Value);
                                                }
                                            }
                                            if (_properties[name] == null || _properties[name] is List<object>)
                                                _properties[name] = new List<T>();
                                            ((List<T>)_properties[name]).Add(instance);
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
                _properties[name] = new List<T>();
            }
        }
    }
}
