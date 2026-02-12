using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class ForStatementArtifact : StatementArtifactBase<ForStatementElement>
    {
        public ForStatementArtifact(ForStatementElement statement) : base(statement)
        {
            AddChild(new CompositeStatementArtifact(statement.Body, true) { Name = nameof(Body) });
        }

        public string? Initializer
        {
            get => StatementElement.Initializer;
            set
            {
                if (StatementElement.Initializer != value)
                {
                    StatementElement.Initializer = value;
                    RaisePropertyChangedEvent(nameof(Initializer));
                }
            }
        }

        public string? Condition
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

        public string? Incrementer
        {
            get => StatementElement.Incrementer;
            set
            {
                if (StatementElement.Incrementer != value)
                {
                    StatementElement.Incrementer = value;
                    RaisePropertyChangedEvent(nameof(Incrementer));
                }
            }
        }

        public CompositeStatement Body => StatementElement.Body;
    }
}
