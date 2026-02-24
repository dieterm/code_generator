using CodeGenerator.Core.Workspaces.Artifacts.Workspace;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Shared;
using CodeGenerator.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.ViewModels.Workspace
{
    public class WorkspaceEditViewModel : ArtifactEditViewModel<WorkspaceArtifact, WorkspaceGeneralTabViewModel>
    {
        public WorkspaceEditViewModel()
            : base("Workspace", new WorkspaceGeneralTabViewModel())
        {

        }
    }

    public class WorkspaceGeneralTabViewModel : ArtifactEditViewTabModel
    {
        public SingleLineTextFieldModel NameField { get; }
        public MultiLineTextFieldModel DescriptionField { get; }
        public SingleLineTextFieldModel RootNamespaceField { get; }
        public FolderFieldModel OutputDirectoryField { get; }
        public ComboboxFieldModel DefaultTargetFrameworkField { get; }
        public ComboboxFieldModel DefaultLanguageField { get; }
        public ComboboxFieldModel CodeArchitectureField { get; }
        public ComboboxFieldModel DependencyInjectionFrameworkField { get; }

        public WorkspaceGeneralTabViewModel() : base("General")
        {
            // Name field
            NameField = new SingleLineTextFieldModel
            {
                Label = "Name",
                Name = nameof(WorkspaceArtifact.Name),
                Tooltip = "Name of the workspace",
                IsRequired = true,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(NameField);

            // Description field
            DescriptionField = new MultiLineTextFieldModel
            {
                Label = "Description",
                Name = nameof(WorkspaceArtifact.Description),
                Tooltip = "Description of the workspace",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(DescriptionField);

            // RootNamespace field
            RootNamespaceField = new SingleLineTextFieldModel
            {
                Label = "Root Namespace",
                Name = nameof(WorkspaceArtifact.RootNamespace),
                Tooltip = "Root namespace for generated code",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(RootNamespaceField);

            // Output Directory field
            OutputDirectoryField = new FolderFieldModel
            {
                Label = "Default Output Directory",
                Name = nameof(WorkspaceArtifact.OutputDirectory),
                Tooltip = "Default output directory for generated files",
                Description = "Select the default output directory",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(OutputDirectoryField);

            // Target Framework field
            DefaultTargetFrameworkField = new ComboboxFieldModel
            {
                Label = "Target Framework",
                Name = nameof(WorkspaceArtifact.DefaultTargetFramework),
                Tooltip = "Default target framework for projects",
                AutoBind = true,
                AutoUpdate = true
            };
            DefaultTargetFrameworkField.Items = TargetFrameworks.AllFrameworks
                .Select(f => new ComboboxItem { DisplayName = f.Name, Value = f.Id })
                .ToList();
            FieldCollection.FieldModels.Add(DefaultTargetFrameworkField);

            // Language field
            DefaultLanguageField = new ComboboxFieldModel
            {
                Label = "Default Language",
                Name = nameof(WorkspaceArtifact.DefaultLanguage),
                Tooltip = "Default programming language",
                AutoBind = true,
                AutoUpdate = true
            };
            DefaultLanguageField.Items = DotNetLanguages.AllLanguages
                .Select(lang => new ComboboxItem { DisplayName = lang.Name, Value = lang.Id })
                .ToList();
            FieldCollection.FieldModels.Add(DefaultLanguageField);

            // Code Architecture field
            CodeArchitectureField = new ComboboxFieldModel
            {
                Label = "Code Architecture",
                Name = nameof(WorkspaceArtifact.CodeArchitectureId),
                Tooltip = "Code architecture pattern",
                AutoBind = true,
                AutoUpdate = true
            };
            var architectureManager = ServiceProviderHolder.GetRequiredService<CodeArchitectureManager>();
            var allArchitectures = architectureManager.GetAllArchitectures();
            CodeArchitectureField.Items = allArchitectures
                .Select(a => new ComboboxItem { DisplayName = a.Name, Value = a.Id })
                .ToList();
            FieldCollection.FieldModels.Add(CodeArchitectureField);

            // Dependency Injection Framework field
            DependencyInjectionFrameworkField = new ComboboxFieldModel
            {
                Label = "Dependency Injection Framework",
                Name = nameof(WorkspaceArtifact.DependencyInjectionFrameworkId),
                Tooltip = "Dependency injection framework",
                AutoBind = true,
                AutoUpdate = true
            };
            var diFrameworkManager = ServiceProviderHolder.GetRequiredService<DependancyInjectionFrameworkManager>();
            var allFrameworks = diFrameworkManager.Frameworks;
            DependencyInjectionFrameworkField.Items = allFrameworks
                .Select(f => new ComboboxItem { DisplayName = f.Name, Value = f.Id })
                .ToList();
            FieldCollection.FieldModels.Add(DependencyInjectionFrameworkField);
        }
    }
}
