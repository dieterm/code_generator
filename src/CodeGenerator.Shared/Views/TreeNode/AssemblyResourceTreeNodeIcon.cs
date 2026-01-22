using CodeGenerator.Core.Artifacts.TreeNode;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Shared.Views.TreeNode
{
    public class AssemblyResourceTreeNodeIcon : ITreeNodeIcon
    {
        private readonly Assembly _assembly;
        private readonly string _resourceName;
        public AssemblyResourceTreeNodeIcon(Assembly assembly, string resourceName)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            _resourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            IconKey = $"{assembly.FullName}:{resourceName}";
        }
        public string IconKey { get; }

        public Image GetIcon()
        {
            using (var stream = _assembly.GetManifestResourceStream(_resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Resource '{_resourceName}' not found in assembly '{_assembly.FullName}'.");
                }
                return Image.FromStream(stream);
            }
        }
    }
}
