using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;
using W3C.CCG.SecurityVocabulary;

namespace W3C.CCG.LinkedDataProofs
{
    public abstract class LinkedDataSignature : LinkedDataProof
    {
        public LinkedDataSignature(string typeName) : base(typeName)
        {
        }

        public JToken VerificationMethod { get; set; }

        public override async Task<JObject> CreateProofAsync(CreateProofOptions options)
        {
            if (VerificationMethod == null) throw new ArgumentNullException(nameof(VerificationMethod), "VerificationMethod must be specified.");
            if (TypeName == null) throw new ArgumentNullException(nameof(TypeName), "TypeName must be specified.");

            var proof = options.Proof != null
                ? JsonLdProcessor.Compact(options.Proof, Constants.SECURITY_CONTEXT_V2_URL, new JsonLdProcessorOptions())
                : new JObject { { "@context", Constants.SECURITY_CONTEXT_V2_URL } };

            proof["type"] = TypeName;
            proof["created"] = DateTime.Now.ToString("s");
            proof["verificationMethod"] = VerificationMethod;

            var verifyData = CreateVerifyData(proof, options);
            proof = await SignAsync(verifyData, proof, options);

            proof.Remove("@context");
            return proof;
        }

        protected abstract Task<JObject> SignAsync(byte[] verifyData, JObject proof, CreateProofOptions options);

        protected virtual byte[] CreateVerifyData(JObject proof, CreateProofOptions options)
        {
            var c14nProofOptions = CanonizeProof(proof, options);
            var c14nDocument = Canonize(options.Document, options);

            var sha256 = SHA256.Create();

            return sha256.ComputeHash(Encoding.UTF8.GetBytes(string.Join(string.Empty, c14nProofOptions)))
                .Concat(sha256.ComputeHash(Encoding.UTF8.GetBytes(string.Join(string.Empty, c14nDocument))))
                .ToArray();
        }

        protected virtual IEnumerable<string> Canonize(JToken document, CreateProofOptions options)
        {
            return Helpers.Canonize(document, new JsonLdProcessorOptions());
        }

        protected virtual IEnumerable<string> CanonizeProof(JObject proof, CreateProofOptions options)
        {
            proof = proof.DeepClone() as JObject;

            proof.Remove("jws");
            proof.Remove("signatureValue");
            proof.Remove("proofValue");

            return Canonize(proof, options);
        }
    }
}
