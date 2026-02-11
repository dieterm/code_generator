using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class ReturnStatementArtifact : StatementArtifactBase<ReturnStatementElement>
    {
        public ReturnStatementArtifact(ReturnStatementElement statement) : base(statement)
        {
        }

        public string? Expression
        {
            get => StatementElement.Expression;
            set
            {
                if (StatementElement.Expression != value)
                {
                    StatementElement.Expression = value;
                    RaisePropertyChangedEvent(nameof(Expression));
                }
            }
        }
    }
}
