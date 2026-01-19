using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.IntellicenseSupport
{
    public class ContextChoiseTriggerCollection : KeyedCollection<string, ContextChoiseTrigger>
    {
        protected override string GetKeyForItem(ContextChoiseTrigger item)
        {
            return item.TriggerText;
        }
    }
}
