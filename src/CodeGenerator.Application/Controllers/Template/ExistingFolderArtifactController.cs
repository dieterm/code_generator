using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using Microsoft.Extensions.Logging;
using CodeGenerator.Shared.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerator.Shared;
using CodeGenerator.Core.Templates;
using CodeGenerator.Application.Services;

namespace CodeGenerator.Application.Controllers.Template
{
    public class ExistingFolderArtifactController : TemplateArtifactControllerBase<ExistingFolderArtifact>
    {
        private readonly IMessageBoxService _messageBoxService;
        public ExistingFolderArtifactController(IMessageBoxService messageBoxService, TemplateTreeViewController treeViewController, ILogger<ExistingFolderArtifactController> logger)
            : base(treeViewController, logger)
        {
            _messageBoxService = messageBoxService ?? throw new ArgumentNullException(nameof(messageBoxService));
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ExistingFolderArtifact folderArtifact)
        {
            var createTemplateCommand = new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "create_template",
                Text = "Create Template",
                IconKey = "file",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };
            // get template engines
            var templateEngineManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
            var templateEngines = templateEngineManager.TemplateEngines.OfType<IFileBasedTemplateEngine>().ToArray();
            foreach (var engine in templateEngines)
            {
                createTemplateCommand.SubCommands.Add(new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
                {
                    Id = $"create_template_{engine.Id}",
                    Text = engine.DisplayName,
                    IconKey = "file",
                    Execute = async (a) =>
                    {
                        var templateFile = folderArtifact.ExistingFolderPath.CreateIndexedFile("NewTemplate", engine.DefaultFileExtension);
                        if (templateFile == null) return;
                        var templateArtifact = new TemplateArtifact(templateFile.FullName, engine);
                        folderArtifact.AddChild(templateArtifact);
                        TreeViewController.OnArtifactAdded(folderArtifact, templateArtifact);
                        TreeViewController.RequestBeginRename(templateArtifact);
                        await Task.CompletedTask;
                    }
                });
            }
            yield return createTemplateCommand;
           
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "create_folder",
                Text = "Create Folder",
                IconKey = "folder",
                Execute = async (a) =>
                {
                    var dirInfo = folderArtifact.ExistingFolderPath.CreateDirectory("New Folder");
                    if (dirInfo == null) return;
                    var newFolderArtifact = new ExistingFolderArtifact(dirInfo.FullName, dirInfo.Name);
                    folderArtifact.AddChild(newFolderArtifact);
                    TreeViewController.OnArtifactAdded(folderArtifact, newFolderArtifact);
                    TreeViewController.RequestBeginRename(newFolderArtifact);
                }
            };
            yield return new ArtifactTreeNodeCommand(ArtifactTreeNodeCommandGroup.COMMAND_GROUP_MANAGE)
            {
                Id = "open_folder",
                Text = "Open in Explorer",
                IconKey = "folder",
                Execute = async (a) =>
                {
                    if (Directory.Exists(folderArtifact.ExistingFolderPath))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", folderArtifact.ExistingFolderPath);
                    }
                    await Task.CompletedTask;
                }
            };
        }

        override public bool CanDelete(ExistingFolderArtifact artifact)
        {
            return true;
        }

        public override void Delete(ExistingFolderArtifact folderArtifact)
        {
            var messageService = ServiceProviderHolder.GetRequiredService<IMessageBoxService>();
            var result = messageService.AskYesNoCancel($"Are you sure you want to delete the folder '{folderArtifact.TreeNodeText}'?", "Delete Folder");
            if (result == CodeGenerator.Application.Services.MessageBoxResult.Yes)
            {
                try
                {
                    if (Directory.Exists(folderArtifact.ExistingFolderPath))
                    {
                        Directory.Delete(folderArtifact.ExistingFolderPath, true);
                    }
                    var parentArtifact = folderArtifact.Parent;
                    parentArtifact.RemoveChild(folderArtifact);
                    TreeViewController.OnArtifactRemoved(parentArtifact, folderArtifact);
                }
                catch (Exception ex)
                {
                    messageService.ShowError($"An error occurred while deleting the folder: {ex.Message}", "Error Deleting Folder");
                }
            }
        }

        public override bool CanPaste(IArtifact artifactToPaste, ExistingFolderArtifact targetArtifact)
        {
            // TODO: add support for folders
            var canPasteFolder = artifactToPaste is ExistingFolderArtifact existingFolderArtifact && existingFolderArtifact.Parent != targetArtifact && existingFolderArtifact != targetArtifact;

            // FOR now, only allow pasting templates
            var canPasteTemplate = artifactToPaste is TemplateArtifact templateArtifact && templateArtifact.Parent != targetArtifact;
            return canPasteTemplate || canPasteFolder;
        }

        override public void Paste(IArtifact artifactToPaste, ExistingFolderArtifact targetFolderArtifact)
        {
            if (artifactToPaste is TemplateArtifact templateArtifact)
            {
                // move the template file to the new folder
                var sourceFilePath = templateArtifact.FilePath;
                var fileName = Path.GetFileName(sourceFilePath);
                var targetFilePath = Path.Combine(targetFolderArtifact.ExistingFolderPath, fileName);
                var targetFileExists = File.Exists(targetFilePath);
                // move template definition file (.def) if exists
                var sourceDefFilePath = TemplateDefinition.GetDefinitionFilePath(sourceFilePath);
                var sourceDefFileExists = File.Exists(sourceDefFilePath);

                var targetDefFilePath = TemplateDefinition.GetDefinitionFilePath(targetFilePath);
                var targetDefFileExists = File.Exists(targetDefFilePath);

                if (targetFileExists || targetDefFileExists)
                {
                    var msgboxResult = _messageBoxService.AskYesNoCancel($"A template with the name '{fileName}' or its definition file already exists in the target folder. Do you want to overwrite it?", "Paste Template");
                    if (msgboxResult == MessageBoxResult.Cancel || msgboxResult == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                if (ArtifactClipboardService.Instance.IsCutOperation)
                {
                    // if cut operation, we can just move the files
                    File.Move(sourceFilePath, targetFilePath, true);

                    if (sourceDefFileExists)
                    {
                        File.Move(sourceDefFilePath, targetDefFilePath, true);
                    }

                    // remove from old parent
                    var oldParent = templateArtifact.Parent;
                    oldParent.RemoveChild(templateArtifact);
                    TreeViewController.OnArtifactRemoved(oldParent, templateArtifact);

                    // paste to new parent
                    // create new template artifact
                    var fileExtension = Path.GetExtension(targetFilePath).Replace(".", "");
                    var templateEngineManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
                    var templateEngine = templateEngineManager.GetTemplateEngineByFileExtension(fileExtension);
                    var newTemplateArtifact = new TemplateArtifact(targetFilePath, templateEngine);

                    targetFolderArtifact.AddChild(newTemplateArtifact);
                    TreeViewController.OnArtifactAdded(targetFolderArtifact, newTemplateArtifact);

                    // clear clipboard
                    ArtifactClipboardService.Instance.Clear();
                }
                else
                {
                    // if copy operation, we need to copy the files first
                    File.Copy(sourceFilePath, targetFilePath, true);
                    if (sourceDefFileExists)
                    {
                        File.Copy(sourceDefFilePath, targetDefFilePath, true);
                    }

                    // create new template artifact
                    var fileExtension = Path.GetExtension(targetFilePath).Replace(".", "");
                    var templateEngineManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
                    var templateEngine = templateEngineManager.GetTemplateEngineByFileExtension(fileExtension);
                    var newTemplateArtifact = new TemplateArtifact(targetFilePath, templateEngine);

                    // paste to new parent
                    targetFolderArtifact.AddChild(newTemplateArtifact);
                    TreeViewController.OnArtifactAdded(targetFolderArtifact, newTemplateArtifact);
                }
            }
            else if (artifactToPaste is ExistingFolderArtifact sourceFolderArtifact)
            {
                var targetFolderPath = Path.Combine(targetFolderArtifact.ExistingFolderPath, sourceFolderArtifact.FolderName);
                if (Directory.Exists(targetFolderPath))
                {
                    var msgboxResult = _messageBoxService.AskYesNoCancel($"A folder with the name '{sourceFolderArtifact.FolderName}' already exists in the target folder. Do you want to overwrite it?", "Paste Folder");
                    if (msgboxResult == MessageBoxResult.Cancel || msgboxResult == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                if (ArtifactClipboardService.Instance.IsCutOperation)
                {
                    // move the folder
                    Directory.Move(sourceFolderArtifact.ExistingFolderPath, targetFolderPath);
                    TreeViewController.RefreshTemplates();

                    ArtifactClipboardService.Instance.Clear();
                }
                else
                {
                    
                    // copy the folder
                    sourceFolderArtifact.ExistingFolderPath.CopyDirectory(targetFolderPath, true);
                    
                    TreeViewController.RefreshTemplates();
                }
            }
        }

        public override bool CanCopy(ExistingFolderArtifact artifact)
        {
            return true;
        }

        public override bool CanCut(ExistingFolderArtifact artifact)
        {
            return true;
        }
    }
}
