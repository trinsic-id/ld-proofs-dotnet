using System;
using System.Threading.Tasks;
using W3C.CCG.DidCore;
using W3C.CCG.LinkedDataProofs;
using W3C.CCG.LinkedDataProofs.Purposes;
using W3C.CCG.LinkedDataProofs.Suites;
using Xunit;
using W3C.CCG.SecurityVocabulary;

namespace W3cCcg.LdProofs.Tests
{
    public class Ed25519SuiteTests
    {
        [Fact(DisplayName = "Create proof from random key")]
        public async Task CreateProofRandomKey()
        {
            var key = Ed25519VerificationKey2018.Generate();

            var document = new DidDocument { Id = "did:example:alice" };

            var signedDocument = await LdSignatures.SignAsync(
                document,
                new SignatureOptions
                {
                    Suite = new Ed25519Signature2018
                    {
                        Signer = key,
                        VerificationMethod = "did:example:alice#keys-1"
                    },
                    Purpose = new AssertionProofPurpose(),
                    DocumentLoader = CachingDocumentLoader.Default
                });

            Assert.NotNull(document);
            Assert.NotNull(document["proof"]);
        }
    }
}
