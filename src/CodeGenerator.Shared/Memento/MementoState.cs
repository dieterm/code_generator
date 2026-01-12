using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Memento
{
    /// <summary>
    /// JSON-serializable state of a memento object.
    /// </summary>
    public abstract class MementoState<T> : IMementoState where T : IMementoObject
    {
        public virtual string TypeName
        {
            get { return typeof(T).FullName ?? typeof(T).Name; }
            set { throw new InvalidOperationException("TypeName is read-only. Implement different behavior in derived class"); }
        }

        public Dictionary<string, object?> Properties { get; set; } = new Dictionary<string, object?>();

        protected TValue GetValue<TValue>(string name, TValue? defaultValue = default)
        {
            return Properties.ContainsKey(name) ? (TValue?)Properties[name]! : defaultValue!;
        }

        protected void SetValue<TValue>(string name, TValue? value)
        {
            if (value is null)
            {
                if (Properties.ContainsKey(name))
                    Properties.Remove(name);
            }
            else
            {
                if (Properties.ContainsKey(name))
                    Properties[name] = value;
                else
                    Properties.Add(name, value);
            }
        }
    }

}
