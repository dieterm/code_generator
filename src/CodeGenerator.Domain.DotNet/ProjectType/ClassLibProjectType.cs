using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet.ProjectType
{
    public class ClassLibProjectType : DotNetProjectType
    {
        public const string PROJECTTYPE_ID = "classlib";

        private static readonly DotNetLanguage[] _supportedLanguages = new[] {
            DotNetLanguages.CSharp,
            DotNetLanguages.FSharp,
            DotNetLanguages.VisualBasic
        };

        private static readonly TargetFramework[] _supportedFrameworks = new[] {
            TargetFrameworks.Net10,
            TargetFrameworks.Net9,
            TargetFrameworks.Net8,
            TargetFrameworks.Net7,
            TargetFrameworks.Net6,
            TargetFrameworks.Net5,
            TargetFrameworks.NetStandard21,
            TargetFrameworks.NetStandard20,
            TargetFrameworks.NetCore31
        };

        public ClassLibProjectType() 
            : base(PROJECTTYPE_ID, "Class Library")
        {
        }

        public override IEnumerable<DotNetLanguage> SupportedLanguages => _supportedLanguages;
        public override IEnumerable<TargetFramework> SupportedFrameworks => _supportedFrameworks;
        
        public override void SetPropertyItems(DotNetProjectArtifact projectArtifact, Microsoft.Build.Evaluation.Project project)
        {
            // <TargetFramework>net8.0</TargetFramework>
            project.SetProperty("TargetFramework", projectArtifact.TargetFramework.DotNetCommandLineArgument);
            // <ImplicitUsings>enable</ImplicitUsings>
            project.SetProperty("ImplicitUsings", "enable");
            // <Nullable>enable</Nullable>
            project.SetProperty("Nullable", "enable");
        }
    }
}
