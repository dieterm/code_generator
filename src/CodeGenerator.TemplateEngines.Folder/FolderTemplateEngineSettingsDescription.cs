using CodeGenerator.Core.Templates.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.TemplateEngines.Folder
{
    public class FolderTemplateEngineSettingsDescription : TemplateEngineSettingsDescription
    {
        public FolderTemplateEngineSettingsDescription(string id, string name, string description, string category = "General", string? iconKey = null) : base(id, name, description, category, iconKey)
        {
        }

        public override TemplateEngineSettings GetDefaultSettings()
        {
            return new FolderTemplateEngineSettings(Id);
        }
    }
}
