using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Settings.Services
{
    public class SettingsService<TSettings> where TSettings : class, new()
    {
        private readonly ILogger<SettingsService<TSettings>> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public SettingsService(ILogger<SettingsService<TSettings>> logger)
        {
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<TSettings?> LoadSettingsAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!File.Exists(path))
                {
                    _logger.LogWarning("Settings file not found at {Path}, using defaults", path);
                    return null;
                }

                var json = await File.ReadAllTextAsync(path, cancellationToken);
                var settings = JsonSerializer.Deserialize<TSettings>(json, _jsonOptions);

                if(settings==null)
                {
                    _logger.LogWarning("Settings file at {Path} is empty or invalid, using defaults", path);
                    return null;
                }
                else
                {
                    _logger.LogInformation("Loaded settings from {Path}", path);
                    return settings;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load settings from {Path}", path);
                throw;
            }
        }

        public async Task SaveSettingsAsync(TSettings settings, string path, CancellationToken cancellationToken = default)
        {
            try
            {
                var directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(settings, _jsonOptions);
                await File.WriteAllTextAsync(path, json, cancellationToken);
                _logger.LogInformation("Saved settings to {Path}", path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save settings to {Path}", path);
                throw;
            }
        }
    }
}
