using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.VVMC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Generators.DotNet.ApplicationScope.Workspace.Artifacts
{
    public class ApplicationViewModelArtifact : ViewModelArtifact
    {
        public ApplicationViewModelArtifact()
            : base("ApplicationViewModel")
        {
        }

        public ApplicationViewModelArtifact(ArtifactState state)
            : base(state)
        {
        }

        public string? ApplicationName { 
            get { return GetValue<string>(nameof(ApplicationName)); } 
            set { SetValue<string>(nameof(ApplicationName), value); } 
        }
    }
}
