using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class ThrowStatementArtifact : StatementArtifactBase<ThrowStatementElement>
    {
        public ThrowStatementArtifact(ThrowStatementElement statement) : base(statement)
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
