using System;
using System.Threading.Tasks;
using W3C.CCG.DidCore;
using LinkedDataProofs;
using LinkedDataProofs.Purposes;
using LinkedDataProofs.Suites;
using Xunit;
using W3C.CCG.SecurityVocabulary;
using Newtonsoft.Json.Linq;
using System.Linq;
using W3C.CCG.AuthorizationCapabilities;

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

        [Fact(DisplayName = "Create proof with valid key")]
        public async Task CreateProofRandomKey()
        {
            var aliceKey = new Ed25519VerificationKey2018(Mock.Alice_Keys.VerificationMethod.First() as JObject);

            var signedDocument = await LdSignatures.SignAsync(
                Mock.ExampleDoc,
                new ProofOptions
                {
                    Suite = new Ed25519Signature2018
                    {
                        Signer = aliceKey,
                        VerificationMethod = aliceKey.Id
                    },
                    Purpose = new AssertionMethodPurpose(),
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
                options: new ProofOptions
                {
                    Suite = new Ed25519Signature2018(),
                    Purpose = new CapabilityInvocation(new PurposeOptions
                    {
                        ExpectedTarget = Mock.RootCapAlpha.Id
                    }),
                    DocumentLoader = Mock.DocumentLoader
                });

            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Verify signed document for incorrect purpose throws")]
        public async Task VerifySignedDocumentIncorrectPurposeThrows()
        {
            var cap = Mock.ExampleDocAlphaInvocation;

            await Assert.ThrowsAsync<ProofValidationException>(() => LdSignatures.VerifyAsync(
                document: cap,
                options: new ProofOptions
                {
                    Suite = new Ed25519Signature2018(),
                    Purpose = new AssertionMethodPurpose(),
                    DocumentLoader = Mock.DocumentLoader
                }));
        }

        [Fact(DisplayName = "Verify proof")]
        public async Task VerifyProofRandomKey()
        {
            var signer = new Ed25519VerificationKey2018(Mock.Diana_Keys.VerificationMethod.First() as JObject);

            var document = new DidDocument { Id = signer.Controller };

            var signedDocument = await LdSignatures.SignAsync(
                document,
                new ProofOptions
                {
                    Suite = new Ed25519Signature2018
                    {
                        Signer = signer,
                        VerificationMethod = signer.Id
                    },
                    Purpose = new AssertionMethodPurpose(),
                    DocumentLoader = Mock.DocumentLoader
                });

            signedDocument["@context"] = new JArray(new[]
            {
                Constants.DID_V1_URL,
                Constants.SECURITY_CONTEXT_V2_URL
            });

            var result = await LdSignatures.VerifyAsync(signedDocument, new ProofOptions
            {
                Suite = new Ed25519Signature2018(),
                Purpose = new AssertionMethodPurpose(),
                DocumentLoader = Mock.DocumentLoader
            });

            Assert.NotNull(result);
        }
    }
}
