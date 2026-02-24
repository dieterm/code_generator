using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.ViewModels
{
    public abstract class FieldViewModelBase : ViewModelBase, IFieldViewModel
    {
        private object? _target;
        private string? _name;
        private object? _value;
        private string? _label;
        private string? _errorMessage;
        private string? _tooltip;
        private bool _isRequired;

        protected FieldViewModelBase()
        {
            ValueGetter = DefaultValueGetter;
            ValueSetter = DefaultValueSetter;
        }

        private object? DefaultValueGetter()
        {
            return Target?.GetType()?.GetProperty(Name)?.GetValue(Target);
        }

        private bool DefaultValueSetter(object? value)
        {
            var propertyInfo = Target?.GetType()?.GetProperty(Name);
            propertyInfo?.SetValue(Target, value);
            return propertyInfo!=null;
        }
        private bool _autoBind = false;
        public bool AutoBind
        {
            get { return _autoBind; }
            set { SetProperty(ref _autoBind, value); }
        }
        private bool _autoUpdate = false;
        public bool AutoUpdate
        {
            get { return _autoUpdate; }
            set { SetProperty(ref _autoUpdate, value); }
        }

        /// <summary>
        /// Name of property that this field is bound to. 
        /// This is used by the view to determine which property of the target object to update when the value changes, and can also be used for validation purposes.
        /// </summary>
        public string Name { 
            get => _name; 
            set => SetProperty(ref _name, value);
        }
        public virtual object Value { 
            get => GetValue<object>();
            set { 
                if (SetValue(value) && AutoBind)
                {
                    ValueSetter(value);
                }
            }
        }
        public string? Label { 
            get => _label;
            set => SetProperty(ref _label, value);
        }

        /// <summary>
        /// The target object that this field is bound to. 
        /// This can be used by the view to determine which object to update when the value changes, and can also be used for validation purposes.
        /// </summary>
        public virtual object? Target
        {
            get => _target;
            set { 
                if(_target != value)
                {
                    if(_target is INotifyPropertyChanged oldNotifyPropertyChanged)
                    {
                        oldNotifyPropertyChanged.PropertyChanged -= ObserveTarget;
                    }
                }
                var changed = SetProperty(ref _target, value);
                if (changed)
                {
                    if(AutoBind)
                    {
                        Value = ValueGetter();
                    }
                    if (AutoUpdate) 
                    { 
                        if(_target is INotifyPropertyChanged notifyPropertyChanged)
                        {
                            notifyPropertyChanged.PropertyChanged += ObserveTarget;
                        }
                    }
                }
            
            }
        }

        private void ObserveTarget(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Name)
            {
                Value = ValueGetter();
            }
        }

        public string ErrorMessage { 
            get => _errorMessage; 
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsRequired { 
            get => _isRequired; 
            set => SetProperty(ref _isRequired, value);
        }

        public string? Tooltip { 
            get => _tooltip; 
            set => SetProperty(ref _tooltip, value);
        }
        public Func<object?> ValueGetter { get; set;}
        public Func<object?, bool> ValueSetter { get; set; }

        public virtual bool SetValue<T>(T value)
        {
            return SetProperty(ref _value, value, nameof(Value));
        }

        public virtual T GetValue<T>()
        {
            return (T)_value;
        }
    }
}
