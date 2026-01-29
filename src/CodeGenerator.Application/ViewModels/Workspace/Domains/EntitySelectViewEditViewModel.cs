using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Application.ViewModels.Workspace.Domains
{
    /// <summary>
    /// ViewModel for editing EntitySelectViewArtifact properties
    /// </summary>
    public class EntitySelectViewEditViewModel : ViewModelBase
    {
        private EntitySelectViewArtifact? _selectView;
        private bool _isLoading;

        public EntitySelectViewEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "View Name", Name = nameof(EntitySelectViewArtifact.Name) };
            DisplayMemberPathField = new ComboboxFieldModel { Label = "Display Member", Name = nameof(EntitySelectViewArtifact.DisplayMemberPath) };
            ValueMemberPathField = new ComboboxFieldModel { Label = "Value Member", Name = nameof(EntitySelectViewArtifact.ValueMemberPath) };
            DisplayFormatField = new SingleLineTextFieldModel { Label = "Display Format", Name = nameof(EntitySelectViewArtifact.DisplayFormat), Tooltip = "E.g., {FirstName} {LastName}" };
            SearchPropertyPathField = new ComboboxFieldModel { Label = "Search Property", Name = nameof(EntitySelectViewArtifact.SearchPropertyPath) };
            SortPropertyPathField = new ComboboxFieldModel { Label = "Sort Property", Name = nameof(EntitySelectViewArtifact.SortPropertyPath) };
            SortAscendingField = new BooleanFieldModel { Label = "Sort Ascending", Name = nameof(EntitySelectViewArtifact.SortAscending) };

            NameField.PropertyChanged += OnFieldChanged;
            DisplayMemberPathField.PropertyChanged += OnComboboxFieldChanged;
            ValueMemberPathField.PropertyChanged += OnComboboxFieldChanged;
            DisplayFormatField.PropertyChanged += OnFieldChanged;
            SearchPropertyPathField.PropertyChanged += OnComboboxFieldChanged;
            SortPropertyPathField.PropertyChanged += OnComboboxFieldChanged;
            SortAscendingField.PropertyChanged += OnFieldChanged;
        }

        public EntitySelectViewArtifact? SelectView
        {
            get => _selectView;
            set
            {
                if (_selectView != null)
                {
                    _selectView.PropertyChanged -= SelectView_PropertyChanged;
                }
                if (SetProperty(ref _selectView, value))
                {
                    LoadAvailableProperties();
                    LoadFromSelectView();
                    if (_selectView != null)
                    {
                        _selectView.PropertyChanged += SelectView_PropertyChanged;
                    }
                }
            }
        }

        private void SelectView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EntitySelectViewArtifact.Name))
            {
                NameField.Value = _selectView?.Name;
            }
        }

        public SingleLineTextFieldModel NameField { get; }
        public ComboboxFieldModel DisplayMemberPathField { get; }
        public ComboboxFieldModel ValueMemberPathField { get; }
        public SingleLineTextFieldModel DisplayFormatField { get; }
        public ComboboxFieldModel SearchPropertyPathField { get; }
        public ComboboxFieldModel SortPropertyPathField { get; }
        public BooleanFieldModel SortAscendingField { get; }

        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadAvailableProperties()
        {
            DisplayMemberPathField.Items.Clear();
            ValueMemberPathField.Items.Clear();
            SearchPropertyPathField.Items.Clear();
            SortPropertyPathField.Items.Clear();

            if (_selectView == null) return;

            var entity = _selectView.FindAncesterOfType<EntityArtifact>();
            if (entity == null) return;

            var defaultState = entity.DefaultState;
            if (defaultState == null) return;

            // Add empty option for optional fields
            var emptyItem = new ComboboxItem { DisplayName = "(None)", Value = null };
            SearchPropertyPathField.Items.Add(emptyItem);
            SortPropertyPathField.Items.Add(new ComboboxItem { DisplayName = "(None)", Value = null });

            // Add Id as ValueMember option
            ValueMemberPathField.Items.Add(new ComboboxItem { DisplayName = "Id", Value = "Id" });

            foreach (var property in defaultState.Properties)
            {
                var item = new ComboboxItem { DisplayName = property.Name, Value = property.Name };
                DisplayMemberPathField.Items.Add(new ComboboxItem { DisplayName = property.Name, Value = property.Name });
                ValueMemberPathField.Items.Add(new ComboboxItem { DisplayName = property.Name, Value = property.Name });
                SearchPropertyPathField.Items.Add(new ComboboxItem { DisplayName = property.Name, Value = property.Name });
                SortPropertyPathField.Items.Add(new ComboboxItem { DisplayName = property.Name, Value = property.Name });
            }
        }

        private void LoadFromSelectView()
        {
            if (_selectView == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _selectView.Name;

                var selectedDisplayMember = DisplayMemberPathField.Items
                    .FirstOrDefault(i => i.Value?.ToString() == _selectView.DisplayMemberPath);
                DisplayMemberPathField.SelectedItem = selectedDisplayMember;

                var selectedValueMember = ValueMemberPathField.Items
                    .FirstOrDefault(i => i.Value?.ToString() == _selectView.ValueMemberPath);
                ValueMemberPathField.SelectedItem = selectedValueMember;

                DisplayFormatField.Value = _selectView.DisplayFormat;

                var selectedSearchProperty = SearchPropertyPathField.Items
                    .FirstOrDefault(i => i.Value?.ToString() == _selectView.SearchPropertyPath);
                SearchPropertyPathField.SelectedItem = selectedSearchProperty;

                var selectedSortProperty = SortPropertyPathField.Items
                    .FirstOrDefault(i => i.Value?.ToString() == _selectView.SortPropertyPath);
                SortPropertyPathField.SelectedItem = selectedSortProperty;

                SortAscendingField.Value = _selectView.SortAscending;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _selectView == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToSelectView();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_selectView, field.Name, field.Value));
            }
        }

        private void OnComboboxFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _selectView == null) return;

            if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem) && sender is ComboboxFieldModel field)
            {
                SaveToSelectView();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_selectView, field.Name, field.SelectedItem?.Value));
            }
        }

        private void SaveToSelectView()
        {
            if (_selectView == null) return;
            _selectView.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) ? NameField.Value as string : "SelectView";
            _selectView.DisplayMemberPath = DisplayMemberPathField.SelectedItem?.Value?.ToString();
            _selectView.ValueMemberPath = ValueMemberPathField.SelectedItem?.Value?.ToString();
            _selectView.DisplayFormat = DisplayFormatField.Value as string;
            _selectView.SearchPropertyPath = SearchPropertyPathField.SelectedItem?.Value?.ToString();
            _selectView.SortPropertyPath = SortPropertyPathField.SelectedItem?.Value?.ToString();
            _selectView.SortAscending = SortAscendingField.Value is bool sortAsc && sortAsc;
        }
    }
}
