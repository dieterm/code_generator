using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Domain.CodeArchitecture
{
    public class CodeArchitectureManager
    {
        public OnionCodeArchitecture OnionArchitecture { get; }
        public HexagonCodeArchitecture HexagonArchitecture { get; }
        public CleanCodeArchitecture CleanArchitecture { get; }
        public NTierCodeArchitecture NTierArchitecture { get; }

        public CodeArchitectureManager() {
            OnionArchitecture = new OnionCodeArchitecture();
            HexagonArchitecture = new HexagonCodeArchitecture();
            CleanArchitecture = new CleanCodeArchitecture();
            NTierArchitecture = new NTierCodeArchitecture();
        }

        public ICodeArchitecture? GetById(string codeArchitectureId)
        {
            return GetAllArchitectures().Single(a => a.Id.Equals(codeArchitectureId, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<ICodeArchitecture> GetAllArchitectures()
        {
            yield return OnionArchitecture;
            yield return HexagonArchitecture;
            yield return CleanArchitecture;
            yield return NTierArchitecture;
        }
    }
}
