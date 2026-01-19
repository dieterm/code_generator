using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.IntellicenseSupport
{
    public class ContextChoiseItem
    {
        public ContextChoiseItem(string text, string toolTip, string imageKey) 
        {
            Text = text;
            Tooltip = toolTip;
            ImageKey = imageKey;
        }

        public string Text { get; }
        public string Tooltip { get; }
        public string ImageKey { get; }
    }
}
