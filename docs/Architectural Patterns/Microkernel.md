# Microkernel Architecture (Plug-in Architecture)

## Overzicht

De **Microkernel Architecture** (ook wel **Plug-in Architecture** genoemd) is een software architectuurpatroon waarbij een minimale kern (core) wordt uitgebreid met **plug-ins** die extra functionaliteit toevoegen. Dit maakt de applicatie uitbreidbaar zonder de core te wijzigen.

## Structuur

```
???????????????????????????????????????????????????????????????????????????????
?                              PLUG-INS                                        ?
?                                                                              ?
?  ???????????????  ???????????????  ???????????????  ???????????????        ?
?  ?  Plug-in A  ?  ?  Plug-in B  ?  ?  Plug-in C  ?  ?  Plug-in D  ?  ...   ?
?  ?             ?  ?             ?  ?             ?  ?             ?        ?
?  ? SqlServer   ?  ?  PostgreSQL ?  ?    MySQL    ?  ?    Excel    ?        ?
?  ? Datasource  ?  ?  Datasource ?  ?  Datasource ?  ?  Datasource ?        ?
?  ???????????????  ???????????????  ???????????????  ???????????????        ?
?         ?                ?                ?                ?                ?
?         ????????????????????????????????????????????????????                ?
?                                   ?                                          ?
?                                   ? implements interface                     ?
?                                   ?                                          ?
?  ?????????????????????????????????????????????????????????????????????????  ?
?  ?                         PLUG-IN CONTRACTS                              ?  ?
?  ?                                                                        ?  ?
?  ?   • IDatasource                                                        ?  ?
?  ?   • ITemplateEngine                                                    ?  ?
?  ?   • IGenerator                                                         ?  ?
?  ?????????????????????????????????????????????????????????????????????????  ?
?                                   ?                                          ?
?                                   ? uses                                     ?
?                                   ?                                          ?
?  ?????????????????????????????????????????????????????????????????????????  ?
?  ?                           CORE SYSTEM                                  ?  ?
?  ?                                                                        ?  ?
?  ?   • Plug-in Registry                                                   ?  ?
?  ?   • Plug-in Loader                                                     ?  ?
?  ?   • Core Business Logic                                                ?  ?
?  ?   • Shared Services                                                    ?  ?
?  ?????????????????????????????????????????????????????????????????????????  ?
?                                                                              ?
???????????????????????????????????????????????????????????????????????????????
```

## Componenten

### 1. Core System (Microkernel)
De minimale kern die:
- Basis functionaliteit biedt
- Plug-ins registreert en laadt
- Communicatie tussen plug-ins coördineert
- Onafhankelijk van specifieke implementaties werkt

### 2. Plug-in Contracts (Interfaces)
Definiëren hoe plug-ins met de core communiceren:
- Stabiele API die niet vaak wijzigt
- Abstractie van specifieke implementaties
- Versioning voor backward compatibility

### 3. Plug-ins
Zelfstandige modules die:
- De contracts implementeren
- Onafhankelijk ontwikkeld en getest kunnen worden
- Runtime of compile-time geladen kunnen worden

### 4. Plug-in Registry
Beheert de beschikbare plug-ins:
- Registratie van plug-ins
- Lookup op basis van criteria
- Lifecycle management

## Real-World Voorbeeld: CodeGenerator

De CodeGenerator applicatie is een **uitstekend voorbeeld** van Microkernel Architecture:

