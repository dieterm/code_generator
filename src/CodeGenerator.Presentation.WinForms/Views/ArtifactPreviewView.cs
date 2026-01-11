using CodeGenerator.Application.ViewModels;
using CodeGenerator.Shared.ViewModels;
using CodeGenerator.Shared.Views;
using Syncfusion.Windows.Forms.Edit;
using Syncfusion.Windows.Forms.Edit.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeGenerator.Presentation.WinForms.Views
{
    public partial class ArtifactPreviewView : UserControl, IView<ArtifactPreviewViewModel>
    {
        public ArtifactPreviewView()
        {
            InitializeComponent();
        }

        public void BindViewModel(ArtifactPreviewViewModel viewModel)
        {
            // reset controls
            editControl.Text = string.Empty;
            editControl.ReadOnly = true;

            if (viewModel == null) return;

            editControl.Visible = viewModel.TextContent != null;
            if (viewModel.TextContent != null) { 
                // default_language;XML;Pascal;C#;JScript;HTML (Light);Java;C;PowerShell;VB.NET;SQL;VBScript; (Parameter 'name')'
                // map ArtifactPreviewViewModel.KnownLanguages to Syncfusion.Windows.Forms.Edit.Enums.KnownLanguages by name
                try
                {
                    KnownLanguages language;
                    if (!Enum.TryParse<KnownLanguages>(viewModel.TextLanguageSchema.ToString(), out language) || language== KnownLanguages.Undefined)
                    {
                        language = KnownLanguages.Text;
                    }

                    editControl.ApplyConfiguration(language);
                }
                catch (Exception ex)
                {

                    // throw;
                }
                editControl.Text = viewModel.TextContent ?? string.Empty;
            }

            
            
        }

        public void BindViewModel<TModel>(TModel viewModel) where TModel : ViewModelBase
        {
            BindViewModel((ArtifactPreviewViewModel)(object)viewModel);
        }
    }
}
