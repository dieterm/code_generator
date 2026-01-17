using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Workspaces.Datasources.Excel.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Mysql.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.PostgreSql.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.SqlServer.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views
{
    public partial class WorkspaceDetailsView : UserControl, IView<ArtifactDetailsViewModel>
    {
        private ArtifactDetailsViewModel? _viewModel;
        public WorkspaceDetailsView()
        {
            InitializeComponent();
            Disposed += WorkspaceDetailsView_Disposed;
        }

        private void WorkspaceDetailsView_Disposed(object? sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }
        }

        public void BindViewModel(ArtifactDetailsViewModel viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }

            ShowDetailsControl();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ArtifactDetailsViewModel.DetailsViewModel))
            {
                ShowDetailsControl();
            }
        }

        private void ShowDetailsControl()
        {
            Controls.Clear();
            UserControl? detailsControl = null;
            
            if (_viewModel?.DetailsViewModel is WorkspaceEditViewModel editViewModel)
            {
                detailsControl = new WorkspaceEditView();
                ((WorkspaceEditView)detailsControl).BindViewModel(editViewModel);
            }
            else if (_viewModel?.DetailsViewModel is MysqlDatasourceEditViewModel mysqlViewModel)
            {
                detailsControl = new MysqlDatasourceEditView();
                ((MysqlDatasourceEditView)detailsControl).BindViewModel(mysqlViewModel);
            }
            else if (_viewModel?.DetailsViewModel is SqlServerDatasourceEditViewModel sqlServerViewModel)
            {
                detailsControl = new SqlServerDatasourceEditView();
                ((SqlServerDatasourceEditView)detailsControl).BindViewModel(sqlServerViewModel);
            }
            else if (_viewModel?.DetailsViewModel is PostgreSqlDatasourceEditViewModel postgreSqlViewModel)
            {
                detailsControl = new PostgreSqlDatasourceEditView();
                ((PostgreSqlDatasourceEditView)detailsControl).BindViewModel(postgreSqlViewModel);
            }
            else if (_viewModel?.DetailsViewModel is ExcelDatasourceEditViewModel excelViewModel)
            {
                detailsControl = new ExcelDatasourceEditView();
                ((ExcelDatasourceEditView)detailsControl).BindViewModel(excelViewModel);
            }
            else if (_viewModel?.DetailsViewModel is ColumnEditViewModel columnViewModel)
            {
                detailsControl = new ColumnEditView();
                ((ColumnEditView)detailsControl).BindViewModel(columnViewModel);
            }
            else if (_viewModel?.DetailsViewModel is IndexEditViewModel indexViewModel)
            {
                detailsControl = new IndexEditView();
                ((IndexEditView)detailsControl).BindViewModel(indexViewModel);
            }
            else if (_viewModel?.DetailsViewModel is TableEditViewModel tableViewModel)
            {
                detailsControl = new TableEditView();
                ((TableEditView)detailsControl).BindViewModel(tableViewModel);
            }
            
            if (detailsControl != null)
            {
                Controls.Add(detailsControl);
                detailsControl.Dock = DockStyle.Fill;
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((ArtifactDetailsViewModel)(object)viewModel);
        }
    }
}
