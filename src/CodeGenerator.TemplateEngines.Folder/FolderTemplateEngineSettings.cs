using CodeGenerator.Core.Templates.Settings;

namespace CodeGenerator.TemplateEngines.Folder
{
    public class FolderTemplateEngineSettings : TemplateEngineSettings
    {
        public FolderTemplateEngineSettings(string templateEngineId) : base(templateEngineId)
        {
        }

        //public string? MySetting { 
        //    get { return this.GetParameter<string>(nameof(MySetting)); } 
        //    set { this.SetParameter<string>(nameof(MySetting), value); }
        //}
    }
}