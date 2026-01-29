using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Application.ViewModels.Workspace.Domains
{
    /// <summary>
    /// ViewModel for editing EntityEditViewFieldArtifact properties
    /// </summary>
    public class EntityEditViewFieldEditViewModel : ViewModelBase
    {
        private EntityEditViewFieldArtifact? _field;
        private bool _isLoading;

        public EntityEditViewFieldEditViewModel()
        {
            PropertyNameField = new SingleLineTextFieldModel { Label = "Property Name", Name = nameof(EntityEditViewFieldArtifact.PropertyName) };
            ControlTypeField = new ComboboxFieldModel { Label = "Control Type", Name = nameof(EntityEditViewFieldArtifact.ControlType) };
            LabelField = new SingleLineTextFieldModel { Label = "Label", Name = nameof(EntityEditViewFieldArtifact.Label) };
            TooltipField = new SingleLineTextFieldModel { Label = "Tooltip", Name = nameof(EntityEditViewFieldArtifact.Tooltip) };
            PlaceholderField = new SingleLineTextFieldModel { Label = "Placeholder", Name = nameof(EntityEditViewFieldArtifact.Placeholder) };
            IsReadOnlyField = new BooleanFieldModel { Label = "Read Only", Name = nameof(EntityEditViewFieldArtifact.IsReadOnly) };
            IsRequiredField = new BooleanFieldModel { Label = "Required", Name = nameof(EntityEditViewFieldArtifact.IsRequired) };
            DisplayOrderField = new IntegerFieldModel { Label = "Display Order", Name = nameof(EntityEditViewFieldArtifact.DisplayOrder), Minimum = 0, Maximum = 1000 };

            // Initialize control type options
            foreach (var controlType in Enum.GetValues<DataFieldControlType>())
            {
                ControlTypeField.Items.Add(new ComboboxItem 
                { 
                    DisplayName = controlType.ToString(), 
                    Value = controlType 
                });
            }

            PropertyNameField.PropertyChanged += OnFieldChanged;
            ControlTypeField.PropertyChanged += OnControlTypeFieldChanged;
            LabelField.PropertyChanged += OnFieldChanged;
            TooltipField.PropertyChanged += OnFieldChanged;
            PlaceholderField.PropertyChanged += OnFieldChanged;
            IsReadOnlyField.PropertyChanged += OnFieldChanged;
            IsRequiredField.PropertyChanged += OnFieldChanged;
            DisplayOrderField.PropertyChanged += OnFieldChanged;
        }

        public EntityEditViewFieldArtifact? Field
        {
            get => _field;
            set
            {
                if (_field != null)
                {
                    _field.PropertyChanged -= Field_PropertyChanged;
                }
                if (SetProperty(ref _field, value))
                {
                    LoadFromField();
                    if (_field != null)
                    {
                        _field.PropertyChanged += Field_PropertyChanged;
                    }
                }
            }
        }

        private void Field_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EntityEditViewFieldArtifact.PropertyName))
            {
                PropertyNameField.Value = _field?.PropertyName;
            }
        }

        public SingleLineTextFieldModel PropertyNameField { get; }
        public ComboboxFieldModel ControlTypeField { get; }
        public SingleLineTextFieldModel LabelField { get; }
        public SingleLineTextFieldModel TooltipField { get; }
        public SingleLineTextFieldModel PlaceholderField { get; }
        public BooleanFieldModel IsReadOnlyField { get; }
        public BooleanFieldModel IsRequiredField { get; }
        public IntegerFieldModel DisplayOrderField { get; }

        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadFromField()
        {
            if (_field == null) return;

            _isLoading = true;
            try
            {
                PropertyNameField.Value = _field.PropertyName;
                
                var selectedControlType = ControlTypeField.Items
                    .FirstOrDefault(i => i.Value is DataFieldControlType ct && ct == _field.ControlType);
                ControlTypeField.SelectedItem = selectedControlType;

                LabelField.Value = _field.Label;
                TooltipField.Value = _field.Tooltip;
                PlaceholderField.Value = _field.Placeholder;
                IsReadOnlyField.Value = _field.IsReadOnly;
                IsRequiredField.Value = _field.IsRequired;
                DisplayOrderField.Value = _field.DisplayOrder;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _field == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToField();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_field, field.Name, field.Value));
            }
        }

        private void OnControlTypeFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _field == null) return;

            if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem))
            {
                if (ControlTypeField.SelectedItem?.Value is DataFieldControlType controlType)
                {
                    _field.ControlType = controlType;
                    ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_field, nameof(EntityEditViewFieldArtifact.ControlType), controlType));
                }
            }
        }

        private void SaveToField()
        {
            if (_field == null) return;
            _field.PropertyName = !string.IsNullOrWhiteSpace(PropertyNameField.Value as string) ? PropertyNameField.Value as string : "Property";
            _field.Label = LabelField.Value as string;
            _field.Tooltip = TooltipField.Value as string;
            _field.Placeholder = PlaceholderField.Value as string;
            _field.IsReadOnly = IsReadOnlyField.Value is bool isReadOnly && isReadOnly;
            _field.IsRequired = IsRequiredField.Value is bool isRequired && isRequired;
            _field.DisplayOrder = DisplayOrderField.Value is int displayOrder ? displayOrder : 0;
        }
    }
}