```
???????????????????????????????????????????????????????????????????????????????
?                            DATASOURCE PLUG-INS                               ?
?                                                                              ?
?  ?????????????????? ?????????????????? ?????????????????? ????????????????  ?
?  ?  SqlServer     ? ?  PostgreSQL    ? ?  MySQL         ? ?  Excel       ?  ?
?  ?  Datasource    ? ?  Datasource    ? ?  Datasource    ? ?  Datasource  ?  ?
?  ?????????????????? ?????????????????? ?????????????????? ????????????????  ?
?  ?????????????????? ?????????????????? ?????????????????? ????????????????  ?
?  ?  Json          ? ?  Xml           ? ?  Yaml          ? ?  Csv         ?  ?
?  ?  Datasource    ? ?  Datasource    ? ?  Datasource    ? ?  Datasource  ?  ?
?  ?????????????????? ?????????????????? ?????????????????? ????????????????  ?
?  ?????????????????? ??????????????????                                      ?
?  ?  OpenApi       ? ?  DotNet        ?                                      ?
?  ?  Datasource    ? ?  Assembly      ?                                      ?
?  ?????????????????? ??????????????????                                      ?
???????????????????????????????????????????????????????????????????????????????
                                    ?
                           implements IDatasource
                                    ?
???????????????????????????????????????????????????????????????????????????????
?                      TEMPLATE ENGINE PLUG-INS                                ?
?                                                                              ?
?  ?????????????????? ?????????????????? ?????????????????? ????????????????  ?
?  ?  Scriban       ? ?  T4            ? ?  PlantUML      ? ?  DotNet      ?  ?
?  ?  Engine        ? ?  Engine        ? ?  Engine        ? ?  Project     ?  ?
?  ?????????????????? ?????????????????? ?????????????????? ????????????????  ?
???????????????????????????????????????????????????????????????????????????????
                                    ?
                          implements ITemplateEngine
                                    ?
???????????????????????????????????????????????????????????????????????????????
?                              CORE SYSTEM                                     ?
?                                                                              ?
?  ????????????????????  ????????????????????  ????????????????????          ?
?  ? CodeGenerator    ?  ? CodeGenerator    ?  ? CodeGenerator    ?          ?
?  ? .Core            ?  ? .Domain          ?  ? .Application     ?          ?
?  ????????????????????  ????????????????????  ????????????????????          ?
?  ????????????????????  ????????????????????  ????????????????????          ?
?  ? CodeGenerator    ?  ? CodeGenerator    ?  ? CodeGenerator    ?          ?
?  ? .Core.Workspaces ?  ? .Core.Templates  ?  ? .Core.Generators ?          ?
?  ????????????????????  ????????????????????  ????????????????????          ?
???????????????????????????????????????????????????????????????????????????????
```

### Project Mapping

| Component | Projects |
|-----------|----------|
| **Core System** | `CodeGenerator.Core`, `CodeGenerator.Domain`, `CodeGenerator.Application` |
| **Datasource Plug-ins** | `CodeGenerator.Core.Workspaces.Datasources.SqlServer`, `.PostgreServer`, `.Mysql`, `.Excel`, `.Json`, `.Xml`, `.Yaml`, `.Csv`, `.OpenApi`, `.DotNetAssembly` |
| **Template Engine Plug-ins** | `CodeGenerator.TemplateEngines.Scriban`, `.T4`, `.PlantUML`, `.DotNetProject` |
| **Generator Plug-ins** | `CodeGenerator.Generators.DotNet`, `.CodeArchitectureLayers` |

## Implementatie

### Plug-in Contract

```csharp
// Contract voor datasources
public interface IDatasource
{
    string Name { get; }
    string Description { get; }
    
    Task<DomainSchema> LoadSchemaAsync(DatasourceConfiguration config, CancellationToken ct = default);
    bool CanHandle(string connectionType);
    
    IEnumerable<ConfigurationProperty> GetConfigurationProperties();
}

// Contract voor template engines
public interface ITemplateEngine
{
    string Name { get; }
    IEnumerable<string> SupportedExtensions { get; }
    
    Task<string> RenderAsync(string template, object model, CancellationToken ct = default);
    Task<TemplateValidationResult> ValidateAsync(string template, CancellationToken ct = default);
}

// Contract voor generators
public interface IGenerator
{
    string Name { get; }
    string Description { get; }
    
    Task GenerateAsync(GeneratorContext context, CancellationToken ct = default);
    IEnumerable<GeneratorOption> GetOptions();
}
```

