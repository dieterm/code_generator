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

using Syncfusion.Windows.Forms.Tools;
using System.ComponentModel;
using CodeGenerator.Core.Artifacts.FileSystem;
using System.Diagnostics;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Application.ViewModels.Template;


namespace CodeGenerator.Presentation.WinForms
{
    public partial class MainView : RibbonForm, IView<MainViewModel>
    {
        private MainViewModel? _mainViewModel;

        /// <summary>
        /// expose DockingManger for WindowManagerService
        /// </summary>
        public DockingManager DockingManager { get { return dockingManager; } }

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

        #region IView Implementation
        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            this.BindViewModel((MainViewModel)(object)viewModel);
        }



        #endregion

    }
}
