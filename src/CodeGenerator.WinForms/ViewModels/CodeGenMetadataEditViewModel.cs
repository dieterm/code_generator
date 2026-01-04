using CodeGenerator.Core.Models.Schema;
using CodeGenerator.Shared.ViewModels;

namespace CodeGenerator.WinForms.ViewModels;

public class CodeGenMetadataEditViewModel : ValidationViewModelBase
{
    private CodeGenMetadata? _metadata;
    private string _namespace = string.Empty;
    private string _outputPath = string.Empty;
    private string _targetLanguage = "CSharp";
    private string _presentationTechnology = string.Empty;
    private string _dataLayerTechnology = "EntityFrameworkCore";
    private bool _useDependencyInjection = true;
    private bool _useLogging = true;
    private bool _useConfiguration = true;

    public CodeGenMetadataEditViewModel()
    {
    }

    public CodeGenMetadataEditViewModel(CodeGenMetadata metadata)
    {
        LoadFromMetadata(metadata);
    }

    public string Namespace
    {
        get => _namespace;
        set => SetProperty(ref _namespace, value);
    }

    public string OutputPath
    {
        get => _outputPath;
        set => SetProperty(ref _outputPath, value);
    }

    public string TargetLanguage
    {
        get => _targetLanguage;
        set => SetProperty(ref _targetLanguage, value);
    }

    public string PresentationTechnology
    {
        get => _presentationTechnology;
        set => SetProperty(ref _presentationTechnology, value);
    }

    public string DataLayerTechnology
    {
        get => _dataLayerTechnology;
        set => SetProperty(ref _dataLayerTechnology, value);
    }

    public bool UseDependencyInjection
    {
        get => _useDependencyInjection;
        set => SetProperty(ref _useDependencyInjection, value);
    }

    public bool UseLogging
    {
        get => _useLogging;
        set => SetProperty(ref _useLogging, value);
    }

    public bool UseConfiguration
    {
        get => _useConfiguration;
        set => SetProperty(ref _useConfiguration, value);
    }

    public void LoadFromMetadata(CodeGenMetadata metadata)
    {
        _metadata = metadata;
        Namespace = metadata.Namespace ?? string.Empty;
        OutputPath = metadata.OutputPath ?? string.Empty;
        TargetLanguage = metadata.TargetLanguage ?? "CSharp";
        PresentationTechnology = metadata.PresentationTechnology ?? string.Empty;
        DataLayerTechnology = metadata.DataLayerTechnology ?? "EntityFrameworkCore";
        UseDependencyInjection = metadata.UseDependencyInjection;
        UseLogging = metadata.UseLogging;
        UseConfiguration = metadata.UseConfiguration;
    }

    public void UpdateMetadata()
    {
        if (_metadata != null)
        {
            _metadata.Namespace = string.IsNullOrWhiteSpace(Namespace) ? null : Namespace;
            _metadata.OutputPath = string.IsNullOrWhiteSpace(OutputPath) ? null : OutputPath;
            _metadata.TargetLanguage = TargetLanguage;
            _metadata.PresentationTechnology = string.IsNullOrWhiteSpace(PresentationTechnology) ? null : PresentationTechnology;
            _metadata.DataLayerTechnology = DataLayerTechnology;
            _metadata.UseDependencyInjection = UseDependencyInjection;
            _metadata.UseLogging = UseLogging;
            _metadata.UseConfiguration = UseConfiguration;
        }
    }

    public override bool Validate()
    {
        ClearValidationErrors();

        if (string.IsNullOrWhiteSpace(Namespace))
        {
            AddValidationError(nameof(Namespace), "Namespace is required.");
        }

        if (string.IsNullOrWhiteSpace(TargetLanguage))
        {
            AddValidationError(nameof(TargetLanguage), "Target Language is required.");
        }

        return IsValid;
    }
}
