using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.CodeElements.ViewModels.Statements;

public class ElseIfBranchEditViewModel : StatementEditViewModelBase<ElseIfBranchArtifact, ElseIfBranch>
{
    public ElseIfBranchEditViewModel()
    {
        ConditionField = new SingleLineTextFieldModel { Label = "Condition", Name = nameof(ElseIfBranchArtifact.Condition) };
        ConditionField.PropertyChanged += OnFieldChanged;
    }

    public SingleLineTextFieldModel ConditionField { get; }

    protected override void LoadFromArtifact()
    {
        if (Artifact == null) return;
        _isLoading = true;
        try
        {
            ConditionField.Value = Artifact.Condition;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveFields()
    {
        if (Artifact == null) return;
        Artifact.Condition = ConditionField.Value as string ?? string.Empty;
    }
}
