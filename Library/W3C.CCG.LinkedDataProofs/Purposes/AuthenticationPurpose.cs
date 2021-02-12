using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace W3C.CCG.LinkedDataProofs.Purposes
{
    public class AuthenticationPurpose : ProofPurpose
    {
        public AuthenticationPurpose(string challenge) : base("authentication")
        {
            Challenge = challenge ?? throw new ArgumentNullException(nameof(challenge), "Challenge is required.");
        }

        public string Challenge { get; set; }

        public string Domain { get; set; }

        public override Task<ValidationResult> ValidateAsync(JToken proof, ProofOptions options)
        {
            if (proof["challenge"]?.ToString() != Challenge)
            {
                throw new Exception("The challenge is not as expected;" +
                    $"challenge = '{proof["challenge"]}', expected = '{Challenge}'");
            }

            if (Domain != null && proof["domain"]?.ToString() != Domain)
            {
                throw new Exception("The domain is not as expected;" +
                    $"domain = '{proof["domain"]}', expected = '{Domain}'");
            }

            return base.ValidateAsync(proof, options);
        }

        public override JObject Update(JObject proof)
        {
            proof = base.Update(proof);

            proof["challenge"] = Challenge ?? throw new Exception("'challenge' must be specified.");
            if (Domain != null)
            {
                proof["domain"] = Domain;
            }

            return proof;
        }
    }
}
