using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class ForEachStatementArtifact : StatementArtifactBase<ForEachStatementElement>
    {
        public ForEachStatementArtifact(ForEachStatementElement statement) : base(statement)
        {
            foreach (var child in statement.Body)
            {
                AddChild(StatementArtifactFactory.Create(child));
            }
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

        public List<StatementElement> Body => StatementElement.Body;
    }
}
