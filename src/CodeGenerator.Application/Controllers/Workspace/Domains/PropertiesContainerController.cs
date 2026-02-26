using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.Operations.Domains;
using CodeGenerator.Domain.DataTypes;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Application.Controllers.Workspace.Domains
{
    public class PropertiesContainerController : WorkspaceArtifactControllerBase<PropertiesContainerArtifact>
    {
        public PropertiesContainerController(OperationExecutor operationExecutor, WorkspaceTreeViewController treeViewController, ILogger<PropertiesContainerController> logger) 
            : base(operationExecutor, treeViewController, logger)
        {

        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(PropertiesContainerArtifact artifact)
        {
            // Add Property command
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "add_property",
                Text = "Add Property",
                IconKey = "plus",
                Execute = async (a) =>
                {
                    var addPropertyOperation = ServiceProviderHolder.GetRequiredService<AddPropertyToEntityOperation>();
                    var addPropertyParams = new AddPropertyToEntityParams
                    {
                        DataType = GenericDataTypes.VarChar.Id,
                        EntityId = artifact.Parent!.Id,
                        PropertyName = "NewProperty",
                        IsNullable = true
                    };
                    var result = OperationExecutor.Execute(addPropertyOperation, addPropertyParams);

                    if (result.Success)
                    {
                        var createdProperty = addPropertyParams.CreatedProperty;
                        if (createdProperty != null)
                        {
                            TreeViewController.OnArtifactAdded(artifact, createdProperty);
                            TreeViewController.RequestBeginRename(createdProperty);
                        }
                    }
                    await Task.CompletedTask;
                }
            };
        }
    }
}
