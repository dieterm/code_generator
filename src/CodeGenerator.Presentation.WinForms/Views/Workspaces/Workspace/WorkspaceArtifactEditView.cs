using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Workspaces.ViewModels.Workspace;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.UserControls.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator.Presentation.WinForms.Views.Workspace
{
    public partial class WorkspaceArtifactEditView : UserControl, IView<WorkspaceEditViewModel>
    {
        private WorkspaceEditViewModel? _viewModel;
        public WorkspaceArtifactEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(WorkspaceEditViewModel viewModel)
        {
            _viewModel = viewModel;
            lblTitle.Text = $"{viewModel.ArtifactName} Details";
            tabFields.SuspendLayout();
            tabFields.TabPages.Clear();
            foreach (var tab in viewModel.Tabs)
            {
                var tabPage = new TabPage(tab.Text);
                var fieldCollection = new FieldCollection() { Dock = DockStyle.Fill };
                fieldCollection.BindViewModel(tab.FieldCollection);
                tabPage.Controls.Add(fieldCollection);
                tabFields.TabPages.Add(tabPage);
            }
            tabFields.ResumeLayout();
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((WorkspaceEditViewModel)(object)viewModel);
        }
    }
}
