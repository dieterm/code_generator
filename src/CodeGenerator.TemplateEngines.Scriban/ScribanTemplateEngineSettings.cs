using CodeGenerator.Core.Templates.Settings;

namespace CodeGenerator.TemplateEngines.Scriban
{
    public class ScribanTemplateEngineSettings : TemplateEngineSettings
    {
        public ScribanTemplateEngineSettings()
        {
        }
        public ScribanTemplateEngineSettings(string templateEngineId) : base(templateEngineId)
        {
        }
        //public string? MySetting { 
        //    get { return this.GetParameter<string>(nameof(MySetting)); } 
        //    set { this.SetParameter<string>(nameof(MySetting), value); }
        //}
    }
}