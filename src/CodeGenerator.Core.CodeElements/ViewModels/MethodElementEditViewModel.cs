using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

public class MethodElementEditViewModel : CodeElementEditViewModel<MethodElement>
{
    private MethodElementArtifact? _artifact;

    public MethodElementEditViewModel()
    {
        ReturnTypeField = new SingleLineTextFieldModel { Label = "Return Type", Name = nameof(MethodElementArtifact.ReturnTypeName) };
        IsExpressionBodiedField = new BooleanFieldModel { Label = "Is Expression Bodied", Name = nameof(MethodElementArtifact.IsExpressionBodied) };
        ExpressionBodyField = new SingleLineTextFieldModel { Label = "Expression Body", Name = nameof(MethodElementArtifact.ExpressionBody) };
        IsExtensionMethodField = new BooleanFieldModel { Label = "Is Extension Method", Name = nameof(MethodElementArtifact.IsExtensionMethod) };

        ReturnTypeField.PropertyChanged += OnFieldChanged;
        IsExpressionBodiedField.PropertyChanged += OnFieldChanged;
        ExpressionBodyField.PropertyChanged += OnFieldChanged;
        IsExtensionMethodField.PropertyChanged += OnFieldChanged;
    }

    public MethodElementArtifact? Artifact
    {
        get => _artifact;
        set
        {
            _artifact = value;
            SetBaseArtifact(value);
            LoadFromArtifact();
        }
    }

    public SingleLineTextFieldModel ReturnTypeField { get; }
    public BooleanFieldModel IsExpressionBodiedField { get; }
    public SingleLineTextFieldModel ExpressionBodyField { get; }
    public BooleanFieldModel IsExtensionMethodField { get; }

    private void LoadFromArtifact()
    {
        if (_artifact == null) return;
        _isLoading = true;
        try
        {
            LoadBaseFields();
            ReturnTypeField.Value = _artifact.ReturnTypeName;
            IsExpressionBodiedField.Value = _artifact.IsExpressionBodied;
            ExpressionBodyField.Value = _artifact.ExpressionBody ?? string.Empty;
            IsExtensionMethodField.Value = _artifact.IsExtensionMethod;
        }
        finally { _isLoading = false; }
    }

    protected override void SaveDerivedFields()
    {
        if (_artifact == null) return;
        _artifact.ReturnTypeName = ReturnTypeField.Value as string ?? "void";
        // Body is a CompositeStatement and is read-only; editing individual statements is not supported here
        _artifact.IsExpressionBodied = IsExpressionBodiedField.Value is bool eb && eb;
        _artifact.ExpressionBody = string.IsNullOrEmpty(ExpressionBodyField.Value as string) ? null : ExpressionBodyField.Value as string;
        _artifact.IsExtensionMethod = IsExtensionMethodField.Value is bool em && em;
    }
}
