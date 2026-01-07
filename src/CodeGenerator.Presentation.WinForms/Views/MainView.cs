using CodeGenerator.Application.Events.DomainSchema;
using CodeGenerator.Application.MessageBus;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Presentation.WinForms.Resources;
using CodeGenerator.Presentation.WinForms.ViewModels;
using CodeGenerator.Presentation.WinForms.Views;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.Shared.Views;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using System.ComponentModel;
using System.Windows.Forms;

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
            if(_mainViewModel == null) throw new InvalidOperationException("MainViewModel is not bound.");
            
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

        public void ShowDomainSchemaTreeView(DomainSchemaTreeViewModel treeViewModel)
        {
            var schemaTreeView = new DomainSchemaTreeView();
            schemaTreeView.BindViewModel(treeViewModel);
            dockingManager.DockControl(schemaTreeView, this, DockingStyle.Left, 4);
            dockingManager.SetEnableDocking(schemaTreeView, true);
            dockingManager.SetControlSize(schemaTreeView, new Size(300, this.Height - 50));
            dockingManager.SetDockLabel(schemaTreeView, "Domain Schema");
        }

        public void ShowGenerationTreeView(GenerationResultTreeViewModel treeViewModel)
        {
            var generationResultTreeView = new GenerationResultTreeView();
            generationResultTreeView.BindViewModel(treeViewModel);
            dockingManager.DockControl(generationResultTreeView, this, DockingStyle.Right, 4);
            dockingManager.SetEnableDocking(generationResultTreeView, true);
            dockingManager.SetControlSize(generationResultTreeView, new Size(300, this.Height - 50));
            dockingManager.SetDockLabel(generationResultTreeView, "Generation Result");

        }

        public void ShowArtifactPreview(IArtifact selectedArtifact)
        {
            // throw new NotImplementedException();
        }
    }
}
