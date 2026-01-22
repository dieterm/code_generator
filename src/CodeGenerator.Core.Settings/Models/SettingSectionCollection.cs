using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Settings.Models
{
    public class SettingSectionCollection : KeyedCollection<string, SettingSection>
    {
        protected override string GetKeyForItem(SettingSection item)
        {
            return item.Key;
        }
    }
}
