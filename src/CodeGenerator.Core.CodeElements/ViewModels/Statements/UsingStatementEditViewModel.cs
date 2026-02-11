using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.CodeElements.ViewModels.Statements;

public class UsingStatementEditViewModel : StatementEditViewModelBase<UsingStatementArtifact, UsingStatementElement>
{
    public UsingStatementEditViewModel()
    {
        ResourceField = new SingleLineTextFieldModel { Label = "Resource", Name = nameof(UsingStatementArtifact.Resource) };
        IsDeclarationField = new BooleanFieldModel { Label = "Is Declaration", Name = nameof(UsingStatementArtifact.IsDeclaration) };

        ResourceField.PropertyChanged += OnFieldChanged;
        IsDeclarationField.PropertyChanged += OnFieldChanged;
    }

    public SingleLineTextFieldModel ResourceField { get; }
    public BooleanFieldModel IsDeclarationField { get; }

    protected override void LoadFromArtifact()
    {
        if (Artifact == null) return;
        _isLoading = true;
        try
        {
            ResourceField.Value = Artifact.Resource;
            IsDeclarationField.Value = Artifact.IsDeclaration;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveFields()
    {
        if (Artifact == null) return;
        Artifact.Resource = ResourceField.Value as string ?? string.Empty;
        Artifact.IsDeclaration = IsDeclarationField.Value is bool b && b;
    }
}
