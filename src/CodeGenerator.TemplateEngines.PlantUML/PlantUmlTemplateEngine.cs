using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Templates;
using CodeGenerator.Shared;
using Microsoft.Extensions.Logging;
using PlantUml.Net;

namespace CodeGenerator.TemplateEngines.PlantUML;

/// <summary>
/// Template engine for rendering PlantUML diagrams to images
/// </summary>
public class PlantUmlTemplateEngine : FileBasedTemplateEngine<PlantUmlTemplate, PlantUmlTemplateInstance>
{
    public const string TEMPLATE_ENGINE_ID = "plantuml_template_engine";
    public const string IMAGE_CONTENT_DECORATOR_KEY = "ImageContent";

    private readonly RendererFactory _rendererFactory;

    public override string DefaultFileExtension => "puml";

    public PlantUmlTemplateEngine(ILogger<PlantUmlTemplateEngine> logger)
        : base(logger, TEMPLATE_ENGINE_ID, "PlantUML Template Engine", TemplateType.ImageFile, new[] { "puml", "plantuml" })
    {
        _rendererFactory = new RendererFactory();
    }

    public override ITemplate CreateTemplateFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Template file not found: {filePath}");

        var content = File.ReadAllText(filePath);
        return new PlantUmlTemplate(filePath, content);
    }

    public override ITemplateInstance CreateTemplateInstance(ITemplate template)
    {
        if (template is not PlantUmlTemplate plantUmlTemplate)
            throw new InvalidOperationException($"Template must be of type {nameof(PlantUmlTemplate)}");

        return new PlantUmlTemplateInstance(plantUmlTemplate);
    }

    public override async Task<TemplateOutput> RenderAsync(PlantUmlTemplateInstance templateInstance, CancellationToken cancellationToken)
    {
        try
        {
            var template = (PlantUmlTemplate)templateInstance.Template;
            var diagramDefinition = template.DiagramDefinition;

            if (string.IsNullOrWhiteSpace(diagramDefinition))
            {
                return new TemplateOutput("Diagram definition is empty.");
            }

            // Preprocess includes to resolve TemplateId syntax
            diagramDefinition = PreprocessDiagramIncludes(diagramDefinition);

            // Render the PlantUML diagram to image bytes
            var renderer = _rendererFactory.CreateRenderer(new PlantUmlSettings());
            var bytes = await renderer.RenderAsync(diagramDefinition, templateInstance.OutputFormat);

            if (bytes == null || bytes.Length == 0)
            {
                return new TemplateOutput("Failed to render PlantUML diagram - no output generated.");
            }

            // Determine output filename
            var outputFileName = templateInstance.OutputFileName ?? GetDefaultFileName(templateInstance.OutputFormat);

            // Create FileArtifact with ImageContentDecorator
            var fileArtifact = new FileArtifact(outputFileName);
            var imageDecorator = new ImageContentDecorator(IMAGE_CONTENT_DECORATOR_KEY)
            {
                Content = bytes
            };
            fileArtifact.AddDecorator(imageDecorator);

            Logger.LogDebug("Rendered PlantUML diagram to {FileName} ({Size} bytes)", outputFileName, bytes.Length);

            return new TemplateOutput(fileArtifact);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error rendering PlantUML diagram");
            return new TemplateOutput(ex.Message);
        }
    }

    /// <summary>
    /// Preprocess diagram definition to resolve TemplateId includes
    /// </summary>
    private string PreprocessDiagramIncludes(string diagramDefinition)
    {
        if (!PlantUmlIncludePreprocessor.HasTemplateIdIncludes(diagramDefinition))
        {
            return diagramDefinition;
        }

        try
        {
            var templateManager = ServiceProviderHolder.GetRequiredService<TemplateManager>();
            var pathResolver = templateManager.PathResolver;
            
            var processed = PlantUmlIncludePreprocessor.ProcessIncludes(diagramDefinition, pathResolver);
            
            Logger.LogDebug("Preprocessed PlantUML includes");
            return processed;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to preprocess PlantUML includes, continuing with original definition");
            return diagramDefinition;
        }
    }

    /// <summary>
    /// Render a PlantUML diagram directly from definition text
    /// </summary>
    public async Task<byte[]> RenderDiagramAsync(string diagramDefinition, OutputFormat format = OutputFormat.Png, CancellationToken cancellationToken = default)
    {
        // Preprocess includes
        diagramDefinition = PreprocessDiagramIncludes(diagramDefinition);
        
        var renderer = _rendererFactory.CreateRenderer(new PlantUmlSettings());
        return await renderer.RenderAsync(diagramDefinition, format);
    }

    private static string GetDefaultFileName(OutputFormat format)
    {
        return format switch
        {
            OutputFormat.Png => "diagram.png",
            OutputFormat.Svg => "diagram.svg",
            OutputFormat.Eps => "diagram.eps",
            OutputFormat.Pdf => "diagram.pdf",
            OutputFormat.Vdx => "diagram.vdx",
            OutputFormat.Xmi => "diagram.xmi",
            OutputFormat.Scxml => "diagram.scxml",
            OutputFormat.Html => "diagram.html",
            OutputFormat.Ascii => "diagram.txt",
            OutputFormat.Ascii_Unicode => "diagram.txt",
            OutputFormat.LaTeX => "diagram.tex",
            _ => "diagram.png"
        };
    }
}
