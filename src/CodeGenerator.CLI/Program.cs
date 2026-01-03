using System.CommandLine;
using System.CommandLine.Invocation;
using CodeGenerator.Core.Events;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Services;
using CodeGenerator.Generators;
using CodeGenerator.Generators.Application;
using CodeGenerator.Generators.Domain;
using CodeGenerator.Generators.Infrastructure;
using CodeGenerator.Generators.Presentation;
using CodeGenerator.Generators.Shared;
using CodeGenerator.Templates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.CLI;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Code Generator CLI - Generate code from JSON Schema with DDD support");

        // Generate command
        var generateCommand = new Command("generate", "Generate code from a schema file");

        var schemaOption = new Option<FileInfo>(
            aliases: new[] { "--schema", "-s" },
            description: "Path to the JSON schema file")
        { IsRequired = true };

        var outputOption = new Option<DirectoryInfo>(
            aliases: new[] { "--output", "-o" },
            description: "Output directory for generated files")
        { IsRequired = true };

        var generatorOption = new Option<string?>(
            aliases: new[] { "--generator", "-g" },
            description: "Specific generator to run (e.g., Entity, DbContext, Repository). Leave empty for all.");

        var namespaceOption = new Option<string>(
            aliases: new[] { "--namespace", "-n" },
            description: "Root namespace for generated code",
            getDefaultValue: () => "Generated");

        var templateOption = new Option<DirectoryInfo?>(
            aliases: new[] { "--templates", "-t" },
            description: "Path to template folder");

        var settingsOption = new Option<FileInfo?>(
            aliases: new[] { "--settings" },
            description: "Path to settings JSON file");

        var frameworkOption = new Option<string>(
            aliases: new[] { "--framework", "-f" },
            description: "Target framework",
            getDefaultValue: () => "net8.0");

        var overwriteOption = new Option<bool>(
            aliases: new[] { "--overwrite" },
            description: "Overwrite existing files",
            getDefaultValue: () => false);

        var verboseOption = new Option<bool>(
            aliases: new[] { "--verbose", "-v" },
            description: "Enable verbose logging");

        generateCommand.AddOption(schemaOption);
        generateCommand.AddOption(outputOption);
        generateCommand.AddOption(generatorOption);
        generateCommand.AddOption(namespaceOption);
        generateCommand.AddOption(templateOption);
        generateCommand.AddOption(settingsOption);
        generateCommand.AddOption(frameworkOption);
        generateCommand.AddOption(overwriteOption);
        generateCommand.AddOption(verboseOption);

        generateCommand.SetHandler(async (context) =>
        {
            var schema = context.ParseResult.GetValueForOption(schemaOption)!;
            var output = context.ParseResult.GetValueForOption(outputOption)!;
            var generator = context.ParseResult.GetValueForOption(generatorOption);
            var ns = context.ParseResult.GetValueForOption(namespaceOption)!;
            var templates = context.ParseResult.GetValueForOption(templateOption);
            var settingsFile = context.ParseResult.GetValueForOption(settingsOption);
            var framework = context.ParseResult.GetValueForOption(frameworkOption)!;
            var overwrite = context.ParseResult.GetValueForOption(overwriteOption);
            var verbose = context.ParseResult.GetValueForOption(verboseOption);

            context.ExitCode = await ExecuteGenerateAsync(
                schema, output, generator, ns, templates, settingsFile,
                framework, overwrite, verbose, context.GetCancellationToken());
        });

        // List command
        var listCommand = new Command("list", "List available generators");
        listCommand.SetHandler(() =>
        {
            var services = ConfigureServices(LogLevel.Warning);
            var orchestrator = services.GetRequiredService<GeneratorOrchestrator>();

            Console.WriteLine("Available Generators:");
            Console.WriteLine(new string('-', 80));

            foreach (var gen in orchestrator.GetAvailableGenerators())
            {
                Console.WriteLine($"  {gen.Id,-20} {gen.Name,-30} [{gen.Layer}]");
                Console.WriteLine($"                       {gen.Description}");
                Console.WriteLine();
            }
        });

        // Validate command
        var validateCommand = new Command("validate", "Validate a schema file");
        var validateSchemaOption = new Option<FileInfo>(
            aliases: new[] { "--schema", "-s" },
            description: "Path to the JSON schema file")
        { IsRequired = true };

        validateCommand.AddOption(validateSchemaOption);
        validateCommand.SetHandler(async (FileInfo schema) =>
        {
            var services = ConfigureServices(LogLevel.Warning);
            var parser = services.GetRequiredService<ISchemaParser>();

            Console.WriteLine($"Validating schema: {schema.FullName}");

            var result = await parser.ValidateSchemaAsync(schema.FullName);

            if (result.IsValid)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Schema is valid!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Schema validation failed:");
            }

            foreach (var error in result.Errors)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ERROR: {error.Code} - {error.Message}");
                if (error.Path != null) Console.WriteLine($"         Path: {error.Path}");
            }

            foreach (var warning in result.Warnings)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  WARNING: {warning.Code} - {warning.Message}");
            }

            Console.ResetColor();
        }, validateSchemaOption);

        // Init command
        var initCommand = new Command("init", "Initialize a new schema file");
        var initOutputOption = new Option<FileInfo>(
            aliases: new[] { "--output", "-o" },
            description: "Output path for the schema file",
            getDefaultValue: () => new FileInfo("domain-schema.json"));

        initCommand.AddOption(initOutputOption);
        initCommand.SetHandler(async (FileInfo output) =>
        {
            var template = CreateSampleSchema();
            await File.WriteAllTextAsync(output.FullName, template);
            Console.WriteLine($"Created sample schema at: {output.FullName}");
        }, initOutputOption);

        rootCommand.AddCommand(generateCommand);
        rootCommand.AddCommand(listCommand);
        rootCommand.AddCommand(validateCommand);
        rootCommand.AddCommand(initCommand);

        return await rootCommand.InvokeAsync(args);
    }

    static async Task<int> ExecuteGenerateAsync(
        FileInfo schemaFile,
        DirectoryInfo outputDir,
        string? generatorId,
        string rootNamespace,
        DirectoryInfo? templatesDir,
        FileInfo? settingsFile,
        string framework,
        bool overwrite,
        bool verbose,
        CancellationToken cancellationToken)
    {
        var logLevel = verbose ? LogLevel.Debug : LogLevel.Information;
        var services = ConfigureServices(logLevel);

        var settingsService = services.GetRequiredService<ISettingsService>();
        var orchestrator = services.GetRequiredService<GeneratorOrchestrator>();

        try
        {
            // Load or create settings
            GeneratorSettings settings;
            if (settingsFile != null && settingsFile.Exists)
            {
                var loaded = await settingsService.LoadSettingsAsync(settingsFile.FullName, cancellationToken);
                settings = settingsService.Settings;
            }
            else
            {
                settings = settingsService.GetDefaultSettings();
            }

            // Override with command line options
            settings.SchemaFilePath = schemaFile.FullName;
            settings.OutputFolder = outputDir.FullName;
            settings.RootNamespace = rootNamespace;
            settings.TargetFramework = framework;
            settings.OverwriteExisting = overwrite;

            if (templatesDir != null)
            {
                settings.TemplateFolder = templatesDir.FullName;
            }

            Console.WriteLine($"Code Generator CLI");
            Console.WriteLine($"==================");
            Console.WriteLine($"Schema:    {settings.SchemaFilePath}");
            Console.WriteLine($"Output:    {settings.OutputFolder}");
            Console.WriteLine($"Namespace: {settings.RootNamespace}");
            Console.WriteLine($"Framework: {settings.TargetFramework}");
            Console.WriteLine();

            // Ensure output directory exists
            if (!outputDir.Exists)
            {
                outputDir.Create();
            }

            // Run generation
            var result = string.IsNullOrEmpty(generatorId)
                ? await orchestrator.GenerateAllAsync(settings, cancellationToken)
                : await orchestrator.GenerateAsync(generatorId, settings, cancellationToken);

            // Output results
            Console.WriteLine();
            Console.WriteLine($"Generation completed in {result.Duration.TotalSeconds:F2} seconds");
            Console.WriteLine($"Files generated: {result.Files.Count}");

            if (result.Warnings.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nWarnings ({result.Warnings.Count}):");
                foreach (var warning in result.Warnings)
                {
                    Console.WriteLine($"  - {warning}");
                }
            }

            if (result.Errors.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nErrors ({result.Errors.Count}):");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"  - {error}");
                }
            }

            Console.ResetColor();

            if (result.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nGeneration completed successfully!");
                Console.ResetColor();
                return 0;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nGeneration completed with errors.");
                Console.ResetColor();
                return 1;
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            if (verbose)
            {
                Console.WriteLine(ex.StackTrace);
            }
            Console.ResetColor();
            return 1;
        }
    }

    static IServiceProvider ConfigureServices(LogLevel logLevel)
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(logLevel);
        });

        // Core services
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<ISchemaParser, SchemaParser>();
        services.AddSingleton<ITemplateEngine, ScribanTemplateEngine>();
        services.AddSingleton<IProjectGenerator, DotNetProjectGenerator>();
        
        // Message Bus
        services.AddSingleton<IGeneratorMessageBus, GeneratorMessageBus>();

        // Generators
        services.AddSingleton<ICodeGenerator, EntityGenerator>();
        services.AddSingleton<ICodeGenerator, DbContextGenerator>();
        services.AddSingleton<ICodeGenerator, RepositoryGenerator>();
        services.AddSingleton<ICodeGenerator, DbScriptGenerator>();
        services.AddSingleton<ICodeGenerator, ControllerGenerator>();
        services.AddSingleton<ICodeGenerator, ViewModelGenerator>();
        services.AddSingleton<ICodeGenerator, WinFormsViewGenerator>();

        // Message Bus Aware Generators
        services.AddSingleton<IMessageBusAwareGenerator, SharedProjectGenerator>();
        services.AddSingleton<IMessageBusAwareGenerator, UserControlsProjectGenerator>();

        // Orchestrator
        services.AddSingleton<GeneratorOrchestrator>();

        return services.BuildServiceProvider();
    }

    static string CreateSampleSchema()
    {
        return """
{
  "$schema": "https://json-schema.org/draft/2020-12/schema",
  "$id": "https://example.com/domain-schema.json",
  "title": "Sample Domain Schema",
  "description": "A sample domain schema for code generation",
  "type": "object",
  "x-codegen": {
    "namespace": "MyCompany.MyProject",
    "targetLanguage": "CSharp",
    "dataLayerTechnology": "EntityFrameworkCore",
    "presentationTechnology": "WinForms",
    "useDependencyInjection": true,
    "useLogging": true,
    "useConfiguration": true,
    "projectSettings": {
      "solutionName": "MyProject",
      "domainProjectName": "MyProject.Domain",
      "applicationProjectName": "MyProject.Application",
      "infrastructureProjectName": "MyProject.Infrastructure",
      "presentationProjectName": "MyProject.Presentation"
    }
  },
  "x-db": {
    "databaseName": "MyDatabase",
    "schema": "dbo",
    "provider": "SqlServer",
    "connectionStringName": "DefaultConnection",
    "useMigrations": true
  },
  "$defs": {
    "Customer": {
      "type": "object",
      "title": "Customer",
      "description": "Represents a customer in the system",
      "properties": {
        "id": {
          "type": "integer",
          "description": "Unique identifier",
          "x-db": {
            "isPrimaryKey": true,
            "isIdentity": true
          }
        },
        "firstName": {
          "type": "string",
          "maxLength": 100,
          "description": "Customer's first name",
          "x-codegen": {
            "displaySettings": {
              "label": "First Name",
              "order": 1
            }
          }
        },
        "lastName": {
          "type": "string",
          "maxLength": 100,
          "description": "Customer's last name"
        },
        "email": {
          "type": "string",
          "format": "email",
          "maxLength": 255,
          "description": "Email address",
          "x-db": {
            "isUnique": true
          }
        },
        "createdAt": {
          "type": "string",
          "format": "date-time",
          "description": "When the customer was created",
          "x-codegen": {
            "isReadOnly": true
          }
        }
      },
      "required": ["firstName", "lastName", "email"],
      "x-codegen": {
        "generateRepository": true,
        "generateController": true,
        "generateViewModel": true,
        "generateView": true
      },
      "x-db": {
        "tableName": "Customers",
        "indexes": [
          {
            "name": "IX_Customers_Email",
            "columns": [{ "name": "email" }],
            "isUnique": true
          }
        ]
      }
    },
    "Order": {
      "type": "object",
      "title": "Order",
      "description": "Represents a customer order",
      "properties": {
        "id": {
          "type": "integer",
          "x-db": {
            "isPrimaryKey": true,
            "isIdentity": true
          }
        },
        "orderNumber": {
          "type": "string",
          "maxLength": 50,
          "x-db": {
            "isUnique": true
          }
        },
        "customerId": {
          "type": "integer",
          "x-db": {
            "isForeignKey": true,
            "foreignKeyReference": {
              "table": "Customers",
              "column": "Id",
              "onDelete": "Cascade"
            }
          }
        },
        "customer": {
          "$ref": "#/$defs/Customer"
        },
        "orderDate": {
          "type": "string",
          "format": "date-time"
        },
        "totalAmount": {
          "type": "number",
          "minimum": 0
        },
        "status": {
          "type": "string",
          "enum": ["Pending", "Processing", "Shipped", "Delivered", "Cancelled"]
        }
      },
      "required": ["orderNumber", "customerId", "orderDate"],
      "x-db": {
        "tableName": "Orders"
      }
    },
    "Address": {
      "type": "object",
      "title": "Address",
      "description": "An address value object",
      "properties": {
        "street": {
          "type": "string",
          "maxLength": 200
        },
        "city": {
          "type": "string",
          "maxLength": 100
        },
        "state": {
          "type": "string",
          "maxLength": 50
        },
        "postalCode": {
          "type": "string",
          "maxLength": 20
        },
        "country": {
          "type": "string",
          "maxLength": 100
        }
      },
      "x-codegen": {
        "isOwnedType": true,
        "generateRepository": false,
        "generateController": false,
        "generateViewModel": false,
        "generateView": false
      }
    }
  }
}
""";
    }
}
