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
    public abstract class TemplateArtifactControllerBase<TTreeView, TArtifact> : ArtifactControllerBase<TTreeView, TArtifact>, ITemplateArtifactController
        where TArtifact : class, IArtifact
        where TTreeView : IArtifactTreeViewController
    {
        protected TemplateArtifactControllerBase(TTreeView treeViewController, ILogger logger) 
            : base(treeViewController, logger)
        {

        }
    }
}
