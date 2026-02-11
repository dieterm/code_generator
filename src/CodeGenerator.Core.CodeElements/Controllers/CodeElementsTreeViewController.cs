using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.CodeElements.Services;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Shared.Operations;
using CodeGenerator.Shared.ViewModels;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.CodeElements.Controllers
{
    public class CodeElementsTreeViewController : ArtifactTreeViewController<CodeElementsTreeViewModel, ICodeElementArtifactController>
    {
        private readonly ICodeElementsWindowManagerService _windowManagerService;
        private CodeElementArtifactDetailsViewModel? _codeElementDetailsViewModel;

        public CodeElementsTreeViewController(ICodeElementsWindowManagerService windowManagerService, OperationExecutor operationExecutor, IMessageBoxService messageBoxService, ILogger<CodeElementsTreeViewController> logger) 
            : base(operationExecutor, messageBoxService, logger)
        {
            _windowManagerService = windowManagerService ?? throw new ArgumentNullException(nameof(windowManagerService));
        }

        public override void ShowArtifactDetailsView(ViewModelBase? detailsModel)
        {
            if (_codeElementDetailsViewModel == null)
            {
                _codeElementDetailsViewModel = new CodeElementArtifactDetailsViewModel();
            }
            _codeElementDetailsViewModel.DetailsViewModel = detailsModel;
            _windowManagerService.ShowCodeElementsDetailsView(_codeElementDetailsViewModel);
        }

        //protected override IEnumerable<IArtifactController> LoadArtifactControllers()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
