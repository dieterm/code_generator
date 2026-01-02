using CodeGenerator.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace CodeGenerator.Core.Services;

/// <summary>
/// File system operations service
/// </summary>
public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;

    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
    }

    public async Task WriteFileAsync(string path, string content, bool createBackup = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                EnsureDirectory(directory);
            }

            if (createBackup && File.Exists(path))
            {
                var backupPath = $"{path}.bak";
                File.Copy(path, backupPath, overwrite: true);
                _logger.LogDebug("Created backup at {BackupPath}", backupPath);
            }

            await File.WriteAllTextAsync(path, content, cancellationToken);
            _logger.LogDebug("Wrote file {Path}", path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write file {Path}", path);
            throw;
        }
    }

    public async Task<string> ReadFileAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            return await File.ReadAllTextAsync(path, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read file {Path}", path);
            throw;
        }
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            _logger.LogDebug("Created directory {Path}", path);
        }
    }

    public void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            _logger.LogDebug("Deleted file {Path}", path);
        }
    }

    public async Task CopyFileAsync(string source, string destination, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(destination);
        if (!string.IsNullOrEmpty(directory))
        {
            EnsureDirectory(directory);
        }

        var content = await File.ReadAllBytesAsync(source, cancellationToken);
        await File.WriteAllBytesAsync(destination, content, cancellationToken);
        _logger.LogDebug("Copied file from {Source} to {Destination}", source, destination);
    }
}
