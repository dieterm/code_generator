using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core.Templates
{
    public interface ITemplateInstance
    {
        ITemplate Template { get; }

    }
}
