using CodeGenerator.Domain.CodeElements.Statements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class AssignmentStatementArtifact : StatementArtifactBase<AssignmentStatement>
    {
        public AssignmentStatementArtifact(AssignmentStatement statement) : base(statement)
        {
        }

        public string Left
        {
            get => StatementElement.Left;
            set
            {
                if (StatementElement.Left != value)
                {
                    StatementElement.Left = value;
                    RaisePropertyChangedEvent(nameof(Left));
                }
            }
        }

        public string Right
        {
            get => StatementElement.Right;
            set
            {
                if (StatementElement.Right != value)
                {
                    StatementElement.Right = value;
                    RaisePropertyChangedEvent(nameof(Right));
                }
            }
        }
    }
}
