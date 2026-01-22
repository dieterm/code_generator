using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Settings.Models
{
    public class SettingSection
    {
        public SettingSection(string key, string name)
        {
            Key = key;
            Name = name;
        }
        public string Key { get; }
        public string Name { get; }
        public string IconKey { get; set; } = "settings";
        public SettingSectionCollection Sections { get; set; } = new SettingSectionCollection();
        public SettingsItemCollection Items { get; set; } = new SettingsItemCollection();
    }
}
