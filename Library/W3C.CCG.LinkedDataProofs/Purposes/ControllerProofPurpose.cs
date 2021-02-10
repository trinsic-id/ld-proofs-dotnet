using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using W3C.CCG.DidCore;

namespace W3C.CCG.LinkedDataProofs.Purposes
{
    public class ControllerProofPurpose : ProofPurpose
    {
        public ControllerProofPurpose(string term, string controller) : base(term)
        {
            Controller = controller;
        }

        public string Controller { get; }

        public override async Task<ValidationResult> ValidateAsync(JToken proof, ValidationRequest request)
        {
            var result = await base.ValidateAsync(proof, request);
            if (!result.Valid)
            {
                return result;
            }

            // TODO: Use correct validation here, with JSONLD framing and document resolution
            result.Controller = Controller ?? request.VerificationMethod.Controller;
            result.Valid = proof["verificationMethod"]?.Equals(result.Controller) ?? false;

            return result;
        }
    }
}
