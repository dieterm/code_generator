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
        public TemplateOutput(IEnumerable<IArtifact> artifacts, IEnumerable<string> errors)
        {
            Artifacts = artifacts;
            Errors = errors;
            Succeeded = !Errors.Any();
        }

        public TemplateOutput(string error)
        {
            Errors = new List<string>() { error };
            Artifacts = new List<IArtifact>();
            Succeeded = false;
        }

        /// <summary>
        /// Creates a TemplateOutput with text content (useful for simple template rendering)
        /// </summary>
        public TemplateOutput(string textContent, bool succeeded)
        {
            TextContent = textContent;
            Artifacts = new List<IArtifact>();
            Errors = new List<string>();
            Succeeded = succeeded;
        }

        public IEnumerable<IArtifact> Artifacts { get; }
        public bool Succeeded { get; set; }

        public IEnumerable<string> Errors { get; }

        /// <summary>
        /// Raw text content output from template rendering (when not creating file artifacts)
        /// </summary>
        public string? TextContent { get; set; }
    }
}
