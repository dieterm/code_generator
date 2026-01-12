using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Services;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace
{
    /// <summary>
    /// Controller for DatasourcesContainerArtifact
    /// Handles context menus for the datasources container node
    /// </summary>
    public class DatasourcesContainerController : ArtifactControllerBase<DatasourcesContainerArtifact>
    {
        private readonly IDatasourceFactory _datasourceFactory;

        public DatasourcesContainerController(IDatasourceFactory datasourceFactory, WorkspaceController workspaceController, ILogger<DatasourcesContainerController> logger)
            : base(workspaceController, logger)
        {
            _datasourceFactory = datasourceFactory;
        }

        protected override IEnumerable<WorkspaceCommand> GetCommands(DatasourcesContainerArtifact artifact)
        {
            var commands = new List<WorkspaceCommand>();

            // Group datasource types by category
            var typesByCategory = _datasourceFactory.GetAvailableTypes()
                .GroupBy(t => t.Category)
                .OrderBy(g => g.Key);

            foreach (var categoryGroup in typesByCategory)
            {
                var categoryCommands = new List<WorkspaceCommand>();

                foreach (var typeInfo in categoryGroup.OrderBy(t => t.DisplayName))
                {
                    categoryCommands.Add(new WorkspaceCommand
                    {
                        Id = $"add_datasource_{typeInfo.TypeId}",
                        Text = typeInfo.DisplayName,
                        IconKey = typeInfo.IconKey,
                        Execute = async (a) => await AddDatasourceAsync(artifact, typeInfo.TypeId)
                    });
                }

                commands.Add(new WorkspaceCommand
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
            // Show the details view for the datasources container
            // for now we just clear the details view
            WorkspaceController.ShowWorkspaceDetailsView(null);
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

            // TODO: Show dialog to configure the datasource
            // For now, just add with default name
            var datasource = WorkspaceController.AddDatasource(typeId, $"New {typeInfo.DisplayName}");
            if (datasource != null)
            {
                await WorkspaceController.SaveWorkspaceAsync();
            }
        }
    }
}
