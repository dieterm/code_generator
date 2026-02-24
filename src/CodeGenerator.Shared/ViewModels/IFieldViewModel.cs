using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.ViewModels
{
    public interface IFieldViewModel : INotifyPropertyChanged
    {
        string Name { get; set; }
        object Value { get; set; }
        object? Target { get; set; }
        string Label { get; set; }
        string ErrorMessage { get; set; }
        bool IsRequired { get; set; }
        bool AutoBind { get; set; }
        bool AutoUpdate { get; set; }
        bool SetValue<T>(T value);
        T GetValue<T>();
        /// <summary>
        /// Gets or sets the delegate used to retrieve the value for a given object instance.
        /// </summary>
        /// <remarks>The delegate should accept an object representing the instance and return the
        /// corresponding value. If set to null, value retrieval may not be possible. This property is typically used to
        /// customize how values are accessed or extracted from objects at runtime.</remarks>
        Func<object?> ValueGetter { get; set; }
        /// <summary>
        /// Try to set value, and return true if successful, false if validation failed. 
        /// If not set, the field will be set directly without validation.
        /// </summary>
        Func<object?, bool> ValueSetter { get; set; }
    }
}
