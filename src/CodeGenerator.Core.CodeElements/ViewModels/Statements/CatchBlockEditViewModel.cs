using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.CodeElements.ViewModels.Statements;

public class CatchBlockEditViewModel : StatementEditViewModelBase<CatchBlockArtifact, CatchBlock>
{
    public CatchBlockEditViewModel()
    {
        ExceptionVariableField = new SingleLineTextFieldModel { Label = "Exception Variable", Name = nameof(CatchBlockArtifact.ExceptionVariable) };
        WhenFilterField = new SingleLineTextFieldModel { Label = "When Filter", Name = nameof(CatchBlockArtifact.WhenFilter) };

        ExceptionVariableField.PropertyChanged += OnFieldChanged;
        WhenFilterField.PropertyChanged += OnFieldChanged;
    }

    public SingleLineTextFieldModel ExceptionVariableField { get; }
    public SingleLineTextFieldModel WhenFilterField { get; }

    protected override void LoadFromArtifact()
    {
        if (Artifact == null) return;
        _isLoading = true;
        try
        {
            ExceptionVariableField.Value = Artifact.ExceptionVariable ?? string.Empty;
            WhenFilterField.Value = Artifact.WhenFilter ?? string.Empty;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveFields()
    {
        if (Artifact == null) return;
        Artifact.ExceptionVariable = string.IsNullOrEmpty(ExceptionVariableField.Value as string) ? null : ExceptionVariableField.Value as string;
        Artifact.WhenFilter = string.IsNullOrEmpty(WhenFilterField.Value as string) ? null : WhenFilterField.Value as string;
    }
}
