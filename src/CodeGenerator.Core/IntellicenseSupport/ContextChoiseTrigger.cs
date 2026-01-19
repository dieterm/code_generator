using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.IntellicenseSupport
{
    public class ContextChoiseTrigger
    {
        public ContextChoiseTrigger(string trigger, string? tooltip = null)
        {
            TriggerText = trigger;
            Tooltip = tooltip;
        }
        public string? Tooltip { get; set; }
        public string TriggerText { get; }
        public List<ContextChoiseItem> Items { get; } = new List<ContextChoiseItem>();
    }
}
