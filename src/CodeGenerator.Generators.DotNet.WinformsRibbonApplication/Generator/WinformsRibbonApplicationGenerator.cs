using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Generators.MessageBus;
using CodeGenerator.Core.Generators.Settings;
using CodeGenerator.Core.Settings.Generators;
using CodeGenerator.Core.Templates;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Scopes;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Shared;
using CodeGenerator.TemplateEngines.Scriban;

namespace CodeGenerator.Generators.DotNet.WinformsRibbonApplication
{
    public class WinformsRibbonApplicationGenerator : GeneratorBase
    {
        private Func<CreatedArtifactEventArgs, Task>? _unsubscribe_handler;

        public override void SubscribeToEvents(GeneratorMessageBus messageBus)
        {
            // Use async Subscribe variant for proper async handling
            _unsubscribe_handler = messageBus.Subscribe<CreatedArtifactEventArgs>(
                async (e) => await OnPresentationDotNetProjectCreated(e),
                PresentationDotNetProjectFilter
            );
        }

        private bool PresentationDotNetProjectFilter(CreatedArtifactEventArgs args)
        {
            if(!Enabled)
                return false;

            if (args.Artifact is DotNetProjectArtifact projectArtifact)
            {
                if(projectArtifact.Parent is FolderArtifact folderArtifact && folderArtifact.HasDecorator<LayerArtifactRefDecorator>())
                {
                    var layerArtifact = folderArtifact.GetDecoratorOfType<LayerArtifactRefDecorator>()?.LayerArtifact;
                    var scopeArtifact = layerArtifact?.Parent as ScopeArtifact;
                    var layer = layerArtifact?.LayerName;
                    var scope = scopeArtifact?.Name;
                    if(layer == CodeArchitectureLayerArtifact.PRESENTATION_LAYER &&
                       scope == CodeArchitectureLayerArtifact.APPLICATION_SCOPE)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task OnPresentationDotNetProjectCreated(CreatedArtifactEventArgs e)
        {
            if (!Enabled)
                return;

            var projectArtifact = (e.Artifact as DotNetProjectArtifact)!;
      
            projectArtifact.AddNuGetPackage(NuGetPackages.Syncfusion_Edit_Windows);
            projectArtifact.AddNuGetPackage(NuGetPackages.Syncfusion_Grouping_Base);
            projectArtifact.AddNuGetPackage(NuGetPackages.Syncfusion_Office2019Theme_WinForms);
            projectArtifact.AddNuGetPackage(NuGetPackages.Syncfusion_SfListView_WinForms);
            projectArtifact.AddNuGetPackage(NuGetPackages.Syncfusion_Shared_Base);
            projectArtifact.AddNuGetPackage(NuGetPackages.Syncfusion_Tools_Windows);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_EntityFrameworkCore);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_EntityFrameworkCore_Relational);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_Configuration);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_Configuration_Json);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_DependencyInjection);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_Hosting);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_Logging);
            projectArtifact.AddNuGetPackage(NuGetPackages.Microsoft_Extensions_Logging_Debug);

            var settings = GetSettings();
            var templateId = TemplateIdParser.BuildGeneratorTemplateId(this.SettingsDescription.Id, WinformsRibbonApplicationGeneratorSettings.TEMPLATE_ID);
            var template = settings.Templates.FirstOrDefault(t => t.TemplateId == templateId);
            var templateFolder = template?.TemplateFilePath;
            
            if (Directory.Exists(templateFolder))
            {
                var templateManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
                await ParseTemplateFolder(templateFolder, projectArtifact, settings, templateManager, e.Result, projectArtifact.Name);
                
            }
        }

        private async Task ParseTemplateFolder(string templateFolder, IArtifact? parentArtifact, GeneratorSettings settings, TemplateEngineManager templateEngineManager, GenerationResult result, string parentNamespace)
        {
            var filesToIgnore = new List<string>();
            foreach(var file in Directory.GetFiles(templateFolder, "*", SearchOption.TopDirectoryOnly))
            {
                // Skip files that are already processed (eg *.def files)
                if (filesToIgnore.Contains(file))
                    continue;

                var fileExtension = Path.GetExtension(file).Replace(".", "");
                if(fileExtension=="def")
                {
                    // Definition file, skip
                    filesToIgnore.Add(file);
                    
                    continue;
                };
                var templateEngine = templateEngineManager.GetTemplateEngineByFileExtension(fileExtension);
                
                if (templateEngine == null)
                {
                    // No template engine found, treat as existing file
                    parentArtifact.AddChild(new ExistingFileArtifact(file));
                    //AddChildArtifactToParent(parentArtifact, new ExistingFileArtifact(file), result);
                    continue;
                }
                filesToIgnore.Add(TemplateDefinition.GetDefinitionFilePath(file));
                
                if(templateEngine is IFileBasedTemplateEngine fileBasedTemplateEngine)
                {
                    //// Check for template definition
                    //var definitionFilePath = TemplateDefinition.GetDefinitionFilePath(file);
                    //if (File.Exists(definitionFilePath))
                    //{
                    //    var templateDefinition = TemplateDefinition.LoadFromFile(definitionFilePath);
                    //    if (templateDefinition != null)
                    //    {
                    //        // You can use templateDefinition as needed
                    //    }
                    //}

                    var template = fileBasedTemplateEngine.CreateTemplateFromFile(file);
                    if (template is ScribanFileTemplate scribanTemplate)
                    {
                        // Set Scriban specific options if needed
                        scribanTemplate.CreateTemplateFileIfMissing = true;
                    }
                    var templateInstance = templateEngine.CreateTemplateInstance(template);

                    templateInstance.SetParameter("Namespace", parentNamespace);
                    var renderResult = await templateEngine.RenderAsync(templateInstance);

                    if (renderResult.Succeeded)
                    {
                        foreach (var artifact in renderResult.Artifacts)
                        {
                            if (artifact is FileArtifact fileArtifact)
                            {
                                // remove extension from filename
                                var filenameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                                fileArtifact.FileName = filenameWithoutExtension;
                                parentArtifact.AddChild(fileArtifact);
                                //AddChildArtifactToParent(parentArtifact, fileArtifact, result);
                            }
                            else
                            {
                                // Handle other artifact types if necessary
                                parentArtifact.AddChild(artifact);
                                //AddChildArtifactToParent(parentArtifact, artifact, result);
                            }
                        }
                    }
                    else
                    {
                        // Log errors
                        foreach (var error in renderResult.Errors)
                        {
                            result.Errors.Add($"Error rendering template '{file}': {error}");
                        }
                    }
                }

                // Create template instance
                

            }

            // Recursively process subdirectories
            foreach(var directory in Directory.GetDirectories(templateFolder, "*", SearchOption.TopDirectoryOnly))
            {
                var directoryName = Path.GetFileName(directory);
                var existingSubFolderArtifact = parentArtifact?.Children.OfType<FolderArtifact>().FirstOrDefault(f => f.FolderName == directoryName);
                var subdirectoryArtifact = existingSubFolderArtifact ?? new FolderArtifact(directoryName);
                var subdirectoryNamespace = $"{parentNamespace}.{directoryName}";
                AddChildArtifactToParent(parentArtifact, subdirectoryArtifact, result);
                await ParseTemplateFolder(directory, subdirectoryArtifact, settings, templateEngineManager, result, subdirectoryNamespace);
            }
        }

        public override void UnsubscribeFromEvents(GeneratorMessageBus messageBus)
        {
            if(_unsubscribe_handler!=null)
                messageBus.Unsubscribe(_unsubscribe_handler!);
        }

        protected override GeneratorSettingsDescription ConfigureSettingsDescription()
        {
            return WinformsRibbonApplicationGeneratorSettings.CreateDescription(this);
        }
    }
}
