namespace CodeGenerator.Core.Enums;

/// <summary>
/// Supported data layer technologies
/// </summary>
public enum DataLayerTechnology
{
    EntityFrameworkCore,
    Dapper,
    ADO_NET,
    MongoDB,
    Redis,
    CosmosDB,
    RawSQL
}