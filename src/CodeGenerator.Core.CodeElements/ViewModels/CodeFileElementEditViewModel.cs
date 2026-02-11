using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.ProgrammingLanguages;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.ViewModels;

/// <summary>
/// ViewModel for editing CodeFileElementArtifact properties
/// </summary>
public class CodeFileElementEditViewModel : CodeElementEditViewModel
{
    private CodeFileElementArtifact? _codeFile;

    public CodeFileElementEditViewModel()
    {
        FileHeaderField = new SingleLineTextFieldModel { Label = "File Header", Name = nameof(CodeFileElementArtifact.FileHeader) };
        NullableContextField = new BooleanFieldModel { Label = "Nullable Context", Name = nameof(CodeFileElementArtifact.NullableContext) };
        UseImplicitUsingsField = new BooleanFieldModel { Label = "Use Implicit Usings", Name = nameof(CodeFileElementArtifact.UseImplicitUsings) };
        LanguageField = new ComboboxFieldModel { Label = "Language", Name = nameof(CodeFileElementArtifact.Language) };

        InitializeLanguageItems();

        FileHeaderField.PropertyChanged += OnFieldChanged;
        NullableContextField.PropertyChanged += OnFieldChanged;
        UseImplicitUsingsField.PropertyChanged += OnFieldChanged;
        LanguageField.PropertyChanged += OnComboboxFieldChanged;
    }

    /// <summary>
    /// The code file element being edited
    /// </summary>
    public CodeFileElementArtifact? CodeFile
    {
        get => _codeFile;
        set
        {
            _codeFile = value;
            SetBaseArtifact(value);
            LoadFromCodeFile();
        }
    }

    public SingleLineTextFieldModel FileHeaderField { get; }
    public BooleanFieldModel NullableContextField { get; }
    public BooleanFieldModel UseImplicitUsingsField { get; }
    public ComboboxFieldModel LanguageField { get; }

    private void InitializeLanguageItems()
    {
        var items = new List<ComboboxItem>();
        foreach (var language in ProgrammingLanguages.All)
        {
            items.Add(new ComboboxItem
            {
                DisplayName = language.Name,
                Value = language
            });
        }
        LanguageField.Items = items;
    }

    protected override void OnBaseArtifactPropertyChanged(string? propertyName)
    {
        base.OnBaseArtifactPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(CodeFileElementArtifact.FileHeader):
                FileHeaderField.Value = _codeFile?.FileHeader;
                break;
            case nameof(CodeFileElementArtifact.NullableContext):
                NullableContextField.Value = _codeFile?.NullableContext;
                break;
            case nameof(CodeFileElementArtifact.UseImplicitUsings):
                UseImplicitUsingsField.Value = _codeFile?.UseImplicitUsings;
                break;
        }
    }

    private void LoadFromCodeFile()
    {
        if (_codeFile == null) return;

        _isLoading = true;
        try
        {
            LoadBaseFields();
            FileHeaderField.Value = _codeFile.FileHeader;
            NullableContextField.Value = _codeFile.NullableContext;
            UseImplicitUsingsField.Value = _codeFile.UseImplicitUsings;

            var selectedLanguage = LanguageField.Items
                .FirstOrDefault(i => i.Value is ProgrammingLanguage lang && lang.Id == _codeFile.Language.Id);
            LanguageField.SelectedItem = selectedLanguage;
        }
        finally
        {
            _isLoading = false;
        }
    }

    protected override void SaveDerivedFields()
    {
        if (_codeFile == null) return;

        _codeFile.FileHeader = FileHeaderField.Value as string;
        _codeFile.NullableContext = NullableContextField.Value is bool nullable ? nullable : null;
        _codeFile.UseImplicitUsings = UseImplicitUsingsField.Value is bool useImplicit && useImplicit;

        if (LanguageField.SelectedItem?.Value is ProgrammingLanguage language)
        {
            _codeFile.Language = language;
        }
    }
}
