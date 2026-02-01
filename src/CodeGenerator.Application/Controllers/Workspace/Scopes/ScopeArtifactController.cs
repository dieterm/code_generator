using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Workspace.Scopes
{
    public class ScopeArtifactController : ArtifactControllerBase<WorkspaceTreeViewController, ScopeArtifact>
    {
        public ScopeArtifactController(WorkspaceTreeViewController treeViewController, ILogger<ScopeArtifactController> logger)
            : base(treeViewController, logger)
        {

        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ScopeArtifact artifact)
        {

            if (artifact.CanBeginEdit()) { 
                // Rename command
                yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_RENAME)
                {
                    Id = "rename_column",
                    Text = "Rename",
                    IconKey = "edit",
                    Execute = async (a) =>
                    {
                        TreeViewController.RequestBeginRename(artifact);
                        await Task.CompletedTask;
                    }
                };
            }
        }

        public override bool CanDelete(ScopeArtifact artifact)
        {
            return !artifact.IsDefaultScope();
        }

        
    }
}
