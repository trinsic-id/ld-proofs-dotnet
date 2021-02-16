using System;
using System.Threading.Tasks;
using BbsDataSignatures;
using BbsSignatures;
using FluentAssertions;
using LinkedDataProofs.Bbs.Tests;
using Microsoft.Extensions.DependencyInjection;
using W3C.CCG.LinkedDataProofs.Purposes;
using Xunit;

namespace W3C.CCG.LinkedDataProofs.Bbs.Tests
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

        [Fact(DisplayName = "Derive proof from document revealing some")]
        public async Task SignDocumentRevealSome()
        {
            var keyPair = BlsKeyPair.GenerateG2();

            var document = Utilities.LoadJson("Data/test_signed_document.json");
            var revealDocument = Utilities.LoadJson("Data/test_reveal_document.json");

            var derivedDocument = await LdSignatures.SignAsync(document, new ProofOptions
            {
                Suite = new BbsBlsSignatureProof2020
                {
                    KeyPair = keyPair,
                    RevealDocument = revealDocument,
                    Nonce = "123",
                    VerificationMethod = "did:example:alice"
                },
                Purpose = new AssertionMethodPurpose()
            });

            derivedDocument.Should().NotBeNull();
            derivedDocument["proof"].Should().NotBeNull();
            derivedDocument["proof"]["proofValue"].Should().NotBeNull();
        }

        //[Fact(DisplayName = "Derive proof from document revealing all")]
        //public void SignDocumentRevealAll()
        //{
        //    var keyPair = BbsProvider.GenerateBlsKey();

        //    var document = Utilities.LoadJson("Data/test_signed_document.json");
        //    var proofRequest = Utilities.LoadJson("Data/test_reveal_all_document.json");

        //    var proof = LdProofService.CreateProof(new ProofOptions
        //    {
        //        Document = document,
        //        ProofRequest = proofRequest,
        //        LdSuiteType = BbsBlsSignatureProof2020.Name,
        //        ProofPurpose = ProofPurposeNames.AssertionMethod,
        //        VerificationMethod = keyPair.ToVerificationMethod("did:example:123#key", "did:example:123")
        //    });

        //    proof.Should().NotBeNull();
        //    proof["proof"].Should().NotBeNull();
        //    proof["proof"]["proofValue"].Should().NotBeNull();
        //}

        //[Fact(DisplayName = "Derive proof from verifiable credential revealing all")]
        //public void DeriveVerifiableCredentialRevealAll()
        //{
        //    var keyPair = BbsProvider.GenerateBlsKey();

        //    var document = Utilities.LoadJson("Data/test_signed_vc.json");
        //    var proofRequest = Utilities.LoadJson("Data/test_vc_reveal_document.json");

        //    var proof = LdProofService.CreateProof(new ProofOptions
        //    {
        //        Document = document,
        //        ProofRequest = proofRequest,
        //        LdSuiteType = BbsBlsSignatureProof2020.Name,
        //        ProofPurpose = ProofPurposeNames.AssertionMethod,
        //        VerificationMethod = keyPair.ToVerificationMethod("did:example:123#key", "did:example:123")
        //    });

        //    proof.Should().NotBeNull();
        //    proof["proof"].Should().NotBeNull();
        //    proof["proof"]["proofValue"].Should().NotBeNull();
        //}

        //[Fact(DisplayName = "Verify proof from signed document revealing all")]
        //public void VerifySignedDocumentRevealAll()
        //{
        //    var document = Utilities.LoadJson("Data/test_proof_document.json");
        //    var proofRequest = Utilities.LoadJson("Data/test_reveal_all_document.json");

        //    var proof = LdProofService.VerifyProof(new ProofOptions
        //    {
        //        Document = document,
        //        ProofRequest = proofRequest,
        //        LdSuiteType = BbsBlsSignatureProof2020.Name,
        //        ProofPurpose = ProofPurposeNames.AssertionMethod
        //    });

        //    proof.Should().BeTrue();
        //}
    }
}