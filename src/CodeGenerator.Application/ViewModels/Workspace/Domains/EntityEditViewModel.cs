using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
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
            IsAggregateRootField = new BooleanFieldModel { Label = "Is Aggregate Root", Name = nameof(EntityArtifact.IsAggregateRoot) };
            DefaultStateField = new ComboboxFieldModel { Label = "Default State", Name = nameof(EntityArtifact.DefaultStateId) };

            NameField.PropertyChanged += OnFieldChanged;
            DescriptionField.PropertyChanged += OnFieldChanged;
            IsAggregateRootField.PropertyChanged += OnFieldChanged;
            DefaultStateField.PropertyChanged += OnFieldChanged;
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
                    _entity.States.ChildAdded -= States_ChildChanged;
                    _entity.States.ChildRemoved -= States_ChildChanged;
                }
                if (SetProperty(ref _entity, value))
                {
                    LoadFromEntity();
                    if (_entity != null)
                    {
                        _entity.PropertyChanged += Entity_PropertyChanged;
                        _entity.States.ChildAdded += States_ChildChanged;
                        _entity.States.ChildRemoved += States_ChildChanged;
                    }
                }
            }
        }

        private void States_ChildChanged(object? sender, EventArgs e)
        {
            // Refresh states list when states are added or removed
            RefreshStatesComboBox();
        }

        /// <summary>
        /// Observe entity property changes from outside
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Entity_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EntityArtifact.Name))
            {
                NameField.Value = _entity?.Name;
            }
            else if (e.PropertyName == nameof(EntityArtifact.DefaultStateId))
            {
                if (!_isLoading)
                {
                    DefaultStateField.Value = _entity?.DefaultStateId;
                    ObserveDefaultEntityState();
                }
            }
            else if (e.PropertyName == nameof(EntityArtifact.IsAggregateRoot))
            {
                IsAggregateRootField.Value = _entity?.IsAggregateRoot;
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
        /// Is aggregate root field
        /// </summary>
        public BooleanFieldModel IsAggregateRootField { get; }

        /// <summary>
        /// Default state selection field
        /// </summary>
        public ComboboxFieldModel DefaultStateField { get; }

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
                IsAggregateRootField.Value = _entity.IsAggregateRoot;

                RefreshStatesComboBox();
                DefaultStateField.Value = _entity.DefaultStateId;

                ObserveDefaultEntityState();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void ObserveDefaultEntityState()
        {
            if (_lastDefaultState != null)
            {
                _lastDefaultState.PropertyChanged -= EntityState_PropertyChanged;
            }
            _lastDefaultState = _entity.DefaultState;
            if (_lastDefaultState != null)
            {
                _lastDefaultState.PropertyChanged += EntityState_PropertyChanged;
            }
        }

        private EntityStateArtifact? _lastDefaultState;
        private void EntityState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // If the name of the default state changes, refresh the combo box to reflect the new name
            if (e.PropertyName == nameof(EntityStateArtifact.Name))
            {
                RefreshStatesComboBox();
                DefaultStateField.Value = _entity.DefaultStateId;
            }
        }

        private void RefreshStatesComboBox()
        {
            if (_entity == null) return;

            var states = _entity.GetStates().ToList();
            var items = new List<ComboboxItem>
            {
                new ComboboxItem { DisplayName = "(None)", Value = null }
            };

            foreach (var state in states)
            {
                items.Add(new ComboboxItem
                {
                    DisplayName = state.Name,
                    Value = state.Id,
                    Tooltip = string.Empty
                });
            }

            DefaultStateField.Items = items;
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _entity == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToEntity();
                if(field == DefaultStateField)
                {
                    ObserveDefaultEntityState();
                }
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_entity, field.Name, field.Value));
            }
        }

        private void SaveToEntity()
        {
            if (_entity == null) return;
            _entity.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) ? NameField.Value as string : "Entity";
            _entity.Description = DescriptionField.Value as string;
            _entity.IsAggregateRoot = IsAggregateRootField.Value is bool isAggregateRoot && isAggregateRoot;
            _entity.DefaultStateId = DefaultStateField.Value as string ?? string.Empty;
        }
    }
}
