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

        /// <summary>
        /// Gets or sets the initial proof object to use.
        /// Can be null.
        /// </summary>
        public JObject Proof { get; set; }

        /// <summary>
        /// Gets or sets the verification method to use for this proof
        /// </summary>
        public JToken VerificationMethod { get; set; }

        /// <summary>
        /// Gets or sets the date to use in this proof
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Create proof
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public override async Task<JObject> CreateProofAsync(CreateProofOptions options)
        {
            if (VerificationMethod == null) throw new ArgumentNullException(nameof(VerificationMethod), "VerificationMethod must be specified.");
            if (TypeName == null) throw new ArgumentNullException(nameof(TypeName), "TypeName must be specified.");

            var proof = Proof != null
                ? JsonLdProcessor.Compact(Proof, Constants.SECURITY_CONTEXT_V2_URL, new JsonLdProcessorOptions { DocumentLoader = options.DocumentLoader.Load, CompactToRelative = false })
                : new JObject { { "@context", Constants.SECURITY_CONTEXT_V2_URL } };

            proof["type"] = TypeName;
            proof["created"] = Date.HasValue ? Date.Value.ToString("s") : DateTime.Now.ToString("s");
            proof["verificationMethod"] = VerificationMethod;

            // allow purpose to update the proof; the `proof` is in the
            // SECURITY_CONTEXT_URL `@context` -- therefore the `purpose` must
            // ensure any added fields are also represented in that same `@context`
            proof = options.Purpose.Update(proof);

            // create data to sign
            var verifyData = CreateVerifyData(proof, options);

            // sign data
            proof = await SignAsync(verifyData, proof, options);

            return proof;
        }

        protected abstract Task<JObject> SignAsync(byte[] verifyData, JObject proof, CreateProofOptions options);

        protected virtual byte[] CreateVerifyData(JObject proof, CreateProofOptions options)
        {
            var c14nProofOptions = CanonizeProof(proof, options);
            var c14nDocument = Canonize(options.Input, options);

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
