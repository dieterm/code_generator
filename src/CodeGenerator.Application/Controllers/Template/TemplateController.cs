using CodeGenerator.Application.Controllers.Base;
using CodeGenerator.Application.Controllers.Workspace;
using CodeGenerator.Application.Services;
using CodeGenerator.Application.ViewModels;
using CodeGenerator.Application.ViewModels.Template;
using CodeGenerator.Application.ViewModels.Workspace;
using CodeGenerator.Core.MessageBus;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared;
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

    public TemplateController(
        TemplateRibbonViewModel templateRibbonViewModel,
        TemplateTreeViewController templateTreeViewController,
        IWindowManagerService windowManagerService,
        RibbonBuilder ribbonBuilder,
        ApplicationMessageBus messageBus,
        IMessageBoxService messageBoxService,
        IFileSystemDialogService fileSystemDialogService,
        ILogger<TemplateController> logger)
        : base(windowManagerService, ribbonBuilder, messageBus, messageBoxService, fileSystemDialogService, logger)
    {
        _templateRibbonViewModel = templateRibbonViewModel;
        _templateTreeViewController = templateTreeViewController;
    }

    public override void Initialize()
    {
        _templateTreeViewController.Initialize();

        _templateRibbonViewModel.RequestShowTemplates += OnRequestShowTemplates;
        _templateRibbonViewModel.RequestRefreshTemplates+= OnRequestRefreshTemplates;
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
        _ribbonBuilder
            .AddTab("tabTemplates", "Templates")
                .AddToolStrip("toolstripTemplates", "Templates")
                    .AddButton("btnShowTemplates", "Browse Templates")
                        .WithSize(RibbonButtonSize.Large)
                        .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                        .WithImage("file_code")
                        .WithToolTip("Browse and execute templates")
                        .WithCommand(_templateRibbonViewModel.RequestShowTemplatesCommand)
                    .AddButton("btnRefreshTemplates", "Refresh")
                        .WithSize(RibbonButtonSize.Small)
                        .WithDisplayStyle(RibbonButtonDisplayStyle.ImageAndText)
                        .WithImage("refresh_cw")
                        .WithToolTip("Refresh template list")
                        .WithCommand(_templateRibbonViewModel.RequestRefreshTemplatesCommand)
                .EndToolStrip()
            .Build();
    }
    public override void Dispose()
    {
        _templateRibbonViewModel.RequestShowTemplates -= OnRequestShowTemplates;
        _templateRibbonViewModel.RequestRefreshTemplates -= OnRequestRefreshTemplates;
    }
}

