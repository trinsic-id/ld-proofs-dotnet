using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;
using W3C.CCG.SecurityVocabulary;

namespace W3C.CCG.LinkedDataProofs
{
    public interface ILinkedDataProofService
    {
        JToken CreateProof(CreateProofOptions proofOptions);

        Task<JToken> CreateProofAsync(CreateProofOptions proofOptions);

        bool VerifyProof(VerifyProofOptions proofOptions);
    }

    internal class DefaultLinkedDataProofService : ILinkedDataProofService
    {
        private readonly ISuiteFactory suiteFactory;
        private readonly IDocumentLoader documentLoader;

        public DefaultLinkedDataProofService(ISuiteFactory suiteFactory, IDocumentLoader documentLoader)
        {
            this.suiteFactory = suiteFactory;
            this.documentLoader = documentLoader;
        }

        public JToken CreateProof(CreateProofOptions options)
        {
            if (options.VerificationMethod is null) throw new Exception("Verification method is required.");
            if (options.ProofPurpose is null) throw new Exception("Proof purpose is required.");
            if (options.LdSuiteType is null) throw new Exception("Suite type is required.");

            var suite = suiteFactory.GetSuite(options.LdSuiteType) ?? throw new Exception($"Suite not found for type '{options.LdSuiteType}'");
            var processorOptions = new JsonLdProcessorOptions
            {
                CompactToRelative = false,
                DocumentLoader = documentLoader.GetDocumentLoader()
            };

            if (options.CompactProof)
            {
                options.Document = JsonLdProcessor.Compact(
                    input: options.Document,
                    context: Constants.SECURITY_CONTEXT_V2_URL,
                    options: processorOptions);
            }

            return suite.CreateProof(options, processorOptions);
        }

        public async Task<JToken> CreateProofAsync(CreateProofOptions options)
        {
            if (options.VerificationMethod is null) throw new Exception("Verification method is required.");
            if (options.ProofPurpose is null) throw new Exception("Proof purpose is required.");
            if (options.LdSuiteType is null) throw new Exception("Suite type is required.");

            var suite = suiteFactory.GetSuite(options.LdSuiteType) ?? throw new Exception($"Suite not found for type '{options.LdSuiteType}'");
            var processorOptions = new JsonLdProcessorOptions
            {
                CompactToRelative = false,
                DocumentLoader = documentLoader.GetDocumentLoader()
            };

            var original = options.Document.DeepClone();

            if (options.CompactProof)
            {
                options.Document = JsonLdProcessor.Compact(
                    input: options.Document,
                    context: Constants.SECURITY_CONTEXT_V2_URL,
                    options: processorOptions);
            }

            original["proof"] = await suite.CreateProofAsync(options, processorOptions);

            return original;
        }

        public bool VerifyProof(VerifyProofOptions proofOptions)
        {
            var suite = suiteFactory.GetSuite(proofOptions.LdSuiteType) ?? throw new Exception($"Suite not found for type '{proofOptions.LdSuiteType}'");

            var processorOptions = new JsonLdProcessorOptions
            {
                CompactToRelative = false,
                DocumentLoader = documentLoader.GetDocumentLoader()
            };

            if (proofOptions.CompactProof)
            {
                proofOptions.Document = JsonLdProcessor.Compact(
                    input: proofOptions.Document,
                    context: Constants.SECURITY_CONTEXT_V2_URL,
                    options: processorOptions);
            }

            var proof = (JObject)proofOptions.Document["proof"].DeepClone();
            proof["@context"] = Constants.SECURITY_CONTEXT_V2_URL;

            proofOptions.Document.Remove("proof");
            proofOptions.Proof = proof;

            return suite.VerifyProof(proofOptions, processorOptions);
        }
    }
}
