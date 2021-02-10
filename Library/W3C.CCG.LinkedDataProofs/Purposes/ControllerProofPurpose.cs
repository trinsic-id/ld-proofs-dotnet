using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using W3C.CCG.DidCore;

namespace W3C.CCG.LinkedDataProofs.Purposes
{
    public class ControllerProofPurpose : ProofPurpose
    {
        public ControllerProofPurpose(string term) : base(term)
        {
        }

        public string Controller { get; set; }

        public override async Task<ValidationResult> ValidateAsync(JToken proof, ValidateOptions options)
        {
            var result = await base.ValidateAsync(proof, options);
            if (!result.Valid)
            {
                return result;
            }

            // TODO: Use correct validation here, with JSONLD framing and document resolution
            result.Controller = Controller ?? options.VerificationMethod.Controller;
            result.Valid = proof["verificationMethod"]?.Equals(result.Controller) ?? false;

            return result;
        }
    }
}
