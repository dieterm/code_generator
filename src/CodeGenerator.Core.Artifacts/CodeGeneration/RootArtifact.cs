using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Artifacts.CodeGeneration
{
    /// <summary>
    /// This artifact represents the root of an artifact tree.
    /// </summary>
    public sealed class RootArtifact : Artifact
    {
        public const string ROOT_ARTIFACT_ID = "ROOT_ARTIFACT";
        public override string Id { get { return ROOT_ARTIFACT_ID; } }

        public override string TreeNodeText { get { return "Output"; } }

        public override ITreeNodeIcon TreeNodeIcon { get; } = new ResourceManagerTreeNodeIcon("root-artifact");


    }
}
