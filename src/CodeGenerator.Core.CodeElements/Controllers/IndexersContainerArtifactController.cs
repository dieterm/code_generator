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
    public class IndexersContainerArtifactController : CodeElementArtifactControllerBase<IndexersContainerArtifact>
    {
        public IndexersContainerArtifactController(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger<IndexersContainerArtifactController> logger)
            : base(operationExecutor, treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(IndexersContainerArtifact artifact)
        {
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "AddIndexer",
                Text = "Add Indexer",
                IconKey = "AddIndexerIcon",
                Execute = async (artifact) =>
                {
                    var newIndexerElement = new IndexerElement(new TypeReference("object"), new ParameterElement("index", new TypeReference("int")));
                    (artifact as IndexersContainerArtifact)!.AddIndexerElement(newIndexerElement);
                }
            };
        }
    }
}
