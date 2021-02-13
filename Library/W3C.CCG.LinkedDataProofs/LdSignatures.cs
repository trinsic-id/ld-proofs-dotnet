using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;
using W3C.CCG.LinkedDataProofs.Purposes;
using W3C.CCG.SecurityVocabulary;

namespace W3C.CCG.LinkedDataProofs
{
    public class LdSignatures
    {
        /// <summary>
        /// Cryptographically signs the provided document by adding a `proof` section,
        /// based on the provided suite and proof purpose.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task<JToken> SignAsync(JToken document, ProofOptions options)
        {
            if (options.Purpose is null) throw new Exception("Proof purpose is required.");
            if (options.Suite is null) throw new Exception("Suite is required.");

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
            var proof = await options.Suite.CreateProofAsync(new ProofOptions
            {
                Input = input as JObject,
                Purpose = options.Purpose,
                Suite = options.Suite,
                CompactProof = options.CompactProof,
                DocumentLoader = options.DocumentLoader
            });

            // TODO: Check compaction again
            proof.Remove("@context");

            document["proof"] = proof;
            return document;
        }

        /// <summary>
        /// Verifies the linked data signature on the provided document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task<ValidationResult> VerifyAsync(JToken document, ProofOptions options)
        {
            if (options.Purpose is null) throw new Exception("Purpose is required.");
            if (options.Suite is null) throw new Exception("Suite is required.");

            // shallow copy to allow for removal of proof set prior to canonize
            var input = document.Type == JTokenType.String
                ? await options.DocumentLoader.LoadAsync(document.ToString())
                : document.DeepClone();

            var (proof, doc) = GetProof(input, options);

            var result = await options.Suite.VerifyProofAsync(proof, new ProofOptions
            {
                Suite = options.Suite,
                Purpose = options.Purpose,
                CompactProof = options.CompactProof,
                AdditonalData = options.AdditonalData,
                DocumentLoader = options.DocumentLoader,
                Input = doc
            });

            return result;
        }

        private static (JToken proof, JToken document) GetProof(JToken document, ProofOptions options)
        {
            if (options.CompactProof)
            {
                document = JsonLdProcessor.Compact(
                    input: document,
                    context: Constants.SECURITY_CONTEXT_V2_URL,
                    options: new JsonLdProcessorOptions
                    {
                        DocumentLoader = options.DocumentLoader.Load,
                        CompactToRelative = false
                    });
            }

            var proof = document["proof"].DeepClone();
            document.Remove("proof");

            if (proof == null)
            {
                throw new Exception("No matching proofs found in the given document.");
            }

            proof["@context"] = Constants.SECURITY_CONTEXT_V2_URL;

            return (proof, document);
        }
    }
}