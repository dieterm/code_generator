using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Core.Workspaces.Artifacts.Domains;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.UserControls.ViewModels;
using System.ComponentModel;

namespace CodeGenerator.Application.ViewModels.Workspace.Domains
{
    /// <summary>
    /// ViewModel for editing EntityListViewArtifact properties
    /// </summary>
    public class EntityListViewEditViewModel : ViewModelBase
    {
        private EntityListViewArtifact? _listView;
        private bool _isLoading;

        public EntityListViewEditViewModel()
        {
            NameField = new SingleLineTextFieldModel { Label = "View Name", Name = nameof(EntityListViewArtifact.Name) };

            NameField.PropertyChanged += OnFieldChanged;
        }

        public EntityListViewArtifact? ListView
        {
            get => _listView;
            set
            {
                if (_listView != null)
                {
                    _listView.PropertyChanged -= ListView_PropertyChanged;
                }
                if (SetProperty(ref _listView, value))
                {
                    LoadFromListView();
                    if (_listView != null)
                    {
                        _listView.PropertyChanged += ListView_PropertyChanged;
                    }
                }
            }
        }

        private void ListView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EntityListViewArtifact.Name))
            {
                NameField.Value = _listView?.Name;
            }
        }

        public SingleLineTextFieldModel NameField { get; }

        public event EventHandler<ArtifactPropertyChangedEventArgs>? ValueChanged;

        private void LoadFromListView()
        {
            if (_listView == null) return;

            _isLoading = true;
            try
            {
                NameField.Value = _listView.Name;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading || _listView == null) return;

            if (e.PropertyName == nameof(FieldViewModelBase.Value) && sender is FieldViewModelBase field)
            {
                SaveToListView();
                ValueChanged?.Invoke(this, new ArtifactPropertyChangedEventArgs(_listView, field.Name, field.Value));
            }
        }

        private void SaveToListView()
        {
            if (_listView == null) return;
            _listView.Name = !string.IsNullOrWhiteSpace(NameField.Value as string) ? NameField.Value as string : "ListView";
        }
    }
}
