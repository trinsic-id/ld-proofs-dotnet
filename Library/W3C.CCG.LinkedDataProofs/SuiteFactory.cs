using System.Collections.Generic;
using System.Linq;

namespace W3C.CCG.LinkedDataProofs
{
    public interface ISuiteFactory
    {
        ILinkedDataSuite GetSuite(string suiteType);
    }

    internal class DefaultSuiteFactory : ISuiteFactory
    {
        private readonly IEnumerable<ILinkedDataSuite> suites;

        public DefaultSuiteFactory(IEnumerable<ILinkedDataSuite> suites)
        {
            this.suites = suites;
        }

        public ILinkedDataSuite GetSuite(string suiteType)
        {
            return suites.FirstOrDefault(x => x.SupportedProofTypes.Contains(suiteType));
        }
    }
}