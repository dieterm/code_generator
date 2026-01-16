using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Templates
{
    public interface ITemplate
    {
        string TemplateId { get; }
        TemplateType TemplateType { get; }
        bool UseCaching { get; }
        /// <summary>
        /// The icon used to represent this template in tree views
        /// </summary>
        ITreeNodeIcon Icon { get; }
    }
}
