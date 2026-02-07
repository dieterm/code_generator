using CodeGenerator.Core.Templates;
using CodeGenerator.Domain.DotNet;

namespace CodeGenerator.TemplateEngines.DotNetProject
{
    public class DotNetProjectTemplateInstance : ITemplateInstance
    {
        private readonly DotNetProjectTemplate _template;
        public ITemplate Template { get { return _template; } }
        public string ProjectName { get; private set; }
        /// <summary>
        /// If set, specifies the output directory for the project.
        /// If null, a temporary directory will be used.
        /// </summary>
        public string? OutputDirectory { get; set; } = null;
        /// <summary>
        /// NuGet packages included
        /// </summary>
        public List<NuGetPackage> Packages { get; private set; } = new();

        /// <summary>
        /// Returns the project file name, e.g. 'MyProject.csproj'
        /// </summary>
        /// <returns></returns>
        public string GetProjectFileName()
        {
            return $"{ProjectName}.{_template.DotNetLanguage.ProjectFileExtension}";
        }

        /// <summary>
        /// Project references
        /// </summary>
        public List<DotNetProjectReference> ProjectReferences { get; } = new();

        public DotNetProjectTemplateInstance(DotNetProjectTemplate template, string projectName)
        {
            _template = template ?? throw new ArgumentNullException(nameof(template));
            ProjectName = projectName ?? throw new ArgumentNullException(nameof(projectName));
           // TargetFolder = targetFolder ?? throw new ArgumentNullException(nameof(targetFolder));
        }
        public Dictionary<string, object?> Parameters { get; } = new Dictionary<string, object?>();
        public void SetParameter(string key, object? value)
        {
            switch (key)
            {
                case nameof(ProjectName):
                    if (value is string projectName)
                    {
                        ProjectName = projectName;
                        //throw new InvalidOperationException("ProjectName is read-only and cannot be changed.");
                    }
                    else
                    {
                        throw new ArgumentException($"Value for parameter '{key}' must be of type string.", nameof(value));
                    }
                    break;
                case nameof(Packages):
                    if (value is IEnumerable<NuGetPackage> packages)
                    {
                        Packages.Clear();
                        Packages.AddRange(packages);
                    }
                    else
                    {
                        throw new ArgumentException($"Value for parameter '{key}' must be of type IEnumerable<NuGetPackage>.", nameof(value));
                    }
                    break;
                default:
                    Parameters[key] = value;
                    break;
            }
        }
    }
}