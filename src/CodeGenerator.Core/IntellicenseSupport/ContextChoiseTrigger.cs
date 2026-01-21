using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.IntellicenseSupport
{
    public class ContextChoiseTrigger
    {
        public ContextChoiseTrigger(string trigger, string tooltip, TypeFormatStyle formatStyle)
        {
            TriggerText = trigger;
            Tooltip = tooltip;
            FormatStyle = formatStyle;
        }
        public TypeFormatStyle FormatStyle { get; } 
        public string? Tooltip { get; set; }
        public string TriggerText { get; }
        public bool UseSplitter { get; set; } = false;
        public List<ContextChoiseItem> Items { get; } = new List<ContextChoiseItem>();
    }
}
