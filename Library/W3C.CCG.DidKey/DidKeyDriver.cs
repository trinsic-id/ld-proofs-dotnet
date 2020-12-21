using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using W3C.CCG.DidCore;

namespace W3C.CCG.DidKey
{
    public class DidKeyDriver : IDidDriver
    {
        private Regex regex = new Regex("^did:key:");

        /// <summary>
        /// Determines whether this instance can resolve the specified did URI.
        /// </summary>
        /// <param name="didUri">The did URI.</param>
        /// <returns>
        ///   <c>true</c> if this instance can resolve the specified did URI; otherwise, <c>false</c>.
        /// </returns>
        public bool CanResolve(string didUri) => regex.IsMatch(didUri);

        /// <summary>
        /// Determines whether this instance can resolve the specified did URI.
        /// </summary>
        /// <param name="didUri">The did URI.</param>
        /// <returns>
        ///   <c>true</c> if this instance can resolve the specified did URI; otherwise, <c>false</c>.
        /// </returns>
        public bool CanResolve(Uri didUri) => CanResolve(didUri.ToString());

        public Task<DidDocument> Resolve(Uri didUri)
        {
            throw new NotImplementedException();
        }

        public Task<DidDocument> ResolveAsync(Uri didUri)
        {
            throw new NotImplementedException();
        }
    }
}
