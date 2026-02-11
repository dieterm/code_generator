using CodeGenerator.Shared.Models;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.UserControls.ViewModels
{
    public class FieldCollectionModel : ViewModelBase
    {
        public ObservableCollection<FieldViewModelBase> FieldModels { get; }

        public FieldCollectionModel()
        {
            FieldModels = new ObservableCollection<FieldViewModelBase>();
        }

        public FieldCollectionModel(IEnumerable<FieldViewModelBase> fieldModels)
        {
            FieldModels = new ObservableCollection<FieldViewModelBase>(fieldModels);
        }
        private readonly Dictionary<FieldViewModelBase, PropertyChangedEventHandler> _handlers = new Dictionary<FieldViewModelBase, PropertyChangedEventHandler>();


        public void BindObject(object obj)
        {
            // Unsubscribe previous handlers
            UnbindObject(obj);

            if (obj == null)
            {
                return;
            }

            foreach (var fieldModel in FieldModels)
            {
                var property = obj.GetType().GetProperty(fieldModel.Name);
                if (property != null)
                {
                    fieldModel.Value = property.GetValue(obj);
                    PropertyChangedEventHandler handler = (s, e) =>
                    {
                        if (e.PropertyName == nameof(FieldViewModelBase.Value))
                        {
                            property.SetValue(obj, fieldModel.Value);
                        }
                    };
                    _handlers[fieldModel] = handler;
                    fieldModel.PropertyChanged += handler;
                }
            }
        }

        public void UnbindObject(object obj)
        {
            foreach (var kvp in _handlers)
            {
                kvp.Key.PropertyChanged -= kvp.Value;
            }
            _handlers.Clear();
        }

        public void GenerateFieldsFromType(Type type, Dictionary<string, Action<FieldViewModelBase>>? fieldInitializer = null)
        {
            FieldModels.Clear();
            foreach (var prop in type.GetProperties())
            {
                var fieldModel = CreateFieldModelForType(prop.PropertyType);
                if (fieldModel != null)
                {
                    fieldModel.Name = prop.Name;
                    
                    if(fieldInitializer != null && fieldInitializer.ContainsKey(prop.Name))
                        fieldInitializer[prop.Name]?.Invoke(fieldModel);

                    FieldModels.Add(fieldModel);
                }
            }
        }

        protected virtual FieldViewModelBase? CreateFieldModelForType(Type propertyType)
        {
            if (propertyType == typeof(string))
                return new SingleLineTextFieldModel();
            if (propertyType == typeof(int) || propertyType == typeof(long))
                return new IntegerFieldModel();
            if (propertyType == typeof(bool))
                return new BooleanFieldModel();
            if(propertyType == typeof(DateOnly))
                return new DateOnlyFieldModel();
            if(propertyType == typeof(Denomination)) 
                return new DenominationFieldModel();
            if (propertyType == typeof(List<string>))
                return new StringListFieldModel();
            if(propertyType==typeof(Dictionary<string, string>))
                return new StringDictionaryFieldModel();
            if (typeof(DateTime).IsAssignableFrom(propertyType))
                return new DateOnlyFieldModel();
            if (propertyType.IsEnum) { 
                var enumComobobox = new ComboboxFieldModel();
                // Populate the combobox with enum values
                var enumValues = Enum.GetValues(propertyType).Cast<object>();
                foreach (var value in enumValues)
                {
                    enumComobobox.Items.Add(new ComboboxItem { Value = value, DisplayName = value.ToString() });
                }
                return enumComobobox;
            }
            Debug.WriteLine($"FieldCollectionModel: Unsupported property type: {propertyType.FullName}");
            return null;
        }
    }
}
