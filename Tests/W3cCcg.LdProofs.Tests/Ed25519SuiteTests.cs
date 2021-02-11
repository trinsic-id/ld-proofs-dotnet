using System;
using System.Threading.Tasks;
using W3C.CCG.DidCore;
using W3C.CCG.LinkedDataProofs;
using W3C.CCG.LinkedDataProofs.Purposes;
using W3C.CCG.LinkedDataProofs.Suites;
using Xunit;
using W3C.CCG.SecurityVocabulary;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace W3cCcg.LdProofs.Tests
{
    [Collection(MockDataFixture.CollectionDefinitionName)]
    public class Ed25519SuiteTests
    {
        public Ed25519SuiteTests(MockDataFixture mock)
        {
            Mock = mock;
        }

        public MockDataFixture Mock { get; }

        [Fact(DisplayName = "Create proof valid key")]
        public async Task CreateProofRandomKey()
        {
            var aliceKey = new Ed25519VerificationKey2018(Mock.Alice_Keys.VerificationMethod.First() as JObject);

            var signedDocument = await LdSignatures.SignAsync(
                Mock.ExampleDoc,
                new SignatureOptions
                {
                    Suite = new Ed25519Signature2018
                    {
                        Signer = aliceKey,
                        VerificationMethod = aliceKey.Id
                    },
                    Purpose = new AssertionProofPurpose(),
                    DocumentLoader = Mock.DocumentLoader
                });

            Assert.NotNull(signedDocument);
            Assert.NotNull(signedDocument["proof"]);
            Assert.Equal("assertionMethod", signedDocument["proof"]?["proofPurpose"]);
            Assert.NotNull(signedDocument["proof"]?["jws"]);
            Assert.Equal(aliceKey.Id, signedDocument["proof"]?["verificationMethod"]);
        }

        [Fact(DisplayName = "Verify signed invocation capability")]
        public async Task VerifySignedInvocationCap()
        {
            var cap = Mock.ExampleDocAlphaInvocation;

            var result = await LdSignatures.VerifyAsync(
                document: cap,
                options: new SignatureOptions
                {
                    Suite = new Ed25519Signature2018(),
                    Purpose = new AssertionProofPurpose(),
                    DocumentLoader = Mock.DocumentLoader
                });

            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Verify proof")]
        public async Task VerifyProofRandomKey()
        {
            var didDoc = new DidDocument(Utilities.LoadJson("TestData/ed25519-alice-keys.json"));
            var key = didDoc.VerificationMethod.First() as VerificationMethod;
            var signer = new Ed25519VerificationKey2018(key);

            var documentLoader = CachingDocumentLoader.Default;
            documentLoader.AddCached(key.Id, didDoc);

            var document = new DidDocument { Id = key.Controller };

            var signedDocument = await LdSignatures.SignAsync(
                document,
                new SignatureOptions
                {
                    Suite = new Ed25519Signature2018
                    {
                        Signer = signer,
                        VerificationMethod = key.Id
                    },
                    Purpose = new AssertionProofPurpose(),
                    DocumentLoader = documentLoader
                });

            signedDocument["@context"] = new JArray(new[] { Constants.DID_V1_URL, Constants.SECURITY_CONTEXT_V2_URL });

            var result = await LdSignatures.VerifyAsync(signedDocument, new SignatureOptions
            {
                Suite = new Ed25519Signature2018
                {
                    Signer = signer,
                    VerificationMethod = "did:example:alice#keys-1"
                },
                Purpose = new AssertionProofPurpose(),
                DocumentLoader = documentLoader
            });

            Assert.NotNull(document);
            Assert.NotNull(document["proof"]);
        }
    }
}
