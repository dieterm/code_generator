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

            this.AddDecorator(new CodeFileDecorator(nameof(CodeFileDecorator)));
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

        public override string? GetTextContent()
        {
            var programmingLanguage = ProgrammingLanguages.ProgrammingLanguages.FindByFileExtension(CodeFile.FileExtension);
            if (programmingLanguage == null)
                throw new InvalidOperationException($"No programming language found for file extension '{CodeFile.FileExtension}'.");
            var generator = ProgrammingLanguageCodeGenerators.GetGenerator(programmingLanguage);
            if (generator == null)
                throw new InvalidOperationException($"No code generator registered for programming language '{programmingLanguage.Name}' (ID: {programmingLanguage.Id}).");
            var code = generator.GenerateCodeElement(CodeFile);
            return code;
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

        public override void SetTextContent(string content)
        {
            CodeFile.RawCode = content;
            
        }

        
    }
}
