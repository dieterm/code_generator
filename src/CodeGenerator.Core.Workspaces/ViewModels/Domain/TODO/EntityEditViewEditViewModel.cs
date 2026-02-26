using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Artifacts.Events;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CodeGenerator.Application.ViewModels.Workspace.Domains
{
    /// <summary>
    /// ViewModel for editing EntityEditViewArtifact properties
    /// </summary>
    public class EntityEditViewEditViewModel : ViewModelBase
    {
        private EntityEditViewArtifact? _editView;
        private bool _isLoading;
        private readonly List<EntityEditViewFieldArtifact> _subscribedFields = new();

        public EntityEditViewEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "View Name", Name = nameof(EntityEditViewArtifact.Name) };
            EntityStateField = new ComboboxFieldModel { Label = "Entity State", Name = nameof(EntityEditViewArtifact.EntityStateId) };
            PreviewFields = new ObservableCollection<EntityEditViewFieldPreviewModel>();

            NameField.PropertyChanged += OnFieldChanged;
            EntityStateField.PropertyChanged += OnEntityStateFieldChanged;
        }

        /// <summary>
        /// The edit view being edited
        /// </summary>
        public EntityEditViewArtifact? EditView
        {
            get => _editView;
            set
            {
                if (_editView != null)
                {
                    _editView.PropertyChanged -= EditView_PropertyChanged;
                    _editView.ChildAdded -= EditView_ChildAdded;
                    _editView.ChildRemoved -= EditView_ChildRemoved;
                    UnsubscribeFromFieldChanges();
                }
                if (SetProperty(ref _editView, value))
                {
                    LoadAvailableStates();
                    LoadFromEditView();
                    LoadPreviewFields();
                    if (_editView != null)
                    {
                        _editView.PropertyChanged += EditView_PropertyChanged;
                        _editView.ChildAdded += EditView_ChildAdded;
                        _editView.ChildRemoved += EditView_ChildRemoved;
                        SubscribeToFieldChanges();
                    }
                }
            }
        }

        private void EditView_ChildAdded(object? sender, ChildAddedEventArgs e)
        {
            if (e.ChildArtifact is EntityEditViewFieldArtifact fieldArtifact)
            {
                fieldArtifact.PropertyChanged += Field_PropertyChanged;
                _subscribedFields.Add(fieldArtifact);
            }
            LoadPreviewFields();
        }

        private void EditView_ChildRemoved(object? sender, ChildRemovedEventArgs e)
        {
            if (e.ChildArtifact is EntityEditViewFieldArtifact fieldArtifact)
            {
                fieldArtifact.PropertyChanged -= Field_PropertyChanged;
                _subscribedFields.Remove(fieldArtifact);
            }
            LoadPreviewFields();
        }

        private void SubscribeToFieldChanges()
        {
            if (_editView == null) return;
            foreach (var field in _editView.GetFields())
            {
                field.PropertyChanged += Field_PropertyChanged;
                _subscribedFields.Add(field);
            }
        }

        private void UnsubscribeFromFieldChanges()
        {
            foreach (var field in _subscribedFields)
            {
                field.PropertyChanged -= Field_PropertyChanged;
            }
            _subscribedFields.Clear();
        }

        private void Field_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Reload preview when any field property changes
            LoadPreviewFields();
        }

        private void EditView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EntityEditViewArtifact.Name))
            {
                NameField.Value = _editView?.Name;
            }
        }

        public SingleLineTextFieldModel NameField { get; }
        public ComboboxFieldModel EntityStateField { get; }
        
        /// <summary>
        /// Collection of preview field models for the preview panel
        /// </summary>
        public ObservableCollection<EntityEditViewFieldPreviewModel> PreviewFields { get; }

        /// <summary>
        /// Event raised when the preview needs to be refreshed
        /// </summary>
        public event EventHandler? PreviewChanged;

        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadAvailableStates()
        {
            EntityStateField.Items.Clear();

            if (_editView == null) return;

            var entity = _editView.FindAncesterOfType<EntityArtifact>();
            if (entity == null) return;

            foreach (var state in entity.GetStates())
            {
                EntityStateField.Items.Add(new ComboboxItem 
                { 
                    DisplayName = state.Name, 
                    Value = state.Id 
                });
            }
        }

        private void LoadFromEditView()
        {
            if (_editView == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _editView.Name;

                var selectedState = EntityStateField.Items
                    .FirstOrDefault(i => i.Value?.ToString() == _editView.EntityStateId);
                EntityStateField.SelectedItem = selectedState;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void LoadPreviewFields()
        {
            PreviewFields.Clear();

            if (_editView == null) return;

            var fields = _editView.GetFields()
                .OrderBy(f => f.DisplayOrder)
                .ToList();

            foreach (var field in fields)
            {
                PreviewFields.Add(new EntityEditViewFieldPreviewModel
                {
                    PropertyName = field.PropertyName,
                    Label = !string.IsNullOrEmpty(field.Label) ? field.Label : field.PropertyName,
                    Tooltip = field.Tooltip,
                    Placeholder = field.Placeholder,
                    ControlType = field.ControlType,
                    IsReadOnly = field.IsReadOnly,
                    IsRequired = field.IsRequired,
                    DisplayOrder = field.DisplayOrder
                });
            }

            // Notify view that preview needs refresh
            PreviewChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _editView == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToEditView();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_editView, field.Name, field.Value));
            }
        }

        private void OnEntityStateFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _editView == null) return;

            if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem))
            {
                _editView.EntityStateId = EntityStateField.SelectedItem?.Value?.ToString();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_editView, nameof(EntityEditViewArtifact.EntityStateId), _editView.EntityStateId));
            }
        }

        private void SaveToEditView()
        {
            if (_editView == null) return;
            _editView.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) ? NameField.Value as string : "EditView";
        }
    }

    /// <summary>
    /// Model representing a field preview in the edit view preview panel
    /// </summary>
    public class EntityEditViewFieldPreviewModel
    {
        public string PropertyName { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string? Tooltip { get; set; }
        public string? Placeholder { get; set; }
        public DataFieldControlType ControlType { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
    }
}
