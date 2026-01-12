using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Main controller for workspace operations
    /// Coordinates between artifact controllers and the UI
    /// </summary>
    public class WorkspaceController
    {
        private readonly IDatasourceFactory _datasourceFactory;
        private readonly WorkspaceFileService _workspaceFileService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWindowManagerService _windowManagerService;
        private readonly ILogger<WorkspaceController> _logger;
        
        private List<IArtifactController>? _controllers;
        private WorkspaceTreeViewModel? _workspaceTreeViewModel;
        private WorkspaceDetailsViewModel? _workspaceDetailsViewModel;
        public WorkspaceController(
            IDatasourceFactory datasourceFactory,
            WorkspaceFileService workspaceFileService,
            IServiceProvider serviceProvider,
            IWindowManagerService windowManagerService,
            ILogger<WorkspaceController> logger)
        {
            _datasourceFactory = datasourceFactory;
            _workspaceFileService = workspaceFileService;
            _serviceProvider = serviceProvider;
            _windowManagerService = windowManagerService;
            _logger = logger;
        }

        /// <summary>
        /// Get all registered artifact controllers (lazy loaded to avoid circular dependency)
        /// </summary>
        private List<IArtifactController> Controllers
        {
            get
            {
                if (_controllers == null)
                {
                    _controllers = _serviceProvider.GetServices<IArtifactController>().ToList();
                    _logger.LogDebug("Loaded {Count} artifact controllers", _controllers.Count);
                }
                return _controllers;
            }
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
        /// Event raised when an artifact is selected
        /// </summary>
        public event EventHandler<IArtifact?>? ArtifactSelected;

        /// <summary>
        /// Event raised when an artifact is renamed
        /// </summary>
        public event EventHandler<ArtifactRenamedEventArgs>? ArtifactRenamed;

        /// <summary>
        /// Event raised when a rename edit should be started for an artifact
        /// </summary>
        public event EventHandler<IArtifact>? BeginRenameRequested;

        /// <summary>
        /// Event raised when an artifact property changes (e.g., from edit view)
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ArtifactPropertyChanged;

        /// <summary>
        /// Register an artifact controller
        /// </summary>
        public void RegisterController(IArtifactController controller)
        {
            Controllers.Add(controller);
        }

        /// <summary>
        /// Load a workspace from a file
        /// </summary>
        public async Task<WorkspaceArtifact> LoadWorkspaceAsync(string filePath, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Loading workspace from {FilePath}", filePath);
            
            CurrentWorkspace = await _workspaceFileService.LoadAsync(filePath, cancellationToken);
            
            // First show the tree view (creates ViewModel and subscribes to events)
            ShowWorkspaceTreeView();
            
            // Then fire the event so the ViewModel receives it
            WorkspaceChanged?.Invoke(this, CurrentWorkspace);
            
            return CurrentWorkspace;
        }

        /// <summary>
        /// Create a new workspace
        /// </summary>
        public async Task<WorkspaceArtifact> CreateWorkspaceAsync(string directory, string name = "Workspace", CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating new workspace '{Name}' in {Directory}", name, directory);
            
            CurrentWorkspace = await _workspaceFileService.CreateNewAsync(directory, name, cancellationToken);
            
            // First show the tree view (creates ViewModel and subscribes to events)
            ShowWorkspaceTreeView();
            
            // Then fire the event so the ViewModel receives it
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
            
            _logger.LogInformation("Saving workspace '{Name}'", CurrentWorkspace.Name);
            await _workspaceFileService.SaveAsync(CurrentWorkspace, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Close the current workspace
        /// </summary>
        public void CloseWorkspace()
        {
            if (CurrentWorkspace != null)
            {
                _logger.LogInformation("Closing workspace '{Name}'", CurrentWorkspace.Name);
            }
            
            CurrentWorkspace = null;
            _workspaceTreeViewModel?.Cleanup();
            _workspaceTreeViewModel = null;
            WorkspaceChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Get the controller for an artifact
        /// </summary>
        public IArtifactController? GetController(IArtifact artifact)
        {
            return Controllers.FirstOrDefault(c => c.CanHandle(artifact));
        }

        /// <summary>
        /// Get context menu commands for an artifact
        /// </summary>
        public IEnumerable<WorkspaceCommand> GetContextMenuCommands(IArtifact artifact)
        {
            var controller = GetController(artifact);
            if (controller != null)
            {
                return controller.GetContextMenuCommands(artifact);
            }
            return Enumerable.Empty<WorkspaceCommand>();
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

            _logger.LogInformation("Adding datasource '{Name}' of type '{TypeId}'", name, typeId);

            // Get the provider and create a new datasource
            if (_datasourceFactory is DatasourceFactory factory)
            {
                // TODO: Get provider from factory and create datasource
                // var provider = factory.GetProvider(typeId);
                // var datasource = provider?.CreateNew(name);
                // if (datasource != null)
                // {
                //     CurrentWorkspace.Datasources.AddDatasource(datasource);
                //     return datasource;
                // }
            }

            return null;
        }

        /// <summary>
        /// Handle artifact selection
        /// </summary>
        public async Task SelectArtifactAsync(IArtifact artifact, CancellationToken cancellationToken = default)
        {
            ArtifactSelected?.Invoke(this, artifact);

            var controller = GetController(artifact);
            if (controller != null)
            {
                await controller.OnSelectedAsync(artifact, cancellationToken);
            }
        }

        /// <summary>
        /// Handle artifact rename
        /// </summary>
        public void OnArtifactRenamed(IArtifact artifact, string oldName, string newName)
        {
            _logger.LogDebug("Artifact renamed: '{OldName}' -> '{NewName}'", oldName, newName);
            ArtifactRenamed?.Invoke(this, new ArtifactRenamedEventArgs(artifact, oldName, newName));
        }

        /// <summary>
        /// Notify that an artifact property has changed
        /// </summary>
        public void OnArtifactPropertyChanged(IArtifact artifact, string propertyName, object? newValue)
        {
            _logger.LogDebug("Artifact property changed: {PropertyName} = {NewValue}", propertyName, newValue);
            ArtifactPropertyChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(artifact, propertyName, newValue));
        }

        /// <summary>
        /// Request the UI to begin rename editing for an artifact
        /// </summary>
        public void RequestBeginRename(IArtifact artifact)
        {
            _logger.LogDebug("Requesting rename for artifact: {Id}", artifact.Id);
            BeginRenameRequested?.Invoke(this, artifact);
        }
        
        /// <summary>
        /// Show or refresh the workspace tree view
        /// </summary>
        private void ShowWorkspaceTreeView()
        {
            // Reuse existing ViewModel if possible, otherwise create new
            if (_workspaceTreeViewModel == null)
            {
                _workspaceTreeViewModel = new WorkspaceTreeViewModel(this);
                _workspaceTreeViewModel.PropertyChanged += WorkspaceTreeViewModel_PropertyChanged;
            }
            
            _windowManagerService.ShowWorkspaceTreeView(_workspaceTreeViewModel);
        }

        private async void WorkspaceTreeViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WorkspaceTreeViewModel.SelectedArtifact))
            {
                if (_workspaceTreeViewModel?.SelectedArtifact != null)
                {
                    var artifactController = GetController(_workspaceTreeViewModel.SelectedArtifact);
                    await artifactController!.OnSelectedAsync(_workspaceTreeViewModel.SelectedArtifact);
                } 
                else
                {
                    ShowWorkspaceDetailsView(null);
                }
            }
        }

        public void ShowWorkspaceDetailsView(ViewModelBase? detailsModel)
        {
            // Reuse existing ViewModel if possible, otherwise create new
            if (_workspaceDetailsViewModel == null)
            {
                _workspaceDetailsViewModel = new WorkspaceDetailsViewModel();
            }
            _workspaceDetailsViewModel.DetailsViewModel = detailsModel;
            _windowManagerService.ShowWorkspaceDetailsView(_workspaceDetailsViewModel);
        }
    }

    /// <summary>
    /// Event args for artifact renamed event
    /// </summary>
    public class ArtifactRenamedEventArgs : EventArgs
    {
        public IArtifact Artifact { get; }
        public string OldName { get; }
        public string NewName { get; }

        public ArtifactRenamedEventArgs(IArtifact artifact, string oldName, string newName)
        {
            Artifact = artifact;
            OldName = oldName;
            NewName = newName;
        }
    }

    /// <summary>
    /// Event args for artifact property changed event
    /// </summary>
    public class ArtifactPropertyChangedEventArgs : EventArgs
    {
        public IArtifact Artifact { get; }
        public string PropertyName { get; }
        public object? NewValue { get; }

        public ArtifactPropertyChangedEventArgs(IArtifact artifact, string propertyName, object? newValue)
        {
            Artifact = artifact;
            PropertyName = propertyName;
            NewValue = newValue;
        }
    }
}
