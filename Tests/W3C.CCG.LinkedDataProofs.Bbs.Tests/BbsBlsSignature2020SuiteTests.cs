using System.IO;
using BbsDataSignatures;
using Hyperledger.Ursa.BbsSignatures;
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

namespace LindedDataProofs.Bbs
{
    [Collection(ServiceFixture.CollectionDefinitionName)]
    public class BbsBlsSignature2020SuiteTests
    {
        public BbsBlsSignature2020SuiteTests(ServiceFixture serviceFixture)
        {
            Provider = serviceFixture.Provider;
            BbsProvider = new BbsSignatureService();
            LdProofService = Provider.GetRequiredService<ILinkedDataProofService>();
        }

        public IServiceProvider Provider { get; }
        public BbsSignatureService BbsProvider { get; }
        public ILinkedDataProofService LdProofService { get; }

        [Fact(DisplayName = "Sign document with BBS suite")]
        public void SignDocument()
        {
            var keyPair = BbsProvider.GenerateBlsKey();

            var document = Utilities.LoadJson("Data/test_document.json");

            var proof = LdProofService.CreateProof(new CreateProofOptions
            {
                Document = document,
                LdSuiteType = BbsBlsSignature2020.Name,
                ProofPurpose = ProofPurposeNames.AssertionMethod,
                VerificationMethod = keyPair.ToVerificationMethod("did:example:12345#test")
            });

            proof.Should().NotBeNull();
            proof["proofValue"].Should().NotBeNull();
        }

        [Fact(DisplayName = "Sign verifiable credential with BBS suite")]
        public void SignVerifiableCredential()
        {
            var keyPair = BbsProvider.GenerateBlsKey();

            var document = Utilities.LoadJson("Data/test_vc.json");

            var proof = LdProofService.CreateProof(new CreateProofOptions
            {
                Document = document,
                LdSuiteType = BbsBlsSignature2020.Name,
                ProofPurpose = ProofPurposeNames.AssertionMethod,
                VerificationMethod = keyPair.ToVerificationMethod("did:example:12345#test")
            });

            proof.Should().NotBeNull();
            proof["proofValue"].Should().NotBeNull();
        }

        [Fact(DisplayName = "Verify signed document with BBS suite")]
        public void VerifySignedDocument()
        {
            var document = Utilities.LoadJson("Data/test_signed_document.json");

            var proof = LdProofService.VerifyProof(new VerifyProofOptions
            {
                Document = document,
                LdSuiteType = BbsBlsSignature2020.Name,
                ProofPurpose = ProofPurposeNames.AssertionMethod
            });

            proof.Should().BeTrue();
        }

        [Fact(DisplayName = "Verify verifiable credentials with BBS suite")]
        public void VerifySignedVerifiableCredential()
        {
            var document = Utilities.LoadJson("Data/test_signed_vc.json");

            var proof = LdProofService.VerifyProof(new VerifyProofOptions
            {
                Document = document,
                LdSuiteType = BbsBlsSignature2020.Name,
                ProofPurpose = ProofPurposeNames.AssertionMethod
            });

            proof.Should().BeTrue();
        }

        [Fact(DisplayName = "Should not verify bad sig with BBS suite")]
        public void ShouldNotVerifyBadDocument()
        {
            var document = Utilities.LoadJson("Data/test_bad_signed_document.json");

            var proof = LdProofService.VerifyProof(new VerifyProofOptions
            {
                Document = document,
                LdSuiteType = BbsBlsSignature2020.Name,
                ProofPurpose = ProofPurposeNames.AssertionMethod
            });

            proof.Should().BeFalse();
        }
    }
}
