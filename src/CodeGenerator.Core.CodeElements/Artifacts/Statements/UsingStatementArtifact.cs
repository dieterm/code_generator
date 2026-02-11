using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class UsingStatementArtifact : StatementArtifactBase<UsingStatementElement>
    {
        public UsingStatementArtifact(UsingStatementElement statement) : base(statement)
        {
            foreach (var child in statement.Body)
            {
                AddChild(StatementArtifactFactory.Create(child));
            }
        }

        public string Resource
        {
            get => StatementElement.Resource;
            set
            {
                if (StatementElement.Resource != value)
                {
                    StatementElement.Resource = value;
                    RaisePropertyChangedEvent(nameof(Resource));
                }
            }
        }

        public bool IsDeclaration
        {
            get => StatementElement.IsDeclaration;
            set
            {
                if (StatementElement.IsDeclaration != value)
                {
                    StatementElement.IsDeclaration = value;
                    RaisePropertyChangedEvent(nameof(IsDeclaration));
                }
            }
        }

        public List<StatementElement> Body => StatementElement.Body;
    }
}
