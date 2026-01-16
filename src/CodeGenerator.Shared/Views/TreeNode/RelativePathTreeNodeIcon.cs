using CodeGenerator.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.TreeNode
{
    public class RelativePathTreeNodeIcon : ITreeNodeIcon
    {
        public RelativePathTreeNodeIcon(string iconKey, string relativePath)
        {
            IconKey = iconKey;
            RelativePath = relativePath;
        }
        public string IconKey { get; }
        public string RelativePath { get; }
        public Image GetIcon()
        {
            return ServiceProviderHolder.GetRequiredService<ITreeNodeIconResolver<RelativePathTreeNodeIcon>>().ResolveIcon(this);
        }
    }
}
