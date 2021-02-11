using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace W3C.CCG.LinkedDataProofs
{
    public abstract class JwsLinkedDataSignature : LinkedDataSignature
    {
        public SignerVerificationMethod Signer { get; set; }
        public abstract string Algorithm { get; }

        protected JwsLinkedDataSignature(string typeName) : base(typeName)
        {
        }

        protected override Task<JObject> SignAsync(byte[] verifyData, JObject proof, CreateProofOptions options)
        {
            // JWS header
            var header = new JObject
            {
                { "alg", Algorithm },
                { "b64", false },
                { "crit", JArray.Parse("[\"b64\"]") }
            };

            /*
            +-------+-----------------------------------------------------------+
            | "b64" | JWS Signing Input Formula                                 |
            +-------+-----------------------------------------------------------+
            | true  | ASCII(BASE64URL(UTF8(JWS Protected Header)) || '.' ||     |
            |       | BASE64URL(JWS Payload))                                   |
            |       |                                                           |
            | false | ASCII(BASE64URL(UTF8(JWS Protected Header)) || '.') ||    |
            |       | JWS Payload                                               |
            +-------+-----------------------------------------------------------+
            */

            // create JWS data and sign
            var encodedHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header)));
            var data = Encoding.ASCII.GetBytes($"{encodedHeader}.")
                .Concat(verifyData)
                .ToArray();
            var signature = Signer.Sign(data);

            // create detached content signature
            var encodedSignature = Convert.ToBase64String(signature);
            proof["jws"] = $"{encodedHeader}..{encodedSignature}";

            return Task.FromResult(proof);
        }
    }
}
