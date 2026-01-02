using CodeGenerator.Core.Models.Configuration;

namespace CodeGenerator.WinForms.ViewModels;

/// <summary>
/// ViewModel for SettingsForm
/// </summary>
public class SettingsViewModel : ViewModelBase
{
    private string _solutionRootFolder = string.Empty;
    private string _rootNamespace = string.Empty;
    private string _templateFolder = string.Empty;
    private string _outputFolder = string.Empty;
    private string _targetFramework = "net8.0";
    private bool _overwriteExisting;
    private bool _createBackup = true;

    public string SolutionRootFolder
    {
        get => _solutionRootFolder;
        set => SetProperty(ref _solutionRootFolder, value);
    }

    public string RootNamespace
    {
        get => _rootNamespace;
        set => SetProperty(ref _rootNamespace, value);
    }

    public string TemplateFolder
    {
        get => _templateFolder;
        set => SetProperty(ref _templateFolder, value);
    }

    public string OutputFolder
    {
        get => _outputFolder;
        set => SetProperty(ref _outputFolder, value);
    }

    public string TargetFramework
    {
        get => _targetFramework;
        set => SetProperty(ref _targetFramework, value);
    }

    public bool OverwriteExisting
    {
        get => _overwriteExisting;
        set => SetProperty(ref _overwriteExisting, value);
    }

    public bool CreateBackup
    {
        get => _createBackup;
        set => SetProperty(ref _createBackup, value);
    }

    /// <summary>
    /// Generator configurations
    /// </summary>
    public Dictionary<string, GeneratorConfiguration> Generators { get; private set; } = new();

    /// <summary>
    /// NuGet packages
    /// </summary>
    public List<NuGetPackageReference> NuGetPackages { get; private set; } = new();

    public SettingsViewModel()
    {
    }

    public SettingsViewModel(GeneratorSettings settings)
    {
        LoadFromSettings(settings);
    }

    public void LoadFromSettings(GeneratorSettings settings)
    {
        SolutionRootFolder = settings.SolutionRootFolder ?? string.Empty;
        RootNamespace = settings.RootNamespace ?? string.Empty;
        TemplateFolder = settings.TemplateFolder ?? string.Empty;
        OutputFolder = settings.OutputFolder ?? string.Empty;
        TargetFramework = settings.TargetFramework ?? "net8.0";
        OverwriteExisting = settings.OverwriteExisting;
        CreateBackup = settings.CreateBackup;
        Generators = settings.Generators ?? new Dictionary<string, GeneratorConfiguration>();
        NuGetPackages = settings.NuGetPackages ?? new List<NuGetPackageReference>();
    }

    public GeneratorSettings ToGeneratorSettings()
    {
        return new GeneratorSettings
        {
            SolutionRootFolder = SolutionRootFolder,
            RootNamespace = RootNamespace,
            TemplateFolder = TemplateFolder,
            OutputFolder = OutputFolder,
            TargetFramework = TargetFramework,
            OverwriteExisting = OverwriteExisting,
            CreateBackup = CreateBackup,
            Generators = Generators,
            NuGetPackages = NuGetPackages
        };
    }

    public override bool Validate()
    {
        ClearValidationErrors();
        // Settings are optional, no required fields
        return IsValid;
    }
}
