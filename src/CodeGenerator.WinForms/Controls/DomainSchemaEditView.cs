using CodeGenerator.Core.Models.Schema;
using CodeGenerator.WinForms.ViewModels;

namespace CodeGenerator.WinForms.Controls;

public partial class DomainSchemaEditView : UserControl
{
    private readonly DomainSchemaEditViewModel _viewModel;
    private bool _isLoading;

    public DomainSchemaEditView()
    {
        _viewModel = new DomainSchemaEditViewModel();
        InitializeComponent();
        SetupBindings();
    }

    public void LoadSchema(DomainSchema schema)
    {
        _isLoading = true;
        _viewModel.LoadFromSchema(schema);
        UpdateControlsFromViewModel();
        _isLoading = false;
    }

    private void SetupBindings()
    {
        _schemaUrlTextBox.TextChanged += (s, e) => 
        {
            if (!_isLoading) _viewModel.SchemaUrl = _schemaUrlTextBox.Text;
        };
        _titleTextBox.TextChanged += (s, e) => 
        {
            if (!_isLoading) _viewModel.Title = _titleTextBox.Text;
        };
        _descriptionTextBox.TextChanged += (s, e) => 
        {
            if (!_isLoading) _viewModel.Description = _descriptionTextBox.Text;
        };

        _schemaUrlTextBox.Leave += (s, e) => 
        {
            if (!_isLoading) _viewModel.UpdateSchema();
        };
        _titleTextBox.Leave += (s, e) => 
        {
            if (!_isLoading) _viewModel.UpdateSchema();
        };
        _descriptionTextBox.Leave += (s, e) => 
        {
            if (!_isLoading) _viewModel.UpdateSchema();
        };
    }

    private void UpdateControlsFromViewModel()
    {
        _schemaUrlTextBox.Text = _viewModel.SchemaUrl;
        _titleTextBox.Text = _viewModel.Title;
        _descriptionTextBox.Text = _viewModel.Description;
    }
}
