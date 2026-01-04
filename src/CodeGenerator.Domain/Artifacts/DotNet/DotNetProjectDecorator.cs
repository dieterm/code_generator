namespace CodeGenerator.Domain.Artifacts.DotNet;

using CodeGenerator.Domain.Artifacts.FileSystem;
using CodeGenerator.Domain.Previewers;

/// <summary>
/// Decorator that marks an artifact as a .NET project
/// </summary>
public class DotNetProjectDecorator : DirectoryDecorator
{
    /// <summary>
    /// Project name
    /// </summary>
    public string ProjectName
    {
        get => Artifact?.GetProperty<string>("ProjectName") ?? Artifact?.Name ?? string.Empty;
        set => Artifact?.SetProperty("ProjectName", value);
    }

    /// <summary>
    /// Programming language
    /// </summary>
    public DotNetLanguage Language
    {
        get => Artifact?.GetProperty<DotNetLanguage>("Language") ?? DotNetLanguage.CSharp;
        set => Artifact?.SetProperty("Language", value);
    }

    /// <summary>
    /// Project type
    /// </summary>
    public DotNetProjectType ProjectType
    {
        get => Artifact?.GetProperty<DotNetProjectType>("ProjectType") ?? DotNetProjectType.ClassLibrary;
        set => Artifact?.SetProperty("ProjectType", value);
    }

    /// <summary>
    /// Target framework version
    /// </summary>
    public DotNetFrameworkVersion TargetFramework
    {
        get => Artifact?.GetProperty<DotNetFrameworkVersion>("TargetFramework") ?? DotNetFrameworkVersion.Net8;
        set => Artifact?.SetProperty("TargetFramework", value);
    }

    /// <summary>
    /// NuGet package references
    /// </summary>
    public List<NugetPackageReference> PackageReferences
    {
        get
        {
            var refs = Artifact?.GetProperty<List<NugetPackageReference>>("PackageReferences");
            if (refs == null)
            {
                refs = new List<NugetPackageReference>();
                Artifact?.SetProperty("PackageReferences", refs);
            }
            return refs;
        }
    }

    /// <summary>
    /// Project references
    /// </summary>
    public List<ProjectReference> ProjectReferences
    {
        get
        {
            var refs = Artifact?.GetProperty<List<ProjectReference>>("ProjectReferences");
            if (refs == null)
            {
                refs = new List<ProjectReference>();
                Artifact?.SetProperty("ProjectReferences", refs);
            }
            return refs;
        }
    }

    /// <summary>
    /// Root namespace for the project
    /// </summary>
    public string? RootNamespace
    {
        get => Artifact?.GetProperty<string>("RootNamespace");
        set => Artifact?.SetProperty("RootNamespace", value);
    }

    /// <summary>
    /// Assembly name
    /// </summary>
    public string? AssemblyName
    {
        get => Artifact?.GetProperty<string>("AssemblyName");
        set => Artifact?.SetProperty("AssemblyName", value);
    }

    /// <summary>
    /// Enable nullable reference types
    /// </summary>
    public bool Nullable
    {
        get => Artifact?.GetProperty<bool>("Nullable") ?? true;
        set => Artifact?.SetProperty("Nullable", value);
    }

    /// <summary>
    /// Enable implicit usings
    /// </summary>
    public bool ImplicitUsings
    {
        get => Artifact?.GetProperty<bool>("ImplicitUsings") ?? true;
        set => Artifact?.SetProperty("ImplicitUsings", value);
    }

    /// <summary>
    /// File extension for the project file
    /// </summary>
    public string ProjectFileExtension => Language switch
    {
        DotNetLanguage.CSharp => ".csproj",
        DotNetLanguage.FSharp => ".fsproj",
        DotNetLanguage.VisualBasic => ".vbproj",
        _ => ".csproj"
    };

    /// <summary>
    /// Full path to the project file
    /// </summary>
    public string ProjectFilePath => System.IO.Path.Combine(FullPath, $"{ProjectName}{ProjectFileExtension}");

    /// <summary>
    /// SDK template name for dotnet new
    /// </summary>
    public string SdkTemplateName => ProjectType switch
    {
        DotNetProjectType.ConsoleApp => "console",
        DotNetProjectType.ClassLibrary => "classlib",
        DotNetProjectType.WebApi => "webapi",
        DotNetProjectType.BlazorApp => "blazorserver",
        DotNetProjectType.BlazorWasm => "blazorwasm",
        DotNetProjectType.WinForms => "winforms",
        DotNetProjectType.WinFormsControlLibrary => "winformscontrollib",
        DotNetProjectType.Wpf => "wpf",
        DotNetProjectType.WpfControlLibrary => "wpfcontrollib",
        DotNetProjectType.UnitTest => "mstest",
        DotNetProjectType.WorkerService => "worker",
        DotNetProjectType.Maui => "maui",
        _ => "classlib"
    };

    public override object? CreatePreview()
    {
        if (Artifact == null) return null;
        return new ProjectPreviewModel(Artifact);
    }

    /// <summary>
    /// Add a NuGet package reference
    /// </summary>
    public void AddPackageReference(string packageId, string? version = null)
    {
        PackageReferences.Add(new NugetPackageReference
        {
            PackageId = packageId,
            Version = version
        });
    }

    /// <summary>
    /// Add a project reference
    /// </summary>
    public void AddProjectReference(Artifact project)
    {
        var projectDecorator = project.GetDecorator<DotNetProjectDecorator>();
        if (projectDecorator != null)
        {
            ProjectReferences.Add(new ProjectReference
            {
                ProjectPath = projectDecorator.ProjectFilePath,
                ReferencedProject = project
            });
        }
    }
}
