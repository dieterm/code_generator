using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for DatasourcesContainerArtifact
    /// Handles context menus for the datasources container node
    /// </summary>
    public class DatasourcesContainerController : WorkspaceArtifactControllerBase<DatasourcesContainerArtifact>
    {
        private readonly IDatasourceFactory _datasourceFactory;

        public DatasourcesContainerController(OperationExecutor operationExecutor, IDatasourceFactory datasourceFactory, WorkspaceTreeViewController workspaceController, ILogger<DatasourcesContainerController> logger)
            : base(operationExecutor, workspaceController, logger)
        {
            _datasourceFactory = datasourceFactory;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DatasourcesContainerArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Group datasource types by category
            var typesByCategory = _datasourceFactory.GetAvailableTypes()
                .GroupBy(t => t.Category)
                .OrderBy(g => g.Key.DisplayName);

            foreach (var categoryGroup in typesByCategory)
            {
                var categoryCommands = new List<ArtifactTreeNodeCommand>();

                foreach (var typeInfo in categoryGroup.OrderBy(t => t.DisplayName))
                {
                    categoryCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                    {
                        Id = $"add_datasource_{typeInfo.TypeId}",
                        Text = typeInfo.DisplayName,
                        IconKey = typeInfo.IconKey,
                        Execute = async (a) => await AddDatasourceAsync(artifact, typeInfo.TypeId)
                    });
                }

                commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                {
                    Id = $"add_datasource_category_{categoryGroup.Key.Id}",
                    Text = $"Add {categoryGroup.Key.DisplayName}",
                    IconKey = categoryGroup.Key.IconKey,
                    SubCommands = categoryCommands
                });
            }

            return commands;
        }

        protected override Task OnSelectedInternalAsync(DatasourcesContainerArtifact artifact, CancellationToken cancellationToken)
        {
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }

        private async Task AddDatasourceAsync(DatasourcesContainerArtifact container, string typeId)
        {
            var datasourceTypes = _datasourceFactory.GetAvailableTypes();
            var datasourceTypeInfo = datasourceTypes.FirstOrDefault(t => t.TypeId == typeId);
            if (datasourceTypeInfo == null) return;

            if (datasourceTypeInfo.Category == DatasourceCategory.File && !string.IsNullOrEmpty(datasourceTypeInfo.FilePickerFilter))
            {
                var filesystemDialogService = ServiceProviderHolder.GetRequiredService<IFileSystemDialogService>();
                var filePaths = filesystemDialogService.OpenFiles(datasourceTypeInfo.FilePickerFilter);
                if (filePaths.Length == 0) return;

                foreach (var filePath in filePaths)
                {
                    var datasourceName = Path.GetFileNameWithoutExtension(filePath);
                    var datasource = TreeViewController.AddDatasource(typeId, datasourceName);
                    if (datasource != null)
                    {
                        var filePathProperty = datasource.GetType().GetProperty("FilePath");
                        filePathProperty?.SetValue(datasource, filePath);
                    }
                }
            }
            else
            {
                TreeViewController.AddDatasource(typeId, $"New {datasourceTypeInfo.DisplayName}");
            }

            await TreeViewController.SaveWorkspaceAsync();
        }
    }
}
