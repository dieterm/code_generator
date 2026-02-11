using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.CodeElements.ViewModels.Statements;

public class ForEachStatementEditViewModel : StatementEditViewModelBase<ForEachStatementArtifact, ForEachStatementElement>
{
    public ForEachStatementEditViewModel()
    {
        VariableNameField = new SingleLineTextFieldModel { Label = "Variable Name", Name = nameof(ForEachStatementArtifact.VariableName) };
        CollectionField = new SingleLineTextFieldModel { Label = "Collection", Name = nameof(ForEachStatementArtifact.Collection) };

        VariableNameField.PropertyChanged += OnFieldChanged;
        CollectionField.PropertyChanged += OnFieldChanged;
    }

    public SingleLineTextFieldModel VariableNameField { get; }
    public SingleLineTextFieldModel CollectionField { get; }

    protected override void LoadFromArtifact()
    {
        if (Artifact == null) return;
        _isLoading = true;
        try
        {
            VariableNameField.Value = Artifact.VariableName;
            CollectionField.Value = Artifact.Collection;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveFields()
    {
        if (Artifact == null) return;
        Artifact.VariableName = VariableNameField.Value as string ?? string.Empty;
        Artifact.Collection = CollectionField.Value as string ?? string.Empty;
    }
}
