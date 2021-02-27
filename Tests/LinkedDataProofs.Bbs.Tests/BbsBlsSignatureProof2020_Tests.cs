using System;
using System.Threading.Tasks;
using BbsSignatures;
using FluentAssertions;
using LinkedDataProofs.Purposes;
using Xunit;
using W3C.CCG.SecurityVocabulary;

namespace LinkedDataProofs.Bbs.Tests
{
    [Collection(ServiceFixture.CollectionDefinitionName)]
    public class BbsBlsSignatureProof2020_Tests
    {
        public BbsBlsSignatureProof2020_Tests(ServiceFixture serviceFixture)
        {
            BbsProvider = new BbsSignatureService();
        }

        public IServiceProvider Provider { get; }
        public BbsSignatureService BbsProvider { get; }

        [Fact(DisplayName = "Sign, derive and proof from vc revealing some")]
        public async Task SignDocumentRevealSome()
        {
            var revealDocument = Utilities.LoadJson("Data/test_vc_reveal_document.json");
            var unsignedDocument = Utilities.LoadJson("Data/test_vc.json");

            var verificationMethod = new Bls12381G2Key2020(Utilities.LoadJson("Data/did_example_489398593_test.json"));

            var signedDocument = await LdSignatures.SignAsync(unsignedDocument, new ProofOptions
            {
                Suite = new BbsBlsSignature2020
                {
                    KeyPair = verificationMethod.ToBlsKeyPair(),
                    VerificationMethod = "did:example:489398593#test"
                },
                Purpose = new AssertionMethodPurpose()
            });

            var derivedDocument = await LdSignatures.SignAsync(signedDocument, new ProofOptions
            {
                Suite = new BbsBlsSignatureProof2020
                {
                    RevealDocument = revealDocument
                },
                Purpose = new AssertionMethodPurpose()
            });

            derivedDocument.Should().NotBeNull();
            derivedDocument["proof"].Should().NotBeNull();
            derivedDocument["proof"]["proofValue"].Should().NotBeNull();

            var verifyDerived = await LdSignatures.VerifyAsync(derivedDocument, new ProofOptions
            {
                Suite = new BbsBlsSignatureProof2020(),
                Purpose = new AssertionMethodPurpose()
            });

            Assert.NotNull(verifyDerived);
            Assert.Equal(verifyDerived.Controller, verificationMethod.Controller);
        }

        [Fact(DisplayName = "Derive proof from document revealing all")]
        public async Task SignDocumentRevealAll()
        {
            var revealDocument = Utilities.LoadJson("Data/test_reveal_all_document.json");
            var signedDocument = Utilities.LoadJson("Data/test_signed_document.json");

            var verificationMethod = new Bls12381G2Key2020(Utilities.LoadJson("Data/did_example_489398593_test.json"));

            var derivedDocument = await LdSignatures.SignAsync(signedDocument, new ProofOptions
            {
                Suite = new BbsBlsSignatureProof2020
                {
                    RevealDocument = revealDocument,
                    VerificationMethod = verificationMethod.Id
                },
                Purpose = new AssertionMethodPurpose()
            });

            derivedDocument.Should().NotBeNull();
            derivedDocument["proof"].Should().NotBeNull();
            derivedDocument["proof"]["proofValue"].Should().NotBeNull();
        }

        [Fact(DisplayName = "Derive proof from verifiable credential revealing all")]
        public async Task DeriveVerifiableCredentialRevealAll()
        {
            var revealDocument = Utilities.LoadJson("Data/test_vc_reveal_document.json");
            var signedDocument = Utilities.LoadJson("Data/test_signed_vc.json");

            var verificationMethod = new Bls12381G2Key2020(Utilities.LoadJson("Data/did_example_489398593_test.json"));

            var derivedDocument = await LdSignatures.SignAsync(signedDocument, new ProofOptions
            {
                Suite = new BbsBlsSignatureProof2020
                {
                    RevealDocument = revealDocument,
                    Nonce = new byte[] { 1, 2, 3 },
                    VerificationMethod = verificationMethod.Id
                },
                Purpose = new AssertionMethodPurpose()
            });

            derivedDocument.Should().NotBeNull();
            derivedDocument["proof"].Should().NotBeNull();
            derivedDocument["proof"]["proofValue"].Should().NotBeNull();
        }

        [Fact(DisplayName = "Should verify derived proof")]
        public async Task VerifyDerivedProof()
        {
            var document = Utilities.LoadJson("Data/test_partial_proof_document.json");

            var result = await LdSignatures.VerifyAsync(document, new ProofOptions
            {
                Suite = new BbsBlsSignatureProof2020(),
                Purpose = new AssertionMethodPurpose()
            });

            Assert.NotNull(result);
            Assert.Equal("did:example:489398593", result.Controller);
        }

        [Fact(DisplayName = "Should verify partial derived proof")]
        public async Task VerifyPartialDerivedProof()
        {
            var document = Utilities.LoadJson("Data/test_partial_proof_document.json");

            var result = await LdSignatures.VerifyAsync(document, new ProofOptions
            {
                Suite = new BbsBlsSignatureProof2020(),
                Purpose = new AssertionMethodPurpose()
            });

            Assert.NotNull(result);
            Assert.Equal("did:example:489398593", result.Controller);
        }

        [Fact(DisplayName = "Should verify a fully revealed derived proof that uses nesting from a vc")]
        public async Task VerifyFullyRevealedDerivedProofFromVc()
        {
            var document = Utilities.LoadJson("Data/test_proof_nested_vc_document.json");

            var result = await LdSignatures.VerifyAsync(document, new ProofOptions
            {
                Suite = new BbsBlsSignatureProof2020(),
                Purpose = new AssertionMethodPurpose()
            });

            Assert.NotNull(result);
            Assert.Equal("did:example:489398593", result.Controller);
        }

        [Fact(DisplayName = "Should verify a partially revealed derived proof that uses nesting from a vc")]
        public async Task VerifyPartialRevealedDerivedProofFromVc()
        {
            var document = Utilities.LoadJson("Data/test_partial_proof_nested_vc_document.json");

            var result = await LdSignatures.VerifyAsync(document, new ProofOptions
            {
                Suite = new BbsBlsSignatureProof2020(),
                Purpose = new AssertionMethodPurpose()
            });

            Assert.NotNull(result);
            Assert.Equal("did:example:489398593", result.Controller);
        }

        [Fact(DisplayName = "Should verify partial derived proof from vc")]
        public async Task VerifyPartialDerivedProofFromVc()
        {
            var document = Utilities.LoadJson("Data/test_partial_proof_vc_document.json");

            var result = await LdSignatures.VerifyAsync(document, new ProofOptions
            {
                Suite = new BbsBlsSignatureProof2020(),
                Purpose = new AssertionMethodPurpose()
            });

            Assert.NotNull(result);
            Assert.Equal("did:example:489398593", result.Controller);
        }
    }
}