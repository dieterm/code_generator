using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Settings.Models
{
    public class SettingsItem<FVM> : ISettingsItem where FVM : IFieldViewModel
    {
        public SettingsItem(FVM fieldViewModel, string key, string name, string? description = null)
        {
            FieldViewModel = fieldViewModel;
            Key = key;
            Name = name;
            Description = description;
        }
        public FVM FieldViewModel { get; }
        /// <summary>
        /// Explicit interface implementation to expose the FieldViewModel as IFieldViewModel
        /// </summary>
        IFieldViewModel ISettingsItem.FieldViewModel => FieldViewModel;
        public string Key { get; }
        public string Name { get; }
        public string? Description { get; }
        public object? Value { get; set; }
        public object? DefaultValue { get; set; }
    }
}
