using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.CodeElements.ViewModels.Statements;

public class RawStatementEditViewModel : StatementEditViewModelBase<RawStatementArtifact, RawStatementElement>
{
    public RawStatementEditViewModel()
    {
        CodeField = new SingleLineTextFieldModel { Label = "Code", Name = nameof(RawStatementArtifact.Code) };
        CodeField.PropertyChanged += OnFieldChanged;
    }

    public SingleLineTextFieldModel CodeField { get; }

    protected override void LoadFromArtifact()
    {
        if (Artifact == null) return;
        _isLoading = true;
        try
        {
            CodeField.Value = Artifact.Code;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveFields()
    {
        if (Artifact == null) return;
        Artifact.Code = CodeField.Value as string ?? string.Empty;
    }
}
