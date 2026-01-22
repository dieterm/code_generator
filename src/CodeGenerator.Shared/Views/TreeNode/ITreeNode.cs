using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Views.TreeNode
{
    public interface ITreeNode
    {
        string TreeNodeText { get; }
        ITreeNodeIcon TreeNodeIcon { get; }
    }
}
