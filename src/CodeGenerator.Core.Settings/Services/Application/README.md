# System.Configuration.ApplicationSettingsBase - Gebruikshandleiding

## ?? Overzicht

Dit voorbeeld toont hoe je `System.Configuration.ApplicationSettingsBase` gebruikt om applicatie-instellingen op te slaan en te laden. De instellingen worden automatisch opgeslagen in een `user.config` bestand.

## ?? Bestanden

- **`ApplicationSettings.cs`** - De settings class met alle instellingen
- **`SettingsManager.cs`** - Service class voor eenvoudig beheer van instellingen
- **`SettingsUsageExample.cs`** - Voorbeelden van gebruik

## ?? Features

### Ondersteunde Instellingen

1. **Interface Taal** (`InterfaceLanguage`)
   - Default: "en-US"
   - Opties: nl-NL, en-US, en-GB, fr-FR, de-DE, es-ES

2. **Syncfusion Theme** (`Theme`)
   - Default: "Office2019Colorful"
   - Verschillende Office en Material themes

3. **Recent Files** (`RecentFiles`)
   - Max 10 recent geopende bestanden
   - Automatisch sorted (meest recente eerst)

4. **Favorite Files** (`FavoriteFiles`)
   - Onbeperkt aantal favoriete bestanden
   - Add/Remove functionaliteit

5. **Window State** (`WindowWidth`, `WindowHeight`, `WindowMaximized`)
   - Sla venster afmetingen en status op

## ?? Gebruik

### Optie 1: Via SettingsManager (Aanbevolen)

```csharp
var settingsManager = new SettingsManager();

// Taal wijzigen
settingsManager.InterfaceLanguage = "nl-NL";

// Theme wijzigen
settingsManager.Theme = "Office2019Black";

// Recent file toevoegen
settingsManager.AddRecentFile(@"C:\Projects\MyProject.json");

// Favorite toevoegen
settingsManager.AddFavoriteFile(@"C:\Projects\Important.json");

// Window state opslaan
settingsManager.SaveWindowState(1920, 1080, false);
```

### Optie 2: Direct via Singleton

```csharp
var settings = ApplicationSettings.Instance;

// Waarde wijzigen
settings.InterfaceLanguage = "nl-NL";
settings.Save(); // Vergeet niet op te slaan!

// Waarde ophalen
string language = settings.InterfaceLanguage;
```

## ?? Waar worden settings opgeslagen?

Settings worden automatisch opgeslagen in:

```
C:\Users\<USERNAME>\AppData\Local\<Company>\<AppName>\<Version>\user.config
```

Bijvoorbeeld:
```
C:\Users\Dieter\AppData\Local\CodeGenerator\CodeGenerator.exe_Url_xyz123\1.0.0.0\user.config
```

Je kunt het exacte pad ophalen met:
```csharp
string path = settingsManager.GetSettingsFilePath();
Console.WriteLine(path);
```

## ?? Belangrijke Concepten

### 1. UserScopedSetting vs ApplicationScopedSetting

- **`[UserScopedSetting]`** - Per gebruiker, kunnen gewijzigd worden (Read/Write)
- **`[ApplicationScopedSetting]`** - Voor alle gebruikers, alleen lezen (Read-only)

In dit voorbeeld gebruiken we alleen `UserScopedSetting`.

### 2. DefaultSettingValue

Geeft de standaard waarde aan bij eerste gebruik:

```csharp
[UserScopedSetting]
[DefaultSettingValue("en-US")]
public string InterfaceLanguage { get; set; }
```

### 3. Automatisch Opslaan

Settings worden **NIET** automatisch opgeslagen bij wijziging. Je moet expliciet `Save()` aanroepen:

```csharp
settings.InterfaceLanguage = "nl-NL";
settings.Save(); // ?? Belangrijk!
```

In de `SettingsManager` class wordt dit automatisch gedaan in de property setters.

### 4. StringCollection voor lijsten

Voor lijsten zoals Recent Files gebruiken we `StringCollection`:

```csharp
[UserScopedSetting]
[DefaultSettingValue("")]
public StringCollection RecentFiles { get; set; }
```

## ?? Integratie in je applicatie

### Stap 1: Registreer in Dependency Injection

```csharp
// In Program.cs of ServiceCollectionExtensions.cs
services.AddSingleton<SettingsManager>();
```

### Stap 2: Gebruik in Controllers

```csharp
public class ApplicationController
{
    private readonly SettingsManager _settingsManager;

    public ApplicationController(SettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    private async void OnOpenRequested(object? sender, EventArgs e)
    {
        var filePath = _fileSystemDialogService.OpenFile("...");
        if (filePath != null)
        {
            // Add to recent files
            _settingsManager.AddRecentFile(filePath);
        }
    }
}
```

### Stap 3: Theme toepassen bij startup

```csharp
// In je MainForm of Startup code
var settingsManager = new SettingsManager();
string theme = settingsManager.Theme;

// Apply Syncfusion theme
SkinManager.SetVisualStyle(this, theme);
```

### Stap 4: Language toepassen

```csharp
var language = _settingsManager.InterfaceLanguage;
Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
```

## ?? Testen

Run de voorbeelden:

```csharp
SettingsUsageExample.RunAllExamples();
```

Dit toont:
- Basis gebruik
- Recent files beheer
- Favorites beheer
- Window state
- Direct access
- Settings info
- Reset functionaliteit
- Real-world scenario

## ?? Settings Upgraden bij nieuwe versie

Als je app update naar een nieuwe versie:

```csharp
if (settings.UpgradeRequired)
{
    settings.Upgrade();
    settings.UpgradeRequired = false;
    settings.Save();
}
```

Voeg een property toe aan ApplicationSettings:

```csharp
[UserScopedSetting]
[DefaultSettingValue("true")]
public bool UpgradeRequired { get; set; }
```

## ??? Debugging

### Settings bekijken

```csharp
var allSettings = settingsManager.GetAllSettings();
foreach (var kvp in allSettings)
{
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
}
```

### Settings resetten

```csharp
settingsManager.ResetSettings();
```

### user.config verwijderen

Delete het bestand handmatig om alles te resetten:
```
C:\Users\<USERNAME>\AppData\Local\<Company>\<AppName>\...
```

## ?? Let op!

1. **Altijd `Save()` aanroepen** na wijzigingen (tenzij je SettingsManager gebruikt)
2. **StringCollection** kan NULL zijn, gebruik de property getters die dit afhandelen
3. **File paths** in Recent/Favorites kunnen niet meer bestaan - check altijd met `File.Exists()`
4. **Thread-safe**: De singleton pattern is thread-safe geïmplementeerd

## ?? Uitbreidingen

Voeg eenvoudig nieuwe settings toe:

```csharp
// In ApplicationSettings.cs
[UserScopedSetting]
[DefaultSettingValue("false")]
public bool AutoSaveEnabled
{
    get { return (bool)this[nameof(AutoSaveEnabled)]; }
    set { this[nameof(AutoSaveEnabled)] = value; }
}
```

En in SettingsManager:

```csharp
public bool AutoSaveEnabled
{
    get => _settings.AutoSaveEnabled;
    set
    {
        _settings.AutoSaveEnabled = value;
        _settings.Save();
    }
}
```

## ?? Meer informatie

- [Microsoft Docs - Application Settings](https://learn.microsoft.com/en-us/dotnet/framework/winforms/advanced/application-settings-overview)
- [ApplicationSettingsBase Class](https://learn.microsoft.com/en-us/dotnet/api/system.configuration.applicationsettingsbase)
