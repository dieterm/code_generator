using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class IfStatementArtifact : StatementArtifactBase<IfStatementElement>
    {
        public IfStatementArtifact(IfStatementElement statement) : base(statement)
        {
            foreach (var child in statement.ThenStatements)
            {
                AddChild(StatementArtifactFactory.Create(child));
            }
            foreach (var child in statement.ElseStatements)
            {
                AddChild(StatementArtifactFactory.Create(child));
            }
            foreach (var branch in statement.ElseIfBranches)
            {
                AddChild(StatementArtifactFactory.Create(branch));
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

        public List<StatementElement> ThenStatements => StatementElement.ThenStatements;

        public List<StatementElement> ElseStatements => StatementElement.ElseStatements;

        public List<ElseIfBranch> ElseIfBranches => StatementElement.ElseIfBranches;
    }
}
