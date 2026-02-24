using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Workspaces.Datasources.Mysql.ViewModels;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CodeGenerator.Presentation.WinForms.Views
{
    public partial class TemplateDetailsView : UserControl, IView<WorkspaceArtifactDetailsViewModel>
    {
        private WorkspaceArtifactDetailsViewModel? _viewModel;
        public TemplateDetailsView()
        {
            InitializeComponent();
            Disposed += TemplateDetailsView_Disposed;
        }

        private void TemplateDetailsView_Disposed(object? sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }
        }

        public void BindViewModel(WorkspaceArtifactDetailsViewModel viewModel)
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
            if (e.PropertyName == nameof(WorkspaceArtifactDetailsViewModel.DetailsViewModel))
            {
                ShowDetailsControl();
            }
        }

        private void ShowDetailsControl()
        {
            Controls.Clear();

            if (_viewModel?.DetailsViewModel == null)
                return;

            var viewFactory = ServiceProviderHolder.GetRequiredService<IViewFactory>();
            var view = viewFactory.CreateView(_viewModel.DetailsViewModel);

            if (view is UserControl detailsControl)
            {
                Controls.Add(detailsControl);
                detailsControl.Dock = DockStyle.Fill;
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
        {
            BindViewModel((WorkspaceArtifactDetailsViewModel)(object)viewModel);
        }
    }
}
