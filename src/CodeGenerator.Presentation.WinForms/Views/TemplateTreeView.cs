using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Template;
using CodeGenerator.Application.ViewModels.Base;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Templates;
using Syncfusion.Windows.Forms.Tools;
using System.ComponentModel;

namespace CodeGenerator.Presentation.WinForms.Views
{
    /// <summary>
    /// TreeView control for displaying and browsing templates
    /// </summary>
    public partial class TemplateTreeView : ArtifactTreeView
    {
        public TemplateTreeView()
            : base()
        {

        }

    }
}
