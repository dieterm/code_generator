using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Copilot;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.Copilot.Settings;
using CodeGenerator.Core.Copilot.ViewModels;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Workspaces.Services;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Ribbon;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Copilot.Controllers
{
    public class CopilotController : CoreControllerBase, ICopilotController
    {
        private readonly CopilotChatViewModel _copilotChatViewModel;
        private IWorkspaceContextProvider? _workspaceContextProvider;
        public CopilotController(CopilotChatViewModel copilotChatViewModel, IWindowManagerService windowManagerService, RibbonBuilder ribbonBuilder, ApplicationMessageBus messageBus, IMessageBoxService messageboxService, IFileSystemDialogService fileSystemDialogService, ILogger<CopilotController> logger) 
            : base(windowManagerService, ribbonBuilder, messageBus, messageboxService, fileSystemDialogService, logger)
        {
            _copilotChatViewModel = copilotChatViewModel;
        }

        public override void Dispose()
        {
            
        }

        public override void Initialize()
        {
            _workspaceContextProvider = ServiceProviderHolder.GetRequiredService<IWorkspaceContextProvider>();
            // Initialize Copilot settings
            var copilotSettings = CopilotSettings.Instance;
            if (string.IsNullOrEmpty(copilotSettings.Username) || string.IsNullOrEmpty(copilotSettings.Password))
            {
                _messageBoxService.ShowError("Copilot settings are not configured properly.");
            }
        }

        public void ManipulateWorkspace()
        {
            if (_workspaceContextProvider == null) throw new InvalidOperationException("Call Initialize() first to set _workspaceContextProvider");
            var workspaceArtifact = _workspaceContextProvider.CurrentWorkspace;

            // do manipulation with workspaceArtifact, for example:
            var sharedScopeArtifact = workspaceArtifact.Scopes.FirstOrDefault(scope => scope.Name == CodeArchitectureScopes.SHARED_SCOPE);
            var domain = sharedScopeArtifact.Domains.AddDomain("Geoservice");
            var countryEntity = domain.AddEntity(new Workspaces.Artifacts.Domains.Entities.EntityArtifact("Country"));

        }

        public void ShowCopilot()
        {
            _windowManagerService.ShowCopilotChatView(_copilotChatViewModel);
            
        }
    }
}
