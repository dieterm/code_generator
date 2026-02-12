using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class IfStatementArtifact : StatementArtifactBase<IfStatementElement>
    {
        public IfStatementArtifact(IfStatementElement statement) : base(statement)
        {
            AddChild(new CompositeStatementArtifact(StatementElement.ThenStatements, true) { Name = nameof(ThenStatements) });
            AddChild(new CompositeStatementArtifact(StatementElement.ElseStatements, true) { Name = nameof(ElseStatements) });
            
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

        public CompositeStatement ThenStatements => StatementElement.ThenStatements;

        public CompositeStatement ElseStatements => StatementElement.ElseStatements;

        public List<ElseIfBranch> ElseIfBranches => StatementElement.ElseIfBranches;
    }
}
