using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared.ExtensionMethods;
using CodeGenerator.Shared.Operations;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.Shared.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.ArtifactPreview
{
    public class ArtifactPreviewController : CoreControllerBase
    {
        private readonly IWindowManagerService _windowManagerService;

        public ArtifactPreviewController(
            OperationExecutor operationExecutor,
            IWindowManagerService windowManagerService,
            RibbonBuilder ribbonBuilder,
            ApplicationMessageBus messageBus,
            IMessageBoxService messageboxService,
            IFileSystemDialogService fileSystemDialogService,
            ILogger<ArtifactPreviewController> logger) 
            : base(operationExecutor, ribbonBuilder, messageBus, messageboxService, fileSystemDialogService, logger)
        {
            _windowManagerService = windowManagerService;
        }

        public void ShowArtifactPreview(ArtifactPreviewViewModel previewViewModel)
        {
            this._windowManagerService.ShowArtifactPreview(previewViewModel);
        }

        public void ShowExistingFile(string filePath, ArtifactPreviewViewModel.KnownLanguages syntax, string? tabLabel = null)
        {
            var fileName = Path.GetFileName(filePath);
            var previewViewModel = new ArtifactPreviewViewModel
            {
                TabLabel = tabLabel??fileName,
                FilePath = filePath,
                TextLanguageSchema = syntax
            };
            this.ShowArtifactPreview(previewViewModel);
        }

        public void ShowExistingFile(string filePath, string? tabLabel = null)
        {
            var fileExtension = Path.GetExtension(filePath);
            var syntax = this.GetSyntaxByFileExtension(fileExtension);
            ShowExistingFile(filePath, syntax, tabLabel);
        }

        public bool ShowExistingFileArtifact(ExistingFileArtifact? existingFileArtifact, string? tabLabel = null)
        {
            if (existingFileArtifact == null || string.IsNullOrWhiteSpace(existingFileArtifact.ExistingFilePath))
            {
                return false;
            }
            ShowExistingFile(existingFileArtifact.ExistingFilePath, GetSyntaxByFileExtension(existingFileArtifact.FileName.GetFileExtension()));
            return true;
        }

        public bool ShowFileArtifact(FileArtifact? fileArtifact, string? tabLabel = null)
        {
            if(fileArtifact==null)
            {
                return false;
            }
            if(fileArtifact is ExistingFileArtifact)
            {
                ShowExistingFileArtifact(fileArtifact as ExistingFileArtifact, tabLabel);
            }
            if (fileArtifact.HasDecorator<TextContentDecorator>())
            {
                // If the file artifact has text content decorator, show that
                ShowArtifactPreview(new ArtifactPreviewViewModel
                {
                    FileName = fileArtifact.FileName,
                    TabLabel = tabLabel??fileArtifact.FileName,
                    TextContent = fileArtifact.GetTextContent(),
                    TextLanguageSchema = GetSyntaxByFileExtension(fileArtifact.FileName.GetFileExtension())
                });
                return true;
            }
            else if (fileArtifact.HasDecorator<ImageContentDecorator>())
            {
                var image = fileArtifact.GetDecoratorOfType<ImageContentDecorator>()?.CreatePreview() as Image;
                if (image != null)
                {
                    ShowArtifactPreview(new ArtifactPreviewViewModel
                    {
                        FileName = fileArtifact.FileName,
                        TabLabel = tabLabel ?? fileArtifact.FileName,
                        ImageContent = image,
                    });
                    return true;
                }
            }
            return false;
        }

        public ArtifactPreviewViewModel.KnownLanguages GetSyntaxByFileExtension(string? fileExtension)
        {
            if(string.IsNullOrWhiteSpace(fileExtension))
            {
                return ArtifactPreviewViewModel.KnownLanguages.Undefined;
            }
            switch (fileExtension)
            {
                case ".cs":
                    return ArtifactPreviewViewModel.KnownLanguages.CSharp;
                case ".js":
                    return ArtifactPreviewViewModel.KnownLanguages.JScript;
                case ".ts":
                    return ArtifactPreviewViewModel.KnownLanguages.CSharp;
                case ".html":
                case ".htm":
                    return ArtifactPreviewViewModel.KnownLanguages.HTML;
                case ".xml":
                case ".csproj":
                case ".sln":
                case ".targets":
                    return ArtifactPreviewViewModel.KnownLanguages.XML;
                case ".json":
                    return ArtifactPreviewViewModel.KnownLanguages.Undefined;
                case ".sql":
                    return ArtifactPreviewViewModel.KnownLanguages.SQL;
                case ".java":
                    return ArtifactPreviewViewModel.KnownLanguages.Java;
                case ".py":
                    return ArtifactPreviewViewModel.KnownLanguages.Undefined;
                default:
                    return ArtifactPreviewViewModel.KnownLanguages.Text;
            }
        }

        public override void Dispose()
        {
            // Nothing todo
        }

        public override void Initialize()
        {
            // Nothing todo
        }

        public void ShowTextContent(string textContent, string tabLabel, ArtifactPreviewViewModel.KnownLanguages syntax = ArtifactPreviewViewModel.KnownLanguages.Text)
        {
            ShowArtifactPreview(new ArtifactPreviewViewModel()
            {
                TabLabel = tabLabel,
                TextContent = textContent,
                TextLanguageSchema = syntax
            });
        }
    }
}
