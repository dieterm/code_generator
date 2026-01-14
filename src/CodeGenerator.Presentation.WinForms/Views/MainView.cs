using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Settings.ViewModels;
using CodeGenerator.Core.Settings.Views;
using CodeGenerator.Presentation.WinForms.Resources;
using CodeGenerator.Presentation.WinForms.ViewModels;
using CodeGenerator.Presentation.WinForms.Views;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using CodeGenerator.Shared.ExtensionMethods;
using Syncfusion.Windows.Forms.Tools;
using System.ComponentModel;
using CodeGenerator.Core.Artifacts.FileSystem;
using System.Diagnostics;


namespace CodeGenerator.Presentation.WinForms
{
    public partial class MainView : RibbonForm, IView<MainViewModel>, IWindowManagerService
    {
        private MainViewModel? _mainViewModel;

        public MainView()
        {
            InitializeComponent();

            if (DesignMode) return;

            dockingManager.DockVisibilityChanged += (s, e) =>
            {
                if (e.Control is GenerationResultTreeView treeView)
                {
                    if (!treeView.Visible)
                    {
                        treeView.Dispose();
                    }
                } else if(e.Control is DomainSchemaTreeView schemaTreeView)
                {
                    if (!schemaTreeView.Visible)
                    {
                        schemaTreeView.Dispose();
                    }
                }
            };
        }

        public void BindViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            // Render Ribbon
            ServiceProviderHolder.GetRequiredService<IRibbonRenderer>()
                .SetResourceManager(LucideIcons__000000.ResourceManager) // Image source
                .Render(_mainViewModel.RibbonViewModel, ribbonControl);

            // Bind New Command
            btnNew.Click += (s, e) => _mainViewModel.NewCommand.Execute(null);
            
            // Bind Open Command
            btnOpen.Click += (s, e) => _mainViewModel.OpenCommand.Execute(null);
            
            // Bind Save Command
            btnSave.Click += (s, e) => _mainViewModel.SaveCommand.Execute(null);
            
            // Bind Generate Command
            //btnGenerate.Click += (s, e) => _mainViewModel.GenerateCommand.Execute(null);
            
            // Bind Exit Command
            btnExit.Click += (s, e) => _mainViewModel.ExitCommand.Execute(null);

            // Handle FormClosing event
            FormClosing += OnFormClosing;

            // Bind Status Label
            _mainViewModel.PropertyChanged += OnMainViewModelPropertyChanged;
            
        }

        private void OnMainViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(InvokeRequired)
            {
                Invoke(new System.Action(() => OnMainViewModelPropertyChanged(sender, e)));
                return;
            }
            if (_mainViewModel == null) throw new InvalidOperationException("MainViewModel is not bound.");
            
