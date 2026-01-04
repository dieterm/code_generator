using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Events;
using CodeGenerator.Core.Generators;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Generators.Presentation;

/// <summary>
/// Generates WinForms UserControl views
/// </summary>
public class WinFormsViewGenerator : MessageBusAwareGeneratorBase
{
    public override string Id => "View_WinForms";
    public override string Name => "WinForms View Generator";
    public override string Description => "Generates WinForms UserControl views";

    public WinFormsViewGenerator(ILogger<WinFormsViewGenerator> logger)
        : base(logger)
    {
    }

    public async Task<GenerationResult> GenerateForEntityAsync(
        EntityModel entity,
        DomainContext context,
        GeneratorSettings settings,
        CancellationToken cancellationToken = default)
    {
        var result = new GenerationResult();
        var config = GetConfiguration(settings);

        if (config == null || !config.Enabled)
        {
            result.Success = true;
            return result;
        }

        // Skip owned types
        if (entity.IsOwnedType)
        {
            result.Success = true;
            return result;
        }

        // Check if View generation is enabled for this entity
        if (entity.CodeGenSettings?.GenerateView == false)
        {
            result.Success = true;
            return result;
        }

        try
        {
            foreach (var template in config.Templates.Where(t => t.PerEntity))
            {
                var file = await RenderAndCreateFile(template, entity, context, settings, cancellationToken);
                result.Files.Add(file);

                if (settings.OverwriteExisting || file.IsNew)
                {
                    await FileService.WriteFileAsync(file.AbsolutePath, file.Content, settings.CreateBackup, cancellationToken);
                    file.Written = true;
                    Logger.LogInformation("Generated WinForms view for {EntityName} at {Path}", entity.Name, file.AbsolutePath);
                }
            }

            result.Success = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to generate WinForms view for {EntityName}", entity.Name);
            result.Errors.Add($"Failed to generate WinForms view for {entity.Name}: {ex.Message}");
            result.Success = false;
        }

        return result;
    }

    protected object CreateTemplateModel(EntityModel entity, DomainContext context, GeneratorSettings settings)
    {
        var primaryKey = entity.PrimaryKeyProperties.FirstOrDefault();

        return new
        {
            Entity = entity,
            Context = context,
            Settings = settings,
            Namespace = $"{settings.RootNamespace}.Presentation.Views",
            ViewModelNamespace = $"{settings.RootNamespace}.Application.ViewModels",
            ControllerNamespace = $"{settings.RootNamespace}.Application.Controllers",
            ViewName = $"{entity.Name}View",
            ViewModelName = $"{entity.Name}ViewModel",
            ControllerName = $"{entity.Name}Controller",
            Controls = GenerateControlDefinitions(entity),
            Generator = new { Id, Name, GeneratedAt = DateTime.UtcNow }
        };
    }

    private List<object> GenerateControlDefinitions(EntityModel entity)
    {
        var controls = new List<object>();
        int yPosition = 20;
        int labelWidth = 120;
        int controlWidth = 200;
        int rowHeight = 30;
        int xLabel = 20;
        int xControl = labelWidth + 30;

        foreach (var property in entity.Properties.Where(p => !p.IsComputed && !p.IsPrimaryKey))
        {
            var display = property.DisplaySettings;
            if (display?.IsVisible == false) continue;

            var controlType = DetermineWinFormsControl(property);
            var controlName = $"{controlType.Replace("Syncfusion.", "").Replace("Windows.Forms.", "")}{property.Name}";

            controls.Add(new
            {
                PropertyName = property.Name,
                LabelName = $"lbl{property.Name}",
                LabelText = display?.Label ?? property.Name,
                ControlName = controlName,
                ControlType = controlType,
                X = xControl,
                Y = yPosition,
                LabelX = xLabel,
                LabelY = yPosition + 3,
                Width = display?.Width ?? controlWidth,
                Height = rowHeight - 5,
                LabelWidth = labelWidth,
                IsReadOnly = display?.IsEditable == false || property.IsReadOnly,
                ToolTip = display?.Tooltip ?? property.Description,
                property.IsRequired,
                property.MaxLength,
                Format = display?.Format,
                DataType = property.DataType.ToString()
            });

            yPosition += rowHeight + 5;
        }

        return controls;
    }

    private string DetermineWinFormsControl(PropertyModel property)
    {
        var displayControlType = property.DisplaySettings?.ControlType;

        if (!string.IsNullOrEmpty(displayControlType))
        {
            return displayControlType switch
            {
                "TextBox" => "Syncfusion.Windows.Forms.Tools.TextBoxExt",
                "TextArea" => "Syncfusion.Windows.Forms.Tools.TextBoxExt",
                "NumericUpDown" => "Syncfusion.Windows.Forms.Tools.NumericUpDownExt",
                "ComboBox" => "Syncfusion.Windows.Forms.Tools.ComboBoxAdv",
                "CheckBox" => "Syncfusion.Windows.Forms.Tools.CheckBoxAdv",
                "DatePicker" => "Syncfusion.Windows.Forms.Tools.DateTimePickerAdv",
                "DateTimePicker" => "Syncfusion.Windows.Forms.Tools.DateTimePickerAdv",
                "TimePicker" => "Syncfusion.Windows.Forms.Tools.DateTimePickerAdv",
                _ => "Syncfusion.Windows.Forms.Tools.TextBoxExt"
            };
        }

        return property.DataType switch
        {
            PropertyDataType.String => "Syncfusion.Windows.Forms.Tools.TextBoxExt",
            PropertyDataType.Integer or PropertyDataType.Long => "Syncfusion.Windows.Forms.Tools.NumericUpDownExt",
            PropertyDataType.Decimal or PropertyDataType.Double or PropertyDataType.Float => "Syncfusion.Windows.Forms.Tools.NumericUpDownExt",
            PropertyDataType.Boolean => "Syncfusion.Windows.Forms.Tools.CheckBoxAdv",
            PropertyDataType.DateTime or PropertyDataType.DateOnly or PropertyDataType.TimeOnly => "Syncfusion.Windows.Forms.Tools.DateTimePickerAdv",
            PropertyDataType.Enum => "Syncfusion.Windows.Forms.Tools.ComboBoxAdv",
            PropertyDataType.Guid => "Syncfusion.Windows.Forms.Tools.TextBoxExt",
            _ => "Syncfusion.Windows.Forms.Tools.TextBoxExt"
        };
    }

    public override void SubscribeToEvents(IGeneratorMessageBus messageBus)
    {
        base.SubscribeToEvents(messageBus);

        messageBus.Subscribe<CreatingProjectEventArgs>(OnCreatingProject);
    }

    private async void OnCreatingProject(CreatingProjectEventArgs args)
    {
        if (!IsProject<PresentationProjectGenerator>(args.Schema, args.Project)) return;
        // only continue if it's the PresentationLayer project


    }
}
