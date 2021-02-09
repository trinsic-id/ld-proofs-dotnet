using System;
using System.Threading.Tasks;

namespace W3C.CCG.LinkedDataProofs.Purposes
{
    public class ProofPurpose
    {
        public ProofPurpose(string term)
        {
            Term = term ?? throw new ArgumentNullException(nameof(term), "this field is required");
        }

        public string Term { get; }

        public virtual Task ValidateAsync()
        {
            return Task.CompletedTask;
        }
    }
}
