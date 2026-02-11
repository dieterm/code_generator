using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public abstract class StatementArtifactBase<T> : CodeElementArtifactBase
        where T : StatementElement, new()
    {
        public T StatementElement { get; }

        protected StatementArtifactBase(T statementElement)
        {
            StatementElement = statementElement;
            Name = GetType().Name.Replace("Artifact", "");
        }

        protected StatementArtifactBase(ArtifactState artifactState) : base(artifactState)
        {
            throw new NotImplementedException("Deserialization constructor not implemented for StatementArtifactBase");
        }
    }
}
