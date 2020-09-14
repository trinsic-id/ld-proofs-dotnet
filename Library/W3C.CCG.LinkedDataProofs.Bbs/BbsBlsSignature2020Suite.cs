using Hyperledger.Ursa.BbsSignatures;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF.JsonLd;
using W3C.CCG.DidCore;
using W3C.CCG.LinkedDataProofs;
using W3C.CCG.SecurityVocabulary;

namespace BbsDataSignatures
{
    public class BbsBlsSignature2020Suite : ILinkedDataSuite
    {
        public BbsBlsSignature2020Suite()
        {
            BbsProvider = new BbsSignatureService();
        }

        public IEnumerable<string> SupportedProofTypes => new[] { BbsBlsSignature2020.Name };

        public BbsSignatureService BbsProvider { get; }

        public JToken CreateProof(CreateProofOptions options, JsonLdProcessorOptions processorOptions)
        {
            if (!(options.VerificationMethod is Bls12381VerificationKey2020 verificationMethod))
            {
                throw new Exception(
                    $"Invalid verification method. " +
                    $"Expected '{nameof(Bls12381VerificationKey2020)}'. " +
                    $"Found '{options.VerificationMethod?.GetType().Name}'.");
            }

            // Prepare proof
            var compactedProof = JsonLdProcessor.Compact(
                input: new BbsBlsSignature2020
                {
                    Context = Constants.SECURITY_CONTEXT_V2_URL,
                    TypeName = "https://w3c-ccg.github.io/ldp-bbs2020/context/v1#BbsBlsSignature2020"
                },
                context: Constants.SECURITY_CONTEXT_V2_URL,
                options: processorOptions);

            var proof = new BbsBlsSignature2020(compactedProof)
            {
                Context = Constants.SECURITY_CONTEXT_V2_URL,
                VerificationMethod = options.VerificationMethod switch
                {
                    VerificationMethodReference reference => (string)reference,
                    VerificationMethod method => method.Id,
                    _ => throw new Exception("Unknown VerificationMethod type")
                },
                ProofPurpose = options.ProofPurpose,
                Created = options.Created ?? DateTimeOffset.Now
            };

            var canonizedProof = Helpers.ToRdf(proof, processorOptions);
            proof.Remove("@context");

            // Prepare document
            var canonizedDocument = Helpers.ToRdf(options.Document, processorOptions);

            var signature = BbsProvider.Sign(new SignRequest(
                keyPair: verificationMethod.ToBlsKeyPair(),
                messages: canonizedProof.Concat(canonizedDocument).ToArray()));

            proof["proofValue"] = Convert.ToBase64String(signature);
            return proof;
        }

        public Task<JToken> CreateProofAsync(CreateProofOptions options, JsonLdProcessorOptions processorOptions) => Task.FromResult(CreateProof(options, processorOptions));

        public bool VerifyProof(VerifyProofOptions options, JsonLdProcessorOptions processorOptions)
        {
            var verifyData = CreateVerifyData(options.Proof, options.Document, processorOptions);

            var verificationMethod = GetVerificationMethod(options.Proof, processorOptions);

            var signature = Convert.FromBase64String(options.Proof["proofValue"]?.Value<string>() ?? throw new Exception("Required property 'proofValue' was not found"));

            try
            {
                return BbsProvider.Verify(new VerifyRequest(verificationMethod.ToBlsKeyPair(), signature, verifyData.ToArray()));
            }
            catch
            {
                // TODO: Add logging

                return false;
            }
        }

        public Task<bool> VerifyProofAsync(VerifyProofOptions options, JsonLdProcessorOptions processorOptions) => Task.FromResult(VerifyProof(options, processorOptions));

        internal static IEnumerable<string> CreateVerifyData(JToken proof, JToken document, JsonLdProcessorOptions options)
        {
            var proofStatements = CreateVerifyProofData(proof, options);
            var documentStatement = CreateVerifyDocumentData(document, options);

            return proofStatements.Concat(documentStatement);
        }

        internal static IEnumerable<string> CreateVerifyProofData(JToken proof, JsonLdProcessorOptions options)
        {
            proof = proof.DeepClone();
            proof.Remove("proofValue");
            proof.Remove("nonce");

            return Helpers.Canonize(proof, options);
        }

        internal static IEnumerable<string> CreateVerifyDocumentData(JToken document, JsonLdProcessorOptions options)
        {
            return Helpers.Canonize(document, options);
        }

        internal static Bls12381VerificationKey2020 GetVerificationMethod(JToken proof, JsonLdProcessorOptions options)
        {
            if (proof["verificationMethod"] == null) throw new Exception("Verification method is required");

            var verificationMethod = proof["verificationMethod"].Type switch
            {
                JTokenType.Object => proof["verificationMethod"]["id"],
                JTokenType.String => proof["verificationMethod"],
                _ => throw new Exception("Unexpected verification method type")
            };

            var opts = options.Clone();
            opts.CompactToRelative = false;
            opts.ExpandContext = Constants.SECURITY_CONTEXT_V2_URL;

            var result = JsonLdProcessor.Frame(
                input: verificationMethod.ToString(),
                frame: new JObject
                {
                    { "@context", Constants.SECURITY_CONTEXT_V2_URL },
                    { "@embed", "@always" },
                    { "id", verificationMethod.ToString() }
                },
                options: opts);

            return new Bls12381VerificationKey2020(result);
        }

        /// <summary>
        /// Determines whether this instance [can create proof] the specified proof type.
        /// </summary>
        /// <param name="proofType">Type of the proof.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can create proof] the specified proof type; otherwise, <c>false</c>.
        /// </returns>
        public bool CanCreateProof(string proofType) => proofType == BbsBlsSignature2020.Name;

        /// <summary>
        /// Determines whether this instance [can verify proof] the specified proof type.
        /// </summary>
        /// <param name="proofType">Type of the proof.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can verify proof] the specified proof type; otherwise, <c>false</c>.
        /// </returns>
        public bool CanVerifyProof(string proofType) => proofType == BbsBlsSignature2020.Name;
    }
}
