using CodeGenerator.Core.Workspaces.Datasources.PostgreSql.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.PostgreSql.Views;

/// <summary>
/// View for editing PostgreSQL datasource properties
/// </summary>
public partial class PostgreSqlDatasourceEditView : UserControl, IView<PostgreSqlDatasourceEditViewModel>
{
    private PostgreSqlDatasourceEditViewModel? _viewModel;

    public PostgreSqlDatasourceEditView()
    {
        InitializeComponent();
    }

    public void BindViewModel(PostgreSqlDatasourceEditViewModel viewModel)
    {
        _viewModel = viewModel;

        if (_viewModel == null) return;

        // Bind fields
        txtName.BindViewModel(_viewModel.NameField);
        txtServer.BindViewModel(_viewModel.ServerField);
        txtPort.BindViewModel(_viewModel.PortField);
        txtDatabase.BindViewModel(_viewModel.DatabaseField);
        txtUsername.BindViewModel(_viewModel.UsernameField);
        txtPassword.BindViewModel(_viewModel.PasswordField);
        cboSslMode.BindViewModel(_viewModel.SslModeField);
        objectImportField.BindViewModel(_viewModel.ObjectImportField);
    }

    public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
    {
        BindViewModel((PostgreSqlDatasourceEditViewModel)(object)viewModel);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
