using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Templates;
using CodeGenerator.Domain.DotNet;

namespace CodeGenerator.TemplateEngines.DotNetProject
{
    public class DotNetProjectTemplate : ITemplate
    {
        public DotNetProjectTemplate(string projectType, DotNetLanguage language, TargetFramework targetFramework)
        {
            DotNetProjectType = projectType ?? throw new ArgumentNullException(nameof(projectType));
            DotNetLanguage = language;
            DotNetTargetFramework = targetFramework ?? throw new ArgumentNullException(nameof(targetFramework));
            TemplateId = $"DotNetProject-{projectType}-{language.DotNetCommandLineArgument}-{targetFramework}";
            Icon = new ResourceManagerTreeNodeIcon($"dotnet-{projectType}-{language.DotNetCommandLineArgument}-template");
        }

        public DotNetProjectTemplate(string id, string projectType, DotNetLanguage language, TargetFramework targetFramework)
        {
            TemplateId = id ?? throw new ArgumentNullException(nameof(id));
            DotNetProjectType = projectType ?? throw new ArgumentNullException(nameof(projectType));
            DotNetLanguage = language;
            DotNetTargetFramework = targetFramework ?? throw new ArgumentNullException(nameof(targetFramework));
            Icon = new ResourceManagerTreeNodeIcon($"dotnet-{projectType}-{language.DotNetCommandLineArgument}-template");
        }

        public string TemplateId { get; }
        /// <summary>
        /// Console parameter to use for the 'dotnet --project-type' argument
        /// eg: "classlib", "winforms"
        /// </summary>
        public string DotNetProjectType { get; set; }
        /// <summary>
        /// Console parameter to use for the 'dotnet --language' argument
        /// </summary>
        public DotNetLanguage DotNetLanguage { get; set; }

        /// <summary>
        /// Console parameter to use for the 'dotnet --framework' argument
        /// eg. "net6.0", "net7.0", "netstandard2.1"
        /// </summary>
        public TargetFramework DotNetTargetFramework { get; set; }

        public TemplateType TemplateType { get { return TemplateType.DotNetProject; } }
        public bool UseCaching { get; set; } = false;

        public ITreeNodeIcon Icon { get;  } 
    }
}