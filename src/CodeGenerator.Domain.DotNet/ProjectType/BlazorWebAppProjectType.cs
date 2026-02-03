using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet.ProjectType
{
    public class BlazorWebAppProjectType : DotNetProjectType
    {
        public const string PROJECTTYPE_ID = "blazor";

        private static readonly DotNetLanguage[] _supportedLanguages = new[] {
            DotNetLanguages.CSharp
        };

        private static readonly TargetFramework[] _supportedFrameworks = new[] {
            TargetFrameworks.Net10,
            TargetFrameworks.Net9,
            TargetFrameworks.Net8
        };

        public BlazorWebAppProjectType() 
            : base(PROJECTTYPE_ID, "Blazor Web App")
        {
        }

        public override IEnumerable<DotNetLanguage> SupportedLanguages => _supportedLanguages;
        public override IEnumerable<TargetFramework> SupportedFrameworks => _supportedFrameworks;
        
        public override void SetPropertyItems(DotNetProjectArtifact projectArtifact, Microsoft.Build.Evaluation.Project project)
        {
            // <TargetFramework>net8.0</TargetFramework>
            project.SetProperty("TargetFramework", projectArtifact.TargetFramework.DotNetCommandLineArgument);
            // <Nullable>enable</Nullable>
            project.SetProperty("Nullable", "enable");
            // <ImplicitUsings>enable</ImplicitUsings>
            project.SetProperty("ImplicitUsings", "enable");
        }
    }
}
