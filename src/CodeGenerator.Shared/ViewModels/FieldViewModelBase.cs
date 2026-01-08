using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.ViewModels
{
    public abstract class FieldViewModelBase : ViewModelBase, IFieldViewModel
    {
        private string _name;
        private object _value;
        private string _label;
        private string _errorMessage;
        private bool _isRequired;
        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public object Value { get => GetValue<object>(); set => SetValue(value); }
        public string? Label { get => _label; set => SetProperty(ref _label, value); }
        public virtual bool SetValue<T>(T value)
        {
            return SetProperty(ref _value, value, nameof(Value));
        }
        public virtual T GetValue<T>()
        {
            return (T)_value;
        }
        public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

        public bool IsRequired { get => _isRequired; set => SetProperty(ref _isRequired, value); }

    }
}
