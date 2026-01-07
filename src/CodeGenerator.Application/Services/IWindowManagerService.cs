using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Services
{
    public interface IWindowManagerService
    {
        void ShowArtifactPreview(IArtifact selectedArtifact);
        void ShowDomainSchemaTreeView(DomainSchemaTreeViewModel treeViewModel);
        void ShowGenerationTreeView(GenerationResultTreeViewModel treeViewModel);
    }
}
