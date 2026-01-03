using CodeGenerator.Core.Models.Schema;
using CodeGenerator.WinForms.ViewModels;

namespace CodeGenerator.WinForms.Controls;

public partial class DatabaseMetadataEditView : UserControl
{
    private readonly DatabaseMetadataEditViewModel _viewModel;
    private bool _isLoading;

    public DatabaseMetadataEditView()
    {
        _viewModel = new DatabaseMetadataEditViewModel();
        InitializeComponent();
        SetupBindings();
    }

    public void LoadMetadata(DatabaseMetadata metadata)
    {
        _isLoading = true;
        _viewModel.LoadFromMetadata(metadata);
        UpdateControlsFromViewModel();
        _isLoading = false;
    }

    private void SetupBindings()
    {
        _databaseNameTextBox.TextChanged += (s, e) => 
        {
            if (!_isLoading) _viewModel.DatabaseName = _databaseNameTextBox.Text;
        };
        _schemaTextBox.TextChanged += (s, e) => 
        {
            if (!_isLoading) _viewModel.Schema = _schemaTextBox.Text;
        };
        _providerComboBox.SelectedIndexChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.Provider = _providerComboBox.Text;
                _viewModel.UpdateMetadata();
            }
        };
        _connectionStringNameTextBox.TextChanged += (s, e) => 
        {
            if (!_isLoading) _viewModel.ConnectionStringName = _connectionStringNameTextBox.Text;
        };
        _useMigrationsCheckBox.CheckedChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.UseMigrations = _useMigrationsCheckBox.Checked;
                _viewModel.UpdateMetadata();
            }
        };
        _seedDataCheckBox.CheckedChanged += (s, e) => 
        {
            if (!_isLoading)
            {
                _viewModel.SeedData = _seedDataCheckBox.Checked;
                _viewModel.UpdateMetadata();
            }
        };

        _databaseNameTextBox.Leave += (s, e) => 
        {
            if (!_isLoading) _viewModel.UpdateMetadata();
        };
        _schemaTextBox.Leave += (s, e) => 
        {
            if (!_isLoading) _viewModel.UpdateMetadata();
        };
        _connectionStringNameTextBox.Leave += (s, e) => 
        {
            if (!_isLoading) _viewModel.UpdateMetadata();
        };
    }

    private void UpdateControlsFromViewModel()
    {
        _databaseNameTextBox.Text = _viewModel.DatabaseName;
        _schemaTextBox.Text = _viewModel.Schema;
        _providerComboBox.Text = _viewModel.Provider;
        _connectionStringNameTextBox.Text = _viewModel.ConnectionStringName;
        _useMigrationsCheckBox.Checked = _viewModel.UseMigrations;
        _seedDataCheckBox.Checked = _viewModel.SeedData;
    }
}
