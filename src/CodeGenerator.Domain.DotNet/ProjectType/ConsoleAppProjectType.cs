using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet.ProjectType
{
    public class ConsoleAppProjectType : DotNetProjectType
    {
        public const string PROJECTTYPE_ID = "console";

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
            TargetFrameworks.NetCore31
        };

        public ConsoleAppProjectType() 
            : base(PROJECTTYPE_ID, "Console Application")
        {
        }

        public override IEnumerable<DotNetLanguage> SupportedLanguages => _supportedLanguages;
        public override IEnumerable<TargetFramework> SupportedFrameworks => _supportedFrameworks;
        
        public override void SetPropertyItems(DotNetProjectArtifact projectArtifact, Microsoft.Build.Evaluation.Project project)
        {
            // <OutputType>Exe</OutputType>
            project.SetProperty("OutputType", "Exe");
            // <TargetFramework>net8.0</TargetFramework>
            project.SetProperty("TargetFramework", projectArtifact.TargetFramework.DotNetCommandLineArgument);
            // <ImplicitUsings>enable</ImplicitUsings>
            project.SetProperty("ImplicitUsings", "enable");
            // <Nullable>enable</Nullable>
            project.SetProperty("Nullable", "enable");
        }
    }
}
