using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Settings.Models
{
    public class SettingsItemCollection : KeyedCollection<string, ISettingsItem>
    {
        protected override string GetKeyForItem(ISettingsItem item)
        {
            return item.Key;
        }
    }
}
