using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace W3C.CCG.LinkedDataProofs.Suites
{
    public class Ed25519Signature2018 : JwsLinkedDataSignature
    {
        public const string Name = "Ed25519Signature2018";

        public Ed25519Signature2018() : base(Name)
        {
        }

        public override string Algorithm => "EdDSA";

        public override Task<VerifyProofResult> VerifyProofAsync(JToken proof, VerifyProofOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}