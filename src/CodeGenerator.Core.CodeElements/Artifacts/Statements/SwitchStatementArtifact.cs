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
            AddChild(new CompositeStatementArtifact(StatementElement.DefaultStatements, true) { Name = nameof(DefaultStatements) });
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

        public CompositeStatement DefaultStatements => StatementElement.DefaultStatements;
    }
}
