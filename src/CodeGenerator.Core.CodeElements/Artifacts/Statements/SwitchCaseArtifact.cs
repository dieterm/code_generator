using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class SwitchCaseArtifact : StatementArtifactBase<SwitchCase>
    {
        public SwitchCaseArtifact(SwitchCase statement) : base(statement)
        {
            foreach (var child in statement.Statements)
            {
                AddChild(StatementArtifactFactory.Create(child));
            }
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

        public List<StatementElement> Statements => StatementElement.Statements;
    }
}
