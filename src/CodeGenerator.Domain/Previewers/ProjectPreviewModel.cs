namespace CodeGenerator.Domain.Previewers;

using CodeGenerator.Domain.Artifacts;
using CodeGenerator.Domain.Artifacts.DotNet;

/// <summary>
/// Model for project preview
/// </summary>
public class ProjectPreviewModel
{
    public string ProjectName { get; }
    public string ProjectType { get; }
    public string Language { get; }
    public string TargetFramework { get; }
    public List<string> PackageReferences { get; } = new();
    public List<string> ProjectReferences { get; } = new();
    public DirectoryPreviewModel? FileStructure { get; }

    public ProjectPreviewModel(Artifact artifact)
    {
        var decorator = artifact.GetDecorator<DotNetProjectDecorator>();

        ProjectName = decorator?.ProjectName ?? artifact.Name;
        ProjectType = decorator?.ProjectType.ToString() ?? "Unknown";
        Language = decorator?.Language.ToString() ?? "CSharp";
        TargetFramework = decorator?.TargetFramework.VersionString ?? "net8.0";

        if (decorator != null)
        {
            foreach (var package in decorator.PackageReferences)
            {
                PackageReferences.Add($"{package.PackageId} ({package.Version ?? "latest"})");
            }

            foreach (var project in decorator.ProjectReferences)
            {
                ProjectReferences.Add(project.ProjectPath);
            }
        }

        FileStructure = new DirectoryPreviewModel(artifact);
    }
}