            if (e.PropertyName == nameof(MainViewModel.StatusLabel))
            {
                lblStatus.Text = _mainViewModel.StatusLabel;
            }
            if(e.PropertyName == nameof(MainViewModel.ProgressLabel))
            {
                lblProgress.Text = _mainViewModel.ProgressLabel;
            }
            if(e.PropertyName == nameof(MainViewModel.ProgressValue))
            {
                if (_mainViewModel.ProgressValue == null)
                {
                    pbProgress.Style = ProgressBarStyle.Marquee;
                    return;
                } else { 
                    pbProgress.Style = ProgressBarStyle.Continuous;
                    pbProgress.Value = _mainViewModel.ProgressValue.Value;
                }
            }
            System.Windows.Forms.Application.DoEvents();
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!_mainViewModel.IsClosing)
            {
                _mainViewModel.ExitCommand.Execute(null);
                e.Cancel = true;
            } else { 
                e.Cancel = !_mainViewModel.IsClosing;
            }
        }
        #region IView IWindowManagerService
        private DomainSchemaTreeView? _domainSchemaTreeView;
        public void ShowDomainSchemaTreeView(DomainSchemaTreeViewModel treeViewModel)
        {
            if (_domainSchemaTreeView == null || _domainSchemaTreeView.IsDisposed)
            {
                _domainSchemaTreeView = new DomainSchemaTreeView();
                
                dockingManager.DockControl(_domainSchemaTreeView, this, DockingStyle.Left, 4);
                dockingManager.SetEnableDocking(_domainSchemaTreeView, true);
                dockingManager.SetControlSize(_domainSchemaTreeView, new Size(300, this.Height - 50));
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
            if(_generationResultTreeView == null || _generationResultTreeView.IsDisposed)
            {
                _generationResultTreeView = new GenerationResultTreeView();
               
                dockingManager.DockControl(_generationResultTreeView, this, DockingStyle.Right, 4);
                dockingManager.SetEnableDocking(_generationResultTreeView, true);
                dockingManager.SetControlSize(_generationResultTreeView, new Size(300, this.Height - 50));
                dockingManager.SetDockLabel(_generationResultTreeView, "Generation Result");
            } else
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
            /*
            //if(_artifactPreviewView == null || _artifactPreviewView.IsDisposed)
            //{
                _artifactPreviewView = new ArtifactPreviewView();
                //dockingManager.DockControl(_artifactPreviewView, this, DockingStyle.Tabbed, 4);
                dockingManager.DockAsDocument(_artifactPreviewView);
                //dockingManager.SetEnableDocking(_artifactPreviewView, true);
                //dockingManager.SetControlSize(_artifactPreviewView, new Size(300, this.Height - 50));
                dockingManager.SetDockLabel(_artifactPreviewView, viewModel.TabLabel?? "Artifact Preview");
           // }
            else
            {
                dockingManager.SetDockVisibility(_artifactPreviewView, true);
                //if (!_artifactPreviewView.Visible)
                //{
                //    dockingManager.DockControl(_artifactPreviewView, this, DockingStyle.Right, 4);
                //}
            }
            _artifactPreviewView.BindViewModel(viewModel);*/
        }
        private WorkspaceTreeView? _workspaceTreeView;
        public void ShowWorkspaceTreeView(WorkspaceTreeViewModel treeViewModel)
        {
            if(_workspaceTreeView==null || _workspaceTreeView.IsDisposed)
            {
                _workspaceTreeView = new WorkspaceTreeView();
                
                dockingManager.DockControl(_workspaceTreeView, this, DockingStyle.Left, 4);
                dockingManager.SetEnableDocking(_workspaceTreeView, true);
                dockingManager.SetControlSize(_workspaceTreeView, new Size(300, this.Height - 50));
                dockingManager.SetDockLabel(_workspaceTreeView, "Workspace");
            } else
            {
                dockingManager.SetDockVisibility(_workspaceTreeView, true);
                //if (!_workspaceTreeView.Visible)
                //{
                //    dockingManager.SetDockVisibility(_workspaceTreeView, true);
                //    //dockingManager.DockControl(_workspaceTreeView, this, DockingStyle.Left, 4);
                //}
            }

            _workspaceTreeView.ViewModel = treeViewModel;
        }

        private WorkspaceDetailsView? _workspaceDetailsView;
        public void ShowWorkspaceDetailsView(WorkspaceDetailsViewModel viewModel)
        {
            if(_workspaceDetailsView == null || _workspaceDetailsView.IsDisposed)
            {
                _workspaceDetailsView = new WorkspaceDetailsView();
                
                dockingManager.DockControl(_workspaceDetailsView, this, DockingStyle.Left, 4);
                dockingManager.SetEnableDocking(_workspaceDetailsView, true);
                dockingManager.SetControlSize(_workspaceDetailsView, new Size(300, this.Height - 50));
                dockingManager.SetDockLabel(_workspaceDetailsView, "Workspace Details");
            } else
            {
                dockingManager.SetDockVisibility(_workspaceDetailsView, true);
                //if (!_workspaceDetailsView.Visible)
                //{
                //    _workspaceDetailsView.Visible = true;
                //    dockingManager.SetDockVisibility(_workspaceDetailsView, true);
                //    //dockingManager.DockControl(_workspaceDetailsView, this, DockingStyle.Left, 4);
                //}
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
            settingsForm.ShowDialog(this);
        }
        #endregion

        #region IView Implementation
        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            this.BindViewModel((MainViewModel)(object)viewModel);
        }



        #endregion

    }
}
