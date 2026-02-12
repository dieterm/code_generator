using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public class TryCatchStatementArtifact : StatementArtifactBase<TryCatchStatementElement>
    {
        public TryCatchStatementArtifact(TryCatchStatementElement statement) : base(statement)
        {
            AddChild(new CompositeStatementArtifact(statement.TryStatements, true) { Name = nameof(TryStatements) });
            AddChild(new CompositeStatementArtifact(statement.FinallyStatements, true) { Name = nameof(FinallyStatements) });

            foreach (var catchBlock in statement.CatchBlocks)
            {
                AddChild(StatementArtifactFactory.Create(catchBlock));
            }

        }

        public CompositeStatement TryStatements => StatementElement.TryStatements;

        public List<CatchBlock> CatchBlocks => StatementElement.CatchBlocks;

        public CompositeStatement FinallyStatements => StatementElement.FinallyStatements;

        public bool HasFinally => StatementElement.HasFinally;
    }
}
