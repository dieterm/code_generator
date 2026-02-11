using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Artifacts
{
    public class NamespaceUsingsContainerArtifact : UsingsContainerArtifact<NamespaceElement>
    {
        public NamespaceUsingsContainerArtifact(NamespaceElement codeElement) : base(codeElement)
        {
        }

        public NamespaceUsingsContainerArtifact(ArtifactState artifactState) : base(artifactState)
        {
        }
    }
}
