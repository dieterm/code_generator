using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared.Views.TreeNode;
using CodeGenerator.TemplateEngines.PlantUML;

namespace CodeGenerator.TemplateEngines.Folder
{
    public class FolderTemplate : ITemplate
    {
        public FolderTemplate(string templateId)
        {
            TemplateId = templateId;
        }
        /// <summary>
        /// The template id is used to identify the folder template and can be used in child templates to reference this folder (e.g., for output path resolution).
        /// Use TemplateManager.ResolveTemplateIdToFolderPath(template.TemplateId) in child templates to get the actual folder path at render time.
        /// </summary>
        public string TemplateId { get; }

        public TemplateType TemplateType => TemplateType.Folder;

        public bool UseCaching { get { return false; } set { throw new InvalidOperationException("FolderTemplate doesn't support caching"); } }

        public ITreeNodeIcon Icon => new AssemblyResourceTreeNodeIcon(typeof(PlantUmlTemplate).Assembly, "CodeGenerator.TemplateEngines.Folder.folder-kanban.ico");
    }
}
