# Code Generator

## Domains

### DotNet Domain
```
DotNetFramework
├── .NETFramework
├── .NETCore
├── .NET5Plus
└── .NETStandard
DotNetFrameworkVersion
├── VersionString : string
└── Framework : DotNetFramework
```

```
DotNetProject
├── DotNetLanguage
    ├── CSharp
    ├── FSharp
    └── VisualBasic
├── DotNetProjectType
    ├── ConsoleApp
    ├── ClassLibrary
    ├── WebApi
    ├── BlazorApp
    ├── WinForms
    ├── UserControlClassLibrary
    └── UnitTest
├── TargetFrameworkVersion : DotNetFrameworkVersion
├── NugetPackageReferences
├── ProjectReferences
```

```
UserControl
-> Preview
Form
```



```
DotNetProjectGenerator : Generator<DotNetProject>
```

#### File system Domain
```
FileSystem
├── Directory
│   ├── Name : string
│   └── Files : File[]
└── File
    ├── Name : string
    ├── Extension : string
    └── Content : string
```

```
ImageFile
 -> Preview
 Thumbnail
```
## Template Domain
```
TemplateDefinition
├──Parameter
TemplateInstance
    Output TextArtifact
TemplateEngine
```

## Generator Domain
```
Artifact
├── PreviewableArtifact

ArtifactPreviewer
- Image
- UserControl
- File & Folder structure
- Project

Artifact
- ArtifactDecorators[]
- ChildArtifacts[]
```
GeneratorOrchestrator steps
```
Announce Creating -> everyone can append decorations
Execute Creating
Notify Created
```
GeneratorOrchestratorRootArtifact -> root object where everthing is append to

## Programing Language Entities

```
Languages
└── CSharp
    ├── Class
    ├── Interface
    ├── Struct
    ├── Enum
    └── Delegate
├── FSharp
├── VisualBasic
├── Typescript
├── Javascript
├── Python
└──
```

## Package Managers
```
PackageManager
├── NugetPackageManager
├── NpmPackageManager
└── ChocolateyPackageManager

```
### Nuget
```
NugetPackage
├── PackageId : string
└── Version : string
```

### Artifact Decorators
```
ArtifactHost
├─ Artifact : Artifact
   └─ Decorators : Dictionary<string, IArtifactDecorator>
```
```
Artifact
├─ Parent: Artifact 
├─ Children : List<Artifact>
├─ Decorators : KeyedCollection<IArtifactDecorator> (use InstanceName as key)
└─ SetProperty<T>(string propertyName, T value)
└─ GetProperty<T>(string propertyName) : T
```
For nested types propertyName can be "Property.SubProperty"
```
ArtifactDecorator
├─ InstanceName : string
├─ Parent : IArtifactDecorator
├─ Children : List<IArtifactDecorator>
└─ GetFullName() : string
```

```
CompositeDecorator : IArtifactDecorator
├─ Decorators : KeyedCollection<IArtifactDecorator> (use InstanceName as key)

```