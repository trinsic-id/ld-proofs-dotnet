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

        public override async Task<ValidationResult> ValidateAsync(JToken proof, ProofOptions options)
        {
            var result = await base.ValidateAsync(proof, options);
            if (!result.Valid)
            {
                return result;
            }

            var controller = Controller ?? (options.AdditonalData["verificationMethod"] as JObject)["id"];
            result.Valid = proof["verificationMethod"]?.Equals(controller) ?? false;
            return result;
        }
    }
}
