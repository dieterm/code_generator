using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;

namespace CodeGenerator.Core.Interfaces;

/// <summary>
/// Interface for settings persistence
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Load settings from file
    /// </summary>
    Task<GeneratorSettings> LoadSettingsAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Save settings to file
    /// </summary>
    Task SaveSettingsAsync(GeneratorSettings settings, string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get default settings
    /// </summary>
    GeneratorSettings GetDefaultSettings();
}
