using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Templates
{
    public class TemplateDefinitionAndLocation
    {
        public TemplateDefinition TemplateDefinition { get; }
        /// <summary>
        /// any subfolders after the "@workspace/@artifactname" part of the TemplateId
        /// </summary>
        public string? TemplateFolderPath { get; }
        public TemplateDefinitionAndLocation(TemplateDefinition templateDefinition, string templateFolderPath)
        {
            TemplateDefinition = templateDefinition;
            TemplateFolderPath = templateFolderPath;
        }
    }
}
