using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public class CodeArchitectureManager
    {
        public CodeArchitecture OnionArchitecture { get; }
        public CodeArchitecture HexagonArchitecture { get; }
        public CodeArchitecture CleanArchitecture { get; }
        public CodeArchitectureManager() { 
            OnionArchitecture = CreateOnionArchitecture();
            HexagonArchitecture = CreateHexagonArchitecture();
            CleanArchitecture = CreateCleanArchitecture();
        }

        public IEnumerable<CodeArchitecture> GetAllArchitectures()
        {
            return new List<CodeArchitecture>
            {
                OnionArchitecture,
                HexagonArchitecture,
                CleanArchitecture
            };
        }

        public CodeArchitecture CreateOnionArchitecture()
        {
            // Implementation for creating an Onion Architecture
            return new CodeArchitecture(
                id: "onion",
                name: "Onion Architecture",
                layers: new List<ICodeArchitectureLayerFactory>
                {
                    new ApplicationLayerFactory(),
                    new DomainLayerFactory(),
                    new InfrastructureLayerFactory(),
                    new PresentationLayerFactory()
                }
            );
        }

        public CodeArchitecture CreateHexagonArchitecture()
        {
            // Implementation for creating a Hexagonal Architecture
            return new CodeArchitecture(
                id: "hexagon",
                name: "Hexagonal Architecture",
                layers: new List<ICodeArchitectureLayerFactory>
                {
                    new ApplicationLayerFactory(),
                    new DomainLayerFactory(),
                    new InfrastructureLayerFactory()
                }
            );
        }

        // CleanArchitecture can be added similarly
        public CodeArchitecture CreateCleanArchitecture()
        {
            // Implementation for creating a Clean Architecture
            return new CodeArchitecture(
                id: "clean",
                name: "Clean Architecture",
                layers: new List<ICodeArchitectureLayerFactory>
                {
                    new ApplicationLayerFactory(),
                    new DomainLayerFactory(),
                    new InfrastructureLayerFactory(),
                    new PresentationLayerFactory()
                }
            );
        }

        public CodeArchitecture? GetById(string defaultCodeArchitectureId)
        {
            return GetAllArchitectures().Single(a => a.Id.Equals(defaultCodeArchitectureId, StringComparison.OrdinalIgnoreCase));
        }
    }
}
