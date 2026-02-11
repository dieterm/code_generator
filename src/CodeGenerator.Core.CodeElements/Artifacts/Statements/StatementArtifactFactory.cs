using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.CodeElements.Statements;

namespace CodeGenerator.Core.CodeElements.Artifacts.Statements
{
    public static class StatementArtifactFactory
    {
        public static CodeElementArtifactBase Create(StatementElement statementElement)
        {
            return statementElement switch
            {
                CompositeStatement composite => new CompositeStatementArtifact(composite),
                CommentStatement comment => new CommentStatementArtifact(comment),
                RawStatementElement raw => new RawStatementArtifact(raw),
                AssignmentStatement assignment => new AssignmentStatementArtifact(assignment),
                CatchBlock catchBlock => new CatchBlockArtifact(catchBlock),
                ElseIfBranch elseIfBranch => new ElseIfBranchArtifact(elseIfBranch),
                ForEachStatementElement forEach => new ForEachStatementArtifact(forEach),
                ForStatementElement forStatement => new ForStatementArtifact(forStatement),
                IfStatementElement ifStatement => new IfStatementArtifact(ifStatement),
                ReturnStatementElement returnStatement => new ReturnStatementArtifact(returnStatement),
                SwitchCase switchCase => new SwitchCaseArtifact(switchCase),
                SwitchStatementElement switchStatement => new SwitchStatementArtifact(switchStatement),
                ThrowStatementElement throwStatement => new ThrowStatementArtifact(throwStatement),
                TryCatchStatementElement tryCatch => new TryCatchStatementArtifact(tryCatch),
                UsingStatementElement usingStatement => new UsingStatementArtifact(usingStatement),
                WhileStatementElement whileStatement => new WhileStatementArtifact(whileStatement),
                _ => throw new NotSupportedException($"Unsupported statement type: {statementElement.GetType().Name}")
            };
        }
    }
}