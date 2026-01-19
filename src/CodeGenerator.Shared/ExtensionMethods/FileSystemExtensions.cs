using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.ExtensionMethods
{
    public static class FileSystemExtensions
    {
        /// <summary>
        /// Creates a new file with a unique indexed name in the specified directory path.
        /// Will append an index to the base file name if a file with that name already exists.
        /// eg. "NewTemplate.txt", "NewTemplate1.txt", "NewTemplate2.txt", etc.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="baseFileName">name without extension</param>
        /// <param name="fileExtension">extension without dot, e.g. "txt"</param>
        /// <returns></returns>
        public static FileInfo? CreateIndexedFile(this string directoryPath, string baseFileName, string fileExtension)
        {
            try
            {
                int? fileIndex = null;
                var filePath = Path.Combine(directoryPath, $"{baseFileName}.{fileExtension}");
                if (File.Exists(filePath))
                {
                    fileIndex = 1;
                    filePath = Path.Combine(directoryPath, $"{baseFileName}{fileIndex}.{fileExtension}");
                    while (File.Exists(filePath))
                    {
                        fileIndex++;
                        filePath = Path.Combine(directoryPath, $"{baseFileName}{fileIndex}{fileExtension}");
                    }
                }
                var fileInfo = new FileInfo(filePath);
                using (var fs = fileInfo.Create())
                {
                    // Just create and close the file
                }
                return fileInfo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating file: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Attempts to create a new directory with a unique name in the specified directory path.
        /// Will append an index to "New Folder" if a folder with that name already exists.
        /// eg. "New Folder", "New Folder1", "New Folder2", etc.
        /// </summary>
        public static DirectoryInfo? CreateDirectory(this string directoryPath, string newFolderName)
        {
            try
            {
                int? folderIndex = null;
                var folderPath = Path.Combine(directoryPath, $"New Folder{folderIndex}");
                if (Directory.Exists(folderPath))
                {
                    folderIndex = 1;
                    folderPath = Path.Combine(directoryPath, $"New Folder{folderIndex}");
                    while (Directory.Exists(folderPath))
                    {
                        folderIndex++;
                    }
                }
                var dirInfo = Directory.CreateDirectory(folderPath);
                return dirInfo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating directory: {ex.Message}");
                return null;
            }
        }

        public static string? GetFileNameWithoutExtension(this string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return null;
            var fileName = System.IO.Path.GetFileName(filePath);
            return System.IO.Path.GetFileNameWithoutExtension(fileName);
        }

        public static string? GetDirectoryName(this string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return null;
            return System.IO.Path.GetDirectoryName(filePath) ?? string.Empty;
        }

        public static string? GetFileExtension(this string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return null;
            return System.IO.Path.GetExtension(filePath);
        }
    }
}
