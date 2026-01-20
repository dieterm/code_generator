using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Settings.ViewModels;
using CodeGenerator.Core.Settings.Views;
using CodeGenerator.Shared.ExtensionMethods;
using CodeGenerator.Presentation.WinForms.Views;
using Syncfusion.Windows.Forms.Tools;


namespace CodeGenerator.Presentation.WinForms.Services
{
    public class WindowManagerService : IWindowManagerService
    {       
        private readonly DockingManager dockingManager;
        private readonly MainView mainView;

        public WindowManagerService(MainView mainView)
        {
            this.mainView = mainView ?? throw new ArgumentNullException(nameof(mainView));
            this.dockingManager = mainView.DockingManager;
        }

        #region IView IWindowManagerService
        private DomainSchemaTreeView? _domainSchemaTreeView;
        public void ShowDomainSchemaTreeView(DomainSchemaTreeViewModel treeViewModel)
        {
            if (_domainSchemaTreeView == null || _domainSchemaTreeView.IsDisposed)
            {
                _domainSchemaTreeView = new DomainSchemaTreeView();

                dockingManager.DockControl(_domainSchemaTreeView, mainView, DockingStyle.Left, 4);
                dockingManager.SetEnableDocking(_domainSchemaTreeView, true);
                dockingManager.SetControlSize(_domainSchemaTreeView, new Size(300, mainView.Height - 50));
                dockingManager.SetDockLabel(_domainSchemaTreeView, "Domain Schema");
            }
            else
            {
                dockingManager.SetDockVisibility(_domainSchemaTreeView, true);
                //if (!_domainSchemaTreeView.Visible)
                //{
                //    dockingManager.DockControl(_domainSchemaTreeView, this, DockingStyle.Left, 4);
                //}
            }
            _domainSchemaTreeView.BindViewModel(treeViewModel);
        }

        private GenerationResultTreeView? _generationResultTreeView;
        public void ShowGenerationTreeView(GenerationResultTreeViewModel treeViewModel)
        {
            if (_generationResultTreeView == null || _generationResultTreeView.IsDisposed)
            {
                _generationResultTreeView = new GenerationResultTreeView();

                dockingManager.DockControl(_generationResultTreeView, mainView, DockingStyle.Right, 4);
                dockingManager.SetEnableDocking(_generationResultTreeView, true);
                dockingManager.SetControlSize(_generationResultTreeView, new Size(300, mainView.Height - 50));
                dockingManager.SetDockLabel(_generationResultTreeView, "Generation Result");
            }
            else
            {
                dockingManager.SetDockVisibility(_generationResultTreeView, true);
                //if (!_generationResultTreeView.Visible)
                //{
                //    dockingManager.DockControl(_generationResultTreeView, this, DockingStyle.Right, 4);
                //}
            }

            _generationResultTreeView.BindViewModel(treeViewModel);
        }

        private readonly List<ArtifactPreviewView> _artifactPreviewViews = new List<ArtifactPreviewView>();
        public void ShowArtifactPreview(ArtifactPreviewViewModel viewModel)
        {
            var artifactPreviewView = new ArtifactPreviewView();
            _artifactPreviewViews.Add(artifactPreviewView);
            artifactPreviewView.BindViewModel(viewModel);
            dockingManager.DockAsDocument(artifactPreviewView);
            dockingManager.SetDockLabel(artifactPreviewView, viewModel.TabLabel ?? "Artifact Preview");
           
        }

        private readonly List<ScribanTemplateEditView> _scribanTemplateEditViews = new List<ScribanTemplateEditView>();
        public void ShowScribanTemplateEditView(ScribanTemplateEditViewModel viewModel)
        {
            var scribanTemplateEditView = new ScribanTemplateEditView();
            _scribanTemplateEditViews.Add(scribanTemplateEditView);
            scribanTemplateEditView.BindViewModel(viewModel);
            dockingManager.DockAsDocument(scribanTemplateEditView);
            dockingManager.SetDockLabel(scribanTemplateEditView, viewModel.TabLabel ?? "Template Editor");
        }

        private ArtifactTreeView? _workspaceTreeView;
        public void ShowWorkspaceTreeView(WorkspaceTreeViewModel treeViewModel)
        {
            if (_workspaceTreeView == null || _workspaceTreeView.IsDisposed)
            {
                _workspaceTreeView = new ArtifactTreeView();

                dockingManager.DockControl(_workspaceTreeView, mainView, DockingStyle.Left, 4);
                dockingManager.SetEnableDocking(_workspaceTreeView, true);
                dockingManager.SetControlSize(_workspaceTreeView, new Size(300, mainView.Height - 50));
                dockingManager.SetDockLabel(_workspaceTreeView, "Workspace");
            }
            else
            {
                dockingManager.SetDockVisibility(_workspaceTreeView, true);
            }

            _workspaceTreeView.ViewModel = treeViewModel;
        }

