using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Workspaces.Datasources.Mysql.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views
{
    public partial class WorkspaceDetailsView : UserControl, IView<WorkspaceDetailsViewModel>
    {
        private WorkspaceDetailsViewModel? _viewModel;
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

        public void BindViewModel(WorkspaceDetailsViewModel viewModel)
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
            if (e.PropertyName == nameof(WorkspaceDetailsViewModel.DetailsViewModel))
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
            
            if (detailsControl != null)
            {
                Controls.Add(detailsControl);
                detailsControl.Dock = DockStyle.Fill;
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((WorkspaceDetailsViewModel)(object)viewModel);
        }
    }
}
