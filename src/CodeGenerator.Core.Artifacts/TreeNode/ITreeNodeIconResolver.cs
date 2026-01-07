using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.TreeNode
{
    public interface ITreeNodeIconResolver<T> where T : ITreeNodeIcon
    {
        Image ResolveIcon(T icon);
    }
}