### Plug-in Implementatie

```csharp
// SqlServer Datasource plug-in
public class SqlServerDatasource : IDatasource
{
    public string Name => "SQL Server";
    public string Description => "Microsoft SQL Server database schema reader";

    public bool CanHandle(string connectionType) 
        => connectionType.Equals("sqlserver", StringComparison.OrdinalIgnoreCase) ||
           connectionType.Equals("mssql", StringComparison.OrdinalIgnoreCase);

    public async Task<DomainSchema> LoadSchemaAsync(
        DatasourceConfiguration config, 
        CancellationToken ct = default)
    {
        using var connection = new SqlConnection(config.ConnectionString);
        await connection.OpenAsync(ct);
        
        var schema = new DomainSchema();
        
        // Read tables
        var tables = await ReadTablesAsync(connection, ct);
        foreach (var table in tables)
        {
            schema.AddEntity(table);
        }
        
        // Read relationships
        var relationships = await ReadRelationshipsAsync(connection, ct);
        foreach (var rel in relationships)
        {
            schema.AddRelationship(rel);
        }
        
        return schema;
    }

    public IEnumerable<ConfigurationProperty> GetConfigurationProperties()
    {
        yield return new ConfigurationProperty("Server", typeof(string), required: true);
        yield return new ConfigurationProperty("Database", typeof(string), required: true);
        yield return new ConfigurationProperty("IntegratedSecurity", typeof(bool), defaultValue: true);
        yield return new ConfigurationProperty("Username", typeof(string), required: false);
        yield return new ConfigurationProperty("Password", typeof(string), required: false, isSecret: true);
    }
    
    // Private helper methods...
}

// PostgreSQL Datasource plug-in
public class PostgreSqlDatasource : IDatasource
{
    public string Name => "PostgreSQL";
    public string Description => "PostgreSQL database schema reader";

    public bool CanHandle(string connectionType) 
        => connectionType.Equals("postgresql", StringComparison.OrdinalIgnoreCase) ||
           connectionType.Equals("postgres", StringComparison.OrdinalIgnoreCase);

    public async Task<DomainSchema> LoadSchemaAsync(
        DatasourceConfiguration config, 
        CancellationToken ct = default)
    {
        using var connection = new NpgsqlConnection(config.ConnectionString);
        await connection.OpenAsync(ct);
        
        // PostgreSQL-specific schema reading...
    }
    
    // ...
}

// Scriban Template Engine plug-in
public class ScribanTemplateEngine : ITemplateEngine
{
    public string Name => "Scriban";
    public IEnumerable<string> SupportedExtensions => new[] { ".scriban", ".sbn", ".html", ".txt" };

    public async Task<string> RenderAsync(
        string template, 
        object model, 
        CancellationToken ct = default)
    {
        var scribanTemplate = Template.Parse(template);
        
        if (scribanTemplate.HasErrors)
        {
            throw new TemplateException(string.Join(", ", scribanTemplate.Messages));
        }
        
        var context = new TemplateContext();
        context.PushGlobal(new ScriptObject());
        context.SetValue("model", model);
        
        return await scribanTemplate.RenderAsync(context);
    }

    public Task<TemplateValidationResult> ValidateAsync(
        string template, 
        CancellationToken ct = default)
    {
        var scribanTemplate = Template.Parse(template);
        
        if (scribanTemplate.HasErrors)
        {
            return Task.FromResult(new TemplateValidationResult(
                false, 
                scribanTemplate.Messages.Select(m => m.Message)));
        }
        
        return Task.FromResult(TemplateValidationResult.Success);
    }
}
```

### Plug-in Registry

