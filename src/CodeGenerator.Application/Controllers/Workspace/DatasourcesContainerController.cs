using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Services;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for DatasourcesContainerArtifact
    /// Handles context menus for the datasources container node
    /// </summary>
    public class DatasourcesContainerController : ArtifactControllerBase<WorkspaceTreeViewController,DatasourcesContainerArtifact>
    {
        private readonly IDatasourceFactory _datasourceFactory;

        //protected WorkspaceTreeViewController TreeViewController => (WorkspaceTreeViewController)base.TreeViewController;

        public DatasourcesContainerController(IDatasourceFactory datasourceFactory, WorkspaceTreeViewController workspaceController, ILogger<DatasourcesContainerController> logger)
            : base(workspaceController, logger)
        {
            _datasourceFactory = datasourceFactory;
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(DatasourcesContainerArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Group datasource types by category
            var typesByCategory = _datasourceFactory.GetAvailableTypes()
                .GroupBy(t => t.Category)
                .OrderBy(g => g.Key);

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
                    Id = $"add_datasource_category_{categoryGroup.Key.Replace(" ", "_").ToLowerInvariant()}",
                    Text = $"Add {categoryGroup.Key}",
                    IconKey = GetCategoryIcon(categoryGroup.Key),
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

        private string GetCategoryIcon(string category)
        {
            return category.ToLowerInvariant() switch
            {
                "relational database" => "database",
                "non-relational database" => "database",
                "file" => "file",
                _ => "plus"
            };
        }

        private async Task AddDatasourceAsync(DatasourcesContainerArtifact container, string typeId)
        {
            var types = _datasourceFactory.GetAvailableTypes();
            var typeInfo = types.FirstOrDefault(t => t.TypeId == typeId);
            if (typeInfo == null) return;

            var datasource = TreeViewController.AddDatasource(typeId, $"New {typeInfo.DisplayName}");
            if (datasource != null)
            {
                await TreeViewController.SaveWorkspaceAsync();
            }
        }
    }
}
