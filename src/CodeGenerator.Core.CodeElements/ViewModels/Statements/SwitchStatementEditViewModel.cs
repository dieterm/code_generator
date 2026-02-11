using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.CodeElements.ViewModels.Statements;

public class SwitchStatementEditViewModel : StatementEditViewModelBase<SwitchStatementArtifact, SwitchStatementElement>
{
    public SwitchStatementEditViewModel()
    {
        ExpressionField = new SingleLineTextFieldModel { Label = "Expression", Name = nameof(SwitchStatementArtifact.Expression) };
        ExpressionField.PropertyChanged += OnFieldChanged;
    }

    public SingleLineTextFieldModel ExpressionField { get; }

    protected override void LoadFromArtifact()
    {
        if (Artifact == null) return;
        _isLoading = true;
        try
        {
            ExpressionField.Value = Artifact.Expression;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveFields()
    {
        if (Artifact == null) return;
        Artifact.Expression = ExpressionField.Value as string ?? string.Empty;
    }
}
