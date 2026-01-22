using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Memento
{
    public class CancelablePropertyChangingEventArgs : PropertyChangingEventArgs
    {
        public bool Cancel { get; set; } = false;
        public object? OldValue { get; }
        public object? NewValue { get; }

        public CancelablePropertyChangingEventArgs(string propertyName, object? oldValue, object? newValue) 
            : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
