using CodeGenerator.Core.Templates;
using CodeGenerator.Domain.DotNet;

namespace CodeGenerator.TemplateEngines.DotNetProject
{
    public class DotNetProjectTemplateInstance : ITemplateInstance
    {
        private readonly DotNetProjectTemplate _template;
        public ITemplate Template { get { return _template; } }
        public string ProjectName { get; }
        
        /// <summary>
        /// NuGet packages included
        /// </summary>
        public List<NuGetPackage> Packages { get; } = new();

        /// <summary>
        /// Project references
        /// </summary>
        //public List<DotNetProjectReference> ProjectReferences { get; } = new();

        public DotNetProjectTemplateInstance(DotNetProjectTemplate template, string projectName)
        {
            _template = template ?? throw new ArgumentNullException(nameof(template));
            ProjectName = projectName ?? throw new ArgumentNullException(nameof(projectName));
           // TargetFolder = targetFolder ?? throw new ArgumentNullException(nameof(targetFolder));
        }

    }
}