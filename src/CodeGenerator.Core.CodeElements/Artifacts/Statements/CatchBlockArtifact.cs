using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class CatchBlockArtifact : StatementArtifactBase<CatchBlock>
    {
        public CatchBlockArtifact(CatchBlock statement) : base(statement)
        {
            AddChild(new CompositeStatementArtifact(statement.Statements, true) { Name = nameof(Statements) });
        }

        public TypeReference? ExceptionType
        {
            get => StatementElement.ExceptionType;
            set
            {
                if (StatementElement.ExceptionType != value)
                {
                    StatementElement.ExceptionType = value;
                    RaisePropertyChangedEvent(nameof(ExceptionType));
                }
            }
        }

        public string? ExceptionVariable
        {
            get => StatementElement.ExceptionVariable;
            set
            {
                if (StatementElement.ExceptionVariable != value)
                {
                    StatementElement.ExceptionVariable = value;
                    RaisePropertyChangedEvent(nameof(ExceptionVariable));
                }
            }
        }

        public string? WhenFilter
        {
            get => StatementElement.WhenFilter;
            set
            {
                if (StatementElement.WhenFilter != value)
                {
                    StatementElement.WhenFilter = value;
                    RaisePropertyChangedEvent(nameof(WhenFilter));
                }
            }
        }

        public CompositeStatement Statements => StatementElement.Statements;
    }
}
