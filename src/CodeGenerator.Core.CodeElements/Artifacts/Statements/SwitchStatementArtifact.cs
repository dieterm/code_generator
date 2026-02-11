using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class SwitchStatementArtifact : StatementArtifactBase<SwitchStatementElement>
    {
        public SwitchStatementArtifact(SwitchStatementElement statement) : base(statement)
        {
            foreach (var switchCase in statement.Cases)
            {
                AddChild(StatementArtifactFactory.Create(switchCase));
            }
            foreach (var child in statement.DefaultStatements)
            {
                AddChild(StatementArtifactFactory.Create(child));
            }
        }

        public string Expression
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

        public List<SwitchCase> Cases => StatementElement.Cases;

        public List<StatementElement> DefaultStatements => StatementElement.DefaultStatements;
    }
}
