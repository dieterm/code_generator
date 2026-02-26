using CodeGenerator.Core.Workspaces.Datasources.Mysql.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.Datasources.Mysql.Views
{
    /// <summary>
    /// View for editing MySQL datasource properties
    /// </summary>
    public partial class MysqlDatasourceEditView : UserControl, IView<MysqlDatasourceEditViewModel>
    {
        private MysqlDatasourceEditViewModel? _viewModel;

        public MysqlDatasourceEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(MysqlDatasourceEditViewModel viewModel)
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
            objectImportField.BindViewModel(_viewModel.ObjectImportField);
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
        {
            BindViewModel((MysqlDatasourceEditViewModel)(object)viewModel);
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
}
