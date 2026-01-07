using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Presentation.WinForms.Resources
{
    public class ResourceManagerTreeNodeIconResolver : ITreeNodeIconResolver<ResourceManagerTreeNodeIcon>
    {
        public Image ResolveIcon(ResourceManagerTreeNodeIcon icon)
        {
            object obj = Resources.LucideIcons__000000.ResourceManager.GetObject(icon.IconKey);
            if(obj == null)
            {
                obj = Resources.DotNetIcons.ResourceManager.GetObject(icon.IconKey);
            }
            if(obj == null)
            {
                throw new KeyNotFoundException($"Icon with key '{icon.IconKey}' was not found in the resource managers.");
            }
            return ((System.Drawing.Bitmap)(obj));
        }
    }
}