        private WorkspaceDetailsView? _workspaceDetailsView;
        public void ShowWorkspaceDetailsView(ArtifactDetailsViewModel viewModel)
        {
            if (_workspaceDetailsView == null || _workspaceDetailsView.IsDisposed)
            {
                _workspaceDetailsView = new WorkspaceDetailsView();

                dockingManager.DockControl(_workspaceDetailsView, mainView, DockingStyle.Left, 4);
                dockingManager.SetEnableDocking(_workspaceDetailsView, true);
                dockingManager.SetControlSize(_workspaceDetailsView, new Size(500, mainView.Height - 50));
                dockingManager.SetDockLabel(_workspaceDetailsView, "Workspace Details");
            }
            else
            {
                dockingManager.SetDockVisibility(_workspaceDetailsView, true);
            }

            _workspaceDetailsView.BindViewModel(viewModel);
        }


        public void ShowSettingsWindow(SettingsViewModel settingsViewModel)
        {
            var settingsView = new SettingsView();
            settingsView.BindViewModel(settingsViewModel);
            // show usercontrol as dialog
            // add 10px padding around the usercontrol
            var padding = 10;
            var settingsForm = new Form()
            {
                Text = "Settings",
                Width = 800,
                Height = 450,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false,
                Icon = Resources.LucideIcons__000000.settings.ToIcon()
            };
            //settingsView.Dock = DockStyle.Fill;
            settingsView.Location = new Point(padding, padding);
            settingsView.Size = new Size(settingsForm.ClientSize.Width - 2 * padding, settingsForm.ClientSize.Height - 2 * padding);
            settingsView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            settingsForm.Controls.Add(settingsView);
            settingsForm.ShowDialog(mainView);
        }

        private TemplateTreeView? _templateTreeView;
        public void ShowTemplateTreeView(TemplateTreeViewModel treeViewModel)
        {
            if (_templateTreeView == null || _templateTreeView.IsDisposed)
            {
                _templateTreeView = new TemplateTreeView();

                dockingManager.DockControl(_templateTreeView, mainView, DockingStyle.Left, 4);
                dockingManager.SetEnableDocking(_templateTreeView, true);
                dockingManager.SetControlSize(_templateTreeView, new Size(300, mainView.Height - 50));
                dockingManager.SetDockLabel(_templateTreeView, "Templates");
            }
            else
            {
                dockingManager.SetDockVisibility(_templateTreeView, true);
            }

            _templateTreeView.ViewModel = treeViewModel;
        }

        //private TemplateParametersView? _templateParametersView;
        //public void ShowTemplateParametersView(TemplateParametersViewModel viewModel)
        //{
        //    if (_templateParametersView == null || _templateParametersView.IsDisposed)
        //    {
        //        _templateParametersView = new TemplateParametersView();

        //        dockingManager.DockControl(_templateParametersView, mainView, DockingStyle.Left, 4);
        //        dockingManager.SetEnableDocking(_templateParametersView, true);
        //        dockingManager.SetControlSize(_templateParametersView, new Size(300, mainView.Height - 50));
        //        dockingManager.SetDockLabel(_templateParametersView, "Template Parameters");
        //    }
        //    else
        //    {
        //        dockingManager.SetDockVisibility(_templateParametersView, true);
        //    }

        //    _templateParametersView.BindViewModel(viewModel);
        //}

        private TemplateDetailsView? _templateDetailsView;
        public void ShowTemplateDetailsView(ArtifactDetailsViewModel artifactDetailsViewModel)
        {
            if (_templateDetailsView == null || _templateDetailsView.IsDisposed)
            {
                _templateDetailsView = new TemplateDetailsView();

                dockingManager.DockControl(_templateDetailsView, mainView, DockingStyle.Left, 4);
                dockingManager.SetEnableDocking(_templateDetailsView, true);
                dockingManager.SetControlSize(_templateDetailsView, new Size(500, mainView.Height - 50));
                dockingManager.SetDockLabel(_templateDetailsView, "Template Details");
            }
            else
            {
                dockingManager.SetDockVisibility(_templateDetailsView, true);
            }

            _templateDetailsView.BindViewModel(artifactDetailsViewModel);
        }
        #endregion

    }
}
