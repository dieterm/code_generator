using System.Text.Json;
using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Interfaces;
using CodeGenerator.Core.Models.Configuration;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.Services;

/// <summary>
/// Service for managing generator settings
/// </summary>
public class SettingsService : ISettingsService
{
    private readonly ILogger<SettingsService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    public GeneratorSettings Settings { get; private set; }
    public SettingsService(ILogger<SettingsService> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<bool> LoadSettingsAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(path))
            {
                _logger.LogWarning("Settings file not found at {Path}, using defaults", path);
                Settings = GetDefaultSettings();
                return false;
            }

            var json = await File.ReadAllTextAsync(path, cancellationToken);
            var settings = JsonSerializer.Deserialize<GeneratorSettings>(json, _jsonOptions);
            Settings = settings ?? GetDefaultSettings();
            return settings!=null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load settings from {Path}", path);
            throw;
        }
    }

    public async Task SaveSettingsAsync(GeneratorSettings settings, string path, CancellationToken cancellationToken = default)
    {
        try
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(settings, _jsonOptions);
            await File.WriteAllTextAsync(path, json, cancellationToken);
            Settings = settings;
            _logger.LogInformation("Saved settings to {Path}", path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save settings to {Path}", path);
            throw;
        }
    }

    public GeneratorSettings GetDefaultSettings()
    {
        var settings = new GeneratorSettings
        {
            TargetFramework = "net8.0",
            OverwriteExisting = false,
            CreateBackup = true
        };

        // Default C# language settings
        settings.LanguageSettings["CSharp"] = new LanguageSettings
        {
            Language = TargetLanguage.CSharp,
            FileExtension = ".cs",
            TypeMappings = new Dictionary<string, string>
            {
                ["string"] = "string",
                ["integer"] = "int",
                ["number"] = "decimal",
                ["boolean"] = "bool",
                ["array"] = "List<T>",
                ["object"] = "object"
            },
            NamingConventions = new NamingConventions
            {
                ClassNaming = "PascalCase",
                PropertyNaming = "PascalCase",
                FieldNaming = "camelCase",
                MethodNaming = "PascalCase",
                InterfacePrefix = "I",
                PrivateFieldPrefix = "_"
            }
        };

        // Default SQL Server settings
        settings.LanguageSettings["SQL_SqlServer"] = new LanguageSettings
        {
            Language = TargetLanguage.SQL_SqlServer,
            FileExtension = ".sql",
            TypeMappings = new Dictionary<string, string>
            {
                ["string"] = "NVARCHAR",
                ["integer"] = "INT",
                ["long"] = "BIGINT",
                ["decimal"] = "DECIMAL(18,2)",
                ["boolean"] = "BIT",
                ["datetime"] = "DATETIME2",
                ["guid"] = "UNIQUEIDENTIFIER"
            }
        };

        // Default generators
        AddDefaultGenerators(settings);

        // Default NuGet packages
        settings.NuGetPackages = new List<NuGetPackageReference>
        {
            new() { PackageId = NuGetPackages.Microsoft_Extensions_DependencyInjection.PackageId, Version = NuGetPackages.Microsoft_Extensions_DependencyInjection.Version, Layers = new[] { ArchitectureLayer.Infrastructure, ArchitectureLayer.Application } },
            new() { PackageId = NuGetPackages.Microsoft_Extensions_Logging.PackageId, Version = NuGetPackages.Microsoft_Extensions_Logging.Version, Layers = new[] { ArchitectureLayer.Infrastructure, ArchitectureLayer.Application } },
            new() { PackageId = NuGetPackages.Microsoft_Extensions_Configuration.PackageId, Version = NuGetPackages.Microsoft_Extensions_Configuration.Version, Layers = new[] { ArchitectureLayer.Infrastructure } },
            new() { PackageId = NuGetPackages.Microsoft_EntityFrameworkCore.PackageId, Version = NuGetPackages.Microsoft_EntityFrameworkCore.Version, Layers = new[] { ArchitectureLayer.Infrastructure } },
            new() { PackageId = NuGetPackages.Microsoft_EntityFrameworkCore_Relational.PackageId, Version = NuGetPackages.Microsoft_EntityFrameworkCore_Relational.Version, Layers = new[] { ArchitectureLayer.Infrastructure } }
        };

        return settings;
    }

    private void AddDefaultGenerators(GeneratorSettings settings)
    {
        // Entity Generator
        settings.Generators["Entity"] = new GeneratorConfiguration
        {
            Id = "Entity",
            Name = "Entity Generator",
            Description = "Generates domain entity classes",
            Enabled = true,
            Type = GeneratorType.Entity,
            Layer = ArchitectureLayer.Domain,
            Language = TargetLanguage.CSharp,
            OutputPathPattern = "{Layer}/Entities",
            FileExtension = ".cs",
            Templates = new List<TemplateReference>
            {
                new() { Id = "entity", FileName = "CSharp/Entity.scriban", OutputFileName = "{Entity}.cs", PerEntity = true }
            }
        };

        // DbContext Generator
        settings.Generators["DbContext"] = new GeneratorConfiguration
        {
            Id = "DbContext",
            Name = "DbContext Generator",
            Description = "Generates Entity Framework Core DbContext",
            Enabled = true,
            Type = GeneratorType.DbContext,
            Layer = ArchitectureLayer.Infrastructure,
            Language = TargetLanguage.CSharp,
            OutputPathPattern = "{Layer}/Data",
            FileExtension = ".cs",
            Templates = new List<TemplateReference>
            {
                new() { Id = "dbcontext", FileName = "CSharp/DbContext.scriban", OutputFileName = "ApplicationDbContext.cs", PerEntity = false }
            }
        };

        // Repository Generator
        settings.Generators["Repository"] = new GeneratorConfiguration
        {
            Id = "Repository",
            Name = "Repository Generator",
            Description = "Generates repository interfaces and implementations",
            Enabled = true,
            Type = GeneratorType.Repository,
            Layer = ArchitectureLayer.Infrastructure,
            Language = TargetLanguage.CSharp,
            OutputPathPattern = "{Layer}/Repositories",
            FileExtension = ".cs",
            Templates = new List<TemplateReference>
            {
                new() { Id = "repository-interface", FileName = "CSharp/IRepository.scriban", OutputFileName = "I{Entity}Repository.cs", PerEntity = true },
                new() { Id = "repository-impl", FileName = "CSharp/Repository.scriban", OutputFileName = "{Entity}Repository.cs", PerEntity = true }
            },
            DependsOn = new List<string> { "Entity", "DbContext" }
        };

        // Controller Generator
        settings.Generators["Controller"] = new GeneratorConfiguration
        {
            Id = "Controller",
            Name = "Controller Generator",
            Description = "Generates controllers for the application layer",
            Enabled = true,
            Type = GeneratorType.Controller,
            Layer = ArchitectureLayer.Application,
            Language = TargetLanguage.CSharp,
            OutputPathPattern = "{Layer}/Controllers",
            FileExtension = ".cs",
            Templates = new List<TemplateReference>
            {
                new() { Id = "controller", FileName = "CSharp/Controller.scriban", OutputFileName = "{Entity}Controller.cs", PerEntity = true }
            },
            DependsOn = new List<string> { "Repository" }
        };

        // ViewModel Generator
        settings.Generators["ViewModel"] = new GeneratorConfiguration
        {
            Id = "ViewModel",
            Name = "ViewModel Generator",
            Description = "Generates ViewModels for the application layer",
            Enabled = true,
            Type = GeneratorType.ViewModel,
            Layer = ArchitectureLayer.Application,
            Language = TargetLanguage.CSharp,
            OutputPathPattern = "{Layer}/ViewModels",
            FileExtension = ".cs",
            Templates = new List<TemplateReference>
            {
                new() { Id = "viewmodel", FileName = "CSharp/ViewModel.scriban", OutputFileName = "{Entity}ViewModel.cs", PerEntity = true }
            },
            DependsOn = new List<string> { "Entity" }
        };

        // View Generator (WinForms)
        settings.Generators["View_WinForms"] = new GeneratorConfiguration
        {
            Id = "View_WinForms",
            Name = "WinForms View Generator",
            Description = "Generates WinForms UserControl views",
            Enabled = true,
            Type = GeneratorType.View,
            Layer = ArchitectureLayer.Presentation,
            Language = TargetLanguage.CSharp,
            OutputPathPattern = "{Layer}/Views",
            FileExtension = ".cs",
            Templates = new List<TemplateReference>
            {
                new() { Id = "view-winforms", FileName = "CSharp/WinForms/View.scriban", OutputFileName = "{Entity}View.cs", PerEntity = true },
                new() { Id = "view-winforms-designer", FileName = "CSharp/WinForms/View.Designer.scriban", OutputFileName = "{Entity}View.Designer.cs", PerEntity = true }
            },
            DependsOn = new List<string> { "ViewModel" }
        };

        // Database Script Generator
        settings.Generators["DbScript"] = new GeneratorConfiguration
        {
            Id = "DbScript",
            Name = "Database Script Generator",
            Description = "Generates SQL database scripts",
            Enabled = true,
            Type = GeneratorType.DbScript,
            Layer = ArchitectureLayer.Infrastructure,
            Language = TargetLanguage.SQL_SqlServer,
            OutputPathPattern = "{Layer}/Scripts",
            FileExtension = ".sql",
            Templates = new List<TemplateReference>
            {
                new() { Id = "create-table", FileName = "SQL/SqlServer/CreateTable.scriban", OutputFileName = "Create_{Entity}.sql", PerEntity = true }
            }
        };

        // Dependency Injection Generator
        settings.Generators["DependencyInjection"] = new GeneratorConfiguration
        {
            Id = "DependencyInjection",
            Name = "DI Registration Generator",
            Description = "Generates dependency injection registration",
            Enabled = true,
            Type = GeneratorType.DependencyInjection,
            Layer = ArchitectureLayer.Infrastructure,
            Language = TargetLanguage.CSharp,
            OutputPathPattern = "{Layer}/Extensions",
            FileExtension = ".cs",
            Templates = new List<TemplateReference>
            {
                new() { Id = "di-registration", FileName = "CSharp/ServiceCollectionExtensions.scriban", OutputFileName = "ServiceCollectionExtensions.cs", PerEntity = false }
            },
            DependsOn = new List<string> { "Repository" }
        };
    }
}
