using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.CodeElements.ViewModels.Statements;

public class ThrowStatementEditViewModel : StatementEditViewModelBase<ThrowStatementArtifact, ThrowStatementElement>
{
    public ThrowStatementEditViewModel()
    {
        ExpressionField = new SingleLineTextFieldModel { Label = "Expression", Name = nameof(ThrowStatementArtifact.Expression) };
        ExpressionField.PropertyChanged += OnFieldChanged;
    }

    public SingleLineTextFieldModel ExpressionField { get; }

    protected override void LoadFromArtifact()
    {
        if (Artifact == null) return;
        _isLoading = true;
        try
        {
            ExpressionField.Value = Artifact.Expression ?? string.Empty;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveFields()
    {
        if (Artifact == null) return;
        Artifact.Expression = string.IsNullOrEmpty(ExpressionField.Value as string) ? null : ExpressionField.Value as string;
    }
}
