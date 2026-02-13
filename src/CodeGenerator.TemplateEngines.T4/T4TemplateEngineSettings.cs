using CodeGenerator.Core.Templates.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.TemplateEngines.T4
{
    public class T4TemplateEngineSettings : TemplateEngineSettings
    {
        public T4TemplateEngineSettings() { }

        public T4TemplateEngineSettings(string templateEngineId) 
            : base(templateEngineId)
        {
        }

        //public string? MySetting { 
        //    get { return this.GetParameter<string>(nameof(MySetting)); } 
        //    set { this.SetParameter<string>(nameof(MySetting), value); }
        //}
    }
}
