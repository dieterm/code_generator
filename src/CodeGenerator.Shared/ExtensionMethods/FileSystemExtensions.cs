using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.ExtensionMethods
{
    public static class FileSystemExtensions
    {
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
