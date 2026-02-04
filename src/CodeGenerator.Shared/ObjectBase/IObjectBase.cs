using CodeGenerator.Shared.Memento;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.ObjectBase
{
    public interface IObjectBase
    {
        IReadOnlyDictionary<string, object?> Properties { get; }

        event PropertyChangedEventHandler? PropertyChanged;
        event PropertyChangingEventHandler? PropertyChanging;

        T? GetValue<T>(string name, T? defaultValue = default);
        bool SetValue<T>(string name, T? value);
    }
}
