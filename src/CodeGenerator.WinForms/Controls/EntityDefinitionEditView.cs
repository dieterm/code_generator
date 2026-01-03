using CodeGenerator.Core.Models.Schema;
using CodeGenerator.WinForms.ViewModels;

namespace CodeGenerator.WinForms.Controls;

public partial class EntityDefinitionEditView : UserControl
{
    private readonly EntityDefinitionEditViewModel _viewModel;
    private bool _isLoading;

    public EntityDefinitionEditView()
    {
        _viewModel = new EntityDefinitionEditViewModel();
        InitializeComponent();
        SetupBindings();
    }

    public void LoadEntity(EntityDefinition entity)
    {
        _isLoading = true;
        _viewModel.LoadFromEntity(entity);
        UpdateControlsFromViewModel();
        _isLoading = false;
    }

    private void SetupBindings()
    {
        _titleTextBox.TextChanged += (s, e) => 
        {
            if (!_isLoading) _viewModel.Title = _titleTextBox.Text;
        };
        _descriptionTextBox.TextChanged += (s, e) => 
        {
            if (!_isLoading) _viewModel.Description = _descriptionTextBox.Text;
        };
        _isValueObjectCheckBox.CheckedChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.IsValueObject = _isValueObjectCheckBox.Checked;
                _viewModel.UpdateEntity();
            }
        };
        _isAggregateRootCheckBox.CheckedChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.IsAggregateRoot = _isAggregateRootCheckBox.Checked;
                _viewModel.UpdateEntity();
            }
        };
        _isHierarchicalCheckBox.CheckedChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.IsHierarchical = _isHierarchicalCheckBox.Checked;
                _viewModel.UpdateEntity();
            }
        };

        _titleTextBox.Leave += (s, e) => 
        {
            if (!_isLoading) _viewModel.UpdateEntity();
        };
        _descriptionTextBox.Leave += (s, e) => 
        {
            if (!_isLoading) _viewModel.UpdateEntity();
        };
    }

    private void UpdateControlsFromViewModel()
    {
        txtKey.Text = _viewModel.Key;
        _titleTextBox.Text = _viewModel.Title;
        _descriptionTextBox.Text = _viewModel.Description;
        _isValueObjectCheckBox.Checked = _viewModel.IsValueObject;
        _isAggregateRootCheckBox.Checked = _viewModel.IsAggregateRoot;
        _isHierarchicalCheckBox.Checked = _viewModel.IsHierarchical;
    }
}
