using CodeGenerator.Application.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator.Presentation.WinForms.Views
{
    public partial class WorkspaceDetailsView : UserControl, IView<WorkspaceDetailsViewModel>
    {
        private WorkspaceDetailsViewModel? _viewModel;
        public WorkspaceDetailsView()
        {
            InitializeComponent();
        }

        public void BindViewModel(WorkspaceDetailsViewModel viewModel)
        {
            _viewModel = viewModel;
            ShowDetailsControl();
        }

        private void ShowDetailsControl()
        {
            Controls.Clear();
            UserControl? detailsControl = null;
            if(_viewModel?.DetailsViewModel is WorkspaceEditViewModel editViewModel)
            {
                detailsControl = new WorkspaceEditView();
                ((WorkspaceEditView)detailsControl).BindViewModel(editViewModel);
            }
            if(detailsControl != null)
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
