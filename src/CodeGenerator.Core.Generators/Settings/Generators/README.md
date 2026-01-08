# Generator Settings System

Dit systeem maakt het mogelijk om generator-specifieke instellingen op te slaan en te laden.

## Architectuur

```
CodeGenerator.Core.Settings
??? Generators/
?   ??? TemplateType.cs              # Enum voor template types
?   ??? TemplateRequirementSettings.cs # Template settings (JSON serializable)
?   ??? GeneratorSettings.cs         # Settings voor één generator
?   ??? AllGeneratorSettings.cs      # Container voor alle generator settings
?   ??? IGeneratorSettingsProvider.cs # Interface voor generators
?   ??? GeneratorSettingsManager.cs  # Manager voor laden/opslaan
```

## Gebruik

### 1. Implementeer IGeneratorSettingsProvider in je Generator

Je generator moet `IGeneratorSettingsProvider` implementeren. De makkelijkste manier is via `GeneratorSettingsDescription`:

```csharp
public class MyEntityGenerator : IMessageBusAwareGenerator
{
    public GeneratorSettingsDescription SettingsDescription { get; }

    public MyEntityGenerator()
    {
        SettingsDescription = new GeneratorSettingsDescription(
            id: "MyEntityGenerator",
            name: "Entity Generator",
            description: "Generates domain entity classes",
            templates: new[]
            {
                new TemplateRequirement(
                    templateId: "EntityTemplate",
                    description: "Template for generating entity classes",
                    outputFileNamePattern: "{{EntityName}}.cs",
                    templateType: TemplateType.Scriban),
                new TemplateRequirement(
                    templateId: "EntityInterfaceTemplate",
                    description: "Template for generating entity interfaces",
                    outputFileNamePattern: "I{{EntityName}}.cs",
                    templateType: TemplateType.Scriban)
            },
            parameters: null, // of een ApplicationSettingsBase subclass
            category: "Domain",
            iconKey: "entity_icon"
        );
    }
    
    // ... IMessageBusAwareGenerator implementatie
}
```

### 2. Registreer generators in DI

```csharp
// In je DI configuratie
services.AddSingleton<IGeneratorSettingsProvider, MyEntityGenerator>();
services.AddSingleton<GeneratorSettingsManager>();
```

### 3. Initialiseer bij applicatie startup

```csharp
var generatorSettingsManager = ServiceProviderHolder.GetRequiredService<GeneratorSettingsManager>();

// Laad bestaande settings
generatorSettingsManager.LoadSettings();

// Ontdek en registreer alle generators
generatorSettingsManager.DiscoverAndRegisterGenerators();

// Sla op (optioneel, om nieuwe generators te persisteren)
generatorSettingsManager.SaveSettings();
```

### 4. Gebruik settings in je generator

```csharp
public void Generate()
{
    var settingsManager = ServiceProviderHolder.GetRequiredService<GeneratorSettingsManager>();
    var settings = settingsManager.GetGeneratorSettings(SettingsDescription.Id);
    
    // Check of generator enabled is
    if (!settings.Enabled)
        return;
    
    // Haal templates op
    foreach (var template in settings.Templates.Where(t => t.Enabled))
    {
        var templatePath = template.TemplateFilePath;
        var outputPattern = template.OutputFileNamePattern;
        // ... genereer code
    }
    
    // Haal parameters op
    var outputFolder = settings.GetParameter<string>("OutputFolder", "./output");
    var generateInterfaces = settings.GetParameter<bool>("GenerateInterfaces", false);
}
```

## Settings Bestand

Settings worden opgeslagen als JSON in:
```
%LOCALAPPDATA%\CodeGenerator\generator-settings.json
```

Voorbeeld JSON:
```json
{
  "version": "1.0",
  "lastModified": "2024-01-15T10:30:00Z",
  "generators": {
    "MyEntityGenerator": {
      "generatorId": "MyEntityGenerator",
      "name": "Entity Generator",
      "description": "Generates domain entity classes",
      "category": "Domain",
      "enabled": true,
      "templates": [
        {
          "templateId": "EntityTemplate",
          "description": "Template for generating entity classes",
          "templateType": "Scriban",
          "outputFileNamePattern": "{{EntityName}}.cs",
          "templateFilePath": "C:\\Templates\\Entity.scriban",
          "enabled": true
        }
      ],
      "parameters": {
        "OutputFolder": "C:\\Output",
        "GenerateInterfaces": true
      }
    }
  }
}
```

## API Reference

### GeneratorSettingsManager

```csharp
// Laden en opslaan
void LoadSettings()
void SaveSettings()

// Generator ontdekking
void DiscoverAndRegisterGenerators()
void RegisterGenerator(IGeneratorSettingsProvider provider)

// Generator settings
GeneratorSettings GetGeneratorSettings(string generatorId)
void UpdateGeneratorSettings(GeneratorSettings settings)
IEnumerable<string> GetRegisteredGeneratorIds()
bool IsGeneratorEnabled(string generatorId)
void SetGeneratorEnabled(string generatorId, bool enabled)

// Template settings
List<TemplateRequirementSettings> GetTemplates(string generatorId)
void UpdateTemplate(string generatorId, TemplateRequirementSettings template)
void SetTemplateFilePath(string generatorId, string templateId, string filePath)
void SetTemplateEnabled(string generatorId, string templateId, bool enabled)

// Parameters
T? GetParameter<T>(string generatorId, string parameterName, T? defaultValue)
void SetParameter<T>(string generatorId, string parameterName, T? value)
```

### GeneratorSettings

```csharp
string GeneratorId { get; set; }
string? Name { get; set; }
string? Description { get; set; }
string? Category { get; set; }
bool Enabled { get; set; }
List<TemplateRequirementSettings> Templates { get; set; }
Dictionary<string, object?> Parameters { get; set; }

T? GetParameter<T>(string key, T? defaultValue)
void SetParameter<T>(string key, T? value)
TemplateRequirementSettings? GetTemplate(string templateId)
void SetTemplate(TemplateRequirementSettings template)
```

### TemplateRequirementSettings

```csharp
string TemplateId { get; set; }
string Description { get; set; }
TemplateType TemplateType { get; set; }
string OutputFileNamePattern { get; set; }
string? TemplateFilePath { get; set; }
bool Enabled { get; set; }
```

## Integratie met SettingsManager

De `GeneratorSettingsManager` kan ook via de algemene `SettingsManager` worden gebruikt:

```csharp
var settingsManager = new SettingsManager();

// Initialiseer generator settings
settingsManager.InitializeGeneratorSettings();

// Toegang tot generator settings
var genSettings = settingsManager.GeneratorSettings;
var myGenerator = genSettings.GetGeneratorSettings("MyEntityGenerator");

// Sla alles op (app settings + generator settings)
settingsManager.SaveAllSettings();
```
