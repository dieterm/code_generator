using CodeGenerator.Core.Models.Schema;
using CodeGenerator.WinForms.ViewModels;

namespace CodeGenerator.WinForms.Controls;

public partial class PropertyDefinitionEditView : UserControl
{
    private readonly PropertyDefinitionEditViewModel _viewModel;
    private bool _isLoading;

    public PropertyDefinitionEditView()
    {
        _viewModel = new PropertyDefinitionEditViewModel();
        InitializeComponent();
        SetupBindings();
    }

    public void LoadProperty(PropertyDefinition property, DomainSchema schema)
    {
        _isLoading = true;
        _viewModel.LoadFromProperty(property);
        _refComboBox.Items.Clear();
        _refComboBox.Items.AddRange(schema.Definitions?.Keys.Select(key => $"#/$defs/{key}").ToArray() ?? Array.Empty<string>());
        UpdateControlsFromViewModel();
        _isLoading = false;
    }
    private void UpdateRefVisibility()
    {
        _refComboBox.Visible = _viewModel.Type == "ref" || _viewModel.Type=="array";
        _refLabel.Visible = _viewModel.Type == "ref" || _viewModel.Type == "array";
    }
    private void SetupBindings()
    {
        _typeComboBox.SelectedIndexChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.Type = _typeComboBox.Text;
                _viewModel.UpdateProperty();

                UpdateRefVisibility();
            }
        };
        _refComboBox.SelectedIndexChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.Ref = _refComboBox.Text;
                _viewModel.UpdateProperty();
            }
        };
        _formatTextBox.TextChanged += (s, e) => 
        {
            if (!_isLoading) _viewModel.Format = _formatTextBox.Text;
        };
        _descriptionTextBox.TextChanged += (s, e) => 
        {
            if (!_isLoading) _viewModel.Description = _descriptionTextBox.Text;
        };
        _isNullableCheckBox.CheckedChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.IsNullable = _isNullableCheckBox.Checked;
                _viewModel.UpdateProperty();
            }
        };
        _minLengthNumeric.ValueChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.MinLength = (int)_minLengthNumeric.Value;
                _viewModel.UpdateProperty();
            }
        };
        _maxLengthNumeric.ValueChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.MaxLength = (int)_maxLengthNumeric.Value;
                _viewModel.UpdateProperty();
            }
        };

        _formatTextBox.Leave += (s, e) => 
        {
            if (!_isLoading) _viewModel.UpdateProperty();
        };
        _descriptionTextBox.Leave += (s, e) => 
        {
            if (!_isLoading) _viewModel.UpdateProperty();
        };
    }

    private void UpdateControlsFromViewModel()
    {
        _typeComboBox.Text = (string.IsNullOrWhiteSpace(_viewModel.Type) && !string.IsNullOrWhiteSpace(_viewModel.Ref)) ? "ref" : _viewModel.Type;
        _refComboBox.Text = _viewModel.Ref;
        UpdateRefVisibility();
        _formatTextBox.Text = _viewModel.Format;
        _descriptionTextBox.Text = _viewModel.Description;
        _isNullableCheckBox.Checked = _viewModel.IsNullable;
        _minLengthNumeric.Value = _viewModel.MinLength;
        _maxLengthNumeric.Value = _viewModel.MaxLength;
    }
}
