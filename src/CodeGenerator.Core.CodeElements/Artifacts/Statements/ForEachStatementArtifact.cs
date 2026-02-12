using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class ForEachStatementArtifact : StatementArtifactBase<ForEachStatementElement>
    {
        public ForEachStatementArtifact(ForEachStatementElement statement) : base(statement)
        {
            AddChild(new CompositeStatementArtifact(statement.Body, true) { Name = nameof(Body) });
        }

        public TypeReference? VariableType
        {
            get => StatementElement.VariableType;
            set
            {
                if (StatementElement.VariableType != value)
                {
                    StatementElement.VariableType = value;
                    RaisePropertyChangedEvent(nameof(VariableType));
                }
            }
        }

        public string VariableName
        {
            get => StatementElement.VariableName;
            set
            {
                if (StatementElement.VariableName != value)
                {
                    StatementElement.VariableName = value;
                    RaisePropertyChangedEvent(nameof(VariableName));
                }
            }
        }

        public string Collection
        {
            get => StatementElement.Collection;
            set
            {
                if (StatementElement.Collection != value)
                {
                    StatementElement.Collection = value;
                    RaisePropertyChangedEvent(nameof(Collection));
                }
            }
        }

        public CompositeStatement Body => StatementElement.Body;
    }
}
