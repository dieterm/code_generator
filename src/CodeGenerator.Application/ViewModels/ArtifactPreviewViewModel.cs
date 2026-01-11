using CodeGenerator.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Application.ViewModels
{
    public class ArtifactPreviewViewModel : ViewModelBase
    {
        private string? _textContent;
        public string? TextContent
        {
            get => _textContent;
            set => SetProperty(ref _textContent, value);
        }

        private KnownLanguages _textLanguageSchema;
        public KnownLanguages TextLanguageSchema
        {
            get => _textLanguageSchema;
            set => SetProperty(ref _textLanguageSchema, value);
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
