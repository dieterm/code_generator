using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Domains.Entities;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Application.ViewModels.Workspace.Domains
{
    /// <summary>
    /// ViewModel for editing EntityListViewColumnArtifact properties
    /// </summary>
    public class EntityListViewColumnEditViewModel : ViewModelBase
    {
        private EntityListViewColumnArtifact? _column;
        private bool _isLoading;

        public EntityListViewColumnEditViewModel()
        {
            PropertyPathField = new SingleLineTextFieldModel { Label = "Property Path", Name = nameof(EntityListViewColumnArtifact.PropertyPath) };
            HeaderTextField = new SingleLineTextFieldModel { Label = "Header Text", Name = nameof(EntityListViewColumnArtifact.HeaderText) };
            ColumnTypeField = new ComboboxFieldModel { Label = "Column Type", Name = nameof(EntityListViewColumnArtifact.ColumnType) };
            WidthField = new IntegerFieldModel { Label = "Width", Name = nameof(EntityListViewColumnArtifact.Width), Minimum = 0, Maximum = 1000 };
            DisplayOrderField = new IntegerFieldModel { Label = "Display Order", Name = nameof(EntityListViewColumnArtifact.DisplayOrder), Minimum = 0, Maximum = 1000 };
            FormatStringField = new SingleLineTextFieldModel { Label = "Format String", Name = nameof(EntityListViewColumnArtifact.FormatString) };
            IsVisibleField = new BooleanFieldModel { Label = "Visible", Name = nameof(EntityListViewColumnArtifact.IsVisible) };
            IsSortableField = new BooleanFieldModel { Label = "Sortable", Name = nameof(EntityListViewColumnArtifact.IsSortable) };
            IsFilterableField = new BooleanFieldModel { Label = "Filterable", Name = nameof(EntityListViewColumnArtifact.IsFilterable) };

            // Initialize column type options
            foreach (var columnType in Enum.GetValues<ListViewColumnType>())
            {
                ColumnTypeField.Items.Add(new ComboboxItem 
                { 
                    DisplayName = columnType.ToString(), 
                    Value = columnType 
                });
            }

            PropertyPathField.PropertyChanged += OnFieldChanged;
            HeaderTextField.PropertyChanged += OnFieldChanged;
            ColumnTypeField.PropertyChanged += OnColumnTypeFieldChanged;
            WidthField.PropertyChanged += OnFieldChanged;
            DisplayOrderField.PropertyChanged += OnFieldChanged;
            FormatStringField.PropertyChanged += OnFieldChanged;
            IsVisibleField.PropertyChanged += OnFieldChanged;
            IsSortableField.PropertyChanged += OnFieldChanged;
            IsFilterableField.PropertyChanged += OnFieldChanged;
        }

        public EntityListViewColumnArtifact? Column
        {
            get => _column;
            set
            {
                if (_column != null)
                {
                    _column.PropertyChanged -= Column_PropertyChanged;
                }
                if (SetProperty(ref _column, value))
                {
                    LoadFromColumn();
                    if (_column != null)
                    {
                        _column.PropertyChanged += Column_PropertyChanged;
                    }
                }
            }
        }

        private void Column_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EntityListViewColumnArtifact.PropertyPath))
            {
                PropertyPathField.Value = _column?.PropertyPath;
            }
        }

        public SingleLineTextFieldModel PropertyPathField { get; }
        public SingleLineTextFieldModel HeaderTextField { get; }
        public ComboboxFieldModel ColumnTypeField { get; }
        public IntegerFieldModel WidthField { get; }
        public IntegerFieldModel DisplayOrderField { get; }
        public SingleLineTextFieldModel FormatStringField { get; }
        public BooleanFieldModel IsVisibleField { get; }
        public BooleanFieldModel IsSortableField { get; }
        public BooleanFieldModel IsFilterableField { get; }

        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadFromColumn()
        {
            if (_column == null) return;

            _isLoading = true;
            try
            {
                PropertyPathField.Value = _column.PropertyPath;
                HeaderTextField.Value = _column.HeaderText;

                var selectedColumnType = ColumnTypeField.Items
                    .FirstOrDefault(i => i.Value is ListViewColumnType ct && ct == _column.ColumnType);
                ColumnTypeField.SelectedItem = selectedColumnType;

                WidthField.Value = _column.Width;
                DisplayOrderField.Value = _column.DisplayOrder;
                FormatStringField.Value = _column.FormatString;
                IsVisibleField.Value = _column.IsVisible;
                IsSortableField.Value = _column.IsSortable;
                IsFilterableField.Value = _column.IsFilterable;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _column == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToColumn();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_column, field.Name, field.Value));
            }
        }

        private void OnColumnTypeFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _column == null) return;

            if (e.PropertyName == nameof(ComboboxFieldModel.SelectedItem))
            {
                if (ColumnTypeField.SelectedItem?.Value is ListViewColumnType columnType)
                {
                    _column.ColumnType = columnType;
                    ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_column, nameof(EntityListViewColumnArtifact.ColumnType), columnType));
                }
            }
        }

        private void SaveToColumn()
        {
            if (_column == null) return;
            _column.PropertyPath = !string.IsNullOrWhiteSpace(PropertyPathField.Value as string) ? PropertyPathField.Value as string : "Property";
            _column.HeaderText = HeaderTextField.Value as string;
            _column.Width = WidthField.Value as int?;
            _column.DisplayOrder = DisplayOrderField.Value is int displayOrder ? displayOrder : 0;
            _column.FormatString = FormatStringField.Value as string;
            _column.IsVisible = IsVisibleField.Value is bool isVisible && isVisible;
            _column.IsSortable = IsSortableField.Value is bool isSortable && isSortable;
            _column.IsFilterable = IsFilterableField.Value is bool isFilterable && isFilterable;
        }
    }
}
