using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class ElseIfBranchArtifact : StatementArtifactBase<ElseIfBranch>
    {
        public ElseIfBranchArtifact(ElseIfBranch statement) : base(statement)
        {
            foreach (var child in statement.Statements)
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

        public List<StatementElement> Statements => StatementElement.Statements;
    }
}
