using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.DesignPatterns.Structural.DependancyInjection
{
    public static class DependancyInjectionFrameworkManagersExtensions
    {
        // make extension methods GetDotNetFrameworkById
        public static DotNetDependancyInjectionFramework? GetDotNetFrameworkById(this DependancyInjectionFrameworkManager manager, string id)
        {
            return manager.Frameworks.OfType<DotNetDependancyInjectionFramework>().FirstOrDefault(f => f.Id == id);
        }
        public static IEnumerable<DotNetDependancyInjectionFramework> GetDotNetFrameworks(this DependancyInjectionFrameworkManager manager)
        {
            return manager.Frameworks.OfType<DotNetDependancyInjectionFramework>();
        }
    }
}
