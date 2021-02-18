using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using LinkedDataProofs.Purposes;

namespace LinkedDataProofs
{
    public abstract class LinkedDataProof
    {
        public LinkedDataProof(string typeName)
        {
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName), "Type name must be specified.");
        }

        public string TypeName { get; }

        public abstract Task<ProofResult> CreateProofAsync(ProofOptions options);

        public abstract Task<ValidationResult> VerifyProofAsync(JToken proof, ProofOptions options);

        public virtual Task<bool> MatchProofAsync(MatchProofOptions options)
        {
            return Task.FromResult(options.TypeName == TypeName);
        }
    }

    public class ProofResult
    {
        public JToken Proof { get; set; }

        public JObject UpdatedDocument { get; set; }
    }
}
