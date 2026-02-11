using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Shared.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Controllers
{
    /// <summary>
    /// Base class where all codeelement artifact controllers inherit from.
    /// This allows us to have common logic for all code element artifacts in one place, such as handling name changes and updating the tree view text.
    /// </summary>
    public abstract class CodeElementArtifactControllerBase<TArtifact> : ArtifactControllerBase<CodeElementsTreeViewController, TArtifact>, ICodeElementArtifactController
        where TArtifact : CodeElementArtifactBase
    {
        protected CodeElementArtifactControllerBase(OperationExecutor operationExecutor, CodeElementsTreeViewController treeViewController, ILogger logger) 
            : base(operationExecutor, treeViewController, logger)
        {

        }
    }
}
