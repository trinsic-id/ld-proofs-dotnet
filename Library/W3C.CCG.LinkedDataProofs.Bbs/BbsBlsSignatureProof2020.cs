using BbsSignatures;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.JsonLd;
using W3C.CCG.LinkedDataProofs;
using W3C.CCG.LinkedDataProofs.Purposes;
using W3C.CCG.SecurityVocabulary;

namespace BbsDataSignatures
{
    public class BbsBlsSignatureProof2020 : LinkedDataSignature
    {
        public const string Name = "sec:BbsBlsSignatureProof2020";

        public BbsBlsSignatureProof2020() : base(Name)
        {
            Service = new BbsSignatureService();
        }

        private BbsSignatureService Service { get; }
        public JToken RevealDocument { get; set; }
        public BlsKeyPair KeyPair { get; set; }
        public string Nonce { get; set; }

        public override Task<JToken> CreateProofAsync(ProofOptions options)
        {
            VerificationMethod = options.AdditonalData["originalDocument"]["proof"]["verificationMethod"];

            return base.CreateProofAsync(options);
        }

        protected override IVerifyData CreateVerifyData(JObject proof, ProofOptions options)
        {
            var originalProof = options.AdditonalData["originalDocument"]["proof"] as JObject;
            var processorOptions = new JsonLdProcessorOptions
            {
                DocumentLoader = options.DocumentLoader == null ? CachingDocumentLoader.Default.Load : options.DocumentLoader.Load,
                CompactToRelative = false
            };

            var proofStatements = Helpers.CanonizeProofStatements(originalProof,processorOptions);
            var documentStatements = Helpers.CanonizeStatements(options.Input, processorOptions);

            var numberOfProofStatements = proofStatements.Count();

            var transformedInputDocumentStatements = documentStatements.Select(TransformBlankNodeToId).ToArray();
            var compactInputDocument = Helpers.FromRdf(transformedInputDocumentStatements);

            if (RevealDocument != null)
            {
                var revealDocument = JsonLdProcessor.Frame(compactInputDocument, RevealDocument, processorOptions);
                var revealDocumentStatements = Helpers.CanonizeStatements(revealDocument, processorOptions);

                var documentRevealIndicies = revealDocumentStatements.Select(x => Array.IndexOf(transformedInputDocumentStatements, x) + numberOfProofStatements).ToArray();

                if (documentRevealIndicies.Count() != revealDocumentStatements.Count())
                {
                    throw new Exception("Some statements in the reveal document not found in original proof");
                }

                var proofRevealIndicies = new int[numberOfProofStatements].Select((_, x) => x).ToArray();
                var revealIndicies = proofRevealIndicies.Concat(documentRevealIndicies);

                options.AdditonalData["revealIndicies"] = new JArray(revealIndicies.ToArray());
            }
            
            var allInputStatements = proofStatements.Concat(documentStatements);

            options.AdditonalData["allInputStatementsCount"] = allInputStatements.Count();
            options.AdditonalData["allInputStatements"] = new JArray(allInputStatements.ToArray());
            
            return (StringArray)allInputStatements.ToArray();
        }

        protected override Task<JObject> SignAsync(IVerifyData verifyData, JObject proof, ProofOptions options)
        {
            Console.WriteLine(verifyData);

            var verifyDataArray = verifyData as StringArray ?? throw new Exception("Unsupported verify data type");

            var derivedProof = new JObject { { "@context", Constants.SECURITY_CONTEXT_V3_URL } };
            var originalProof = options.AdditonalData["originalDocument"]["proof"];
            var signature = Convert.FromBase64String(originalProof["proofValue"].ToString());

            var keyPair = new Bls12381G2Key2020(GetVerificationMethod(originalProof as JObject, options));
            var bbsKey = keyPair.ToBlsKeyPair().GetBbsKey((uint)verifyDataArray.Data.Length);
            var nonce = Nonce ?? Guid.NewGuid().ToString();

            var proofMessages = GetProofMessages(
                allInputStatements: verifyDataArray.Data,
                revealIndicies: options.AdditonalData["revealIndicies"].ToObject<int[]>());

            var outputProof = Service.CreateProof(new CreateProofRequest(
                publicKey: bbsKey,
                messages: proofMessages.ToArray(),
                signature: signature,
                blindingFactor: null,
                nonce: nonce));

            // Set the proof value on the derived proof
            derivedProof["proofValue"] = Convert.ToBase64String(outputProof);
            derivedProof["nonce"] = nonce;
            derivedProof["created"] = originalProof["created"];
            derivedProof["proofPurpose"] = originalProof["proofPurpose"];
            derivedProof["verificationMethod"] = originalProof["verificationMethod"];
            derivedProof["type"] = "BbsBlsSignatureProof2020";

            return Task.FromResult(derivedProof);
        }

