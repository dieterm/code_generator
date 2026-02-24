using CodeGenerator.Core.CodeElements.Views.EditFields;
using CodeGenerator.Generators.DotNet.Repositories.Csv.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System.ComponentModel;

namespace CodeGenerator.Generators.DotNet.Repositories.Csv.Views
{
    /// <summary>
    /// View for editing CsvValueObjectReaderImplementationArtifact properties
    /// </summary>
    public partial class CsvValueObjectReaderImplementationArtifactEditView : UserControl, IView<CsvValueObjectReaderImplementationArtifactEditViewModel>
    {
        private CsvValueObjectReaderImplementationArtifactEditViewModel? _viewModel;

        public CsvValueObjectReaderImplementationArtifactEditView()
        {
            InitializeComponent();
        }

        public void BindViewModel(CsvValueObjectReaderImplementationArtifactEditViewModel? viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel == null) return;

            txtName.BindViewModel(_viewModel.NameField);
            txtDescription.BindViewModel(_viewModel.DescriptionField);
            cmbValueType.BindViewModel(_viewModel.ValueTypeField);
            codeFileField.BindViewModel(_viewModel.CodeFileField);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Handle property changes if needed
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : IViewModel
        {
            BindViewModel((CsvValueObjectReaderImplementationArtifactEditViewModel)(object)viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                }
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
