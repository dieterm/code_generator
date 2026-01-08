using CodeGenerator.Shared;
using CodeGenerator.Shared.Models;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeGenerator.UserControls.ViewModels
{
    public class ParameterizedStringFieldModel : FieldViewModelBase
    {
        private ParameterizedString _parameterizedString;

        public ParameterizedStringFieldModel()
            :this(null)
        {
            
        }

        public ParameterizedStringFieldModel(IEnumerable<ParameterizedStringParameter>? initialParameters)
        {
            if(initialParameters!=null)
                _parameters.AddRange(initialParameters);
            RefreshParameterizedString();
            this.PropertyChanged += ParameterizedStringFieldModel_PropertyChanged;
            InsertParameter = new RelayCommand((parameter) =>
            {
                // Default caret position at the end
                AddParameterToValue(parameter as ParameterizedStringParameter, SelectionStart);
            });
        }

        private void ParameterizedStringFieldModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            bool previewChanged = false;
            if(e.PropertyName== nameof(ParameterTemplate) || Parameters.Count != _parameterizedString.Parameters.Count)
            {
                RefreshParameterizedString();
                previewChanged = true;
            }

            if(e.PropertyName == nameof(Value))
            {
                _parameterizedString.Template = Value?.ToString() ?? string.Empty;
                previewChanged = true;
            }
            if(previewChanged)
            {
                OnPropertyChanged(nameof(Preview));
            }
        }

        public void RefreshParameterizedString()
        {
            _parameterizedString = new ParameterizedString(Value?.ToString() ?? string.Empty, ParameterTemplate);
            foreach (var parameter in _parameters)
            {
                _parameterizedString.Parameters.Add(parameter);
            }
        }
        public void AddParameterToValue(ParameterizedStringParameter parameter, int caretPosition)
        {
            var parameterPattern = ParameterTemplate.Replace(ParameterizedString.ParameterTemplatePlaceholder, parameter.Parameter);
            var currentValue = Value?.ToString() ?? string.Empty;
            // first check if caretPosition is valid
            if(caretPosition < 0 || caretPosition > currentValue.Length)
                caretPosition = currentValue.Length;
            var newValue = currentValue.Insert(caretPosition, parameterPattern);
            Value = newValue;
        }
        private List<ParameterizedStringParameter> _parameters = new List<ParameterizedStringParameter>();

        /// <summary>
        /// Gets or sets the list of available parameters
        /// </summary>
        public List<ParameterizedStringParameter> Parameters
        {
            get => _parameters;
        }

        private string _parameterTemplate = ParameterizedString.DefaultParameterFormat;
        public string ParameterTemplate
        {
            get { return _parameterTemplate; }
            set {
                SetProperty(ref _parameterTemplate, value);
            }
        }

        private int _selectionStart = 0;
        public int SelectionStart
        {
            get { return _selectionStart; }
            set { SetProperty(ref _selectionStart, value); }
        }


        public string Preview
        {
            get
            {
                return _parameterizedString.GetPreview();
            }
        }

        public ICommand? InsertParameter { get; set; }

        public void AddParameter(ParameterizedStringParameter parameter)
        {
            _parameters.Add(parameter);
            _parameterizedString.Parameters.Add(parameter);
            OnPropertyChanged(nameof(Parameters));
            OnPropertyChanged(nameof(Preview));
        }
        public void AddParameters(IEnumerable<ParameterizedStringParameter> parameters)
        {
            _parameters.AddRange(parameters);
            foreach (var parameter in parameters)
            {
                _parameterizedString.Parameters.Add(parameter);
            }
            OnPropertyChanged(nameof(Parameters));
            OnPropertyChanged(nameof(Preview));
        }
    }
}
