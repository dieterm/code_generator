using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.ViewModels
{
    public class ArtifactPreviewViewModel : ViewModelBase
    {
        private string? _tabLabel;
        public string TabLabel
        {
            get { return _tabLabel; }
            set { SetProperty(ref _tabLabel, value); }
        }

        private string? _textContent;
        public string? TextContent
        {
            get => _textContent;
            set => SetProperty(ref _textContent, value);
        }

        private string? _filePath;
        public string? FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        private string? _fileName;
        /// <summary>
        /// Proposed filename for artifact content
        /// </summary>
        public string? FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        private KnownLanguages _textLanguageSchema;
        public KnownLanguages TextLanguageSchema
        {
            get => _textLanguageSchema;
            set => SetProperty(ref _textLanguageSchema, value);
        }

        private Image? _imageContent;
        public Image? ImageContent
        {
            get => _imageContent;
            set => SetProperty(ref _imageContent, value);
        }
        public bool IsTextContent()
        {
            return (!string.IsNullOrWhiteSpace(TextContent) || !string.IsNullOrWhiteSpace(FilePath)) && ImageContent==null;
        }
        public bool IsImageContent()
        {
             return ImageContent != null && string.IsNullOrWhiteSpace(TextContent) && string.IsNullOrWhiteSpace(FilePath);
        }
        public enum KnownLanguages
        {
            //
            // Summary:
            //     This option represents the undefined language.
            Undefined,
            //
            // Summary:
            //     This option represents the plain text.
            Text,
            //
            // Summary:
            //     This option represents the C# language.
            CSharp,
            //
            // Summary:
            //     This option represents the Delphi language.
            Delphi,
            //
            // Summary:
            //     This option represents the XML language.
            XML,
            //
            // Summary:
            //     This option represents the HTML language.
            HTML,
            //
            // Summary:
            //     This option represents the VB .NET language.
            VBNET,
            //
            // Summary:
            //     This option represents the SQL language.
            SQL,
            //
            // Summary:
            //     This option represents the Java language.
            Java,
            //
            // Summary:
            //     This option represents the VBScript language.
            VBScript,
            //
            // Summary:
            //     This option represents the JScript language.
            JScript,
            //
            // Summary:
            //     This option represents the C language.
            C,
            //
            // Summary:
            //     This option represents the PowerShell language.
            PowerShell
        }
    }
}
