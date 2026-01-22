using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Memento
{
    /// <summary>
    /// JSON-serializable state of a memento object.
    /// </summary>
    public abstract class MementoState : IMementoState// where T : IMementoObject
    {
        private string? _typeName = null;
        public string TypeName {
            get { 
                if (string.IsNullOrWhiteSpace(_typeName)) 
                    throw new InvalidOperationException($"The TypeName of this '{this.GetType().FullName}'-instance is not set"); 
                return _typeName; 
            }
            set { 
                _typeName = value;
            }
        }

        public Dictionary<string, object?> Properties { get; set; } = new Dictionary<string, object?>();

        public abstract object Clone();

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
