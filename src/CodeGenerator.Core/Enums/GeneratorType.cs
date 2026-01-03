namespace CodeGenerator.Core.Enums;

/// <summary>
/// Types of generators available
/// </summary>
public enum GeneratorType
{
    Entity,
    DbContext,
    Repository,
    Controller,
    ViewModel,
    View,
    DbScript,
    Migration,
    ServiceInterface,
    ServiceImplementation,
    DependencyInjection,
    Configuration,
    Mapper,
    Validator,
    UnitTest,
    IntegrationTest,
    Other
}