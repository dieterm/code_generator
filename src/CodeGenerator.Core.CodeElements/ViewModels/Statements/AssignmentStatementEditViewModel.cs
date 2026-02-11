using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements.Statements;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.CodeElements.ViewModels.Statements;

public class AssignmentStatementEditViewModel : StatementEditViewModelBase<AssignmentStatementArtifact, AssignmentStatement>
{
    public AssignmentStatementEditViewModel()
    {
        LeftField = new SingleLineTextFieldModel { Label = "Left", Name = nameof(AssignmentStatementArtifact.Left) };
        RightField = new SingleLineTextFieldModel { Label = "Right", Name = nameof(AssignmentStatementArtifact.Right) };

        LeftField.PropertyChanged += OnFieldChanged;
        RightField.PropertyChanged += OnFieldChanged;
    }

    public SingleLineTextFieldModel LeftField { get; }
    public SingleLineTextFieldModel RightField { get; }

    protected override void LoadFromArtifact()
    {
        if (Artifact == null) return;
        _isLoading = true;
        try
        {
            LeftField.Value = Artifact.Left;
            RightField.Value = Artifact.Right;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveFields()
    {
        if (Artifact == null) return;
        Artifact.Left = LeftField.Value as string ?? string.Empty;
        Artifact.Right = RightField.Value as string ?? string.Empty;
    }
}
