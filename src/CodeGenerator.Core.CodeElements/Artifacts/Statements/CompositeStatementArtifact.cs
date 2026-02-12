using CodeGenerator.Core.Artifacts;
using CodeGenerator.Domain.CodeElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class CompositeStatementArtifact : StatementArtifactBase<CompositeStatement>
    {
        public bool IsReadOnly { get; private set; }
        public CompositeStatementArtifact(CompositeStatement codeElement, bool isReadOnly) : base(codeElement)
        {
            IsReadOnly = isReadOnly;
            foreach (var statement in codeElement.Statements)
            {
                AddChild(StatementArtifactFactory.Create(statement));
            }
        }
    
        protected CompositeStatementArtifact(ArtifactState artifactState) : base(artifactState)
        { }
        
        public List<StatementElement> Statements { get { return StatementElement.Statements; } }
    }
}
