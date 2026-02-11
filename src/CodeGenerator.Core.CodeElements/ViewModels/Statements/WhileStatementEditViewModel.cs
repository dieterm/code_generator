using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.CodeElements.ViewModels.Statements;

public class WhileStatementEditViewModel : StatementEditViewModelBase<WhileStatementArtifact, WhileStatementElement>
{
    public WhileStatementEditViewModel()
    {
        ConditionField = new SingleLineTextFieldModel { Label = "Condition", Name = nameof(WhileStatementArtifact.Condition) };
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
