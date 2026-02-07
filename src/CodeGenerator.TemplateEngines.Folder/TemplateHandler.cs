using CodeGenerator.Core.Templates;

namespace CodeGenerator.TemplateEngines.Folder
{
    public class TemplateHandler
    {
        public TemplateHandler()
        {
            FileName = string.Empty;
        }
        public TemplateHandler(string fileName)
        {
            FileName = fileName;
        }
        public string FileName { get; set; }
        
        public Action<ITemplate>? PrepareTemplate { get; set; } = null;
        public Action<ITemplateInstance>? PrepareTemplateInstance { get; set; } = null;
        public Func<TemplateOutput, TemplateOutput>? TransformOutput { get; set; } = null;
    }
}