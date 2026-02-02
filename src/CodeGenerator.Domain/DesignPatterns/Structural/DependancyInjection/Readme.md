# Dependency Injection Frameworks

Dit onderdeel van de codegenerator is gericht op het ondersteunen van verschillende Dependency Injection (DI) frameworks. Het biedt functionaliteit om code te genereren die compatibel is met populaire DI frameworks zoals Autofac, Microsoft.Extensions.DependencyInjection, Ninject, en Unity.

## Overzicht
De `DependancyInjectionFrameworkManager` klasse beheert verschillende DI frameworks en biedt methoden om code te genereren voor het registreren van services en het opzetten van DI containers. Elk framework heeft zijn eigen implementatie van de `IDependancyInjectionFramework` interface, die specifieke codegeneratie logica bevat.

## Voorbeeldgebruik
```csharp
// Alle frameworks ophalen
var manager = DependancyInjectionFrameworkManager.CreateWithBuiltInFrameworks();

// Framework selecteren
var framework = manager.GetFrameworkById("Autofac");

// Code genereren
var singletonCode = framework.GenerateRegisterSingleton(
    new TypeReference("IMyService"), 
    new TypeReference("MyService"));
// Output: "builder.RegisterType<MyService>().As<IMyService>().SingleInstance();"

// Container setup class genereren
var setupClass = framework.GenerateContainerSetupClass(codeFile, "MyModule");
// Output: C# class code voor het instellen van de DI container met Autofac
```

## Ondersteunde Frameworks
- **Autofac**: Een populaire DI container voor .NET.
- **Microsoft.Extensions.DependencyInjection**: De standaard DI container voor ASP.NET Core.
- **Ninject**: Een flexibel DI framework voor .NET.
- **Castle Windsor**: Een krachtig DI framework met uitgebreide functionaliteit.
- **Simple Injector**: Een snelle en eenvoudige DI container voor .NET.

## Nog te implementeren in de toekomst
- **Unity**: Een DI container ontwikkeld door Microsoft.
- **StructureMap**: Een van de oudste DI containers voor .NET.
- **DryIoc**: Een snelle en flexibele DI container voor .NET.
- **LightInject**: Een lichtgewicht DI container voor .NET.
- **Autofac.Extensions.DependencyInjection**: Integratie van Autofac met Microsoft.Extensions.DependencyInjection

