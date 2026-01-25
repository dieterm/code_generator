using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Templates
{
    /// <summary>
    /// Associates a template definition with its expected location using TemplateId syntax.
    /// </summary>
    public class TemplateDefinitionAndLocation
    {
        public TemplateDefinition TemplateDefinition { get; }
        
        /// <summary>
        /// The path segments for this template (e.g., ["@Workspace", "@TableArtifact", "scripts", "mysql"])
        /// </summary>
        public string[] TemplateFolderPath { get; }

        /// <summary>
        /// The parsed template ID based on folder path and template name
        /// </summary>
        public ParsedTemplateId ParsedTemplateId { get; }

        /// <summary>
        /// The full TemplateId including special folder path (e.g., "@Workspace/@TableArtifact/scripts/mysql/create_table")
        /// </summary>
        public string TemplateId => ParsedTemplateId.FullTemplateId;
        
        public TemplateDefinitionAndLocation(TemplateDefinition templateDefinition, params string[] templateFolderPath)
        {
            TemplateDefinition = templateDefinition;
            TemplateFolderPath = templateFolderPath ?? Array.Empty<string>();
            
            // Build the full TemplateId from folder path and template name
            var fullTemplateId = BuildFullTemplateId(TemplateFolderPath, templateDefinition.TemplateName);
            ParsedTemplateId = TemplateIdParser.Parse(fullTemplateId);
        }

        /// <summary>
        /// Build the full TemplateId from folder path segments and template name
        /// </summary>
        private static string BuildFullTemplateId(string[] folderPath, string templateName)
        {
            if (folderPath == null || folderPath.Length == 0)
            {
                return templateName;
            }

            var parts = new List<string>(folderPath) { templateName };
            return string.Join(TemplateIdParser.PathSeparator.ToString(), parts);
        }

        /// <summary>
        /// Create a TemplateDefinitionAndLocation for a workspace artifact template
        /// </summary>
        /// <param name="artifactTypeName">The artifact type name (e.g., "TableArtifact")</param>
        /// <param name="templateName">The template name</param>
        /// <param name="displayName">Display name for the template</param>
        /// <param name="description">Description of the template</param>
        /// <param name="subFolders">Optional subfolders between artifact name and template</param>
        public static TemplateDefinitionAndLocation ForWorkspaceArtifact(
            string artifactTypeName, 
            string templateName, 
            string? displayName = null,
            string? description = null,
            params string[] subFolders)
        {
            var definition = new TemplateDefinition
            {
                TemplateName = templateName,
                DisplayName = displayName ?? templateName,
                Description = description ?? $"Template for {artifactTypeName}"
            };
            
            // Build folder path: @Workspace, @ArtifactName, subfolders...
            var folderPath = new List<string>
            {
                TemplateIdParser.SpecialFolderPrefix.ToString() + TemplateIdParser.SpecialFolders.Workspace,
                TemplateIdParser.SpecialFolderPrefix.ToString() + artifactTypeName
            };
            folderPath.AddRange(subFolders);
            
            return new TemplateDefinitionAndLocation(definition, folderPath.ToArray());
        }

        /// <summary>
        /// Create a TemplateDefinitionAndLocation for a generator template
        /// </summary>
        public static TemplateDefinitionAndLocation ForGenerator(
            string generatorId,
            string templateName,
            string? displayName = null,
            string? description = null,
            params string[] subFolders)
        {
            var definition = new TemplateDefinition
            {
                TemplateName = templateName,
                DisplayName = displayName ?? templateName,
                Description = description ?? $"Template for generator {generatorId}"
            };

            // Build folder path: @Generators, generatorId, subfolders...
            var folderPath = new List<string>
            {
                TemplateIdParser.SpecialFolderPrefix.ToString() + TemplateIdParser.SpecialFolders.Generators,
                generatorId
            };
            folderPath.AddRange(subFolders);

            return new TemplateDefinitionAndLocation(definition, folderPath.ToArray());
        }

        /// <summary>
        /// Create a TemplateDefinitionAndLocation for a template engine template
        /// </summary>
        public static TemplateDefinitionAndLocation ForTemplateEngine(
            string templateEngineId,
            string templateName,
            string? displayName = null,
            string? description = null,
            params string[] subFolders)
        {
            var definition = new TemplateDefinition
            {
                TemplateName = templateName,
                DisplayName = displayName ?? templateName,
                Description = description ?? $"Template for engine {templateEngineId}"
            };

            // Build folder path: @TemplateEngines, templateEngineId, subfolders...
            var folderPath = new List<string>
            {
                TemplateIdParser.SpecialFolderPrefix.ToString() + TemplateIdParser.SpecialFolders.TemplateEngines,
                templateEngineId
            };
            folderPath.AddRange(subFolders);

            return new TemplateDefinitionAndLocation(definition, folderPath.ToArray());
        }

        /// <summary>
        /// Create a TemplateDefinitionAndLocation for a user-defined template
        /// </summary>
        public static TemplateDefinitionAndLocation ForUserDefined(
            string templateName,
            string? displayName = null,
            string? description = null,
            params string[] subFolders)
        {
            var definition = new TemplateDefinition
            {
                TemplateName = templateName,
                DisplayName = displayName ?? templateName,
                Description = description ?? "User-defined template"
            };

            // Build folder path: @UserDefined, subfolders...
            var folderPath = new List<string>
            {
                TemplateIdParser.SpecialFolderPrefix.ToString() + TemplateIdParser.SpecialFolders.UserDefined
            };
            folderPath.AddRange(subFolders);

            return new TemplateDefinitionAndLocation(definition, folderPath.ToArray());
        }
    }
}
