using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Application.ViewModels.Workspace.Domains
{
    /// <summary>
    /// ViewModel for editing entity properties
    /// </summary>
    public class EntityEditViewModel : ViewModelBase
    {
        private EntityArtifact? _entity;
        private bool _isLoading;

        public EntityEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "Entity Name", Name = nameof(EntityArtifact.Name) };
            DescriptionField = new SingleLineTextFieldModel { Label = "Description", Name = nameof(EntityArtifact.Description) };

            NameField.PropertyChanged += OnFieldChanged;
            DescriptionField.PropertyChanged += OnFieldChanged;
        }

        /// <summary>
        /// The entity being edited
        /// </summary>
        public EntityArtifact? Entity
        {
            get => _entity;
            set
            {
                if (_entity != null)
                {
                    _entity.PropertyChanged -= Entity_PropertyChanged;
                }
                if (SetProperty(ref _entity, value))
                {
                    LoadFromEntity();
                    if (_entity != null)
                    {
                        _entity.PropertyChanged += Entity_PropertyChanged;
                    }
                }
            }
        }

        private void Entity_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EntityArtifact.Name))
            {
                NameField.Value = _entity?.Name;
            }
        }

        /// <summary>
        /// Entity name field
        /// </summary>
        public SingleLineTextFieldModel NameField { get; }

        /// <summary>
        /// Entity description field
        /// </summary>
        public SingleLineTextFieldModel DescriptionField { get; }

        /// <summary>
        /// Event raised when any field value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadFromEntity()
        {
            if (_entity == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _entity.Name;
                DescriptionField.Value = _entity.Description;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _entity == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToEntity();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_entity, field.Name, field.Value));
            }
        }

        private void SaveToEntity()
        {
            if (_entity == null) return;
            _entity.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) ? NameField.Value as string : "Entity";
            _entity.Description = DescriptionField.Value as string;
        }
    }
}
