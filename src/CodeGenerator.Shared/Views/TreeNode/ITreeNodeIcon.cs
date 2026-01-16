using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace CodeGenerator.Core.Artifacts.TreeNode
{
    public interface ITreeNodeIcon
    {
        string IconKey { get; }
        Image GetIcon();
    }
}
