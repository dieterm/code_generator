using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator.Core.CodeElements.Views
{
    public partial class CodeElementsEditorView : UserControl, IView<CodeElementsEditorViewModel>
    {
        public CodeElementsEditorView()
        {
            InitializeComponent();
        }

        public CodeElementsEditorViewModel? ViewModel { get; set; }

        public void BindViewModel(CodeElementsEditorViewModel viewModel)
        {
            editControl.DataBindings.Clear();

            ViewModel = viewModel;

            if (viewModel != null) { 
                editControl.DataBindings.Clear();
                editControl.DataBindings.Add("Text", ViewModel, nameof(ViewModel.Text), false, DataSourceUpdateMode.OnPropertyChanged);
                editControl.ApplyConfiguration(ViewModel.Syntax);
                editControl.Closing += (s, e) => e.Action = Syncfusion.Windows.Forms.Edit.Enums.SaveChangesAction.Discard;
            }
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((CodeElementsEditorViewModel)(object)viewModel);
        }
    }
}
