using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Factories;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Application.ViewModels.Workspace.Domains.Factories
{
    /// <summary>
    /// ViewModel for editing domain factory properties
    /// </summary>
    public class DomainFactoryEditViewModel : ViewModelBase
    {
        private DomainFactoryArtifact? _factory;
        private bool _isLoading;

        public DomainFactoryEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "Factory Name", Name = nameof(DomainFactoryArtifact.Name) };
            DescriptionField = new SingleLineTextFieldModel { Label = "Description", Name = nameof(DomainFactoryArtifact.Description) };
            CategoryField = new SingleLineTextFieldModel { Label = "Category", Name = nameof(DomainFactoryArtifact.Category) };
            CreatesAggregatesField = new BooleanFieldModel { Label = "Creates Aggregates", Name = nameof(DomainFactoryArtifact.CreatesAggregates) };
            IsStatelessField = new BooleanFieldModel { Label = "Is Stateless", Name = nameof(DomainFactoryArtifact.IsStateless) };
            HasDependenciesField = new BooleanFieldModel { Label = "Has Dependencies", Name = nameof(DomainFactoryArtifact.HasDependencies) };

            NameField.PropertyChanged += OnFieldChanged;
            DescriptionField.PropertyChanged += OnFieldChanged;
            CategoryField.PropertyChanged += OnFieldChanged;
            CreatesAggregatesField.PropertyChanged += OnFieldChanged;
            IsStatelessField.PropertyChanged += OnFieldChanged;
            HasDependenciesField.PropertyChanged += OnFieldChanged;
        }

        /// <summary>
        /// The factory being edited
        /// </summary>
        public DomainFactoryArtifact? Factory
        {
            get => _factory;
            set
            {
                if (_factory != null)
                {
                    _factory.PropertyChanged -= Factory_PropertyChanged;
                }
                if (SetProperty(ref _factory, value))
                {
                    LoadFromFactory();
                    if (_factory != null)
                    {
                        _factory.PropertyChanged += Factory_PropertyChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Observe factory property changes from outside
        /// </summary>
        private void Factory_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DomainFactoryArtifact.Name))
            {
                NameField.Value = _factory?.Name;
            }
            else if (e.PropertyName == nameof(DomainFactoryArtifact.Description))
            {
                DescriptionField.Value = _factory?.Description;
            }
            else if (e.PropertyName == nameof(DomainFactoryArtifact.Category))
            {
                CategoryField.Value = _factory?.Category;
            }
            else if (e.PropertyName == nameof(DomainFactoryArtifact.CreatesAggregates))
            {
                CreatesAggregatesField.Value = _factory?.CreatesAggregates;
            }
            else if (e.PropertyName == nameof(DomainFactoryArtifact.IsStateless))
            {
                IsStatelessField.Value = _factory?.IsStateless;
            }
            else if (e.PropertyName == nameof(DomainFactoryArtifact.HasDependencies))
            {
                HasDependenciesField.Value = _factory?.HasDependencies;
            }
        }

        /// <summary>
        /// Factory name field
        /// </summary>
        public SingleLineTextFieldModel NameField { get; }

        /// <summary>
        /// Factory description field
        /// </summary>
        public SingleLineTextFieldModel DescriptionField { get; }

        /// <summary>
        /// Category field
        /// </summary>
        public SingleLineTextFieldModel CategoryField { get; }

        /// <summary>
        /// Creates aggregates field
        /// </summary>
        public BooleanFieldModel CreatesAggregatesField { get; }

        /// <summary>
        /// Is stateless field
        /// </summary>
        public BooleanFieldModel IsStatelessField { get; }

        /// <summary>
        /// Has dependencies field
        /// </summary>
        public BooleanFieldModel HasDependenciesField { get; }

        /// <summary>
        /// Event raised when any field value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadFromFactory()
        {
            if (_factory == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _factory.Name;
                DescriptionField.Value = _factory.Description;
                CategoryField.Value = _factory.Category;
                CreatesAggregatesField.Value = _factory.CreatesAggregates;
                IsStatelessField.Value = _factory.IsStateless;
                HasDependenciesField.Value = _factory.HasDependencies;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _factory == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToFactory();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_factory, field.Name, field.Value));
            }
        }

        private void SaveToFactory()
        {
            if (_factory == null) return;

            _factory.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) 
                ? NameField.Value as string 
                : "Factory";
            _factory.Description = DescriptionField.Value as string;
            _factory.Category = CategoryField.Value as string;
            _factory.CreatesAggregates = CreatesAggregatesField.Value is bool createsAggregates && createsAggregates;
            _factory.IsStateless = IsStatelessField.Value is bool isStateless && isStateless;
            _factory.HasDependencies = HasDependenciesField.Value is bool hasDependencies && hasDependencies;
        }
    }
}
