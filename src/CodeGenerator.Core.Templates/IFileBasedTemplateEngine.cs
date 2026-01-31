using CodeGenerator.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Templates
{
    public interface IFileBasedTemplateEngine : ITemplateEngine
    {
        string DefaultFileExtension { get; }
        string[] SupportedFileExtensions { get; }

        /// <summary>
        /// This method checks whether the given fileOrFolderName ends with one of the supported file extensions.<br />
        /// It does not check if the file or folder actually exists.<br />
        /// These checks need to be done by the derived classes if needed.
        /// </summary>
        bool SupportsTemplatePath(string fileOrFolderName);
        /// <summary>
        /// File extensions supported by this template engine (without dot) eg. "scriban", "tt"<br />
        /// Used to identify template files for this engine.
        /// For folder-based templates, should throw exception or return false.
        /// </summary>
        bool SupportsTemplateFileExtension(string fileExtension);
        ITemplate CreateTemplateFromFile(string filePath);
    }
}
