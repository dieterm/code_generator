# Design Patterns in C#

Dit is een verzameling van de belangrijkste design patterns met uitleg en praktische C# voorbeelden.

## Categorieën

Design patterns worden traditioneel ingedeeld in drie categorieën volgens het "Gang of Four" (GoF) boek:

### Creational Patterns (Creatie Patronen)
Patronen die gaan over het creëren van objecten.

| Pattern | Beschrijving |
|---------|--------------|
| [Singleton](Creational/Singleton.md) | Eén enkele instance van een class |
| [Factory Method](Creational/FactoryMethod.md) | Creatie delegeren aan subclasses |
| [Abstract Factory](Creational/AbstractFactory.md) | Families van gerelateerde objecten creëren |
| [Builder](Creational/Builder.md) | Complexe objecten stap voor stap bouwen |
| [Prototype](Creational/Prototype.md) | Objecten klonen |

### Structural Patterns (Structurele Patronen)
Patronen die gaan over de compositie van classes en objecten.

| Pattern | Beschrijving |
|---------|--------------|
| [Adapter](Structural/Adapter.md) | Incompatibele interfaces compatibel maken |
| [Bridge](Structural/Bridge.md) | Abstractie scheiden van implementatie |
| [Composite](Structural/Composite.md) | Boomstructuren van objecten |
| [Decorator](Structural/Decorator.md) | Dynamisch gedrag toevoegen |
| [Facade](Structural/Facade.md) | Vereenvoudigde interface voor complex systeem |
| [Flyweight](Structural/Flyweight.md) | Geheugen besparen door delen |
| [Proxy](Structural/Proxy.md) | Plaatsvervanger voor een ander object |

### Behavioral Patterns (Gedragspatronen)
Patronen die gaan over communicatie tussen objecten.

| Pattern | Beschrijving |
|---------|--------------|
| [Chain of Responsibility](Behavioral/ChainOfResponsibility.md) | Keten van handlers |
| [Command](Behavioral/Command.md) | Acties als objecten |
| [Iterator](Behavioral/Iterator.md) | Sequentieel door collectie lopen |
| [Mediator](Behavioral/Mediator.md) | Gecentraliseerde communicatie |
| [Memento](Behavioral/Memento.md) | State opslaan en herstellen |
| [Observer](Behavioral/Observer.md) | Publish-Subscribe notificaties |
| [State](Behavioral/State.md) | Gedrag wijzigen op basis van state |
| [Strategy](Behavioral/Strategy.md) | Algoritmes uitwisselbaar maken |
| [Template Method](Behavioral/TemplateMethod.md) | Algoritme skelet met aanpasbare stappen |
| [Visitor](Behavioral/Visitor.md) | Operaties toevoegen zonder class te wijzigen |

## Wanneer welk pattern?

| Probleem | Pattern |
|----------|---------|
| Eén instance nodig voor hele applicatie | Singleton |
| Object creatie flexibel maken | Factory Method, Abstract Factory |
| Complex object stapsgewijs bouwen | Builder |
| Bestaand object aanpassen zonder te wijzigen | Decorator, Adapter |
| Acties ongedaan kunnen maken | Command, Memento |
| Objecten informeren over wijzigingen | Observer |
| Gedrag wisselen afhankelijk van situatie | Strategy, State |
| Complexe subsysteem vereenvoudigen | Facade |

## Moderne C# Alternatieven

Sommige patterns zijn in moderne C# minder nodig door taalfeatures:

| Pattern | Modern Alternatief |
|---------|-------------------|
| Iterator | `IEnumerable<T>`, `yield return` |
| Observer | `event`, `IObservable<T>`, Reactive Extensions |
| Strategy | Delegates, `Func<T>`, Lambda expressions |
| Command | `Action<T>`, `Func<T>` |
| Singleton | Dependency Injection met `AddSingleton<T>()` |
