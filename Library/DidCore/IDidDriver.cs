using System;
using System.Threading.Tasks;

namespace W3C.CCG.DidCore
{
    public interface IDidDriver
    {
        bool CanResolve(string didUri);
        bool CanResolve(Uri didUri);

        Task<DidDocument> ResolveAsync(Uri didUri);
        Task<DidDocument> Resolve(Uri didUri);
    }
}
