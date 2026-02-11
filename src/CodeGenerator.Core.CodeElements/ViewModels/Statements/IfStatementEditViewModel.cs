using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.CodeElements.ViewModels.Statements;

public class IfStatementEditViewModel : StatementEditViewModelBase<IfStatementArtifact, IfStatementElement>
{
    public IfStatementEditViewModel()
    {
        ConditionField = new SingleLineTextFieldModel { Label = "Condition", Name = nameof(IfStatementArtifact.Condition) };
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
