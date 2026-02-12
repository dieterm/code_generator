using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.CodeElements;
using CodeGenerator.Application.Services;
using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.Events;
using CodeGenerator.Core.CodeElements.Artifacts;
using CodeGenerator.Core.CodeElements.Services;
using CodeGenerator.Core.CodeElements.ViewModels;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Workspaces.Artifacts;
using CodeGenerator.Domain.CodeElements;
using CodeGenerator.Domain.ProgrammingLanguages;
using CodeGenerator.Domain.ProgrammingLanguages.CSharp;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Operations;
using CodeGenerator.Shared.ViewModels;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace CodeGenerator.Core.CodeElements.Controllers
{
    public class CodeElementsTreeViewController : ArtifactTreeViewController<CodeElementsTreeViewModel, ICodeElementArtifactController>
    {
        private readonly ICodeElementsWindowManagerService _windowManagerService;
        private CodeElementArtifactDetailsViewModel? _codeElementDetailsViewModel;
        
        public CodeElementsTreeViewController( ICodeElementsWindowManagerService windowManagerService, OperationExecutor operationExecutor, IMessageBoxService messageBoxService, ILogger<CodeElementsTreeViewController> logger) 
            : base(operationExecutor, messageBoxService, logger)
        {
            _windowManagerService = windowManagerService ?? throw new ArgumentNullException(nameof(windowManagerService));
            HasUnsavedChangesChanged += RefreshCodeOutput;
        }

        private void RefreshCodeOutput(object? sender, EventArgs e)
        {
            if (HasUnsavedChanges) { 
                var codeFileElementArtifact = this.TreeViewModel?.RootArtifact as CodeFileElementArtifact;
                if (codeFileElementArtifact != null)
                {
                    ShowCodeOutputAsync(codeFileElementArtifact);
                    HasUnsavedChanges = false;
                }
            }
        }

        public void ShowCodeOutputAsync(CodeFileElementArtifact artifact)
        {
            if (artifact.Language == null) return;
            var generator = ProgrammingLanguageCodeGenerators.GetGenerator(artifact.Language);
            if (generator == null) return;
            var code = generator.GenerateCodeFile(artifact.CodeElement);
            var codeElementsController = ServiceProviderHolder.GetRequiredService<CodeElementsController>();
            codeElementsController.ShowCodeElementsEditor(code, artifact.Language);
        }

        public event EventHandler? HasUnsavedChangesChanged;
        private bool _hasUnsavedChanges = false;
        /// <summary>
        /// TODO: Subscribe to all events of all artifacts recursively.
        /// Also keep track of unsubscribing when artifacts are removed or when workspace is closed.
        /// </summary>
        public bool HasUnsavedChanges
        {
            get { return _hasUnsavedChanges; }
            private set
            {
                if (_hasUnsavedChanges != value)
                {
                    _hasUnsavedChanges = value;
                    HasUnsavedChangesChanged?.Invoke(this, EventArgs.Empty);
                }
            }
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

        public void ShowNewCodeFileElementTreeView(string name, ProgrammingLanguage language)
        {
            TreeViewModel.RootArtifact = new CodeFileElementArtifact(new CodeFileElement(name, language));
            
            _windowManagerService.ShowCodeElementsTreeView(TreeViewModel);

            ObserveCodeFileElementChanges((CodeFileElementArtifact)TreeViewModel.RootArtifact);
        }

        private void ObserveCodeFileElementChanges(CodeFileElementArtifact workspace)
        {
            // Clear previous subscriptions
            foreach (var artifact in _observedArtifacts)
            {
                artifact.PropertyChanged -= OnArtifactPropertyChanged;
                artifact.ChildAdded -= OnArtifactChildAdded;
                artifact.ChildRemoved -= OnArtifactRemoved;
            }
            _observedArtifacts.Clear();

            ObserveArtifactChanges(workspace);
        }
        private List<IArtifact> _observedArtifacts = new List<IArtifact>();
        private void ObserveArtifactChanges(IArtifact artifact)
        {
            _observedArtifacts.Add(artifact);
            artifact.PropertyChanged += OnArtifactPropertyChanged;

            artifact.ChildAdded += OnArtifactChildAdded;

            artifact.ChildRemoved += OnArtifactRemoved;

            foreach (var child in artifact.Children)
            {
                ObserveArtifactChanges(child);
            }
        }

        private void OnArtifactRemoved(object? sender, ChildRemovedEventArgs e)
        {
            var childArtifact = e.ChildArtifact;
            childArtifact.PropertyChanged -= OnArtifactPropertyChanged;
            _observedArtifacts.Remove(childArtifact);
            HasUnsavedChanges = true;
        }

        private void OnArtifactChildAdded(object? sender, ChildAddedEventArgs e)
        {
            ObserveArtifactChanges(e.ChildArtifact);
            HasUnsavedChanges = true;
        }

        private void OnArtifactPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //Logger.LogDebug("Artifact property changed: {Artifact} - {PropertyName}", (sender as IArtifact)?.TreeNodeText, e.PropertyName);
            HasUnsavedChanges = true;
        }

        public void ShowGenerationCodeOutputAsync(CodeFileElementArtifact artifact)
        {
            var messageBus = ServiceProviderHolder.GetRequiredService<ApplicationMessageBus>();
            messageBus.ShowArtifactPreview(new Application.ViewModels.ArtifactPreviewViewModel
            {
                FileName = "GeneratedCode.cs",
                TextContent = CodeElementGenerationService.GenerateCode(artifact.CodeElement),
                TextLanguageSchema = Application.ViewModels.ArtifactPreviewViewModel.KnownLanguages.Text
            });
        }

        public void ParseCodeFileAsync(CodeFileElementArtifact artifact)
        {
            var messageboxService = ServiceProviderHolder.GetRequiredService<IMessageBoxService>();
            var fileSystemDialogService = ServiceProviderHolder.GetRequiredService<IFileSystemDialogService>();
            var filter = ProgrammingLanguages.All.Select(pl => pl.Name + " Files|*" + pl.FileExtension).Aggregate((a, b) => a + "|" + b);
            filter += "|All Files|*.*";
            var filePath = fileSystemDialogService.OpenFile(filter);
            if (filePath == null) return;
            
            var programmingLanguage = ProgrammingLanguages.FindByFileExtension(System.IO.Path.GetExtension(filePath));
            if(programmingLanguage == null)
            {
                messageboxService.ShowError("Unsupported file type", "Error");
                return;
            }
            var code = System.IO.File.ReadAllText(filePath);
            var parser = ProgrammingLanguageCodeParsers.GetParser(programmingLanguage);
            if(parser == null)
            {
                messageboxService.ShowError("No parser available for the selected file type", "Error");
                return;
            }
            var fileTitle = System.IO.Path.GetFileNameWithoutExtension(filePath);
            var codeFileElement = parser.ParseCodeFile(code, fileTitle);
            TreeViewModel.RootArtifact = new CodeFileElementArtifact(codeFileElement);
        }
    }
}
