using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet.ProjectType
{
    public class MsTestProjectType : DotNetProjectType
    {
        public const string PROJECTTYPE_ID = "mstest";

        private static readonly DotNetLanguage[] _supportedLanguages = new[] {
            DotNetLanguages.CSharp,
            DotNetLanguages.FSharp,
            DotNetLanguages.VisualBasic
        };

        private static readonly TargetFramework[] _supportedFrameworks = new[] {
            TargetFrameworks.Net10,
            TargetFrameworks.Net10WindowsOnly,
            TargetFrameworks.Net9,
            TargetFrameworks.Net9WindowsOnly,
            TargetFrameworks.Net8,
            TargetFrameworks.Net8WindowsOnly,
            TargetFrameworks.Net7,
            TargetFrameworks.Net7WindowsOnly,
            TargetFrameworks.Net6,
            TargetFrameworks.Net6WindowsOnly,
            TargetFrameworks.Net5,
            TargetFrameworks.NetCore31,
            TargetFrameworks.Net48,
            TargetFrameworks.Net481,
            TargetFrameworks.Net47,
            TargetFrameworks.Net471,
            TargetFrameworks.Net472,
            TargetFrameworks.Net462
        };

        public MsTestProjectType() 
            : base(PROJECTTYPE_ID, "MSTest Test Project")
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
            // <IsPackable>false</IsPackable>
            project.SetProperty("IsPackable", "false");
            // <IsTestProject>true</IsTestProject>
            project.SetProperty("IsTestProject", "true");
        }
    }
}
