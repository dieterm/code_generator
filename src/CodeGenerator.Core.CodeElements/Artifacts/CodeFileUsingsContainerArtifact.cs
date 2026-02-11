using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Artifacts
{
    public class CodeFileUsingsContainerArtifact : UsingsContainerArtifact<CodeFileElement>
    {
        public CodeFileUsingsContainerArtifact(CodeFileElement codeFileElement)
            : base(codeFileElement)
        {
        }

        public CodeFileUsingsContainerArtifact(ArtifactState artifactState)
            : base(artifactState)
        {
        }
    }
}
