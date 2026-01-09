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
