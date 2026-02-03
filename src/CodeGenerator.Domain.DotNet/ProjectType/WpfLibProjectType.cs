using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet.ProjectType
{
    public class WpfLibProjectType : DotNetProjectType
    {
        public const string PROJECTTYPE_ID = "wpf";

        private static readonly DotNetLanguage[] _supportedLanguages = new[] {
            DotNetLanguages.CSharp,
            DotNetLanguages.VisualBasic
        };

        private static readonly TargetFramework[] _supportedFrameworks = new[] {
            TargetFrameworks.Net10,
            TargetFrameworks.Net9,
            TargetFrameworks.Net8,
            TargetFrameworks.Net7,
            TargetFrameworks.Net6,
            TargetFrameworks.Net5,
            TargetFrameworks.NetCore30,
            TargetFrameworks.NetCore31
        };

        public WpfLibProjectType() 
            : base(PROJECTTYPE_ID, "WPF Control Library")
        {
        }

        public override IEnumerable<DotNetLanguage> SupportedLanguages => _supportedLanguages;
        public override IEnumerable<TargetFramework> SupportedFrameworks => _supportedFrameworks;
        
        public override void SetPropertyItems(DotNetProjectArtifact projectArtifact, Microsoft.Build.Evaluation.Project project)
        {
            // <TargetFramework>net8.0-windows</TargetFramework>
            var targetFramework = projectArtifact.TargetFramework.DotNetCommandLineArgument;
            if (!targetFramework.EndsWith("-windows"))
            {
                targetFramework += "-windows";
            }
            project.SetProperty("TargetFramework", targetFramework);
            // <OutputType>Library</OutputType>
            project.SetProperty("OutputType", "Library");
            // <UseWPF>true</UseWPF>
            project.SetProperty("UseWPF", "true");
            // <ImplicitUsings>enable</ImplicitUsings>
            project.SetProperty("ImplicitUsings", "enable");
            // <Nullable>enable</Nullable>
            project.SetProperty("Nullable", "enable");
        }
    }
}
