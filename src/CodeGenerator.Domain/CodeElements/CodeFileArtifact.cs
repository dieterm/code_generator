using CodeGenerator.Core.Artifacts;
using CodeGenerator.Core.Artifacts.FileSystem;
using CodeGenerator.Core.Artifacts.TreeNode;
using CodeGenerator.Domain.ProgrammingLanguages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeElements
{
    public class CodeFileArtifact : FileArtifact
    {
        public CodeFileArtifact(string fileTitle, ProgrammingLanguage programmingLanguage)
            : base(fileTitle)
        {
            CodeFile = new CodeFileElement(fileTitle, programmingLanguage);
            FileName = CodeFile.FullFileName;
            AddDecorator(new TextContentDecorator(TEXT_CONTENT_DECORATOR_KEY)).Generating += CodeFileArtifact_Generating;
        }


        private void CodeFileArtifact_Generating(object? sender, EventArgs e)
        {
            var programmingLanguage = ProgrammingLanguages.ProgrammingLanguages.FindByFileExtension(CodeFile.FileExtension);
            if (programmingLanguage == null)
                throw new InvalidOperationException($"No programming language found for file extension '{CodeFile.FileExtension}'.");
            var generator = ProgrammingLanguageCodeGenerators.GetGenerator(programmingLanguage);
            if (generator == null)
                throw new InvalidOperationException($"No code generator registered for programming language '{programmingLanguage.Name}' (ID: {programmingLanguage.Id}).");
            var code = generator.GenerateCodeElement(CodeFile);
            SetTextContent(code);
        }

        public CodeFileArtifact(ArtifactState state) : base(state)
        {
            throw new NotSupportedException("TODO: Deserialization constructor is not implemented for CodeFileArtifact. Use the default constructor instead.");
        }

        public CodeFileElement CodeFile
        {
            get { return GetValue<CodeFileElement>(nameof(CodeFile))!; }
            private set
            {
                SetValue<CodeFileElement>(nameof(CodeFile), value);
            }
        }

        public override string FileName { 
            get {
                // Ensure the base FileName is in sync with CodeFile.FullFileName
                if (CodeFile.FullFileName!= base.FileName)
                    base.FileName = CodeFile.FullFileName;
                return base.FileName;
            }
            set { 
                base.FileName = value;
                if (CodeFile != null) { 
                    // Update CodeFile properties based on the new filename
                    CodeFile.FileName = System.IO.Path.GetFileNameWithoutExtension(value);
                }
            }
        }
    }
}
