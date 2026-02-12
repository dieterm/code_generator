using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class ElseIfBranchArtifact : StatementArtifactBase<ElseIfBranch>
    {
        public ElseIfBranchArtifact(ElseIfBranch statement) : base(statement)
        {
            AddChild(new CompositeStatementArtifact(statement.Statements, true) { Name = nameof(Statements) });
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

        public CompositeStatement Statements => StatementElement.Statements;
    }
}
