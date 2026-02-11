using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class WhileStatementArtifact : StatementArtifactBase<WhileStatementElement>
    {
        public WhileStatementArtifact(WhileStatementElement statement) : base(statement)
        {
            foreach (var child in statement.Body)
            {
                AddChild(StatementArtifactFactory.Create(child));
            }
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

        public List<StatementElement> Body => StatementElement.Body;
    }
}
