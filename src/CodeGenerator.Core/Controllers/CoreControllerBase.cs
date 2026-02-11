using CodeGenerator.Core.MessageBus;
using CodeGenerator.Application.Services;
using CodeGenerator.Shared.Controllers;
using CodeGenerator.Shared.Ribbon;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerator.Shared.Operations;

namespace CodeGenerator.Application.Controllers.Base
{
    public abstract class CoreControllerBase : ControllerBase
    {
        protected readonly IMessageBoxService _messageBoxService;
        protected readonly IFileSystemDialogService _fileSystemDialogService;
        protected readonly ILogger _logger;
        protected readonly ApplicationMessageBus _messageBus;
        protected readonly RibbonBuilder _ribbonBuilder;
        protected readonly OperationExecutor _operationExecutor;

        public CoreControllerBase(OperationExecutor operationExecutor, RibbonBuilder ribbonBuilder, ApplicationMessageBus messageBus, IMessageBoxService messageboxService, IFileSystemDialogService fileSystemDialogService, ILogger logger)
        {
            _ribbonBuilder = ribbonBuilder;
            _messageBus = messageBus;
            _messageBoxService = messageboxService;
            _fileSystemDialogService = fileSystemDialogService;
            _logger = logger;
            _operationExecutor = operationExecutor;
        }


    }
}
