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
        string Label { get; set; }
        string ErrorMessage { get; set; }
        bool IsRequired { get; set; }
        bool SetValue<T>(T value);
        T GetValue<T>();

    }
}
