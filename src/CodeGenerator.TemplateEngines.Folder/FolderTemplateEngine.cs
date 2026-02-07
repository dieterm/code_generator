using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.CodeGeneration;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Templates.Settings;
using CodeGenerator.Shared;
using CodeGenerator.TemplateEngines.Scriban;
using CodeGenerator.TemplateEngines.T4;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.TemplateEngines.Folder
{
    public class FolderTemplateEngine : TemplateEngine<FolderTemplate, FolderTemplateInstance>
    {
        public const string TEMPLATE_ENGINE_ID = "folder_template_engine";
        public const string TEMPLATE_PARAMETER_FOLDER_NAMESPACE = "FolderNamespace";

        public FolderTemplateEngine(ILogger<FolderTemplateEngine> logger)
            : base(logger, TEMPLATE_ENGINE_ID, "Folder Template Engine", TemplateType.Folder)
        {
            
        }

        public override ITemplateInstance CreateTemplateInstance(ITemplate template)
        {
            var folderTemplate = template as FolderTemplate ?? throw new ArgumentException($"Template must be of type {nameof(FolderTemplate)}", nameof(template));
            return new FolderTemplateInstance(folderTemplate);
        }

        public override async Task<TemplateOutput> RenderAsync(FolderTemplateInstance templateInstance, CancellationToken cancellationToken)
        {
            try
            {
                var _templateManager = ServiceProviderHolder.GetRequiredService<TemplateManager>();
                var templateRootFolder = _templateManager.ResolveTemplateIdToFolderPath(templateInstance.Template.TemplateId);
                if (!Directory.Exists(templateRootFolder))
                {
                    return new TemplateOutput($"Template folder '{templateRootFolder}' does not exist.");
                }
                var settings = GetSettings();
                var artifacts = new List<IArtifact>();
                var errors = new List<string>();
                await ParseTemplateFolder(templateRootFolder, templateRootFolder, templateInstance, artifacts, errors, settings, cancellationToken);
                var output = new TemplateOutput(artifacts, errors);
                return output;
            }
            catch (Exception ex)
            {
                return new TemplateOutput($"Error rendering template: {ex.Message}");
            }
        }

        private async Task ParseTemplateFolder(string templateRootFolder, string templateFolder, FolderTemplateInstance templateInstance, List<IArtifact> artifactsContainer, List<string> errors, TemplateEngineSettings settings, CancellationToken cancellationToken, string parentNamespace = "")
        {
            var ignoredFiles = new List<string>();
            var files = Directory.GetFiles(templateFolder); // , "*.*", SearchOption.TopDirectoryOnly
            foreach (var file in files)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                var fileRelativePath = Path.GetRelativePath(templateRootFolder, file);
                var fileExtension = Path.GetExtension(file).Replace(".", "");
                if (templateInstance.ExcludedFileNames.Contains(fileRelativePath) || templateInstance.ExcludedExtensions.Contains(fileExtension)) 
                { 
                    ignoredFiles.Add(fileRelativePath);
                    continue;
                }

                await ParseFile(file, fileRelativePath, fileExtension, templateRootFolder, templateFolder, templateInstance, artifactsContainer, errors, settings, cancellationToken, parentNamespace);
            }
            var ignoredFolders = new List<string>();
            var directories = Directory.GetDirectories(templateFolder);
            foreach (var directory in directories)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                try
                {
                    var folderRelativePath = Path.GetRelativePath(templateRootFolder, directory);
                    var folderName = Path.GetFileName(folderRelativePath);
                    if(templateInstance.ExcludedFolderNames.Contains(folderName) || templateInstance.ExcludedFolders.Contains(folderRelativePath))
                    {
                        ignoredFolders.Add(folderRelativePath);
                        continue;
                    }

                    var folderArtifact = new FolderArtifact(folderName);
                    var childArtifacts = new List<IArtifact>();
                    await ParseTemplateFolder(templateRootFolder, directory, templateInstance, childArtifacts, errors, settings, cancellationToken, string.IsNullOrWhiteSpace(parentNamespace) ? folderName : $"{parentNamespace}.{folderName}");
                    childArtifacts.ForEach((artifact) => folderArtifact.AddChild(artifact));
                    artifactsContainer.Add(folderArtifact);
                }
                catch (Exception ex)
                {
                    errors.Add($"Error processing folder '{directory}': {ex.Message}");
                }
            }
        }

        private async Task ParseFile(string file, string fileRelativePath, string fileExtension, string templateRootFolder, string templateFolder, FolderTemplateInstance folderTemplateInstance, List<IArtifact> artifactsContainer, List<string> errors, TemplateEngineSettings settings, CancellationToken cancellationToken, string parentNamespace)
        {
            try
            {
                var _templateEngineManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
                var templateEngine = _templateEngineManager.GetTemplateEngineByFileExtension(fileExtension);
                if (templateEngine == null)
                {
                    // No template engine found, treat as existing file
                    artifactsContainer.Add(new ExistingFileArtifact(file));
                    return;
                }

                // if we get here, the file is a known template file extension

                var templateDefinitionFile = TemplateDefinition.GetDefinitionFilePath(file);

                if (templateEngine is IFileBasedTemplateEngine fileBasedTemplateEngine)
                {
                    var template = fileBasedTemplateEngine.CreateTemplateFromFile(file);
                    var templateInstance = templateEngine.CreateTemplateInstance(template);
                    templateInstance.SetParameter(TEMPLATE_PARAMETER_FOLDER_NAMESPACE, parentNamespace);
                    foreach(var parameter in folderTemplateInstance.Parameters)
                    {
                        templateInstance.SetParameter(parameter.Key, parameter.Value);
                    }
                    // common handler for all files in the folder template
                    if (folderTemplateInstance.TemplateHandler != null) { 
                        folderTemplateInstance.TemplateHandler.PrepareTemplate?.Invoke(template);
                        folderTemplateInstance.TemplateHandler.PrepareTemplateInstance?.Invoke(templateInstance);
                    }
                    // file specific handler
                    var templateHandler = folderTemplateInstance.TemplateHandlers.FirstOrDefault(th => th.FileName == fileRelativePath);
                    if (templateHandler != null)
                    {
                        templateHandler.PrepareTemplate?.Invoke(template);
                        templateHandler.PrepareTemplateInstance?.Invoke(templateInstance);
                    }
                    var renderResult = await templateEngine.RenderAsync(templateInstance);
                    if (folderTemplateInstance.TemplateHandler != null)
                    {
                        renderResult = folderTemplateInstance.TemplateHandler.TransformOutput?.Invoke(renderResult) ?? renderResult;
                    }
                    if (templateHandler != null)
                    {
                        renderResult = templateHandler.TransformOutput?.Invoke(renderResult) ?? renderResult;
                    }
                    if (renderResult?.Succeeded==true)
                    {
                        foreach(var artifact in renderResult.Artifacts)
                        {
                            artifactsContainer.Add(artifact);
                        }
                    } 
                    else
                    {
                        if(renderResult?.Errors != null)
                        {
                            foreach(var  error in renderResult.Errors)
                            {
                                errors.Add(error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error processing file '{fileRelativePath}': {ex.Message}");
            }
        }

        //private async Task<IEnumerable<IArtifact>> ParseTemplateFolder(string templateFolder, IArtifact? parentArtifact, GeneratorSettings settings, TemplateEngineManager templateEngineManager, GenerationResult result, string parentNamespace)
        //{


        //    var filesToIgnore = new List<string>();
        //    foreach (var file in Directory.GetFiles(templateFolder, "*", SearchOption.TopDirectoryOnly))
        //    {
        //        // Skip files that are already processed (eg *.def files)
        //        if (filesToIgnore.Contains(file))
        //            continue;

        //        var fileExtension = Path.GetExtension(file).Replace(".", "");
        //        if (fileExtension == "def")
        //        {
        //            // Definition file, skip
        //            filesToIgnore.Add(file);

        //            continue;
        //        }
        //        ;
        //        var templateEngine = templateEngineManager.GetTemplateEngineByFileExtension(fileExtension);

        //        if (templateEngine == null)
        //        {
        //            // No template engine found, treat as existing file
        //            parentArtifact.AddChild(new ExistingFileArtifact(file));
        //            //AddChildArtifactToParent(parentArtifact, new ExistingFileArtifact(file), result);
        //            continue;
        //        }
        //        filesToIgnore.Add(TemplateDefinition.GetDefinitionFilePath(file));

        //        if (templateEngine is IFileBasedTemplateEngine fileBasedTemplateEngine)
        //        {
        //            var template = fileBasedTemplateEngine.CreateTemplateFromFile(file);
        //            if (template is ScribanFileTemplate scribanTemplate)
        //            {
        //                // Set Scriban specific options if needed
        //                scribanTemplate.CreateTemplateFileIfMissing = true;
        //            }
        //            var templateInstance = templateEngine.CreateTemplateInstance(template);

        //            templateInstance.SetParameter("Namespace", parentNamespace);
        //            var renderResult = await templateEngine.RenderAsync(templateInstance);

        //            if (renderResult.Succeeded)
        //            {
        //                foreach (var artifact in renderResult.Artifacts)
        //                {
        //                    if (artifact is FileArtifact fileArtifact)
        //                    {
        //                        // remove extension from filename
        //                        var filenameWithoutExtension = Path.GetFileNameWithoutExtension(file);
        //                        fileArtifact.FileName = filenameWithoutExtension;
        //                        parentArtifact.AddChild(fileArtifact);
        //                        //AddChildArtifactToParent(parentArtifact, fileArtifact, result);
        //                    }
        //                    else
        //                    {
        //                        // Handle other artifact types if necessary
        //                        parentArtifact.AddChild(artifact);
        //                        //AddChildArtifactToParent(parentArtifact, artifact, result);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // Log errors
        //                foreach (var error in renderResult.Errors)
        //                {
        //                    result.Errors.Add($"Error rendering template '{file}': {error}");
        //                }
        //            }
        //        }

        //        // Create template instance


        //    }

        //    // Recursively process subdirectories
        //    foreach (var directory in Directory.GetDirectories(templateFolder, "*", SearchOption.TopDirectoryOnly))
        //    {
        //        var directoryName = Path.GetFileName(directory);
        //        var existingSubFolderArtifact = parentArtifact?.Children.OfType<FolderArtifact>().FirstOrDefault(f => f.FolderName == directoryName);
        //        var subdirectoryArtifact = existingSubFolderArtifact ?? new FolderArtifact(directoryName);
        //        var subdirectoryNamespace = $"{parentNamespace}.{directoryName}";
        //        AddChildArtifactToParent(parentArtifact, subdirectoryArtifact, result);
        //        await ParseTemplateFolder(directory, subdirectoryArtifact, settings, templateEngineManager, result, subdirectoryNamespace);
        //    }
        //}
    }
}