```csharp
public interface IPluginRegistry<TPlugin> where TPlugin : class
{
    void Register(TPlugin plugin);
    void Unregister(string name);
    TPlugin? GetPlugin(string name);
    IEnumerable<TPlugin> GetAll();
    bool HasPlugin(string name);
}

public class DatasourceRegistry : IPluginRegistry<IDatasource>
{
    private readonly ConcurrentDictionary<string, IDatasource> _datasources = new();
    private readonly ILogger<DatasourceRegistry> _logger;

    public DatasourceRegistry(
        IEnumerable<IDatasource> datasources,
        ILogger<DatasourceRegistry> logger)
    {
        _logger = logger;
        
        foreach (var ds in datasources)
        {
            Register(ds);
        }
    }

    public void Register(IDatasource datasource)
    {
        if (_datasources.TryAdd(datasource.Name, datasource))
        {
            _logger.LogInformation("Registered datasource: {Name}", datasource.Name);
        }
        else
        {
            _logger.LogWarning("Datasource already registered: {Name}", datasource.Name);
        }
    }

    public void Unregister(string name)
    {
        if (_datasources.TryRemove(name, out _))
        {
            _logger.LogInformation("Unregistered datasource: {Name}", name);
        }
    }

    public IDatasource? GetPlugin(string name)
    {
        return _datasources.TryGetValue(name, out var ds) ? ds : null;
    }

    public IDatasource? GetDatasourceFor(string connectionType)
    {
        return _datasources.Values.FirstOrDefault(d => d.CanHandle(connectionType));
    }

    public IEnumerable<IDatasource> GetAll() => _datasources.Values;
    
    public bool HasPlugin(string name) => _datasources.ContainsKey(name);
}

public class TemplateEngineRegistry : IPluginRegistry<ITemplateEngine>
{
    private readonly ConcurrentDictionary<string, ITemplateEngine> _engines = new();

    public ITemplateEngine? GetEngineForExtension(string extension)
    {
        return _engines.Values.FirstOrDefault(e => 
            e.SupportedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase));
    }
    
    // ... rest of implementation
}
```

### Dependency Injection Registration

```csharp
// Compile-time registration (simpel, type-safe)
public static class DatasourceServiceCollectionExtensions
{
    public static IServiceCollection AddDatasources(this IServiceCollection services)
    {
        // Core datasource service
        services.AddScoped<IDatasourceService, DatasourceService>();
        services.AddSingleton<IPluginRegistry<IDatasource>, DatasourceRegistry>();
        
        // Register all datasource plug-ins
        services.AddScoped<IDatasource, SqlServerDatasource>();
        services.AddScoped<IDatasource, PostgreSqlDatasource>();
        services.AddScoped<IDatasource, MySqlDatasource>();
        services.AddScoped<IDatasource, ExcelDatasource>();
        services.AddScoped<IDatasource, JsonDatasource>();
        services.AddScoped<IDatasource, XmlDatasource>();
        services.AddScoped<IDatasource, YamlDatasource>();
        services.AddScoped<IDatasource, CsvDatasource>();
        services.AddScoped<IDatasource, OpenApiDatasource>();
        services.AddScoped<IDatasource, DotNetAssemblyDatasource>();
        
        return services;
    }
    
    public static IServiceCollection AddTemplateEngines(this IServiceCollection services)
    {
        services.AddSingleton<IPluginRegistry<ITemplateEngine>, TemplateEngineRegistry>();
        
        services.AddScoped<ITemplateEngine, ScribanTemplateEngine>();
        services.AddScoped<ITemplateEngine, T4TemplateEngine>();
        services.AddScoped<ITemplateEngine, PlantUmlTemplateEngine>();
        services.AddScoped<ITemplateEngine, DotNetProjectTemplateEngine>();
        
        return services;
    }
}

// In Program.cs
builder.Services.AddDatasources();
builder.Services.AddTemplateEngines();
builder.Services.AddGenerators();
```

## Plug-in Loading Strategieën

### 1. Compile-time Registration
Plug-ins zijn compile-time bekend en worden via DI geregistreerd.

