using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Templates
{
    public enum TemplateType
    {
        Scriban,
        T4,
        TextFile,
        BinaryFile,
        ImageFile,
        Folder,
        DotNetProject
    }
}
