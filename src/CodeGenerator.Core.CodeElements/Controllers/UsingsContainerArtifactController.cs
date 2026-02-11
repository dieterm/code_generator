using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Controllers
{
    public abstract class UsingsContainerArtifactController<T, U> : CodeElementArtifactControllerBase<U>
        where T : CodeElement, ICodeElementWithUsings
        where U : UsingsContainerArtifact<T>
    {
        public UsingsContainerArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger logger) 
            : base(operationExecutor, treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(U artifact)
        {
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE) {
                Id = "add_new_using",
                Text = "Add new using",
                Execute = async (a) => {
                    artifact.AddNewUsing();

                    
                }
            };
        }

       
    }

    public class NamespacesUsingsContainerArtifactController : UsingsContainerArtifactController<NamespaceElement, NamespaceUsingsContainerArtifact>
    {
        public NamespacesUsingsContainerArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<NamespacesUsingsContainerArtifactController> logger)
            : base(operationExecutor, treeViewController, logger)
        {
        }
    }

    public class CodeFileUsingsContainerArtifactController : UsingsContainerArtifactController<CodeFileElement, CodeFileUsingsContainerArtifact>
    {
        public CodeFileUsingsContainerArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<CodeFileUsingsContainerArtifactController> logger)
            : base(operationExecutor, treeViewController, logger)
        {

        }
    }
}
