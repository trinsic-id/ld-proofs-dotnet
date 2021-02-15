using System.IO;
using BbsDataSignatures;
using BbsSignatures;
using LinkedDataProofs.Bbs.Tests;
using Newtonsoft.Json.Linq;
using VDS.RDF;
using VDS.RDF.JsonLd;
using VDS.RDF.Parsing;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using W3C.CCG.LinkedDataProofs;
using W3C.CCG.SecurityVocabulary;
using System;
using System.Diagnostics;
using W3C.CCG.LinkedDataProofs.Purposes;
using System.Threading.Tasks;

namespace LindedDataProofs.Bbs
{
    [Collection(ServiceFixture.CollectionDefinitionName)]
    public class BbsBlsSignature2020Tests
    {
        public BbsBlsSignature2020Tests(ServiceFixture serviceFixture)
        {
            BbsProvider = new BbsSignatureService();
        }

        public BbsSignatureService BbsProvider { get; }

        [Fact(DisplayName = "Sign document with BBS suite")]
        public async Task SignDocument()
        {
            var keyPair = BlsKeyPair.GenerateG2();
            var verificationMethod = new Bls12381G2Key2020(keyPair);

            var document = Utilities.LoadJson("Data/test_document.json");

            var signedDocument = await LdSignatures.SignAsync(document, new ProofOptions
            {
                Suite = new BbsBlsSignature2020
                {
                    Signer = verificationMethod,
                    VerificationMethod = verificationMethod
                },
                Purpose = new AssertionMethodPurpose()
            });

            signedDocument.Should().NotBeNull();
            signedDocument["proof"].Should().NotBeNull();
            signedDocument["proof"]["proofValue"].Should().NotBeNull();
        }

        [Fact(DisplayName = "Sign verifiable credential with BBS suite")]
        public async Task SignVerifiableCredential()
        {
            var keyPair = BlsKeyPair.GenerateG2();
            var verificationMethod = new Bls12381G2Key2020(keyPair);

            var document = Utilities.LoadJson("Data/test_vc.json");

            var signedDocument = await LdSignatures.SignAsync(document, new ProofOptions
            {
                Suite = new BbsBlsSignature2020
                {
                    Signer = verificationMethod,
                    VerificationMethod = verificationMethod
                },
                Purpose = new AssertionMethodPurpose()
            });

            signedDocument.Should().NotBeNull();
            signedDocument["proof"].Should().NotBeNull();
            signedDocument["proof"]["proofValue"].Should().NotBeNull();
        }

        //[Fact(DisplayName = "Verify signed document with BBS suite")]
        //public void VerifySignedDocument()
        //{
        //    var document = Utilities.LoadJson("Data/test_signed_document.json");

        //    var proof = LdProofService.VerifyProof(new ProofOptions
        //    {
        //        Document = document,
        //        LdSuiteType = BbsBlsSignature2020.Name,
        //        ProofPurpose = ProofPurposeNames.AssertionMethod
        //    });

        //    proof.Should().BeTrue();
        //}

        //[Fact(DisplayName = "Verify verifiable credentials with BBS suite")]
        //public void VerifySignedVerifiableCredential()
        //{
        //    var document = Utilities.LoadJson("Data/test_signed_vc.json");

        //    var proof = LdProofService.VerifyProof(new ProofOptions
        //    {
        //        Document = document,
        //        LdSuiteType = BbsBlsSignature2020.Name,
        //        ProofPurpose = ProofPurposeNames.AssertionMethod
        //    });

        //    proof.Should().BeTrue();
        //}

        //[Fact(DisplayName = "Should not verify bad sig with BBS suite")]
        //public void ShouldNotVerifyBadDocument()
        //{
        //    var document = Utilities.LoadJson("Data/test_bad_signed_document.json");

        //    var proof = LdProofService.VerifyProof(new ProofOptions
        //    {
        //        Document = document,
        //        LdSuiteType = BbsBlsSignature2020.Name,
        //        ProofPurpose = ProofPurposeNames.AssertionMethod
        //    });

        //    proof.Should().BeFalse();
        //}
    }
}
