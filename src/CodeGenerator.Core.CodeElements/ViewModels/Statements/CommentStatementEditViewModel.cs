using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.CodeElements.ViewModels.Statements;

public class CommentStatementEditViewModel : StatementEditViewModelBase<CommentStatementArtifact, CommentStatement>
{
    public CommentStatementEditViewModel()
    {
        TextField = new SingleLineTextFieldModel { Label = "Text", Name = nameof(CommentStatementArtifact.Text) };
        TextField.PropertyChanged += OnFieldChanged;
    }

    public SingleLineTextFieldModel TextField { get; }

    protected override void LoadFromArtifact()
    {
        if (Artifact == null) return;
        _isLoading = true;
        try
        {
            TextField.Value = Artifact.Text;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveFields()
    {
        if (Artifact == null) return;
        Artifact.Text = TextField.Value as string ?? string.Empty;
    }
}
