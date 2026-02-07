using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Settings.Application;
using CodeGenerator.Core.Settings.ViewModels;
using CodeGenerator.Core.Settings.Views;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Presentation.WinForms.Resources;
using CodeGenerator.Presentation.WinForms.ViewModels;
using CodeGenerator.Presentation.WinForms.Views;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using Syncfusion.Data.Extensions;
using Syncfusion.Windows.Forms.Tools;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;


namespace CodeGenerator.Presentation.WinForms.Views.Application
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
                }
            };
        }

        public void BindViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            // Render Ribbon
            ServiceProviderHolder.GetRequiredService<IRibbonRenderer>()
                .AddResourceManager(LucideIcons__000000.ResourceManager) // Image source
                .AddResourceManager(DotNetIcons.ResourceManager)
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
            if (InvokeRequired)
            {
                Invoke(new System.Action(() => OnMainViewModelPropertyChanged(sender, e)));
                return;
            }
            if (_mainViewModel == null) throw new InvalidOperationException("MainViewModel is not bound.");

            if (e.PropertyName == nameof(MainViewModel.StatusLabel))
            {
                lblStatus.Text = _mainViewModel.StatusLabel;
            }
            if (e.PropertyName == nameof(MainViewModel.ProgressLabel))
            {
                lblProgress.Text = _mainViewModel.ProgressLabel;
            }
            if (e.PropertyName == nameof(MainViewModel.ProgressValue))
            {
                if (_mainViewModel.ProgressValue == null)
                {
                    pbProgress.Style = ProgressBarStyle.Marquee;
                    return;
                }
                else
                {
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
            }
            else
            {
                e.Cancel = !_mainViewModel.IsClosing;
            }
        }
        bool isRefreshingFiles = false;
        private async Task RefreshRecentFiles()
        {
            if(isRefreshingFiles) return;
            isRefreshingFiles = true;
            try
            {
                lvRecentWorkspaces.Items.Clear();

                foreach (var file in ApplicationSettings.Instance.FavoriteFiles)
                {
                    var workspaceName = await ServiceProviderHolder.GetRequiredService<WorkspaceFileService>().LoadWorkspaceName(file);
                    var workspaceLocation = Path.GetDirectoryName(file);
                    var workspaceItem = lvRecentWorkspaces.Items.Add(workspaceName);
                    workspaceItem.Tag = file;
                    workspaceItem.SubItems.Add(workspaceLocation);
                    workspaceItem.ImageKey = "favorite";
                }

                foreach (var file in ApplicationSettings.Instance.RecentFiles)
                {
                    if (ApplicationSettings.Instance.FavoriteFiles.Contains(file)) continue;

                    var workspaceName = await ServiceProviderHolder.GetRequiredService<WorkspaceFileService>().LoadWorkspaceName(file);
                    var workspaceLocation = Path.GetDirectoryName(file);
                    var workspaceItem = lvRecentWorkspaces.Items.Add(workspaceName);
                    workspaceItem.Tag = file;
                    workspaceItem.SubItems.Add(workspaceLocation);
                    workspaceItem.ImageKey = "file";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                isRefreshingFiles = false;
            }
        }

        private async void back_tab_open_VisibleChanged(object sender, EventArgs e)
        {
            if (back_tab_open.TabVisible == true)
            {
                await RefreshRecentFiles();
            }
        }

        private void lvRecentWorkspaces_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = lvRecentWorkspaces.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                _mainViewModel.OpenRecentFileCommand.Execute(item.Tag);
                backStage1.Hide();
            }
        }

        private void mniToggleRecentFileAsFavorite_Click(object sender, EventArgs e)
        {
            if (lvRecentWorkspaces.SelectedItems.Count == 0) return;
            var selectedWorkspaceFilePath = lvRecentWorkspaces.SelectedItems[0].Tag as string;
            if (ApplicationSettings.Instance.FavoriteFiles.Contains(selectedWorkspaceFilePath))
                ApplicationSettings.Instance.RemoveFavoriteFile(selectedWorkspaceFilePath);
            else
                ApplicationSettings.Instance.AddFavoriteFile(selectedWorkspaceFilePath);
        }

        private void mniRemoveRecentFileFromList_Click(object sender, EventArgs e)
        {
            if (lvRecentWorkspaces.SelectedItems.Count == 0) return;
            var selectedWorkspaceFilePath = lvRecentWorkspaces.SelectedItems[0].Tag as string;
            if(ApplicationSettings.Instance.RecentFiles.Contains(selectedWorkspaceFilePath))
                ApplicationSettings.Instance.RecentFiles.Remove(selectedWorkspaceFilePath);
            if(ApplicationSettings.Instance.FavoriteFiles.Contains(selectedWorkspaceFilePath))
                ApplicationSettings.Instance.RemoveFavoriteFile(selectedWorkspaceFilePath);
            RefreshRecentFiles();
        }

        #region IView Implementation
        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            this.BindViewModel((MainViewModel)(object)viewModel);
        }



        #endregion

    }
}
