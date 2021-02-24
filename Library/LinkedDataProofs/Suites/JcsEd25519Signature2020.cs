using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Multiformats.Base;
using Newtonsoft.Json.Linq;
using Org.Webpki.JsonCanonicalizer;

namespace LinkedDataProofs.Suites
{
    public class JcsEd25519Signature2020 : LinkedDataSignature
    {
        public ISigner Signer { get; set; }

        public JcsEd25519Signature2020() : base("JcsEd25519Signature2020")
        {
        }

        protected override Task<JObject> SignAsync(IVerifyData verifyData, JObject proof, ProofOptions options)
        {
            var data = verifyData as ByteArray ?? throw new Exception("Invalid verify data type");

            var signature = Signer.Sign(data);
            proof["signatureValue"] = Multibase.Base58.Encode(signature);

            return Task.FromResult(proof);
        }

        protected override Task VerifyAsync(IVerifyData verifyData, JToken proof, JToken verificationMethod, ProofOptions options)
        {
            var data = verifyData as ByteArray ?? throw new Exception("Invalid verify data type");

            var signature = Multibase.Base58.Decode(proof["signatureValue"].ToString());
            var valid = Signer.Verify(signature, data);
            if (!valid)
            {
                throw new Exception("Invalid signature");
            }
            return Task.CompletedTask;
        }

        protected override IVerifyData CreateVerifyData(JObject proof, ProofOptions options)
        {
            var documentCopy = options.Input.DeepClone();
            var proofCopy = proof.DeepClone();

            proofCopy.Remove("signatureValue");
            documentCopy["proof"] = proofCopy;

            var canonicalizer = new JsonCanonicalizer(documentCopy.ToString())
                .GetEncodedUTF8();
            var hashed = SHA256.Create().ComputeHash(canonicalizer);

            return (ByteArray)hashed;
        }
    }
}
