using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace W3C.CCG.LinkedDataProofs
{
    public class LdSignatures
    {
        public static async Task<JToken> SignAsync(JToken document, SignatureOptions options)
        {
            var proof = await options.Suite.CreateProofAsync(new CreateProofOptions
            {
                Document = document,
                Purpose = options.Purpose,
                Suite = options.Suite
            });

            document["proof"] = proof;

            return document;
        }

        public static Task<VerifyProofResult> VerifyAsync(JToken document, SignatureOptions options)
        {
            return Task.FromResult(new VerifyProofResult());
        }
    }
}
