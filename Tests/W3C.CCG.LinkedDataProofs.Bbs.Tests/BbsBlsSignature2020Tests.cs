using BbsDataSignatures;
using BbsSignatures;
using LinkedDataProofs.Bbs.Tests;
using Xunit;
using FluentAssertions;
using W3C.CCG.LinkedDataProofs;
using W3C.CCG.LinkedDataProofs.Purposes;
using System.Threading.Tasks;
using System;

namespace LindedDataProofs.Bbs
{
    [Collection(ServiceFixture.CollectionDefinitionName)]
    public class BbsBlsSignature2020_Tests
    {
        public BbsBlsSignature2020_Tests(ServiceFixture serviceFixture)
        {
            BbsProvider = new BbsSignatureService();
        }

        public BbsSignatureService BbsProvider { get; }

        [Fact(DisplayName = "Sign document")]
        public async Task SignDocument()
        {
            var keyPair = BlsKeyPair.GenerateG2();
            var document = Utilities.LoadJson("Data/test_document.json");

            var signedDocument = await LdSignatures.SignAsync(document, new ProofOptions
            {
                Suite = new BbsBlsSignature2020
                {
                    KeyPair = keyPair,
                    VerificationMethod = "did:example:alice#key-1"
                },
                Purpose = new AssertionMethodPurpose()
            });

            signedDocument.Should().NotBeNull();
            signedDocument["proof"].Should().NotBeNull();
            signedDocument["proof"]["proofValue"].Should().NotBeNull();
        }

        [Fact(DisplayName = "Sign verifiable credential")]
        public async Task SignVerifiableCredential()
        {
            var keyPair = BlsKeyPair.GenerateG2();
            var document = Utilities.LoadJson("Data/test_vc.json");

            var signedDocument = await LdSignatures.SignAsync(document, new ProofOptions
            {
                Suite = new BbsBlsSignature2020
                {
                    KeyPair = keyPair,
                    VerificationMethod = "did:example:alice#key-1"
                },
                Purpose = new AssertionMethodPurpose()
            });

            signedDocument.Should().NotBeNull();
            signedDocument["proof"].Should().NotBeNull();
            signedDocument["proof"]["proofValue"].Should().NotBeNull();
        }

        [Fact(DisplayName = "Verify signed document")]
        public async Task VerifySignedDocument()
        {
            var document = Utilities.LoadJson("Data/test_signed_document.json");

            var result = await LdSignatures.VerifyAsync(document, new ProofOptions
            {
                Suite = new BbsBlsSignature2020(),
                Purpose = new AssertionMethodPurpose()
            });

            result.Should().NotBeNull();
        }

        [Fact(DisplayName = "Verify verifiable credentials")]
        public async Task VerifySignedVerifiableCredential()
        {
            var document = Utilities.LoadJson("Data/test_signed_vc.json");

            var result = await LdSignatures.VerifyAsync(document, new ProofOptions
            {
                Suite = new BbsBlsSignature2020(),
                Purpose = new AssertionMethodPurpose()
            });

            result.Should().NotBeNull();
        }

        [Fact(DisplayName = "Should not verify bad signature")]
        public async Task ShouldNotVerifyBadDocument()
        {
            var document = Utilities.LoadJson("Data/test_bad_signed_document.json");

            var actual = LdSignatures.VerifyAsync(document, new ProofOptions
            {
                Suite = new BbsBlsSignature2020(),
                Purpose = new AssertionMethodPurpose()
            });

            await Assert.ThrowsAsync<BbsException>(() => actual);
        }

        [Fact(DisplayName = "Should not verify with additional unsigned information")]
        public async Task ShouldNotVerifyAdditionalUnsignedStatement()
        {
            var document = Utilities.LoadJson("Data/test_signed_document.json");
            document["unsignedClaim"] = "oops";

            var actual = LdSignatures.VerifyAsync(document, new ProofOptions
            {
                Suite = new BbsBlsSignature2020(),
                Purpose = new AssertionMethodPurpose()
            });

            await Assert.ThrowsAsync<Exception>(() => actual);
        }

        [Fact(DisplayName = "Should not verify with modified information")]
        public async Task ShouldNotVerifyModifiedSignedDocument()
        {
            var document = Utilities.LoadJson("Data/test_signed_document.json");
            document["email"] = "someOtherEmail@example.com";

            var actual = LdSignatures.VerifyAsync(document, new ProofOptions
            {
                Suite = new BbsBlsSignature2020(),
                Purpose = new AssertionMethodPurpose()
            });

            await Assert.ThrowsAsync<Exception>(() => actual);
        }
    }
}
