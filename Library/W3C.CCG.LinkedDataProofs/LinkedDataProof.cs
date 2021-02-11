using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using W3C.CCG.LinkedDataProofs.Purposes;

namespace W3C.CCG.LinkedDataProofs
{
    public abstract class LinkedDataProof
    {
        public LinkedDataProof(string typeName)
        {
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName), "Type name must be specified.");
        }

        public string TypeName { get; }

        public abstract Task<JToken> CreateProofAsync(ProofOptions options);

        public abstract Task<VerifyProofResult> VerifyProofAsync(JToken proof, ProofOptions options);

        public virtual Task<bool> MatchProofAsync(MatchProofOptions options)
        {
            return Task.FromResult(options.TypeName == TypeName);
        }
    }

    public class MatchProofOptions
    {
        public string TypeName { get; set; }
    }

    public class VerifyProofResult
    {

    }
}