        protected override Task VerifyAsync(IVerifyData verifyData, JToken proof, JToken verificationMethod, ProofOptions options)
        {
            Console.WriteLine(verifyData);

            var stringArray = verifyData as StringArray ?? throw new Exception("Unsupported verify data type");

            var proofData = Helpers.FromBase64String(proof["proofValue"]?.ToString() ?? throw new Exception("Proof value not found"));
            var keyPair = new Bls12381G2Key2020(GetVerificationMethod(proof as JObject, options));
            var nonce = proof["nonce"]?.ToString() ?? throw new Exception("Nonce not found");

            var verifyResult = Service.VerifyProof(new VerifyProofRequest(
                publicKey: keyPair.ToBlsKeyPair().GetBbsKey((uint)stringArray.Data.Length),
                proof: proofData,
                messages: stringArray.Data,
                nonce: nonce));

            if (!verifyResult)
            {
                throw new Exception("Invalid signature proof");
            }
            return Task.CompletedTask;
        }

        //public bool VerifyProof(ProofOptions options, JsonLdProcessorOptions processorOptions)
        //{
        //    options.Proof["type"] = "https://w3c-ccg.github.io/ldp-bbs2020/context/v1#BbsBlsSignature2020";

        //    var documentStatements = BbsBlsSignature2020Suite.CreateVerifyDocumentData(options.Document, processorOptions);
        //    var proofStatements = BbsBlsSignature2020Suite.CreateVerifyProofData(options.Proof, processorOptions);

        //    var transformedInputDocumentStatements = documentStatements.Select(TransformIdToBlankNode).ToArray();

        //    var statementsToVerify = proofStatements.Concat(transformedInputDocumentStatements);

        //    var verificationMethod = BbsBlsSignature2020Suite.GetVerificationMethod(options.Proof, processorOptions);

        //    var proofData = Convert.FromBase64String(options.Proof["proofValue"].ToString());
        //    var nonce = options.Proof["nonce"].ToString();

        //    var verifyResult = BbsProvider.VerifyProof(new VerifyProofRequest(
        //        publicKey: verificationMethod.ToBlsKeyPair().GeyBbsKeyPair((uint)statementsToVerify.Count()),
        //        proof: proofData,
        //        messages: GetIndexedMessages(statementsToVerify.ToArray()).ToArray(),
        //        nonce: nonce));

        //    return verifyResult == SignatureProofStatus.Success;
        //}

        //public Task<bool> VerifyProofAsync(ProofOptions options, JsonLdProcessorOptions processorOptions) => Task.FromResult(VerifyProof(options, processorOptions));

        #region Private methods

        private IEnumerable<ProofMessage> GetProofMessages(string[] allInputStatements, int[] revealIndicies)
        {
            for (var i = 0; i < allInputStatements.Count(); i++)
            {
                yield return new ProofMessage
                {
                    Message = allInputStatements[i],
                    ProofType = revealIndicies.Contains(i) ? ProofMessageType.Revealed : ProofMessageType.HiddenProofSpecificBlinding
                };
            }
        }

        private string TransformBlankNodeToId(string element)
        {
            var nodeIdentifier = element.Split(" ").First();
            if (nodeIdentifier.StartsWith("_:c14n"))
            {
                return element.Replace(
                    oldValue: nodeIdentifier,
                    newValue: $"<urn:bnid:{nodeIdentifier}>");
            }
            return element;
        }

        private string TransformIdToBlankNode(string element)
        {
            var ln = "<urn:bnid:".Length;

            var nodeIdentifier = element.Split(" ").First();
            if (nodeIdentifier.StartsWith("<urn:bnid:_:c14n"))
            {
                return element.Replace(
                    oldValue: nodeIdentifier,
                    newValue: nodeIdentifier[ln..^1]);
            }
            return element;
        }

        #endregion

    }
}
