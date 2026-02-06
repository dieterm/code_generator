using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Application.ViewModels.Workspace.Domains
{
    /// <summary>
    /// ViewModel for editing entity state properties
    /// </summary>
    public class EntityStateEditViewModel : ViewModelBase
    {
        private EntityStateArtifact? _entityState;
        private bool _isLoading;

        public EntityStateEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "State Name", Name = nameof(EntityStateArtifact.Name) };
            NameField.PropertyChanged += OnFieldChanged;
        }

        /// <summary>
        /// The entity state being edited
        /// </summary>
        public EntityStateArtifact? EntityState
        {
            get => _entityState;
            set
            {
                if (_entityState != null)
                {
                    _entityState.PropertyChanged -= EntityState_PropertyChanged;
                }
                if (SetProperty(ref _entityState, value))
                {
                    LoadFromEntityState();
                    if (_entityState != null)
                    {
                        _entityState.PropertyChanged += EntityState_PropertyChanged;
                    }
                }
            }
        }

        private void EntityState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EntityStateArtifact.Name))
            {
                NameField.Value = _entityState?.Name;
            }
        }

        /// <summary>
        /// State name field
        /// </summary>
        public SingleLineTextFieldModel NameField { get; }

        /// <summary>
        /// Event raised when any field value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadFromEntityState()
        {
            if (_entityState == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _entityState.Name;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _entityState == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToEntityState();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_entityState, field.Name, field.Value));
            }
        }

        private void SaveToEntityState()
        {
            if (_entityState == null) return;
            _entityState.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) ? NameField.Value as string : "State";
        }
    }
}
