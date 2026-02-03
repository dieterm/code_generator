using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DotNet
{
    public class EmbeddedResource
    {
        public EmbeddedResource(string fileName)
        {
            FileName = fileName;
        }
        /// <summary>
        /// filename & path within the project
        /// </summary>
        public string FileName { get; set; }
        public EmbeddedResourceBuildAction BuildAction { get; set; } = EmbeddedResourceBuildAction.EmbeddedResource;
        public EmbeddedResourceAccessModifier AccessModifier { get; set; } = EmbeddedResourceAccessModifier.Public;
    }

    public enum EmbeddedResourceBuildAction
    {
        EmbeddedResource,
        None,
        Compile,
        Content,
        Resource
    }

    public enum EmbeddedResourceAccessModifier
    {
        Public,
        Internal,
        Private
    }
}
