using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;

namespace CodeGenerator.Core.CodeElements.ViewModels.Statements;

public class CompositeStatementEditViewModel : StatementEditViewModelBase<CompositeStatementArtifact, CompositeStatement>
{
    protected override void LoadFromArtifact()
    {
        // CompositeStatement has no editable scalar fields; children are managed via the tree
    }

    protected override void SaveFields()
    {
        // No scalar fields to save
    }
}
