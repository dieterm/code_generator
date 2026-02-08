using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.Template
{
    public abstract class TemplateArtifactControllerBase<TArtifact> : ArtifactControllerBase<TemplateTreeViewController, TArtifact>, ITemplateArtifactController
        where TArtifact : class, IArtifact
    {
        protected TemplateArtifactControllerBase(TemplateTreeViewController treeViewController, ILogger logger) 
            : base(treeViewController, logger)
        {

        }
    }
}
