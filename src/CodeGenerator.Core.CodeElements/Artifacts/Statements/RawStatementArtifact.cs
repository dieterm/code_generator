using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class RawStatementArtifact : StatementArtifactBase<RawStatementElement>
    {
        public RawStatementArtifact(RawStatementElement raw) : base(raw)
        {
            
        }

        public string Code { 
            get { return StatementElement.Code; }
            set {
                if (StatementElement.Code != value)
                {
                    StatementElement.Code = value;
                    RaisePropertyChangedEvent(nameof(Code));
                }
            } 
        }
    }
}