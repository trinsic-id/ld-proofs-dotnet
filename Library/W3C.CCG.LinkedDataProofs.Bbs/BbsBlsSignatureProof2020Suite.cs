//using Hyperledger.Ursa.BbsSignatures;
//using Microsoft.Extensions.DependencyInjection;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using VDS.RDF;
//using VDS.RDF.JsonLd;
//using VDS.RDF.Parsing;
//using VDS.RDF.Writing;
//using W3C.CCG.LinkedDataProofs;
//using W3C.CCG.SecurityVocabulary;

//namespace BbsDataSignatures
//{
//    public class BbsBlsSignatureProof2020Suite : ILinkedDataSuite
//    {
//        public BbsBlsSignatureProof2020Suite()
//        {
//            BbsProvider = new BbsSignatureService();
//        }

//        public IEnumerable<string> SupportedProofTypes => new[] { BbsBlsSignatureProof2020.Name };

//        public BbsSignatureService BbsProvider { get; }

//        public JToken CreateProof(ProofOptions options, JsonLdProcessorOptions processorOptions)
//        {
//            var (document, proofs) = options.Document.GetProofs(processorOptions);
//            var proof = new BbsBlsSignature2020(proofs.FirstOrDefault() ?? throw new Exception("Proof not found"));
//            proof.Context = Constants.SECURITY_CONTEXT_V2_URL;

//            var signature = Convert.FromBase64String(proof.ProofValue);
//            var derivedProof = JsonLdProcessor.Compact(new BbsBlsSignatureProof2020(), Constants.SECURITY_CONTEXT_V2_URL, processorOptions);

//            var documentStatements = BbsBlsSignature2020Suite.CreateVerifyDocumentData(document, processorOptions);
//            var proofStatements = BbsBlsSignature2020Suite.CreateVerifyProofData(proof, processorOptions);

//            var transformedInputDocumentStatements = documentStatements.Select(TransformBlankNodeToId).ToArray();

//            var compactInputDocument = Helpers.FromRdf(transformedInputDocumentStatements);

//            var revealDocument = JsonLdProcessor.Frame(compactInputDocument, options.ProofRequest, processorOptions);
//            var revealDocumentStatements = BbsBlsSignature2020Suite.CreateVerifyDocumentData(revealDocument, processorOptions);

//            var numberOfProofStatements = proofStatements.Count();

//            var proofRevealIndicies = EnumerableFromInt(numberOfProofStatements).ToArray();
//            var documentRevealIndicies = revealDocumentStatements.Select(x => Array.IndexOf(transformedInputDocumentStatements, x) + numberOfProofStatements).ToArray();

//            if (documentRevealIndicies.Count() != revealDocumentStatements.Count())
//            {
//                throw new Exception("Some statements in the reveal document not found in original proof");
//            }

//            var revealIndicies = proofRevealIndicies.Concat(documentRevealIndicies);

//            derivedProof["nonce"] = options.Nonce ?? Guid.NewGuid().ToString();

//            //Combine all the input statements that
//            //were originally signed to generate the proof
//            var allInputStatements = proofStatements.Concat(documentStatements);

//            var verificationMethod = BbsBlsSignature2020Suite.GetVerificationMethod(proofs.First(), processorOptions);

//            var outputProof = BbsProvider.CreateProof(new CreateProofRequest(
//                publicKey: verificationMethod.ToBlsKeyPair().GeyBbsKeyPair((uint)allInputStatements.Count()),
//                messages: GetProofMessages(allInputStatements.ToArray(), revealIndicies).ToArray(),
//                signature: signature,
//                blindingFactor: null,
//                nonce: derivedProof["nonce"].ToString()));

//            // Set the proof value on the derived proof
//            derivedProof["proofValue"] = Convert.ToBase64String(outputProof);

//            // Set the relevant proof elements on the derived proof from the input proof
//            derivedProof["verificationMethod"] = proof["verificationMethod"];
//            derivedProof["proofPurpose"] = proof["proofPurpose"];
//            derivedProof["created"] = proof["created"];

//            revealDocument["proof"] = derivedProof;

//            return revealDocument;
//        }

//        public Task<JToken> CreateProofAsync(ProofOptions options, JsonLdProcessorOptions processorOptions) => Task.FromResult(CreateProof(options, processorOptions));

