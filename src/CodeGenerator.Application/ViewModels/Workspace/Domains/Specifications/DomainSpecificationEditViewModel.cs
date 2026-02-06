using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Specifications;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Application.ViewModels.Workspace.Domains.Specifications
{
    /// <summary>
    /// ViewModel for editing domain specification properties
    /// </summary>
    public class DomainSpecificationEditViewModel : ViewModelBase
    {
        private DomainSpecificationArtifact? _specification;
        private bool _isLoading;

        public DomainSpecificationEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "Specification Name", Name = nameof(DomainSpecificationArtifact.Name) };
            DescriptionField = new SingleLineTextFieldModel { Label = "Description", Name = nameof(DomainSpecificationArtifact.Description) };
            CriteriaField = new SingleLineTextFieldModel { Label = "Criteria", Name = nameof(DomainSpecificationArtifact.Criteria) };
            CategoryField = new SingleLineTextFieldModel { Label = "Category", Name = nameof(DomainSpecificationArtifact.Category) };
            IsCompositeField = new BooleanFieldModel { Label = "Is Composite", Name = nameof(DomainSpecificationArtifact.IsComposite) };
            IsReusableField = new BooleanFieldModel { Label = "Is Reusable", Name = nameof(DomainSpecificationArtifact.IsReusable) };

            NameField.PropertyChanged += OnFieldChanged;
            DescriptionField.PropertyChanged += OnFieldChanged;
            CriteriaField.PropertyChanged += OnFieldChanged;
            CategoryField.PropertyChanged += OnFieldChanged;
            IsCompositeField.PropertyChanged += OnFieldChanged;
            IsReusableField.PropertyChanged += OnFieldChanged;
        }

        /// <summary>
        /// The specification being edited
        /// </summary>
        public DomainSpecificationArtifact? Specification
        {
            get => _specification;
            set
            {
                if (_specification != null)
                {
                    _specification.PropertyChanged -= Specification_PropertyChanged;
                }
                if (SetProperty(ref _specification, value))
                {
                    LoadFromSpecification();
                    if (_specification != null)
                    {
                        _specification.PropertyChanged += Specification_PropertyChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Observe specification property changes from outside
        /// </summary>
        private void Specification_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DomainSpecificationArtifact.Name))
            {
                NameField.Value = _specification?.Name;
            }
            else if (e.PropertyName == nameof(DomainSpecificationArtifact.Description))
            {
                DescriptionField.Value = _specification?.Description;
            }
            else if (e.PropertyName == nameof(DomainSpecificationArtifact.Criteria))
            {
                CriteriaField.Value = _specification?.Criteria;
            }
            else if (e.PropertyName == nameof(DomainSpecificationArtifact.Category))
            {
                CategoryField.Value = _specification?.Category;
            }
            else if (e.PropertyName == nameof(DomainSpecificationArtifact.IsComposite))
            {
                IsCompositeField.Value = _specification?.IsComposite;
            }
            else if (e.PropertyName == nameof(DomainSpecificationArtifact.IsReusable))
            {
                IsReusableField.Value = _specification?.IsReusable;
            }
        }

        /// <summary>
        /// Specification name field
        /// </summary>
        public SingleLineTextFieldModel NameField { get; }

        /// <summary>
        /// Specification description field
        /// </summary>
        public SingleLineTextFieldModel DescriptionField { get; }

        /// <summary>
        /// Specification criteria field
        /// </summary>
        public SingleLineTextFieldModel CriteriaField { get; }

        /// <summary>
        /// Category field
        /// </summary>
        public SingleLineTextFieldModel CategoryField { get; }

        /// <summary>
        /// Is composite specification field
        /// </summary>
        public BooleanFieldModel IsCompositeField { get; }

        /// <summary>
        /// Is reusable specification field
        /// </summary>
        public BooleanFieldModel IsReusableField { get; }

        /// <summary>
        /// Event raised when any field value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadFromSpecification()
        {
            if (_specification == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _specification.Name;
                DescriptionField.Value = _specification.Description;
                CriteriaField.Value = _specification.Criteria;
                CategoryField.Value = _specification.Category;
                IsCompositeField.Value = _specification.IsComposite;
                IsReusableField.Value = _specification.IsReusable;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _specification == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToSpecification();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_specification, field.Name, field.Value));
            }
        }

        private void SaveToSpecification()
        {
            if (_specification == null) return;

            _specification.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) 
                ? NameField.Value as string 
                : "Specification";
            _specification.Description = DescriptionField.Value as string;
            _specification.Criteria = CriteriaField.Value as string;
            _specification.Category = CategoryField.Value as string;
            _specification.IsComposite = IsCompositeField.Value is bool isComposite && isComposite;
            _specification.IsReusable = IsReusableField.Value is bool isReusable && isReusable;
        }
    }
}
