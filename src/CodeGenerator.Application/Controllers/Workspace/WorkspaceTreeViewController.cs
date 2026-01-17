using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Main controller for workspace operations
    /// Coordinates between artifact controllers and the UI
    /// </summary>
    public class WorkspaceTreeViewController : ArtifactTreeViewController<WorkspaceTreeViewModel>
    {
        private readonly IDatasourceFactory _datasourceFactory;
        private readonly WorkspaceFileService _workspaceFileService;
        
        private ArtifactDetailsViewModel? _workspaceDetailsViewModel;

        public WorkspaceTreeViewController(
            IDatasourceFactory datasourceFactory,
            WorkspaceFileService workspaceFileService,
            IWindowManagerService windowManagerService,
            IMessageBoxService messageBoxService,
            ILogger<WorkspaceTreeViewController> logger)
            : base(windowManagerService, messageBoxService, logger)
        {
            _datasourceFactory = datasourceFactory;
            _workspaceFileService = workspaceFileService;
        }

        /// <summary>
        /// Load artifact controllers from DI
        /// </summary>
        protected override IEnumerable<IArtifactController> LoadArtifactControllers()
        {
            return new List<IArtifactController>
            {
                ServiceProviderHolder.ServiceProvider.GetRequiredService<WorkspaceArtifactController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<DatasourcesContainerController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<MysqlDatasourceController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<SqlServerDatasourceController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<TableArtifactController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<ViewArtifactController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<ColumnArtifactController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<IndexArtifactController>(),
                // Add other controllers here as needed
            };
        }

        /// <summary>
        /// Current workspace
        /// </summary>
        public WorkspaceArtifact? CurrentWorkspace { get; private set; }

        /// <summary>
        /// Event raised when the workspace changes
        /// </summary>
        public event EventHandler<WorkspaceArtifact?>? WorkspaceChanged;

        /// <summary>
        /// Event raised when a rename edit should be started for an artifact
        /// </summary>
        //public event EventHandler<IArtifact>? BeginRenameRequested;

        ///// <summary>
        ///// Event raised when an artifact property changes (e.g., from edit view)
        ///// </summary>
        //public event EventHandler<ArtifactPropertyChangedEventArgs>? ArtifactPropertyChanged;

        ///// <summary>
        ///// Event raised when a child artifact is added
        ///// </summary>
        //public event EventHandler<ArtifactChildChangedEventArgs>? ArtifactAdded;

        ///// <summary>
        ///// Event raised when a child artifact is removed
        ///// </summary>
        //public event EventHandler<ArtifactChildChangedEventArgs>? ArtifactRemoved;

        /// <summary>
        /// Load a workspace from a file
        /// </summary>
        public async Task<WorkspaceArtifact> LoadWorkspaceAsync(string filePath, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Loading workspace from {FilePath}", filePath);
            
            CurrentWorkspace = await _workspaceFileService.LoadAsync(filePath, cancellationToken);
            
            ShowWorkspaceTreeView();
            WorkspaceChanged?.Invoke(this, CurrentWorkspace);
            
            return CurrentWorkspace;
        }

        /// <summary>
        /// Create a new workspace
        /// </summary>
        public async Task<WorkspaceArtifact> CreateWorkspaceAsync(string directory, string name = "Workspace", CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Creating new workspace '{Name}' in {Directory}", name, directory);
            
            CurrentWorkspace = await _workspaceFileService.CreateNewAsync(directory, name, cancellationToken);
            
            ShowWorkspaceTreeView();
            WorkspaceChanged?.Invoke(this, CurrentWorkspace);
            
            return CurrentWorkspace;
        }

        /// <summary>
        /// Save the current workspace
        /// </summary>
        public async Task SaveWorkspaceAsync(CancellationToken cancellationToken = default)
        {
            if (CurrentWorkspace == null)
            {
                throw new InvalidOperationException("No workspace is loaded");
            }
            
            Logger.LogInformation("Saving workspace '{Name}'", CurrentWorkspace.Name);
            await _workspaceFileService.SaveAsync(CurrentWorkspace, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Close the current workspace
        /// </summary>
        public void CloseWorkspace()
        {
            if (CurrentWorkspace != null)
            {
                Logger.LogInformation("Closing workspace '{Name}'", CurrentWorkspace.Name);
            }
            
            CurrentWorkspace = null;
            TreeViewModel?.Cleanup();
            TreeViewModel = null;
            WorkspaceChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Get available datasource types for adding new datasources
        /// </summary>
        public IEnumerable<DatasourceTypeInfo> GetAvailableDatasourceTypes()
        {
            return _datasourceFactory.GetAvailableTypes();
        }

        /// <summary>
        /// Add a new datasource to the current workspace
        /// </summary>
        public DatasourceArtifact? AddDatasource(string typeId, string name)
        {
            if (CurrentWorkspace == null)
            {
                throw new InvalidOperationException("No workspace is loaded");
            }

            var types = _datasourceFactory.GetAvailableTypes();
            var typeInfo = types.FirstOrDefault(t => t.TypeId == typeId);
            if (typeInfo == null)
            {
                throw new ArgumentException($"Unknown datasource type: {typeId}");
            }

            Logger.LogInformation("Adding datasource '{Name}' of type '{TypeId}'", name, typeId);

            if (_datasourceFactory is DatasourceFactory factory)
            {
                var provider = factory.GetProvider(typeId);
                var datasource = provider?.CreateNew(name);
                if (datasource != null)
                {
                    CurrentWorkspace.Datasources.AddDatasource(datasource);
                    return datasource;
                }
            }

            return null;
        }



        /// <summary>
        /// Notify that an artifact property has changed
        /// </summary>
        //public void OnArtifactPropertyChanged(IArtifact artifact, string propertyName, object? newValue)
        //{
        //    Logger.LogDebug("Artifact property changed: {PropertyName} = {NewValue}", propertyName, newValue);
        //    ArtifactPropertyChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(artifact, propertyName, newValue));
        //}

        /// <summary>
        /// Notify that a child artifact was added
        /// </summary>
        //public void OnArtifactAdded(IArtifact parent, IArtifact child)
        //{
        //    Logger.LogDebug("Artifact added: {ChildId} to parent {ParentId}", child.Id, parent.Id);
        //    ArtifactAdded?.Invoke(this, new ArtifactChildChangedEventArgs(parent, child));
        //}

        /// <summary>
        /// Notify that a child artifact was removed
        /// </summary>
        //public void OnArtifactRemoved(IArtifact parent, IArtifact child)
        //{
        //    Logger.LogDebug("Artifact removed: {ChildId} from parent {ParentId}", child.Id, parent.Id);
        //    ArtifactRemoved?.Invoke(this, new ArtifactChildChangedEventArgs(parent, child));
        //}

        /// <summary>
        /// Request the UI to begin rename editing for an artifact
        /// </summary>
        //public void RequestBeginRename(IArtifact artifact)
        //{
        //    Logger.LogDebug("Requesting rename for artifact: {Id}", artifact.Id);
        //    BeginRenameRequested?.Invoke(this, artifact);
        //}
        
        /// <summary>
        /// Show or refresh the workspace tree view
        /// </summary>
        private void ShowWorkspaceTreeView()
        {
            WindowManagerService.ShowWorkspaceTreeView(TreeViewModel!);
        }

        private async void WorkspaceTreeViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WorkspaceTreeViewModel.SelectedArtifact))
            {
                if (TreeViewModel?.SelectedArtifact != null)
                {
                    OnArtifactSelected(TreeViewModel.SelectedArtifact);

                    var artifactController = GetController(TreeViewModel.SelectedArtifact);
                    if (artifactController != null)
                        await artifactController.OnSelectedAsync(TreeViewModel.SelectedArtifact);
                    else
                        Debug.WriteLine("No controller found for selected artifact {0}", TreeViewModel.SelectedArtifact.TreeNodeText);
                } 
                else
                {
                    ShowArtifactDetailsView(null);
                }
            }
        }

        public override void ShowArtifactDetailsView(ViewModelBase? detailsModel)
        {
            if (_workspaceDetailsViewModel == null)
            {
                _workspaceDetailsViewModel = new ArtifactDetailsViewModel();
            }
            _workspaceDetailsViewModel.DetailsViewModel = detailsModel;
            WindowManagerService.ShowWorkspaceDetailsView(_workspaceDetailsViewModel);
        }
    }
}
