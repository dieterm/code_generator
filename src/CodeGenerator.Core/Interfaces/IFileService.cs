using CodeGenerator.Core.Enums;
using CodeGenerator.Core.Models.Configuration;
using CodeGenerator.Core.Models.Domain;
using CodeGenerator.Core.Models.Output;

namespace CodeGenerator.Core.Interfaces;


/// <summary>
/// Interface for file operations
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Write content to a file
    /// </summary>
    Task WriteFileAsync(string path, string content, bool createBackup = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Read content from a file
    /// </summary>
    Task<string> ReadFileAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a file exists
    /// </summary>
    bool FileExists(string path);

    /// <summary>
    /// Create directory if it doesn't exist
    /// </summary>
    void EnsureDirectory(string path);

    /// <summary>
    /// Delete a file
    /// </summary>
    void DeleteFile(string path);

    /// <summary>
    /// Copy a file
    /// </summary>
    Task CopyFileAsync(string source, string destination, CancellationToken cancellationToken = default);
}