**Voordelen:**
- Type-safe
- Geen runtime fouten
- Eenvoudig te debuggen

**Nadelen:**
- Hercompilatie nodig voor nieuwe plug-ins
- Alle plug-ins altijd geladen

### 2. Runtime Assembly Loading

```csharp
public class RuntimePluginLoader<TPlugin> where TPlugin : class
{
    private readonly ILogger<RuntimePluginLoader<TPlugin>> _logger;
    private readonly List<Assembly> _loadedAssemblies = new();

    public RuntimePluginLoader(ILogger<RuntimePluginLoader<TPlugin>> logger)
    {
        _logger = logger;
    }

    public IEnumerable<TPlugin> LoadPlugins(string pluginDirectory)
    {
        var plugins = new List<TPlugin>();
        
        if (!Directory.Exists(pluginDirectory))
        {
            _logger.LogWarning("Plugin directory not found: {Directory}", pluginDirectory);
            return plugins;
        }

        foreach (var file in Directory.GetFiles(pluginDirectory, "*.dll"))
        {
            try
            {
                var assembly = LoadAssembly(file);
                var pluginTypes = FindPluginTypes(assembly);
                
                foreach (var type in pluginTypes)
                {
                    if (CreateInstance(type) is TPlugin plugin)
                    {
                        plugins.Add(plugin);
                        _logger.LogInformation(
                            "Loaded plugin: {PluginType} from {Assembly}", 
                            type.Name, 
                            Path.GetFileName(file));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load plugin from {File}", file);
            }
        }

        return plugins;
    }

    private Assembly LoadAssembly(string path)
    {
        var loadContext = new PluginLoadContext(path);
        var assembly = loadContext.LoadFromAssemblyPath(path);
        _loadedAssemblies.Add(assembly);
        return assembly;
    }

    private IEnumerable<Type> FindPluginTypes(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(t => typeof(TPlugin).IsAssignableFrom(t) 
                     && !t.IsInterface 
                     && !t.IsAbstract);
    }

    private object? CreateInstance(Type type)
    {
        try
        {
            return Activator.CreateInstance(type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create instance of {Type}", type.Name);
            return null;
        }
    }
}

// Custom AssemblyLoadContext voor isolatie
public class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string pluginPath) : base(isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
    }
}
```

### 3. MEF (Managed Extensibility Framework)

```csharp
// In plug-in assembly
[Export(typeof(IDatasource))]
[ExportMetadata("Name", "SQL Server")]
[ExportMetadata("Description", "Microsoft SQL Server datasource")]
public class SqlServerDatasource : IDatasource
{
    // Implementation...
}

// In core system
public class MefPluginHost
{
    private CompositionContainer? _container;
    
    [ImportMany]
    public IEnumerable<Lazy<IDatasource, IDatasourceMetadata>> Datasources { get; set; } 
        = Array.Empty<Lazy<IDatasource, IDatasourceMetadata>>();

    [ImportMany]
    public IEnumerable<Lazy<ITemplateEngine, ITemplateEngineMetadata>> TemplateEngines { get; set; }
        = Array.Empty<Lazy<ITemplateEngine, ITemplateEngineMetadata>>();

    public void LoadPlugins(string pluginsDirectory)
    {
        var catalog = new AggregateCatalog();
        
        // Add plug-in directories
        if (Directory.Exists(pluginsDirectory))
        {
            catalog.Catalogs.Add(new DirectoryCatalog(pluginsDirectory));
        }
        
        // Add current assembly for built-in plug-ins
        catalog.Catalogs.Add(new AssemblyCatalog(typeof(MefPluginHost).Assembly));

        _container = new CompositionContainer(catalog);
        _container.ComposeParts(this);
    }

    public IDatasource? GetDatasource(string name)
    {
        return Datasources
            .FirstOrDefault(d => d.Metadata.Name == name)?
            .Value;
    }
}

public interface IDatasourceMetadata
{
    string Name { get; }
    string Description { get; }
}
```

