using System.Linq;
using System.Threading.Tasks;
using LinkedDataProofs;
using LinkedDataProofs.Purposes;
using LinkedDataProofs.Suites;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LinkedDataProfss.Tests
{
    [Collection(MockDataFixture.CollectionDefinitionName)]
    public class JcsEd25519Signature2020_Tests
    {
        public JcsEd25519Signature2020_Tests(MockDataFixture mock)
        {
            Mock = mock;
        }

        public MockDataFixture Mock { get; }

        [Fact(DisplayName = "Sign document with Jcs")]
        public async Task SignDocument()
        {
            var aliceKey = new Ed25519VerificationKey2018(Mock.Alice_Keys.VerificationMethod.First() as JObject);

            var signedDocument = await LdSignatures.SignAsync(
                Mock.ExampleDoc,
                new ProofOptions
                {
                    Suite = new JcsEd25519Signature2020
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
            Assert.NotNull(signedDocument["proof"]?["signatureValue"]);
            Assert.Equal(aliceKey.Id, signedDocument["proof"]?["verificationMethod"]);
        }

        [Fact(DisplayName = "Sign and verify document with Jcs")]
        public async Task SignAndVerifyDocument()
        {
            var aliceKey = new Ed25519VerificationKey2018(Mock.Alice_Keys.VerificationMethod.First() as JObject);

            var signedDocument = await LdSignatures.SignAsync(
                Mock.ExampleDoc,
                new ProofOptions
                {
                    Suite = new JcsEd25519Signature2020
                    {
                        Signer = aliceKey,
                        VerificationMethod = aliceKey.Id
                    },
                    Purpose = new AssertionMethodPurpose(),
                    DocumentLoader = Mock.DocumentLoader
                });

            var verified = await LdSignatures.VerifyAsync(
                document: signedDocument,
                options: new ProofOptions
                {
                    Suite = new JcsEd25519Signature2020
                    {
                        Signer = aliceKey,
                        VerificationMethod = aliceKey.Id
                    },
                    Purpose = new AssertionMethodPurpose(),
                    DocumentLoader = Mock.DocumentLoader
                });

            Assert.NotNull(verified);
        }
    }
}
