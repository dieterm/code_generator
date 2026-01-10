using CodeGenerator.Core.Settings.Application;
using CodeGenerator.Core.Settings.Models;
using CodeGenerator.Core.Workspaces.Settings;
using CodeGenerator.Domain.CodeArchitecture;
using CodeGenerator.Domain.DotNet;
using CodeGenerator.Shared;
using CodeGenerator.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.Settings
{
    public class WorkspaceSettingsViewModelGenerator
    {
        private readonly WorkspaceSettings _settings;
        public WorkspaceSettingsViewModelGenerator(WorkspaceSettings settings)
        {
            _settings = settings;
        }
        public SettingSection GetSection()
        {
            var workspaceSection = new SettingSection("workspace_settings","Workspace Settings");

            //RootNamespace:SingleLineTextFieldModel
            var rootNamespaceField = new SingleLineTextFieldModel
            {
                Label = "Root Namespace",
                Name = nameof(_settings.RootNamespace),
                IsRequired = true,
                Value = _settings.RootNamespace
            };
            rootNamespaceField.PropertyChanged += RootNamespaceField_PropertyChanged;
            rootNamespaceField.Disposed += RootNamespaceField_Disposed;
            var rootNamespaceSetting = new SettingsItem<SingleLineTextFieldModel>(rootNamespaceField, nameof(_settings.RootNamespace), "Root Namespace", _settings.RootNamespace);
            workspaceSection.Items.Add(rootNamespaceSetting);

            //DefaultOutputDirectory:FolderFieldModel
            var outputDirectoryField = new FolderFieldModel
            {
                Label = "Default Output Directory",
                Name = nameof(_settings.DefaultOutputDirectory),
                IsRequired = true,
                Value = _settings.DefaultOutputDirectory,
                Description = "The default directory where generated code files will be saved."
            };
            outputDirectoryField.PropertyChanged += OutputDirectoryField_PropertyChanged;
            outputDirectoryField.Disposed += OutputDirectoryField_Disposed;
            var outputDirectorySetting = new SettingsItem<FolderFieldModel>(outputDirectoryField, nameof(_settings.DefaultOutputDirectory), "Default Output Directory", _settings.DefaultOutputDirectory);
            workspaceSection.Items.Add(outputDirectorySetting);

            //DefaultTargetFramework:ComboboxFieldModel
            var targetFrameworkField = new ComboboxFieldModel
            {
                Label = "Default Target Framework",
                Name = nameof(_settings.DefaultTargetFramework),
                IsRequired = true,
                Items = TargetFrameworks.AllFrameworks.Select(f => new { Id = f, DisplayName = f }).ToList(),
                Value = _settings.DefaultTargetFramework
            };
            targetFrameworkField.PropertyChanged += TargetFrameworkField_PropertyChanged;
            targetFrameworkField.Disposed += TargetFrameworkField_Disposed;
            var targetFrameworkSetting = new SettingsItem<ComboboxFieldModel>(targetFrameworkField, nameof(_settings.DefaultTargetFramework), "Default Target Framework", _settings.DefaultTargetFramework);
            workspaceSection.Items.Add(targetFrameworkSetting);

            //DefaultLanguage:ComboboxFieldModel
            var defaultLanguageField = new ComboboxFieldModel
            {
                Label = "Default Language",
                Name = nameof(_settings.DefaultLanguage),
                IsRequired = true,
                Items = DotNetLanguages.AllLanguages.Select(language => new { Id = language.DotNetCommandLineArgument, DisplayName = $"{language.DotNetCommandLineArgument} (*.{language.ProjectFileExtension})" }).ToList(),
                Value = _settings.DefaultLanguage
            };
            defaultLanguageField.PropertyChanged += DefaultLanguageField_PropertyChanged;
            defaultLanguageField.Disposed += DefaultLanguageField_Disposed;
            var defaultLanguageSetting = new SettingsItem<ComboboxFieldModel>(defaultLanguageField, nameof(_settings.DefaultLanguage), "Default Language", _settings.DefaultLanguage);
            workspaceSection.Items.Add(defaultLanguageSetting);

            //UseNullableReferenceTypes:BooleanFieldModel
            var useNullableReferenceTypesField = new BooleanFieldModel
            {
                Label = "Use Nullable Reference Types",
                Name = nameof(_settings.UseNullableReferenceTypes),
                IsRequired = true,
                Value = _settings.UseNullableReferenceTypes,
            };
            useNullableReferenceTypesField.PropertyChanged += UseNullableReferenceTypesField_PropertyChanged;
            useNullableReferenceTypesField.Disposed += UseNullableReferenceTypesField_Disposed;
            var useNullableReferenceTypesSetting = new SettingsItem<BooleanFieldModel>(useNullableReferenceTypesField, nameof(_settings.UseNullableReferenceTypes), "Use Nullable Reference Types");
            workspaceSection.Items.Add(useNullableReferenceTypesSetting);

            var architectureManager = ServiceProviderHolder.GetRequiredService<CodeArchitectureManager>();
            //DefaultCodeArchitectureId:ComboboxFieldModel
            var defaultCodeArchitectureField = new ComboboxFieldModel
            {
                Label = "Default Code Architecture",
                Name = nameof(_settings.DefaultCodeArchitectureId),
                IsRequired = true,
                Items = architectureManager.GetAllArchitectures().Select(def => new { Id = def.Id, DisplayName = def.Name }).ToList(),
                Value = _settings.DefaultCodeArchitectureId
            };
            defaultCodeArchitectureField.PropertyChanged += DefaultCodeArchitectureField_PropertyChanged;
            defaultCodeArchitectureField.Disposed += DefaultCodeArchitectureField_Disposed;
            var defaultCodeArchitectureSetting = new SettingsItem<ComboboxFieldModel>(defaultCodeArchitectureField, nameof(_settings.DefaultCodeArchitectureId), "Default Code Architecture", _settings.DefaultCodeArchitectureId);
            workspaceSection.Items.Add(defaultCodeArchitectureSetting);

            // DefaultTemplateFolder:FolderFieldModel
            var defaultTemplateFolderField = new FolderFieldModel
            {
                Label = "Default Template Folder",
                Name = nameof(_settings.DefaultTemplateFolder),
                IsRequired = false,
                Value = _settings.DefaultTemplateFolder,
                Description = "The default folder where code generation templates are stored."
            };
            defaultTemplateFolderField.PropertyChanged += DefaultTemplateFolderField_PropertyChanged;
            defaultTemplateFolderField.Disposed += DefaultTemplateFolderField_Disposed;
            var defaultTemplateFolderSetting = new SettingsItem<FolderFieldModel>(defaultTemplateFolderField, nameof(_settings.DefaultTemplateFolder), "Default Template Folder");
            workspaceSection.Items.Add(defaultTemplateFolderSetting);

            return workspaceSection;
        }

        private void DefaultTemplateFolderField_Disposed(object? sender, EventArgs e)
        {
            ((FolderFieldModel)sender!).PropertyChanged -= DefaultTemplateFolderField_PropertyChanged;
            ((FolderFieldModel)sender!).Disposed -= DefaultTemplateFolderField_Disposed;
        }

        private void DefaultTemplateFolderField_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _settings.DefaultTemplateFolder = ((FolderFieldModel)sender!).Value as string;
        }

        private void DefaultCodeArchitectureField_Disposed(object? sender, EventArgs e)
        {
            ((ComboboxFieldModel)sender!).PropertyChanged -= DefaultCodeArchitectureField_PropertyChanged;
            ((ComboboxFieldModel)sender!).Disposed -= DefaultCodeArchitectureField_Disposed;
        }

        private void DefaultCodeArchitectureField_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _settings.DefaultCodeArchitectureId = ((ComboboxFieldModel)sender!).Value as string;
        }

        private void UseNullableReferenceTypesField_Disposed(object? sender, EventArgs e)
        {
            ((BooleanFieldModel)sender!).PropertyChanged -= UseNullableReferenceTypesField_PropertyChanged;
            ((BooleanFieldModel)sender!).Disposed -= UseNullableReferenceTypesField_Disposed;
        }

        private void UseNullableReferenceTypesField_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _settings.UseNullableReferenceTypes = (bool)((BooleanFieldModel)sender!).Value;
        }

        private void DefaultLanguageField_Disposed(object? sender, EventArgs e)
        {
            ((ComboboxFieldModel)sender!).PropertyChanged -= DefaultLanguageField_PropertyChanged;
            ((ComboboxFieldModel)sender!).Disposed -= DefaultLanguageField_Disposed;
        }

        private void DefaultLanguageField_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _settings.DefaultLanguage = ((ComboboxFieldModel)sender!).Value as string;
        }

        private void TargetFrameworkField_Disposed(object? sender, EventArgs e)
        {
            ((ComboboxFieldModel)sender!).PropertyChanged -= TargetFrameworkField_PropertyChanged;
            ((ComboboxFieldModel)sender!).Disposed -= TargetFrameworkField_Disposed;
        }

        private void TargetFrameworkField_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _settings.DefaultTargetFramework = ((ComboboxFieldModel)sender!).Value as string;
        }

        private void OutputDirectoryField_Disposed(object? sender, EventArgs e)
        {
            ((FolderFieldModel)sender!).PropertyChanged -= OutputDirectoryField_PropertyChanged;
            ((FolderFieldModel)sender!).Disposed -= OutputDirectoryField_Disposed;
        }

        private void OutputDirectoryField_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _settings.DefaultOutputDirectory = ((FolderFieldModel)sender!).Value as string;
        }

        private void RootNamespaceField_Disposed(object? sender, EventArgs e)
        {
            ((SingleLineTextFieldModel)sender!).PropertyChanged -= RootNamespaceField_PropertyChanged;
            ((SingleLineTextFieldModel)sender!).Disposed -= RootNamespaceField_Disposed;
        }

        private void RootNamespaceField_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _settings.RootNamespace = ((SingleLineTextFieldModel)sender!).Value as string;
        }
    }
}
