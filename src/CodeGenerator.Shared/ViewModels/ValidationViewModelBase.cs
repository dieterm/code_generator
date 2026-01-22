using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.ViewModels
{
    public abstract class ValidationViewModelBase : ViewModelBase
    {

        /// <summary>
        /// Validation errors for the ViewModel
        /// </summary>
        public Dictionary<string, string> ValidationErrors { get; } = new();

        /// <summary>
        /// Check if ViewModel is valid
        /// </summary>
        public virtual bool IsValid => ValidationErrors.Count == 0;

        /// <summary>
        /// Validate the ViewModel
        /// </summary>
        public abstract bool Validate();

        /// <summary>
        /// Clear all validation errors
        /// </summary>
        protected void ClearValidationErrors()
        {
            ValidationErrors.Clear();
            OnPropertyChanged(nameof(IsValid));
        }

        /// <summary>
        /// Add a validation error
        /// </summary>
        protected void AddValidationError(string propertyName, string error)
        {
            ValidationErrors[propertyName] = error;
            OnPropertyChanged(nameof(IsValid));
        }
    }
}
