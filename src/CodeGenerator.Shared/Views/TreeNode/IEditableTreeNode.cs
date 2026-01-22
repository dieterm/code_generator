using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Views.TreeNode
{
    public interface IEditableTreeNode : ITreeNode
    {
        
        bool CanBeginEdit();
        bool Validating(string newName);
        void EndEdit(string oldName, string newName);
    }
}
