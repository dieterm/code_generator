using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class SwitchCaseArtifact : StatementArtifactBase<SwitchCase>
    {
        public SwitchCaseArtifact(SwitchCase statement) : base(statement)
        {
            AddChild(new CompositeStatementArtifact(StatementElement.Statements, true) { Name = nameof(Statements) });
        }

        public List<string> Labels => StatementElement.Labels;

        public string? Pattern
        {
            get => StatementElement.Pattern;
            set
            {
                if (StatementElement.Pattern != value)
                {
                    StatementElement.Pattern = value;
                    RaisePropertyChangedEvent(nameof(Pattern));
                }
            }
        }

        public string? WhenClause
        {
            get => StatementElement.WhenClause;
            set
            {
                if (StatementElement.WhenClause != value)
                {
                    StatementElement.WhenClause = value;
                    RaisePropertyChangedEvent(nameof(WhenClause));
                }
            }
        }

        public CompositeStatement Statements => StatementElement.Statements;
    }
}
