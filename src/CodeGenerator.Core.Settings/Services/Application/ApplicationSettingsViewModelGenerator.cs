using CodeGenerator.Core.Settings.Application;
using CodeGenerator.Core.Settings.Models;
using CodeGenerator.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Settings.Services.Application
{
    public class ApplicationSettingsViewModelGenerator
    {
        private readonly ApplicationSettings _settings;

        public ApplicationSettingsViewModelGenerator(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public SettingSection GetSection()
        {
            var applicationSection = new SettingSection("application", "Application Settings") { IconKey = "settings" };

            var interfaceLanguageField = new ComboboxFieldModel
            {
                Items = ApplicationSettingsManager.GetAvailableLanguages().Select(language => new ComboboxItem { Value = language, DisplayName = language }).ToArray(),
                Label = "Interface Language",
                Name = nameof(_settings.InterfaceLanguage),
                IsRequired = true,
                Value = _settings.InterfaceLanguage
            };
            interfaceLanguageField.PropertyChanged += InterfaceLanguageField_PropertyChanged;
            interfaceLanguageField.Disposed += InterfaceLanguageField_Disposed;
            var interfaceLanguageSetting = new SettingsItem<ComboboxFieldModel>(interfaceLanguageField, nameof(_settings.InterfaceLanguage), "Interface Language", _settings.InterfaceLanguage);
            applicationSection.Items.Add(interfaceLanguageSetting);

            var themeField = new ComboboxFieldModel
            {
                Items = ApplicationSettingsManager.GetAvailableThemes().Select(theme => new ComboboxItem { Value = theme, DisplayName = theme }).ToArray(),
                Label = "Theme",
                Name = nameof(_settings.Theme),
                IsRequired = true,
                Value = _settings.Theme
            };
            themeField.PropertyChanged += ThemeField_PropertyChanged;
            themeField.Disposed += ThemeField_Disposed;
            var themeSetting = new SettingsItem<ComboboxFieldModel>(themeField, nameof(_settings.Theme), "Theme", _settings.Theme);

            applicationSection.Items.Add(themeSetting);

            return applicationSection;
        }

        private void InterfaceLanguageField_Disposed(object? sender, EventArgs e)
        {
            ((ComboboxFieldModel)sender!).PropertyChanged -= InterfaceLanguageField_PropertyChanged;
            ((ComboboxFieldModel)sender!).Disposed -= InterfaceLanguageField_Disposed;
        }

        private void InterfaceLanguageField_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _settings.InterfaceLanguage = ((ComboboxFieldModel)sender!).Value as string;
        }

        private void ThemeField_Disposed(object? sender, EventArgs e)
        {
            ((ComboboxFieldModel)sender!).PropertyChanged -= ThemeField_PropertyChanged;
            ((ComboboxFieldModel)sender!).Disposed -= ThemeField_Disposed;
        }

        private void ThemeField_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _settings.Theme = ((ComboboxFieldModel)sender!).Value as string;
        }
    }
}
