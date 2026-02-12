using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class WhileStatementArtifact : StatementArtifactBase<WhileStatementElement>
    {
        public WhileStatementArtifact(WhileStatementElement statement) : base(statement)
        {
            AddChild(new CompositeStatementArtifact(statement.Body, true) { Name = nameof(Body) });
        }

        public string Condition
        {
            get => StatementElement.Condition;
            set
            {
                if (StatementElement.Condition != value)
                {
                    StatementElement.Condition = value;
                    RaisePropertyChangedEvent(nameof(Condition));
                }
            }
        }

        public CompositeStatement Body => StatementElement.Body;
    }
}
