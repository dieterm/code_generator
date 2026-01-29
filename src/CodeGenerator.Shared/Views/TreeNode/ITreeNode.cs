using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Views.TreeNode
{
    public interface ITreeNode
    {
        /// <summary>
        /// The text to display for the tree node representing this artifact.
        /// </summary>
        string TreeNodeText { get; }
        /// <summary>
        /// Gets the icon associated with the tree node.
        /// </summary>
        ITreeNodeIcon TreeNodeIcon { get; }
        /// <summary>
        /// Get the text color for the tree node representing this artifact.<br />
        /// By default, null (use default color).<br />
        /// Override in derived classes to provide custom color.
        /// </summary>
        Color? TreeNodeTextColor { get; }
    }
}
