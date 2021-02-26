using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace W3C.CCG.DidCore
{
    public interface IDocumentProvider
    {
        bool CanResolve(string didUri);

        Task<JObject> ResolveAsync(string didUri);
    }
}
