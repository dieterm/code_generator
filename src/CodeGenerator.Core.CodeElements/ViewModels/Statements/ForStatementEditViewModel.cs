using CodeGenerator.Core.CodeElements.Artifacts.Statements;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.UserControls.ViewModels;

namespace CodeGenerator.Core.CodeElements.ViewModels.Statements;

public class ForStatementEditViewModel : StatementEditViewModelBase<ForStatementArtifact, ForStatementElement>
{
    public ForStatementEditViewModel()
    {
        InitializerField = new SingleLineTextFieldModel { Label = "Initializer", Name = nameof(ForStatementArtifact.Initializer) };
        ConditionField = new SingleLineTextFieldModel { Label = "Condition", Name = nameof(ForStatementArtifact.Condition) };
        IncrementerField = new SingleLineTextFieldModel { Label = "Incrementer", Name = nameof(ForStatementArtifact.Incrementer) };

        InitializerField.PropertyChanged += OnFieldChanged;
        ConditionField.PropertyChanged += OnFieldChanged;
        IncrementerField.PropertyChanged += OnFieldChanged;
    }

    public SingleLineTextFieldModel InitializerField { get; }
    public SingleLineTextFieldModel ConditionField { get; }
    public SingleLineTextFieldModel IncrementerField { get; }

    protected override void LoadFromArtifact()
    {
        if (Artifact == null) return;
        _isLoading = true;
        try
        {
            InitializerField.Value = Artifact.Initializer ?? string.Empty;
            ConditionField.Value = Artifact.Condition ?? string.Empty;
            IncrementerField.Value = Artifact.Incrementer ?? string.Empty;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveFields()
    {
        if (Artifact == null) return;
        Artifact.Initializer = string.IsNullOrEmpty(InitializerField.Value as string) ? null : InitializerField.Value as string;
        Artifact.Condition = string.IsNullOrEmpty(ConditionField.Value as string) ? null : ConditionField.Value as string;
        Artifact.Incrementer = string.IsNullOrEmpty(IncrementerField.Value as string) ? null : IncrementerField.Value as string;
    }
}
