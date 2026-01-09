using CodeGenerator.Core.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Templates
{
    public class TemplateOutput
    {
        public TemplateOutput(IEnumerable<IArtifact> artifacts)
        {
            Artifacts = artifacts;
            Errors = new List<string>();
            Succeeded = true;
        }
        public TemplateOutput(IArtifact artifacts)
        {
            Artifacts = new List<IArtifact>() { artifacts };
            Errors = new List<string>();
            Succeeded = true;
        }
        public TemplateOutput(IEnumerable<string> errors)
        {
            Errors = errors;
            Artifacts = new List<IArtifact>();
            Succeeded = false;
        }

        public TemplateOutput(string error)
        {
            Errors = new List<string>() { error };
            Artifacts = new List<IArtifact>();
            Succeeded = false;
        }

        public IEnumerable<IArtifact> Artifacts { get; }
        public bool Succeeded { get; set; }

        public IEnumerable<string> Errors { get; } 
    }
}
