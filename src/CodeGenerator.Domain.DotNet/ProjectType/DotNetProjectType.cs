using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet
{
    public abstract class DotNetProjectType
    {
        public string Id { get; }
        public string Name { get; }

        public abstract void SetPropertyItems(DotNetProjectArtifact projectArtifact, Project project);

        protected DotNetProjectType(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public abstract IEnumerable<DotNetLanguage> SupportedLanguages { get; }
        public abstract IEnumerable<TargetFramework> SupportedFrameworks { get; }

    }
}
