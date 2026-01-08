using CodeGenerator.Application.Events.DomainSchema;
using CodeGenerator.Application.MessageBus;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Core.DomainSchema.Schema;
using CodeGenerator.Core.DomainSchema.Services;
using CodeGenerator.Shared.Ribbon;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.Controllers
{
    public class DomainSchemaController : CoreControllerBase
    {
        private readonly DomainSchemaTreeViewModel _treeViewModel;
        private readonly DomainSchemaParser _schemaParser;
        private readonly IMessageBoxService _messageService;
        private string? _schemaPath;
        private DomainSchema? _domainSchema;
        public DomainSchema? DomainSchema { get { return _domainSchema; } }
        public DomainSchemaController(IWindowManagerService windowManagerService, RibbonBuilder ribbonBuilder, DomainSchemaTreeViewModel treeViewModel,  DomainSchemaParser schemaParser, IMessageBoxService messageService, ApplicationMessageBus messageBus, IFileSystemDialogService fileSystemDialogService, ILogger<DomainSchemaController> logger)
            : base(windowManagerService, ribbonBuilder, messageBus, messageService, fileSystemDialogService, logger)
        {
            _treeViewModel = treeViewModel;
            _schemaParser = schemaParser;
            _messageService = messageService;
        }
        public override void Initialize()
        {
            // No initialization needed currently
        }

        public async Task<bool> LoadDomainSchemaAsync(string filePath)
        {
            _logger.LogInformation("Loading domain schema from {FilePath}", filePath);
            try
            {
                var newDomainSchema = await _schemaParser.LoadSchemaAsync(filePath);
                
                // if we get here, schema was successfully loaded
                _domainSchema = newDomainSchema;
                _schemaPath = filePath;
                _treeViewModel.DomainSchema = _domainSchema;
                _messageBus.Publish(new DomainSchemaLoadedEvent(filePath, _domainSchema, _treeViewModel));
                _windowManagerService.ShowDomainSchemaTreeView(_treeViewModel);
                _logger.LogInformation("Domain schema loaded successfully from {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _messageService.ShowError("Error loading domain schema: " + ex.Message);
                _logger.LogError("Failed to load domain schema from {FilePath}: {ErrorMessage}", filePath, ex.Message);
                return false;
            }

            return true;
        }

        public async Task UnloadDomainSchema()
        {
            _logger.LogInformation("Unloading domain schema");
            _domainSchema = null;
            _schemaPath = null;
            _messageBus.Publish(new DomainSchemaUnloadedEvent());
            _logger.LogInformation("Domain schema unloaded successfully");
        }

        public override void Dispose()
        {
            UnloadDomainSchema().GetAwaiter().GetResult();
        }


    }
}
