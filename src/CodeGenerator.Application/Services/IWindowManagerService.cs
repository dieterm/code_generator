using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Settings.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Services
{
    public interface IWindowManagerService
    {
        void ShowArtifactPreview(IArtifact selectedArtifact);
        void ShowDomainSchemaTreeView(DomainSchemaTreeViewModel treeViewModel);
        void ShowGenerationTreeView(GenerationResultTreeViewModel treeViewModel);
        void ShowSettingsWindow(SettingsViewModel settingsViewModel);
    }
}
