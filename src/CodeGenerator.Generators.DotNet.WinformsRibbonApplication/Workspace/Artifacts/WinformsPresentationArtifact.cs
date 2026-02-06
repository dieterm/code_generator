using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Domain.DotNet.ProjectType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.WinformsRibbonApplication.Workspace.Artifacts
{
    public class WinformsPresentationArtifact : WorkspaceArtifactBase
    {
        public WinformsPresentationArtifact(DotNetProjectType projectType)
        {
            ProjectType = projectType;
        }
        public WinformsPresentationArtifact(ArtifactState state)
            : base(state)
        {

        }
        public override string TreeNodeText => ProjectType==DotNetProjectTypes.WinFormsExe ? "Winforms Executable" : "Winforms UserControl Library";

        public override ITreeNodeIcon TreeNodeIcon => new ResourceManagerTreeNodeIcon("presentation");

        public DotNetProjectType ProjectType {
            get { return DotNetProjectTypes.GetById(GetValue<string>(nameof(ProjectType))!); }
            private set { SetValue(nameof(ProjectType), value.Id); }
        }
    }
}