//        public bool VerifyProof(ProofOptions options, JsonLdProcessorOptions processorOptions)
//        {
//            options.Proof["type"] = "https://w3c-ccg.github.io/ldp-bbs2020/context/v1#BbsBlsSignature2020";

//            var documentStatements = BbsBlsSignature2020Suite.CreateVerifyDocumentData(options.Document, processorOptions);
//            var proofStatements = BbsBlsSignature2020Suite.CreateVerifyProofData(options.Proof, processorOptions);

//            var transformedInputDocumentStatements = documentStatements.Select(TransformIdToBlankNode).ToArray();

//            var statementsToVerify = proofStatements.Concat(transformedInputDocumentStatements);

//            var verificationMethod = BbsBlsSignature2020Suite.GetVerificationMethod(options.Proof, processorOptions);

//            var proofData = Convert.FromBase64String(options.Proof["proofValue"].ToString());
//            var nonce = options.Proof["nonce"].ToString();

//            var verifyResult = BbsProvider.VerifyProof(new VerifyProofRequest(
//                publicKey: verificationMethod.ToBlsKeyPair().GeyBbsKeyPair((uint)statementsToVerify.Count()),
//                proof: proofData,
//                messages: GetIndexedMessages(statementsToVerify.ToArray()).ToArray(),
//                nonce: nonce));

//            return verifyResult == SignatureProofStatus.Success;
//        }

//        private IEnumerable<IndexedMessage> GetIndexedMessages(string[] statementsToVerify)
//        {
//            for (var i = 0; i < statementsToVerify.Count(); i++)
//            {
//                yield return new IndexedMessage
//                {
//                    Message = statementsToVerify[i],
//                    Index = (uint)i
//                };
//            }
//        }

//        public Task<bool> VerifyProofAsync(ProofOptions options, JsonLdProcessorOptions processorOptions) => Task.FromResult(VerifyProof(options, processorOptions));

//        #region Private methods

//        private IEnumerable<ProofMessage> GetProofMessages(string[] allInputStatements, IEnumerable<int> revealIndicies)
//        {
//            for (var i = 0; i < allInputStatements.Count(); i++)
//            {
//                yield return new ProofMessage
//                {
//                    Message = allInputStatements[i],
//                    ProofType = revealIndicies.Contains(i) ? ProofMessageType.Revealed : ProofMessageType.HiddenProofSpecificBlinding
//                };
//            }
//        }

//        private IEnumerable<int> EnumerableFromInt(int numberOfProofStatements, int startIndex = 0)
//        {
//            for (int i = 0; i < numberOfProofStatements; i++)
//            {
//                yield return i;
//            }
//        }

//        private string TransformBlankNodeToId(string element)
//        {
//            var nodeIdentifier = element.Split(" ").First();
//            if (nodeIdentifier.StartsWith("_:c14n"))
//            {
//                return element.Replace(
//                    oldValue: nodeIdentifier,
//                    newValue: $"<urn:bnid:{nodeIdentifier}>");
//            }
//            return element;
//        }

//        private string TransformIdToBlankNode(string element)
//        {
//            var ln = "<urn:bnid:".Length;

//            var nodeIdentifier = element.Split(" ").First();
//            if (nodeIdentifier.StartsWith("<urn:bnid:_:c14n"))
//            {
//                return element.Replace(
//                    oldValue: nodeIdentifier,
//                    newValue: nodeIdentifier[ln..^1]);
//            }
//            return element;
//        }

//        /// <summary>
//        /// Determines whether this instance [can create proof] the specified proof type.
//        /// </summary>
//        /// <param name="proofType">Type of the proof.</param>
//        /// <returns>
//        ///   <c>true</c> if this instance [can create proof] the specified proof type; otherwise, <c>false</c>.
//        /// </returns>
//        public bool CanCreateProof(string proofType) => proofType == BbsBlsSignatureProof2020.Name;

//        /// <summary>
//        /// Determines whether this instance [can verify proof] the specified proof type.
//        /// </summary>
//        /// <param name="proofType">Type of the proof.</param>
//        /// <returns>
//        ///   <c>true</c> if this instance [can verify proof] the specified proof type; otherwise, <c>false</c>.
//        /// </returns>
//        public bool CanVerifyProof(string proofType) => proofType == BbsBlsSignatureProof2020.Name;

//        #endregion

//    }
//}
