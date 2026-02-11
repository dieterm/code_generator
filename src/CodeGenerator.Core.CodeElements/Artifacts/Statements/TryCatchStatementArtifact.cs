using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class TryCatchStatementArtifact : StatementArtifactBase<TryCatchStatementElement>
    {
        public TryCatchStatementArtifact(TryCatchStatementElement statement) : base(statement)
        {
            foreach (var child in statement.TryStatements)
            {
                AddChild(StatementArtifactFactory.Create(child));
            }
            foreach (var catchBlock in statement.CatchBlocks)
            {
                AddChild(StatementArtifactFactory.Create(catchBlock));
            }
            foreach (var child in statement.FinallyStatements)
            {
                AddChild(StatementArtifactFactory.Create(child));
            }
        }

        public List<StatementElement> TryStatements => StatementElement.TryStatements;

        public List<CatchBlock> CatchBlocks => StatementElement.CatchBlocks;

        public List<StatementElement> FinallyStatements => StatementElement.FinallyStatements;

        public bool HasFinally => StatementElement.HasFinally;
    }
}
