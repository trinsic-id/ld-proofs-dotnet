using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace W3C.CCG.DidCore
{
    public interface IDidDriver
    {
        bool CanResolve(string didUri);

        Task<DidDocument> ResolveAsync(string didUri);
    }

    public class DidResolver
    {
        public IList<IDidDriver> drivers = new List<IDidDriver>();

        public void Register(IDidDriver didDriver)
        {
            drivers.Add(didDriver);
        }

        public Task<DidDocument> ResolveAsync(string didUri)
        {
            foreach (var item in drivers)
            {
                if (item.CanResolve(didUri))
                {
                    return item.ResolveAsync(didUri);
                }
            }
            throw new Exception("Cannot find suitable DID driver");
        }
    }
}
