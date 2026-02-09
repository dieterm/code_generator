using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Core.Workspaces.Operations.Domains;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Operations;
using CodeGenerator.Shared.UndoRedo;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains.Entities
{
    /// <summary>
    /// Controller for EntitiesContainerArtifact
    /// </summary>
    public class EntitiesContainerController : WorkspaceArtifactControllerBase<EntitiesContainerArtifact>
    {
        public EntitiesContainerController(
            OperationExecutor operationExecutor,
            WorkspaceTreeViewController workspaceController,
            ILogger<EntitiesContainerController> logger)
            : base(operationExecutor,workspaceController, logger)
        {
        }

        protected override void OnArtifactRenamedInternal(EntitiesContainerArtifact artifact, string oldName, string newName)
        {
            // Container cannot be renamed
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(EntitiesContainerArtifact artifact)
        {
            var commands = new List<ArtifactTreeNodeCommand>();

            // Add Entity command
            commands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_entity",
                Text = "Add Entity",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var addDomainOperation = ServiceProviderHolder.GetRequiredService<AddEntityToDomainOperation>();
                    var addDomainParams = new AddEntityToDomainParams
                    {
                        EntitiesContainerId = artifact?.Id ?? string.Empty,
                        EntityName = "New Entity"
                    };
                    var result = OperationExecutor.Execute(addDomainOperation, addDomainParams);

                    if(result.Success)
                    {
                        var createdEntity = addDomainParams.CreatedEntity;
                        if(createdEntity != null)
                        {
                            TreeViewController.OnArtifactAdded(artifact, createdEntity);
                            TreeViewController.RequestBeginRename(createdEntity);
                        }
                    }
                    
                    await Task.CompletedTask;
                }
            });

            return commands;
        }

        protected override Task OnSelectedInternalAsync(EntitiesContainerArtifact artifact, CancellationToken cancellationToken)
        {
            // No edit view for container
            TreeViewController.ShowArtifactDetailsView(null);
            return Task.CompletedTask;
        }
    }
}
