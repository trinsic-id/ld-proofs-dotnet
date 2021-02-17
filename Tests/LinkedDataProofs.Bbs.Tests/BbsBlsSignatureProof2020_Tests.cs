using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BbsDataSignatures;
using BbsSignatures;
using FluentAssertions;
using LinkedDataProofs.Bbs.Tests;
using Microsoft.Extensions.DependencyInjection;
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

        [Fact(DisplayName = "Derive proof from document revealing some")]
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

            signedDocument["proof"]["@context"] = Constants.SECURITY_CONTEXT_V3_URL;

            var derivedDocument = await LdSignatures.SignAsync(signedDocument, new ProofOptions
            {
                Suite = new BbsBlsSignatureProof2020
                {
                    RevealDocument = revealDocument,
                    Nonce = "123"
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