using BbsSignatures;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.JsonLd;
using W3C.CCG.LinkedDataProofs;
using W3C.CCG.SecurityVocabulary;

namespace BbsDataSignatures
{
    public class BbsBlsSignatureProof2020 : LinkedDataSignature
    {
        public const string Name = "https://w3c-ccg.github.io/ldp-bbs2020/contexts/v1#BbsBlsSignature2020";

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

            var transformedInputDocumentStatements = documentStatements.Select(TransformBlankNodeToId).ToArray();
            var compactInputDocument = Helpers.FromRdf(transformedInputDocumentStatements);

            var revealDocument = JsonLdProcessor.Frame(compactInputDocument, RevealDocument, processorOptions);
            var revealDocumentStatements = Helpers.CanonizeStatements(revealDocument, processorOptions);

            var numberOfProofStatements = proofStatements.Count();

            var proofRevealIndicies = new int[numberOfProofStatements].Select((_, x) => x).ToArray();
            var documentRevealIndicies = revealDocumentStatements.Select(x => Array.IndexOf(transformedInputDocumentStatements, x) + numberOfProofStatements).ToArray();

            if (documentRevealIndicies.Count() != revealDocumentStatements.Count())
            {
                throw new Exception("Some statements in the reveal document not found in original proof");
            }

            var revealIndicies = proofRevealIndicies.Concat(documentRevealIndicies);
            var allInputStatements = proofStatements.Concat(documentStatements);

            options.AdditonalData["allInputStatementsCount"] = allInputStatements.Count();
            options.AdditonalData["allInputStatements"] = new JArray(allInputStatements.ToArray());
            options.AdditonalData["revealIndicies"] = new JArray(revealIndicies.ToArray());

            return (StringArray)allInputStatements.ToArray();
        }

        protected override Task<JObject> SignAsync(IVerifyData verifyData, JObject proof, ProofOptions options)
        {
            Console.WriteLine();
            Console.WriteLine(verifyData);

            var derivedProof = new JObject { { "@context", Constants.SECURITY_CONTEXT_V2_URL } };
            var originalProof = options.AdditonalData["originalDocument"]["proof"];
            var signature = Convert.FromBase64String(originalProof["proofValue"].ToString());

            derivedProof["nonce"] = Nonce ?? Guid.NewGuid().ToString();
            var keyPair = new Bls12381G2Key2020(GetVerificationMethod(originalProof as JObject, options));
            var messageCount = options.AdditonalData["allInputStatementsCount"].Value<uint>();
            var bbsKey = keyPair.ToBlsKeyPair().GetBbsKey(messageCount);
            var nonce = derivedProof["nonce"].ToString();

            var proofMessages = GetProofMessages(options.AdditonalData["allInputStatements"].ToObject<string[]>(),
                                    options.AdditonalData["revealIndicies"].ToObject<int[]>()).ToArray();

            var outputProof = Service.CreateProof(new CreateProofRequest(
                publicKey: bbsKey,
                messages: proofMessages,
                signature: signature,
                blindingFactor: null,
                nonce: nonce));

            // Set the proof value on the derived proof
            derivedProof["proofValue"] = Convert.ToBase64String(outputProof);
            return Task.FromResult(derivedProof);
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

        private IEnumerable<IndexedMessage> GetIndexedMessages(string[] statementsToVerify)
        {
            for (var i = 0; i < statementsToVerify.Count(); i++)
            {
                yield return new IndexedMessage
                {
                    Message = statementsToVerify[i],
                    Index = (uint)i
                };
            }
        }

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

        protected override Task VerifyAsync(IVerifyData verifyData, JToken proof, JToken verificationMethod, ProofOptions options)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
