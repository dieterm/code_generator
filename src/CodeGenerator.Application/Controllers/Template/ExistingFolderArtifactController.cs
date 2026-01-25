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
    public class ExistingFolderArtifactController : ArtifactControllerBase<TemplateTreeViewController, ExistingFolderArtifact>
    {
        public ExistingFolderArtifactController(TemplateTreeViewController treeViewController, ILogger<ExistingFolderArtifactController> logger)
            : base(treeViewController, logger)
        {
        }

        protected override IEnumerable<ArtifactTreeNodeCommand> GetCommands(ExistingFolderArtifact folderArtifact)
        {
            var createTemplateCommand =  new ArtifactTreeNodeCommand
            {
                Id = "create_template",
                Text = "Create Template",
                IconKey = "file",
                SubCommands = new List<ArtifactTreeNodeCommand>()
            };
            // get template engines
            var templateEngineManager = ServiceProviderHolder.GetRequiredService<TemplateEngineManager>();
            var templateEngines = templateEngineManager.TemplateEngines.Where(t => !string.IsNullOrEmpty(t.DefaultFileExtension)).ToArray();
            foreach(var engine in templateEngines)
            {
                createTemplateCommand.SubCommands.Add(new ArtifactTreeNodeCommand
                {
                    Id = $"create_template_{engine.Id}",
                    Text = engine.DisplayName,
                    IconKey = "file",
                    Execute = async (a) =>
                    {
                        var templateFile = folderArtifact.ExistingFolderPath.CreateIndexedFile("NewTemplate", engine.DefaultFileExtension);
                        if(templateFile == null) return;
                        var templateArtifact = new TemplateArtifact(templateFile.FullName, engine);
                        folderArtifact.AddChild(templateArtifact);
                        TreeViewController.OnArtifactAdded(folderArtifact, templateArtifact);
                        TreeViewController.RequestBeginRename(templateArtifact);
                        await Task.CompletedTask;
                    }
                });
            }
            yield return createTemplateCommand;
            // add seperator
            yield return ArtifactTreeNodeCommand.Separator;
            
            yield return new ArtifactTreeNodeCommand
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
            yield return new ArtifactTreeNodeCommand
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
    }
}
