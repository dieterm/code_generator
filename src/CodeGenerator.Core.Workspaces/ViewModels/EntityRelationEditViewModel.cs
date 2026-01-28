using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts.Views;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Core.Workspaces.ViewModels
{
    /// <summary>
    /// ViewModel for editing entity relation properties
    /// </summary>
    public class EntityRelationEditViewModel : ViewModelBase
    {
        private EntityRelationArtifact? _relation;
        private bool _isLoading;

        public EntityRelationEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "Relation Name", Name = nameof(EntityRelationArtifact.Name) };
            TargetEntityField = new ComboboxFieldModel { Label = "Target Entity", Name = nameof(EntityRelationArtifact.TargetEntityId) };
            SourceCardinalityField = new ComboboxFieldModel { Label = "Source Cardinality", Name = nameof(EntityRelationArtifact.SourceCardinality) };
            TargetCardinalityField = new ComboboxFieldModel { Label = "Target Cardinality", Name = nameof(EntityRelationArtifact.TargetCardinality) };
            SourcePropertyNameField = new SingleLineTextFieldModel { Label = "Source Property Name", Name = nameof(EntityRelationArtifact.SourcePropertyName) };
            TargetPropertyNameField = new SingleLineTextFieldModel { Label = "Target Property Name", Name = nameof(EntityRelationArtifact.TargetPropertyName) };

            // Initialize cardinality options
            InitializeCardinalityItems(SourceCardinalityField);
            InitializeCardinalityItems(TargetCardinalityField);

            NameField.PropertyChanged += OnFieldChanged;
            TargetEntityField.PropertyChanged += OnTargetEntityFieldChanged;
            SourceCardinalityField.PropertyChanged += OnCardinalityFieldChanged;
            TargetCardinalityField.PropertyChanged += OnCardinalityFieldChanged;
            SourcePropertyNameField.PropertyChanged += OnFieldChanged;
            TargetPropertyNameField.PropertyChanged += OnFieldChanged;
        }

        private void InitializeCardinalityItems(ComboboxFieldModel field)
        {
            field.Items.Add(new ComboboxItem { DisplayName = "1 (Exactly one)", Value = RelationCardinality.One });
            field.Items.Add(new ComboboxItem { DisplayName = "0..1 (Zero or one)", Value = RelationCardinality.ZeroOrOne });
            field.Items.Add(new ComboboxItem { DisplayName = "0..* (Zero or many)", Value = RelationCardinality.ZeroOrMany });
            field.Items.Add(new ComboboxItem { DisplayName = "1..* (One or many)", Value = RelationCardinality.OneOrMany });
        }

        /// <summary>
        /// The relation being edited
        /// </summary>
        public EntityRelationArtifact? Relation
        {
            get => _relation;
            set
            {
                if (_relation != null)
                {
                    _relation.PropertyChanged -= Relation_PropertyChanged;
                }
                if (SetProperty(ref _relation, value))
                {
                    LoadAvailableEntities();
                    LoadFromRelation();
                    if (_relation != null)
                    {
                        _relation.PropertyChanged += Relation_PropertyChanged;
                    }
                }
            }
        }

        private void Relation_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EntityRelationArtifact.Name))
            {
                NameField.Value = _relation?.Name;
            }
        }

        // Field ViewModels
        public SingleLineTextFieldModel NameField { get; }
        public ComboboxFieldModel TargetEntityField { get; }
        public ComboboxFieldModel SourceCardinalityField { get; }
        public ComboboxFieldModel TargetCardinalityField { get; }
        public SingleLineTextFieldModel SourcePropertyNameField { get; }
        public SingleLineTextFieldModel TargetPropertyNameField { get; }

        /// <summary>
        /// Event raised when any field value changes
        /// </summary>
        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadAvailableEntities()
        {
            TargetEntityField.Items.Clear();

            if (_relation == null) return;

            // Get the parent entity
            var parentEntity = _relation.FindAncesterOfType<EntityArtifact>();
            var domain = _relation.FindAncesterOfType<DomainArtifact>();

            if (domain == null) return;

            // Get all entities except the parent entity
            var entities = domain.Entities.GetEntities()
                .Where(e => e.Id != parentEntity?.Id)
                .OrderBy(e => e.Name);

            foreach (var entity in entities)
            {
                TargetEntityField.Items.Add(new ArtifactComboboxItem(entity));
            }
        }

        private void LoadFromRelation()
        {
            if (_relation == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _relation.Name;

                // Set selected target entity
                var selectedEntity = TargetEntityField.Items
                    .FirstOrDefault(i => i.Value?.ToString() == _relation.TargetEntityId);
                TargetEntityField.SelectedItem = selectedEntity;

                // Set selected cardinalities
                var selectedSourceCardinality = SourceCardinalityField.Items
                    .FirstOrDefault(i => i.Value is RelationCardinality c && c == _relation.SourceCardinality);
                SourceCardinalityField.SelectedItem = selectedSourceCardinality;

                var selectedTargetCardinality = TargetCardinalityField.Items
                    .FirstOrDefault(i => i.Value is RelationCardinality c && c == _relation.TargetCardinality);
                TargetCardinalityField.SelectedItem = selectedTargetCardinality;

                // Set property names
                SourcePropertyNameField.Value = _relation.SourcePropertyName;
                TargetPropertyNameField.Value = _relation.TargetPropertyName;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _relation == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToRelation();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_relation, field.Name, field.Value));
            }
        }

        private void OnTargetEntityFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _relation == null) return;

            if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem))
            {
                _relation.TargetEntityId = TargetEntityField.SelectedItem?.Value?.ToString();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_relation, nameof(EntityRelationArtifact.TargetEntityId), _relation.TargetEntityId));
            }
        }

        private void OnCardinalityFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _relation == null) return;

            if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem) && sender is ComboboxFieldModel field)
            {
                if (field == SourceCardinalityField && SourceCardinalityField.SelectedItem?.Value is RelationCardinality sourceCardinality)
                {
                    _relation.SourceCardinality = sourceCardinality;
                    ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_relation, nameof(EntityRelationArtifact.SourceCardinality), sourceCardinality));
                }
                else if (field == TargetCardinalityField && TargetCardinalityField.SelectedItem?.Value is RelationCardinality targetCardinality)
                {
                    _relation.TargetCardinality = targetCardinality;
                    ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_relation, nameof(EntityRelationArtifact.TargetCardinality), targetCardinality));
                }
            }
        }

        private void SaveToRelation()
        {
            if (_relation == null) return;

            _relation.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) ? NameField.Value as string : "Relation";
            _relation.SourcePropertyName = SourcePropertyNameField.Value as string;
            _relation.TargetPropertyName = TargetPropertyNameField.Value as string;
        }
    }
}
