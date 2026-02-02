using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace.Datasources;
using CodeGenerator.Application.Controllers.Workspace.Domains;
using CodeGenerator.Application.Controllers.Workspace.Scopes;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Events;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Relational;
using CodeGenerator.Core.Workspaces.MessageBus;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.Shared.ViewModels;
using DocumentFormat.OpenXml.Office2013.Drawing.Chart;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Diagnostics;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Main controller for workspace operations
    /// Coordinates between artifact controllers and the UI
    /// </summary>
    public class WorkspaceTreeViewController : ArtifactTreeViewController<WorkspaceTreeViewModel>, IWorkspaceContextProvider
    {
        private readonly IDatasourceFactory _datasourceFactory;
        private readonly WorkspaceFileService _workspaceFileService;
        private readonly TemplateManager _templateManager;
        private readonly WorkspaceMessageBus _workspaceMessageBus;
        private ArtifactDetailsViewModel? _workspaceDetailsViewModel;

        public WorkspaceTreeViewController(
            WorkspaceMessageBus workspaceMessageBus,
            TemplateManager templateManager,
            IDatasourceFactory datasourceFactory,
            WorkspaceFileService workspaceFileService,
            RibbonBuilder ribbonBuilder,
            IWindowManagerService windowManagerService,
            IMessageBoxService messageBoxService,
            ILogger<WorkspaceTreeViewController> logger)
            : base(windowManagerService, messageBoxService, logger)
        {
            _workspaceMessageBus = workspaceMessageBus;
            _datasourceFactory = datasourceFactory;
            _workspaceFileService = workspaceFileService;
            _templateManager = templateManager;
        }

        /// <summary>
        /// Load artifact controllers from DI
        /// </summary>
        protected override IEnumerable<IArtifactController> LoadArtifactControllers()
        {
            var controllers = new List<IArtifactController>
            {
                ServiceProviderHolder.ServiceProvider.GetRequiredService<WorkspaceArtifactController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<DatasourcesContainerController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<MysqlDatasourceController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<SqlServerDatasourceController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<PostgreSqlDatasourceController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<ExcelDatasourceController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<CsvDatasourceController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<JsonDatasourceController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<XmlDatasourceController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<YamlDatasourceController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<DotNetAssemblyDatasourceController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<OpenApiDatasourceController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<TableArtifactController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<ViewArtifactController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<ColumnArtifactController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<IndexArtifactController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<ForeignKeyArtifactController>(),

                ServiceProviderHolder.ServiceProvider.GetRequiredService<DomainsContainerController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<ScopesContainerController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<SubScopesContainerController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<ScopeArtifactController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<DomainController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<EntitiesContainerController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<EntityController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<EntityStatesContainerController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<EntityStateController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<PropertyController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<EntityRelationsContainerController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<EntityRelationController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<ValueTypesContainerController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<ValueTypeController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<EntityViewsContainerController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<EntityEditViewArtifactController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<EntityEditViewFieldController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<EntityListViewArtifactController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<EntityListViewColumnController>(),
                ServiceProviderHolder.ServiceProvider.GetRequiredService<EntitySelectViewArtifactController>(),

                // Add other controllers here as needed
            };

            // Register required templates from all controllers
            foreach (var controller in controllers)
            {
                var requiredTemplates = controller.RegisterRequiredTemplates();
                _templateManager.RegisterRequiredTemplates(requiredTemplates);
            }

            return controllers;
        }

        /// <summary>
        /// Current workspace
        /// </summary>
        public WorkspaceArtifact? CurrentWorkspace { get; private set; }

        /// <summary>
        /// Event raised when the workspace changes
        /// </summary>
        public event EventHandler<WorkspaceArtifact?>? WorkspaceChanged;
        public event EventHandler? WorkspaceHasUnsavedChangesChanged;
        private bool _hasUnsavedChanges = false;
        /// <summary>
        /// TODO: Subscribe to all events of all artifacts recursively.
        /// Also keep track of unsubscribing when artifacts are removed or when workspace is closed.
        /// </summary>
        public bool HasUnsavedChanges {
            get {return _hasUnsavedChanges; } 
            private set { 
                if (_hasUnsavedChanges != value) {
                    _hasUnsavedChanges = value;
                    WorkspaceHasUnsavedChangesChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void Initialize()
        {
           
        }


        /// <summary>
        /// Load a workspace from a file
        /// </summary>
        public async Task<WorkspaceArtifact> LoadWorkspaceAsync(string filePath, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Loading workspace from {FilePath}", filePath);
            
            CurrentWorkspace = await _workspaceFileService.LoadAsync(filePath, cancellationToken);
            
            if(CurrentWorkspace != null )
            { 
                // Set workspace directory on TemplateManager (ensures template folders exist)
                _templateManager.SetWorkspaceDirectory(CurrentWorkspace.WorkspaceDirectory);
            }
            
            ShowWorkspaceTreeView();
            WorkspaceChanged?.Invoke(this, CurrentWorkspace);
            HasUnsavedChanges = false;
            ObserveWorkspaceChanges(CurrentWorkspace);
            return CurrentWorkspace;
        }

        /// <summary>
        /// Create a new workspace
        /// </summary>
        public async Task<WorkspaceArtifact> CreateWorkspaceAsync(string directory, string name = "Workspace", CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Creating new workspace '{Name}' in {Directory}", name, directory);
            
            CurrentWorkspace = await _workspaceFileService.CreateNewAsync(directory, name, cancellationToken);

            // Set workspace directory on TemplateManager (ensures template folders exist)
            _templateManager.SetWorkspaceDirectory(CurrentWorkspace.WorkspaceDirectory);

            ShowWorkspaceTreeView();
            WorkspaceChanged?.Invoke(this, CurrentWorkspace);
            HasUnsavedChanges = false;
            ObserveWorkspaceChanges(CurrentWorkspace);
            return CurrentWorkspace;
        }

        private void ObserveWorkspaceChanges(WorkspaceArtifact workspace)
        {
            // Clear previous subscriptions
            foreach (var artifact in _observedArtifacts)
            {
                artifact.PropertyChanged -= OnArtifactPropertyChanged;
                artifact.ChildAdded -= OnArtifactChildAdded;
                artifact.ChildRemoved -= OnArtifactRemoved;
            }
            _observedArtifacts.Clear();
            
            ObserveArtifactChanges(workspace);
        }
        private List<IArtifact> _observedArtifacts = new List<IArtifact>();
        private void ObserveArtifactChanges(IArtifact artifact)
        {
            _observedArtifacts.Add(artifact);
            artifact.PropertyChanged += OnArtifactPropertyChanged;

            artifact.ChildAdded += OnArtifactChildAdded;

            artifact.ChildRemoved += OnArtifactRemoved; 

            foreach(var child in artifact.Children)
            {
                ObserveArtifactChanges(child);
            }
        }

        private void OnArtifactRemoved(object? sender, ChildRemovedEventArgs e)
        {
            var childArtifact = e.ChildArtifact;
            childArtifact.PropertyChanged -= OnArtifactPropertyChanged;
            _observedArtifacts.Remove(childArtifact);
            HasUnsavedChanges = true;
        }

        private void OnArtifactChildAdded(object? sender, ChildAddedEventArgs e)
        {
            ObserveArtifactChanges(e.ChildArtifact);
            HasUnsavedChanges = true;
        }

        private void OnArtifactPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //Logger.LogDebug("Artifact property changed: {Artifact} - {PropertyName}", (sender as IArtifact)?.TreeNodeText, e.PropertyName);
            HasUnsavedChanges = true;
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
            HasUnsavedChanges = false;
        }

        /// <summary>
        /// Close the current workspace
        /// </summary>
        public void CloseWorkspace()
        {
            if (CurrentWorkspace != null)
            {
                Logger.LogInformation("Closing workspace '{Name}'", CurrentWorkspace.Name);
                // Clear workspace directory from TemplateManager
                _templateManager.SetWorkspaceDirectory(null);
            }
            
            CurrentWorkspace = null;
            TreeViewModel?.Cleanup();
            TreeViewModel = null;
            ShowArtifactDetailsView(null);
            HasUnsavedChanges = false;
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

        //public DomainArtifact AddDomain(string domainName)
        //{
        //    var domainArtifact = new DomainArtifact(domainName);
        //    CurrentWorkspace?.Domains.AddDomain(domainArtifact);
        //    return domainArtifact;
        //}

        public void CreateEntityFromTableInDomain(TableArtifact table, DomainArtifact domain)
        {
            var entityArtifact = new EntityArtifact(table.Name);
            var entityState = entityArtifact.AddEntityState(table.Name);
            foreach (var column in table.GetColumns())
            {
                entityState.AddProperty(new PropertyArtifact(column.Name, column.DataType, column.IsNullable) { 
                    MaxLength = column.MaxLength, 
                    Precision = column.Precision, 
                    Scale = column.Scale 
                });
            }
            domain.AddEntity(entityArtifact);
        }
    }
}
