using CodeGenerator.Core.Templates.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.TemplateEngines.DotNetProject.Settings
{
    public class DotNetProjectTemplateEngineSettings : TemplateEngineSettings
    {
        public DotNetProjectTemplateEngineSettings()
        {
        }

        public DotNetProjectTemplateEngineSettings(string templateEngineId) : base(templateEngineId)
        {
        }

        //public string? MySetting { 
        //    get { return this.GetParameter<string>(nameof(MySetting)); } 
        //    set { this.SetParameter<string>(nameof(MySetting), value); }
        //}
    }
}
