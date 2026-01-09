using CodeGenerator.Core.Templates;
using CodeGenerator.Domain.DotNet;

namespace CodeGenerator.TemplateEngines.DotNetProject
{
    public class DotNetProjectTemplate : ITemplate
    {
        public DotNetProjectTemplate(string id, string projectType, DotNetLanguage language, string targetFramework)
        {
            TemplateId = id ?? throw new ArgumentNullException(nameof(id));
            DotNetProjectType = projectType ?? throw new ArgumentNullException(nameof(projectType));
            DotNetLanguage = language;
            DotNetTargetFramework = targetFramework ?? throw new ArgumentNullException(nameof(targetFramework));
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
        public string DotNetTargetFramework { get; set; }

        public TemplateType TemplateType { get { return TemplateType.DotNetProject; } }
    }
}