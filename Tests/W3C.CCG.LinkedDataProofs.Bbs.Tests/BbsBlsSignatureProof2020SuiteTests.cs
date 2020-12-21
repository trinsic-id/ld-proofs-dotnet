using System;
using BbsDataSignatures;
using Hyperledger.Ursa.BbsSignatures;
using FluentAssertions;
using LinkedDataProofs.Bbs.Tests;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace W3C.CCG.LinkedDataProofs.Bbs.Tests
{
    [Collection(ServiceFixture.CollectionDefinitionName)]
    public class BbsBlsSignatureProof2020SuiteTests
    {
        public BbsBlsSignatureProof2020SuiteTests(ServiceFixture serviceFixture)
        {
            Provider = serviceFixture.Provider;
            BbsProvider = new BbsSignatureService();
            LdProofService = Provider.GetRequiredService<ILinkedDataProofService>();
        }

        public IServiceProvider Provider { get; }
        public BbsSignatureService BbsProvider { get; }
        public ILinkedDataProofService LdProofService { get; }

        [Fact(DisplayName = "Derive proof from document revealing some")]
        public void SignDocumentRevealSome()
        {
            var keyPair = BbsProvider.GenerateBlsKey();

            var document = Utilities.LoadJson("Data/test_signed_document.json");
            var proofRequest = Utilities.LoadJson("Data/test_reveal_document.json");

            var proof = LdProofService.CreateProof(new CreateProofOptions
            {
                Document = document,
                ProofRequest = proofRequest,
                LdSuiteType = BbsBlsSignatureProof2020.Name,
                ProofPurpose = ProofPurposeNames.AssertionMethod,
                VerificationMethod = keyPair.ToVerificationMethod("did:example:123#key", "did:example:123")
            });

            proof.Should().NotBeNull();
            proof["proof"].Should().NotBeNull();
            proof["proof"]["proofValue"].Should().NotBeNull();
        }

        [Fact(DisplayName = "Derive proof from document revealing all")]
        public void SignDocumentRevealAll()
        {
            var keyPair = BbsProvider.GenerateBlsKey();

            var document = Utilities.LoadJson("Data/test_signed_document.json");
            var proofRequest = Utilities.LoadJson("Data/test_reveal_all_document.json");

            var proof = LdProofService.CreateProof(new CreateProofOptions
            {
                Document = document,
                ProofRequest = proofRequest,
                LdSuiteType = BbsBlsSignatureProof2020.Name,
                ProofPurpose = ProofPurposeNames.AssertionMethod,
                VerificationMethod = keyPair.ToVerificationMethod("did:example:123#key", "did:example:123")
            });

            proof.Should().NotBeNull();
            proof["proof"].Should().NotBeNull();
            proof["proof"]["proofValue"].Should().NotBeNull();
        }

        [Fact(DisplayName = "Derive proof from verifiable credential revealing all")]
        public void DeriveVerifiableCredentialRevealAll()
        {
            var keyPair = BbsProvider.GenerateBlsKey();

            var document = Utilities.LoadJson("Data/test_signed_vc.json");
            var proofRequest = Utilities.LoadJson("Data/test_vc_reveal_document.json");

            var proof = LdProofService.CreateProof(new CreateProofOptions
            {
                Document = document,
                ProofRequest = proofRequest,
                LdSuiteType = BbsBlsSignatureProof2020.Name,
                ProofPurpose = ProofPurposeNames.AssertionMethod,
                VerificationMethod = keyPair.ToVerificationMethod("did:example:123#key", "did:example:123")
            });

            proof.Should().NotBeNull();
            proof["proof"].Should().NotBeNull();
            proof["proof"]["proofValue"].Should().NotBeNull();
        }

        [Fact(DisplayName = "Verify proof from signed document revealing all")]
        public void VerifySignedDocumentRevealAll()
        {
            var document = Utilities.LoadJson("Data/test_proof_document.json");
            var proofRequest = Utilities.LoadJson("Data/test_reveal_all_document.json");

            var proof = LdProofService.VerifyProof(new VerifyProofOptions
            {
                Document = document,
                ProofRequest = proofRequest,
                LdSuiteType = BbsBlsSignatureProof2020.Name,
                ProofPurpose = ProofPurposeNames.AssertionMethod
            });

            proof.Should().BeTrue();
        }
    }
}