using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.CodeElements.ViewModels.Statements;

public class SwitchCaseEditViewModel : StatementEditViewModelBase<SwitchCaseArtifact, SwitchCase>
{
    public SwitchCaseEditViewModel()
    {
        PatternField = new SingleLineTextFieldModel { Label = "Pattern", Name = nameof(SwitchCaseArtifact.Pattern) };
        WhenClauseField = new SingleLineTextFieldModel { Label = "When Clause", Name = nameof(SwitchCaseArtifact.WhenClause) };

        PatternField.PropertyChanged += OnFieldChanged;
        WhenClauseField.PropertyChanged += OnFieldChanged;
    }

    public SingleLineTextFieldModel PatternField { get; }
    public SingleLineTextFieldModel WhenClauseField { get; }

    protected override void LoadFromArtifact()
    {
        if (Artifact == null) return;
        _isLoading = true;
        try
        {
            PatternField.Value = Artifact.Pattern ?? string.Empty;
            WhenClauseField.Value = Artifact.WhenClause ?? string.Empty;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveFields()
    {
        if (Artifact == null) return;
        Artifact.Pattern = string.IsNullOrEmpty(PatternField.Value as string) ? null : PatternField.Value as string;
        Artifact.WhenClause = string.IsNullOrEmpty(WhenClauseField.Value as string) ? null : WhenClauseField.Value as string;
    }
}