## Plug-in Versioning

```csharp
// Plug-in contract met versie
public interface IPluginInfo
{
    string Name { get; }
    Version Version { get; }
    Version MinimumCoreVersion { get; }
}

public interface IDatasource : IPluginInfo
{
    // Existing members...
}

// Versie check bij laden
public class VersionedPluginLoader
{
    private readonly Version _coreVersion;
    
    public VersionedPluginLoader(Version coreVersion)
    {
        _coreVersion = coreVersion;
    }
    
    public bool IsCompatible(IPluginInfo plugin)
    {
        return _coreVersion >= plugin.MinimumCoreVersion;
    }
    
    public IEnumerable<TPlugin> LoadCompatiblePlugins<TPlugin>(
        IEnumerable<TPlugin> plugins) where TPlugin : IPluginInfo
    {
        return plugins.Where(p => IsCompatible(p));
    }
}
```

## Bekende Voorbeelden

| Applicatie | Core | Plug-ins |
|------------|------|----------|
| **Visual Studio Code** | Editor core | Extensions (languages, themes, debuggers) |
| **Eclipse** | Platform | Plug-ins (Java, C++, Git) |
| **Chrome / Edge** | Browser engine | Extensions |
| **WordPress** | CMS core | Plugins & Themes |
| **AutoCAD** | CAD engine | Add-ins |
| **Photoshop** | Image editor | Filters & Plug-ins |
| **Jenkins** | CI/CD core | Plug-ins |
| **NuGet** | Package manager | Custom package sources |

## Voordelen

| Voordeel | Beschrijving |
|----------|--------------|
| ? **Uitbreidbaar** | Nieuwe features zonder core te wijzigen |
| ? **Flexibel** | Plug-ins kunnen runtime worden toegevoegd |
| ? **Modulair** | Plug-ins zijn onafhankelijk ontwikkelbaar |
| ? **Testbaar** | Core en plug-ins apart testbaar |
| ? **Open/Closed** | Open voor extensie, gesloten voor modificatie |
| ? **Separation of Concerns** | Duidelijke scheiding van verantwoordelijkheden |
| ? **Third-party extensies** | Anderen kunnen plug-ins ontwikkelen |

## Nadelen

| Nadeel | Beschrijving |
|--------|--------------|
| ? **Complexiteit** | Plug-in systeem moet robuust zijn |
| ? **Versioning** | Compatibiliteit tussen core en plug-ins beheren |
| ? **Performance** | Runtime loading kan overhead geven |
| ? **Security** | Plug-ins kunnen security risico's introduceren |
| ? **Debugging** | Problemen in plug-ins kunnen lastig te debuggen zijn |
| ? **API stabiliteit** | Contract wijzigingen kunnen plug-ins breken |

## Wanneer Gebruiken?

| Scenario | Geschikt? |
|----------|-----------|
| Product met veel extensies | ? Ja |
| IDE's, Editors | ? Ja |
| Code generators | ? Ja |
| ETL tools | ? Ja |
| Build systems | ? Ja |
| CMS systemen | ? Ja |
| Simpele single-purpose app | ? Nee |
| Prototype / MVP | ? Nee |

## Gerelateerde Patterns

- **Strategy Pattern**: Plug-ins zijn vaak Strategy implementaties
- **Factory Pattern**: Voor plug-in creatie
- **Dependency Injection**: Voor plug-in registratie en resolutie
- **Service Locator**: Alternatief voor plug-in lookup (minder gewenst)

## Zie Ook

- [MVC Pattern](MVC.md) - Presentation layer pattern
- [MVP Pattern](MVP.md) - Presentation layer pattern
- [MVVM Pattern](MVVM.md) - Presentation layer pattern
- [Clean Architecture](../Software%20Architecture/CleanArchitecture.md) - Application architecture
