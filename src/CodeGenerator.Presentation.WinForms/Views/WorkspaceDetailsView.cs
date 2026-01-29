using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Application.ViewModels.Workspace.Domains;
using CodeGenerator.Core.Workspaces.Datasources.Csv.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.DotNetAssembly.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Excel.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Json.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Mysql.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.OpenApi.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.PostgreSql.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.SqlServer.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Xml.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Yaml.ViewModels;
using CodeGenerator.Core.Workspaces.ViewModels;
using CodeGenerator.Presentation.WinForms.Views.Domains;
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
            else if (_viewModel?.DetailsViewModel is CsvDatasourceEditViewModel csvViewModel)
            {
                detailsControl = new CsvDatasourceEditView();
                ((CsvDatasourceEditView)detailsControl).BindViewModel(csvViewModel);
            }
            else if (_viewModel?.DetailsViewModel is JsonDatasourceEditViewModel jsonViewModel)
            {
                detailsControl = new JsonDatasourceEditView();
                ((JsonDatasourceEditView)detailsControl).BindViewModel(jsonViewModel);
            }
            else if (_viewModel?.DetailsViewModel is XmlDatasourceEditViewModel xmlViewModel)
            {
                detailsControl = new XmlDatasourceEditView();
                ((XmlDatasourceEditView)detailsControl).BindViewModel(xmlViewModel);
            }
            else if (_viewModel?.DetailsViewModel is YamlDatasourceEditViewModel yamlViewModel)
            {
                detailsControl = new YamlDatasourceEditView();
                ((YamlDatasourceEditView)detailsControl).BindViewModel(yamlViewModel);
            }
            else if (_viewModel?.DetailsViewModel is DotNetAssemblyDatasourceEditViewModel dotNetAssemblyViewModel)
            {
                detailsControl = new DotNetAssemblyDatasourceEditView();
                ((DotNetAssemblyDatasourceEditView)detailsControl).BindViewModel(dotNetAssemblyViewModel);
            }
            else if (_viewModel?.DetailsViewModel is OpenApiDatasourceEditViewModel openApiViewModel)
            {
                detailsControl = new OpenApiDatasourceEditView();
                ((OpenApiDatasourceEditView)detailsControl).BindViewModel(openApiViewModel);
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
            else if (_viewModel?.DetailsViewModel is ForeignKeyEditViewModel foreignKeyViewModel)
            {
                detailsControl = new ForeignKeyEditView();
                ((ForeignKeyEditView)detailsControl).BindViewModel(foreignKeyViewModel);
            }
            else if (_viewModel?.DetailsViewModel is DomainEditViewModel domainViewModel)
            {
                detailsControl = new DomainEditView();
                ((DomainEditView)detailsControl).BindViewModel(domainViewModel);
            }
            else if (_viewModel?.DetailsViewModel is EntityEditViewModel entityViewModel)
            {
                detailsControl = new EntityEditView();
                ((EntityEditView)detailsControl).BindViewModel(entityViewModel);
            }
            else if (_viewModel?.DetailsViewModel is EntityStateEditViewModel entityStateViewModel)
            {
                detailsControl = new EntityStateEditView();
                ((EntityStateEditView)detailsControl).BindViewModel(entityStateViewModel);
            }
            else if (_viewModel?.DetailsViewModel is ValueTypeEditViewModel valueTypeViewModel)
            {
                detailsControl = new ValueTypeEditView();
                ((ValueTypeEditView)detailsControl).BindViewModel(valueTypeViewModel);
            }
            else if (_viewModel?.DetailsViewModel is PropertyEditViewModel propertyViewModel)
            {
                detailsControl = new PropertyEditView();
                ((PropertyEditView)detailsControl).BindViewModel(propertyViewModel);
            }
            else if (_viewModel?.DetailsViewModel is EntityRelationEditViewModel entityRelationViewModel)
            {
                detailsControl = new EntityRelationEditView();
                ((EntityRelationEditView)detailsControl).BindViewModel(entityRelationViewModel);
            }
            else if (_viewModel?.DetailsViewModel is TableEditViewModel tableViewModel)
            {
                detailsControl = new TableEditView();
                ((TableEditView)detailsControl).BindViewModel(tableViewModel);
            }
            else if (_viewModel?.DetailsViewModel is EntityEditViewEditViewModel entityEditViewViewModel)
            {
                detailsControl = new EntityEditViewEditView();
                ((EntityEditViewEditView)detailsControl).BindViewModel(entityEditViewViewModel);
            }
            else if (_viewModel?.DetailsViewModel is EntityEditViewFieldEditViewModel entityEditViewFieldViewModel)
            {
                detailsControl = new EntityEditViewFieldEditView();
                ((EntityEditViewFieldEditView)detailsControl).BindViewModel(entityEditViewFieldViewModel);
            }
            else if (_viewModel?.DetailsViewModel is EntityListViewEditViewModel entityListViewViewModel)
            {
                detailsControl = new EntityListViewEditView();
                ((EntityListViewEditView)detailsControl).BindViewModel(entityListViewViewModel);
            }
            else if (_viewModel?.DetailsViewModel is EntityListViewColumnEditViewModel entityListViewColumnViewModel)
            {
                detailsControl = new EntityListViewColumnEditView();
                ((EntityListViewColumnEditView)detailsControl).BindViewModel(entityListViewColumnViewModel);
            }
            else if (_viewModel?.DetailsViewModel is EntitySelectViewEditViewModel entitySelectViewViewModel)
            {
                detailsControl = new EntitySelectViewEditView();
                ((EntitySelectViewEditView)detailsControl).BindViewModel(entitySelectViewViewModel);
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
