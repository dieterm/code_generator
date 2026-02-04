# 4/2/2026
## Settings-mechanisme
Ieder object in de applicatie erft over van ObjectBase-class.
De ObjectBase-class heeft twee methodes:
- GetSettings<T>(string key, T? defaultValue=default)
- GetTypeSetting<T>(string key, T? defaultValue=default)
- GetInstanceSetting<T>(string key, T? defaultValue=default)
- SetInstanceSetting<T>(string key, T value)
- SetTypeSetting<T>(string key, T value)
De GetSettings-methode haalt een settings-object op via de SettingsProvider.
Probeert eerst de settings via InstanceSettings op te halen.
- Als er geen InstanceSettings zijn, wordt er gekeken naar TypeSettings.
De ObjectBase-class heeft property:
- SettingsProvider: IObjectBaseSettingsProvider
- SettingsInstanceId: string (unieke id per instantie van ObjectBase)

De IObjectBaseSettingsProvider heeft methodes:
- GetInstanceSetting<T>(string instanceId, string key, T? defaultValue=default)
- GetTypeSetting<T>(Type type, string key, T? defaultValue=default)
- SetInstanceSetting<T>(string instanceId, string key, T value)
- SetTypeSetting<T>(Type type, string key, T value)
De IObjectBaseSettingsProvider heeft events:
- InstanceSettingChanged(string instanceId, string key, object? newValue)
- TypeSettingChanged(Type type, string key, object? newValue)

# 2/2/2026
## DONE: ADD embedded resources to RibbonProject template

## DONE: GenerationOrchestrator 
TODO: controleren waarom deze als Transient en niet als Singleton geregistreerd is in DI-container

## DONE: WorkspaceArtifact.ArtifactContext

De ArtifactContext property op een WorkspaceArtifact kan gebruikt worden om:
- bv. Namespace: de resulterende namespace voor dit artifact
- bv. NamespaceParameters: lijst met namespace parameters om te gebruiken in NamespacePattern-property
- bv. Scope: de resulterende scope van het artifact (Application, Shared, Geoservice, ...)
- bv. Layer: de resulterende architectuurlaag van het artifact (Application, Presentation, Domain, Infrastructure, ...)
- bv. OutputPath: de output path voor gegenereerde bestanden

## DONE: Add ScopeArtifact NamespacePattern property

## ProgrammingLanguageCodeGenerator regisration
Registreer ProgrammingLanguageCodeGenerator per programmeertaal via DI-container,
ipv via CSharpCodeGeneratorExtensions.RegisterCSharpCodeGenerator();

## DONE: Verwijder de LayerArtifact in de Generation TreeView
Gebruik in de plaats decorators op de LayerArtifact om de scope en layer op te slaan
Gebruik een WorkspaceArtifactReferenceDecorator om een link te leggen naar een workspace artifact

## ArtifactTreeView baseclass
ArtifactTreeViewController<TTreeViewModel, TArtifactBase> 
	where TArtifactBase : IArtifact
	where TTreeViewModel : IArtifactTreeViewModel

Elke Artifact in de ArtifactTreeView erft van een baseclass
- WorkspaceTreeView 
	- WorkspaceArtifactBase = abstract
		- WorkspaceArtifact : concrete
		- WorkspaceArtifactReferenceDecorator : concrete
- TemplateTreeView
	- TemplateArtifactBase = abstract
		- TemplateArtifact : concrete
		- TemplateArtifactReferenceDecorator : concrete
- GenerationTreeView
	- GenerationArtifactBase = abstract
		- GenerationArtifact : concrete
		- GenerationArtifactReferenceDecorator : concrete
--------------
# Structuur voor codegeneratie framework
Maak voor ieder programmeerconcept een aparte Artifact.
Zet properties rechtstreeks op de Artifact, niet via decorators.

```
ArchitectureLayerArtifact : abstract
--> ApplicationLayerArtifact : scope=Application, Shared, Geoservice, ...
	-> DotNetProjectDecorator : classlib, c#
	-> FolderDecorator : FolderName="Application"
	--> ApplicationControllerCollectionArtifact
		--> ApplicationControllerArtifact
			-> MethodArtifact: MethodeA(..)
			-> MethodArtifact: MethodeB(..)
--> PresentationLayerArtifact
	--> DotNetProjectDecorator : winformslib, c#
--> ...

ControllerArtifact : abstract
--> ApplicationControllerArtifact
	-> MethodArtifact: MethodeA(..)
	-> MethodArtifact: MethodeB(..)
--> SettingsControllerArtifact
```
Daarna gericht luisteren naar event:
```
MessageBus.Subscribe<ArtifactCreated>(handler, (e) => e.Artifact is ApplicationControllerArtifact)
```

Artifacts per generator definiëren:
```
SettingsGenerator
--> SettingsControllerArtifact
--> SettingsViewArtifact
--> SettingsViewModelArtifact
```

Geef elke artifact een vast id (guid)
zet `CODEGENERATOR_MARKER:<guid>` in templates en gegenereerde code
via FileChangeWatcher kun je template updaten vanuit de workspace en omgekeerd
In .cs files -> via comment of region
In folder een `.CodeGenerator_<guid>`-bestand

Een generator geeft een lijst met images en een key terug die gebruikt worden voor 
visualizatie in de treeview
-> ImageList 
Iedere artifact heeft een TreeNodeText en TreeNodeImageKey property
