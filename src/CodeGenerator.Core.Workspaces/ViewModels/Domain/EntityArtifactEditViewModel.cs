using CodeGenerator.Core.Artifacts.Events;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Core.Workspaces.ViewModels.Common;
using CodeGenerator.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Workspaces.ViewModels.Domain
{
    public class EntityArtifactEditViewModel : ArtifactEditViewModel<EntityArtifact, EntityGeneralTabViewModel>
    {
        public EntityArtifactEditViewModel()
            : base("Entity", new EntityGeneralTabViewModel())
        {
        }
    }

    public class EntityGeneralTabViewModel : ArtifactEditViewTabModel<EntityArtifact>
    {
        public SingleLineTextFieldModel NameField { get; }
        public SingleLineTextFieldModel DescriptionField { get; }
        public BooleanFieldModel IsAggregateRootField { get; }
        public ComboboxFieldModel DefaultStateField { get; }

        private EntityStateArtifact? _lastDefaultState;

        public EntityGeneralTabViewModel() : base("General")
        {
            // Name field
            NameField = new SingleLineTextFieldModel
            {
                Label = "Entity Name",
                Name = nameof(EntityArtifact.Name),
                Tooltip = "Name of the entity",
                IsRequired = true,
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(NameField);

            // Description field
            DescriptionField = new SingleLineTextFieldModel
            {
                Label = "Description",
                Name = nameof(EntityArtifact.Description),
                Tooltip = "Description of the entity",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(DescriptionField);

            // Is Aggregate Root field
            IsAggregateRootField = new BooleanFieldModel
            {
                Label = "Is Aggregate Root",
                Name = nameof(EntityArtifact.IsAggregateRoot),
                Tooltip = "Indicates if this entity is an aggregate root in DDD",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(IsAggregateRootField);

            // Default State field
            DefaultStateField = new ComboboxFieldModel
            {
                Label = "Default State",
                Name = nameof(EntityArtifact.DefaultStateId),
                Tooltip = "The default state for this entity",
                AutoBind = true,
                AutoUpdate = true
            };
            FieldCollection.FieldModels.Add(DefaultStateField);
        }

        public override void BindArtifact(WorkspaceArtifactBase? artifactBase)
        {
            // Unsubscribe from old artifact events
            if (Artifact != null)
            {
                Artifact.EntityStates.ChildAdded -= States_ChildChanged;
                Artifact.EntityStates.ChildRemoved -= States_ChildChanged;
                UnobserveDefaultEntityState();
            }

            base.BindArtifact(artifactBase);

            // Subscribe to new artifact events
            if (Artifact != null)
            {
                RefreshStatesComboBox();
                Artifact.EntityStates.ChildAdded += States_ChildChanged;
                Artifact.EntityStates.ChildRemoved += States_ChildChanged;
                ObserveDefaultEntityState();
            }
        }

        protected override void OnArtifactPropertyChanged(EntityArtifact artifact, string propertyName)
        {
            if (propertyName == nameof(EntityArtifact.DefaultStateId))
            {
                ObserveDefaultEntityState();
            }
        }

        private void States_ChildChanged(object? sender, EventArgs e)
        {
            RefreshStatesComboBox();
        }

        private void RefreshStatesComboBox()
        {
            if (Artifact == null) return;

            var states = Artifact.GetStates().ToList();
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

        private void ObserveDefaultEntityState()
        {
            UnobserveDefaultEntityState();

            _lastDefaultState = Artifact?.DefaultState;
            if (_lastDefaultState != null)
            {
                _lastDefaultState.PropertyChanged += DefaultEntityState_PropertyChanged;
            }
        }

        private void UnobserveDefaultEntityState()
        {
            if (_lastDefaultState != null)
            {
                _lastDefaultState.PropertyChanged -= DefaultEntityState_PropertyChanged;
                _lastDefaultState = null;
            }
        }

        private void DefaultEntityState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EntityStateArtifact.Name))
            {
                RefreshStatesComboBox();
                DefaultStateField.Value = Artifact?.DefaultStateId;
            }
        }
    }
}
