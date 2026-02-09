using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared;
using CodeGenerator.Shared.Operations;
using CodeGenerator.Shared.Ribbon;
using Microsoft.Extensions.Logging;


namespace CodeGenerator.Application.Controllers.Template;

/// <summary>
/// Controller for managing templates and the template tree view
/// </summary>
public class TemplateController : CoreControllerBase
{
    private readonly TemplateRibbonViewModel _templateRibbonViewModel;
    private readonly TemplateTreeViewController _templateTreeViewController;

    public TemplateController(OperationExecutor operationExecutor,
        TemplateRibbonViewModel templateRibbonViewModel,
        TemplateTreeViewController templateTreeViewController,
        IWindowManagerService windowManagerService,
        RibbonBuilder ribbonBuilder,
        ApplicationMessageBus messageBus,
        IMessageBoxService messageBoxService,
        IFileSystemDialogService fileSystemDialogService,
        ILogger<TemplateController> logger)
        : base(operationExecutor, windowManagerService, ribbonBuilder, messageBus, messageBoxService, fileSystemDialogService, logger)
    {
        _templateRibbonViewModel = templateRibbonViewModel;
        _templateTreeViewController = templateTreeViewController;
    }

    public override void Initialize()
    {
        _templateTreeViewController.Initialize();

        _templateRibbonViewModel.RequestShowTemplates += OnRequestShowTemplates;
        _templateRibbonViewModel.RequestRefreshTemplates+= OnRequestRefreshTemplates;
        _templateRibbonViewModel.RequestShowCodeElementsTool += OnRequestShowCodeElementsTool;
    }

    private void OnRequestShowCodeElementsTool(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void OnRequestRefreshTemplates(object? sender, EventArgs e)
    {
        _templateTreeViewController.RefreshTemplates();
    }

    private void OnRequestShowTemplates(object? sender, EventArgs e)
    {
        _templateTreeViewController.ShowTemplateTreeView(TargetTemplateFolder.Default);
    }

    public void CreateRibbon()
    {
        var templateTabBuilder = _ribbonBuilder
            .AddTab("tabTemplates", "Templates");

        templateTabBuilder
            .AddToolStrip("toolstripTemplates", "Templates")
                .AddButton("btnShowTemplates", "Browse Templates")
                    .WithSize(RibbonButtonSize.Large)
                    .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                    .WithImage("file_code")
                    .WithToolTip("Browse and execute templates")
                    .WithCommand(_templateRibbonViewModel.ShowTemplatesCommand)
                .AddButton("btnRefreshTemplates", "Refresh")
                    .WithSize(RibbonButtonSize.Small)
                    .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                    .WithImage("refresh_cw")
                    .WithToolTip("Refresh template list")
                    .WithCommand(_templateRibbonViewModel.RefreshTemplatesCommand)
            .EndToolStrip()
        .Build();
        templateTabBuilder
            .AddToolStrip("toolstripTemplateTools", "Tools")
                .AddDropDownButton("btnTemplateTools", "Tools")
                    .WithSize(RibbonButtonSize.Large)
                    .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                    .WithImage("pocket-knife")
                    .WithToolTip("Template related tools")
                    .AddDropDownItem("btnCodeElements", "Code Elements", (e) => _templateRibbonViewModel.ShowCodeElementsToolCommand.Execute(null))
                        
            .EndToolStrip()
        .Build();
    }
    public override void Dispose()
    {
        _templateRibbonViewModel.RequestShowTemplates -= OnRequestShowTemplates;
        _templateRibbonViewModel.RequestRefreshTemplates -= OnRequestRefreshTemplates;
    }
}

