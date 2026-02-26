using CodeGenerator.Core.Workspaces.Datasources.SqlServer.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.SqlServer.Views;

/// <summary>
/// View for editing SQL Server datasource properties
/// </summary>
public partial class SqlServerDatasourceEditView : UserControl, IView<SqlServerDatasourceEditViewModel>
{
    private SqlServerDatasourceEditViewModel? _viewModel;

    public SqlServerDatasourceEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(SqlServerDatasourceEditViewModel viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            _viewModel.IntegratedSecurityField.PropertyChanged -= IntegratedSecurityField_PropertyChanged;
        }

        _viewModel = viewModel;

        if (_viewModel == null) return;

        // Bind fields
        txtName.BindViewModel(_viewModel.NameField);
        txtServer.BindViewModel(_viewModel.ServerField);
        txtDatabase.BindViewModel(_viewModel.DatabaseField);
        chkIntegratedSecurity.BindViewModel(_viewModel.IntegratedSecurityField);
        txtUsername.BindViewModel(_viewModel.UsernameField);
        txtPassword.BindViewModel(_viewModel.PasswordField);
        chkTrustServerCertificate.BindViewModel(_viewModel.TrustServerCertificateField);
        objectImportField.BindViewModel(_viewModel.ObjectImportField);

        // Subscribe to events
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        _viewModel.IntegratedSecurityField.PropertyChanged += IntegratedSecurityField_PropertyChanged;

        UpdateCredentialsFieldsVisibility();
    }

    private void IntegratedSecurityField_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_viewModel.IntegratedSecurityField.Value))
        {
            UpdateCredentialsFieldsVisibility();
        }
    }

    private void UpdateCredentialsFieldsVisibility()
    {
        var useIntegratedSecurity = _viewModel?.IntegratedSecurityField.Value is bool integrated && integrated;
        txtUsername.Enabled = !useIntegratedSecurity;
        txtPassword.Enabled = !useIntegratedSecurity;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
    {
        BindViewModel((SqlServerDatasourceEditViewModel)(object)viewModel);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.IntegratedSecurityField.PropertyChanged -= IntegratedSecurityField_PropertyChanged;
            }

            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
