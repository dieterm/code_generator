using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.Shared.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers.ArtifactPreview
{
    public class ArtifactPreviewController : CoreControllerBase
    {
        public ArtifactPreviewController(IWindowManagerService windowManagerService, RibbonBuilder ribbonBuilder, ApplicationMessageBus messageBus, IMessageBoxService messageboxService, IFileSystemDialogService fileSystemDialogService, ILogger<ArtifactPreviewController> logger) 
            : base(windowManagerService, ribbonBuilder, messageBus, messageboxService, fileSystemDialogService, logger)
        {
            
        }

        public void ShowArtifactPreview(ArtifactPreviewViewModel previewViewModel)
        {
            this._windowManagerService.ShowArtifactPreview(previewViewModel);
        }

        public override void Dispose()
        {
            // Nothing todo
        }

        public override void Initialize()
        {
            // Nothing todo
        }


    }
}
