using CodeGenerator.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.TreeNode
{
    public class ResourceManagerTreeNodeIcon : ITreeNodeIcon
    {
        public ResourceManagerTreeNodeIcon(string iconKey)
        {
            IconKey = iconKey;
        }
        public string IconKey { get; }

        public Image GetIcon()
        {
            return ServiceProviderHolder.GetRequiredService<ITreeNodeIconResolver<ResourceManagerTreeNodeIcon>>().ResolveIcon(this);
        }
    }
}
