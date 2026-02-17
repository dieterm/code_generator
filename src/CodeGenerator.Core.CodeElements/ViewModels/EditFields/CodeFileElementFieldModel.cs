using CodeGenerator.Core.CodeElements.Controllers;
using CodeGenerator.Core.CodeElements.Services;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Shared;
using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeGenerator.Core.CodeElements.ViewModels.EditFields
{
    public class CodeFileElementFieldModel : FieldViewModelBase
    {
        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }
        
        public CodeFileElementFieldModel()
        {
            LoadCommand = new RelayCommand(Load, (obj) => Value is CodeFileElement);
            SaveCommand = new RelayCommand(Save, (obj) => _clonedElement !=null);

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Value))
                {
                    (LoadCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            };
        }

        private void Save(object? obj)
        {
            // clone the edited element back to the original element
            Value = _clonedElement.Clone();
        }

        private CodeFileElement? _clonedElement;
        private void Load(object? obj)
        {
            var codeFileElement = Value as CodeFileElement;
            if (codeFileElement != null)
            {
                // make a clone of the codefile element to edit, so that changes are not applied until the user clicks save
                _clonedElement = codeFileElement.Clone();
            } 
            else
            {
                _clonedElement = new CodeFileElement();
            }
            var codeElementsController = ServiceProviderHolder.GetRequiredService<CodeElementsController>();
            codeElementsController.ShowCodeElements(_clonedElement);
            codeElementsController.TreeViewController.HasUnsavedChangesChanged += (s, e) =>
            {
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            };
        }
    }
}
