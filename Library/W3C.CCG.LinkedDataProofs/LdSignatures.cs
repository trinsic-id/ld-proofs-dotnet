using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;
using W3C.CCG.LinkedDataProofs.Purposes;
using W3C.CCG.SecurityVocabulary;

namespace W3C.CCG.LinkedDataProofs
{
    public class LdSignatures
    {
        public static async Task<JToken> SignAsync(JToken document, SignatureOptions options)
        {
            if (options.Purpose is null) throw new Exception("Proof purpose is required.");
            if (options.Suite is null) throw new Exception("Suite type is required.");

            var processorOptions = new JsonLdProcessorOptions
            {
                CompactToRelative = false,
                DocumentLoader = options.DocumentLoader == null
                    ? CachingDocumentLoader.Default.Load
                    : options.DocumentLoader.Load
            };

            var input = options.CompactProof
                ? JsonLdProcessor.Compact(document, Constants.SECURITY_CONTEXT_V2_URL, processorOptions)
                : document.DeepClone();
            input.Remove("proof");

            // create the new proof (suites MUST output a proof using the security-v2
            // `@context`)
            var proof = await options.Suite.CreateProofAsync(new CreateProofOptions
            {
                Input = input as JObject,
                Purpose = options.Purpose,
                Suite = options.Suite,
                CompactProof = options.CompactProof,
                AdditonalData = options.AdditonalData
            });

            // TODO: Check compaction again
            proof.Remove("@context");

            document["proof"] = proof;
            return document;
        }

        public static Task<VerifyProofResult> VerifyAsync(JToken document, SignatureOptions options)
        {
            return Task.FromResult(new VerifyProofResult());
        }
    }

    public class SignatureOptions
    {
        public LinkedDataProof Suite { get; set; }

        public ProofPurpose Purpose { get; set; }

        public bool CompactProof { get; set; } = true;

        public IDocumentLoader DocumentLoader { get; set; }

        public IDictionary<string, JToken> AdditonalData { get; set; }
    }
}
