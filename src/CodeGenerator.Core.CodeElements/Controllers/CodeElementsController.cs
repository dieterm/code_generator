using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.CodeElements;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.Artifacts.ViewModels;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.Services;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.ProgrammingLanguages.CSharp;
using CodeGenerator.Shared.Operations;
using CodeGenerator.Shared.Ribbon;
using CodeGenerator.Shared.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.CodeElements.Controllers
{
    public class CodeElementsController : CoreControllerBase, ICodeElementsController
    {
        private readonly ICodeElementsWindowManagerService _windowManagerService;
        //private readonly CodeElementsTreeViewModel _treeViewModel;
        private readonly CodeElementsEditorViewModel _editorViewModel;
        private readonly CodeElementArtifactDetailsViewModel _detailsViewModel;
        private readonly CodeElementsTreeViewController _treeViewController;
        public CodeElementsController(
            CodeElementsTreeViewController treeViewController,
            //CodeElementsTreeViewModel treeViewModel,
            CodeElementArtifactDetailsViewModel detailsViewModel,
            CodeElementsEditorViewModel editorViewModel,
            ICodeElementsWindowManagerService windowManagerService,
            OperationExecutor operationExecutor,
            RibbonBuilder ribbonBuilder, 
            ApplicationMessageBus messageBus,
            IMessageBoxService messageboxService, 
            IFileSystemDialogService fileSystemDialogService, 
            ILogger<CodeElementsController> logger
            )
            : base(operationExecutor, ribbonBuilder, messageBus, messageboxService, fileSystemDialogService, logger)
        {
            _windowManagerService = windowManagerService;
            //_treeViewModel = treeViewModel;
            _editorViewModel = editorViewModel;
            _detailsViewModel = detailsViewModel;
            _treeViewController = treeViewController;
        }

        public override void Dispose()
        {
            //throw new NotImplementedException();
        }

        public override void Initialize()
        {
            //throw new NotImplementedException();
        }

        public void ShowCodeElementsDetailsView(ViewModelBase? viewModel)
        {
            _detailsViewModel.DetailsViewModel = viewModel;
            _windowManagerService.ShowCodeElementsDetailsView(_detailsViewModel);
        }

        public void ShowCodeElementsEditor(string code, Domain.ProgrammingLanguages.ProgrammingLanguage language)
        {
            _editorViewModel.Text = code;
            _editorViewModel.Syntax = language switch
            {
                Domain.ProgrammingLanguages.CSharp.CSharpLanguage => Syncfusion.Windows.Forms.Edit.Enums.KnownLanguages.CSharp,
                Domain.ProgrammingLanguages.VisualBasic.VisualBasicLanguage => Syncfusion.Windows.Forms.Edit.Enums.KnownLanguages.VBNET,
                Domain.ProgrammingLanguages.Java.JavaLanguage => Syncfusion.Windows.Forms.Edit.Enums.KnownLanguages.Java,
                _ => Syncfusion.Windows.Forms.Edit.Enums.KnownLanguages.CSharp
            };
            _windowManagerService.ShowCodeElementsEditor(_editorViewModel);
        }
        public void ShowCodeElementsTreeView()
        {
            //_treeViewController.Sho
            _treeViewController.TreeViewModel.RootArtifact = new CodeFileElementArtifact(new CodeFileElement("Example", CSharpLanguage.Instance));
            //_treeViewModel.RootArtifact = new CodeFileElementArtifact(new CodeFileElement("Example", CSharpLanguage.Instance));
            _windowManagerService.ShowCodeElementsTreeView(_treeViewController.TreeViewModel);
        }

        public void ShowCodeElements()
        {
            ShowCodeElementsTreeView();
            ShowCodeElementsDetailsView(null);
            ShowCodeElementsEditor(string.Empty, CSharpLanguage.Instance);
        }

        public void ShowCodeElements(CodeFileElement codeFileElement)
        {
            throw new NotImplementedException();
        }
    }
}
