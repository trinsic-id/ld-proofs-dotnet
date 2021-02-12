using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace W3C.CCG.LinkedDataProofs.Purposes
{
    public abstract class ProofPurpose
    {
        public ProofPurpose(string term)
        {
            Term = term ?? throw new ArgumentNullException(nameof(term), "this field is required");
        }

        public PurposeOptions Options { get; set; } = new PurposeOptions();

        public string Term { get; }

        public virtual Task<ValidationResult> ValidateAsync(JToken proof, ProofOptions options)
        {
            if (proof["proofPurpose"]?.ToString() == Term)
            {
                return Task.FromResult(new ValidationResult());
            }
            throw new ProofValidationException($"Invalid proof purpose. Expected '{Term}', found '{proof["proofPurpose"]}'");
        }

        public virtual JObject Update(JObject proof)
        {
            proof["proofPurpose"] = Term;
            return proof;
        }

        public virtual bool Match(JObject proof)
        {
            return proof["proofPurpose"]?.ToString() == Term;
        }
    }

    public class ValidationResult
    {
        public string Controller { get; set; }
        public string Invoker { get; set; }
        public IEnumerable<string> CapabilityChain { get; set; }
    }
}
