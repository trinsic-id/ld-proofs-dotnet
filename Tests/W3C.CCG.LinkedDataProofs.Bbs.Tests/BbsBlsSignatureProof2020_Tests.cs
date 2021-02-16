using System;
using System.Collections.Generic;
using System.Linq;
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
            //var document = Utilities.LoadJson("Data/test_signed_document.json");
            var revealDocument = Utilities.LoadJson("Data/test_vc_reveal_document_1.json");
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

            Console.WriteLine(signedDocument);


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
        }

        [Fact]
        public void TestFailingSigs()
        {
            var messages = new[]
            {
                "_:c14n0 <http://purl.org/dc/terms/created> \"2021-02-15T21:44:52\"^^<http://www.w3.org/2001/XMLSchema#dateTime> .", "_:c14n0 <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://w3c-ccg.github.io/ldp-bbs2020/contexts/v1#BbsBlsSignature2020> .", "_:c14n0 <https://w3id.org/security#proofPurpose> <https://w3id.org/security#assertionMethod> .", "_:c14n0 <https://w3id.org/security#verificationMethod> <did:example:489398593#test> .", "<did:example:b34ca6cd37bbf23> <http://schema.org/birthDate> \"1958-07-17\"^^<http://www.w3.org/2001/XMLSchema#dateTime> .", "<did:example:b34ca6cd37bbf23> <http://schema.org/familyName> \"SMITH\" .", "<did:example:b34ca6cd37bbf23> <http://schema.org/gender> \"Male\" .", "<did:example:b34ca6cd37bbf23> <http://schema.org/givenName> \"JOHN\" .", "<did:example:b34ca6cd37bbf23> <http://schema.org/image> <data:image/png;base64,iVBORw0KGgokJggg==> .", "<did:example:b34ca6cd37bbf23> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://schema.org/Person> .", "<did:example:b34ca6cd37bbf23> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://w3id.org/citizenship#PermanentResident> .", "<did:example:b34ca6cd37bbf23> <https://w3id.org/citizenship#birthCountry> \"Bahamas\" .", "<did:example:b34ca6cd37bbf23> <https://w3id.org/citizenship#commuterClassification> \"C1\" .", "<did:example:b34ca6cd37bbf23> <https://w3id.org/citizenship#lprCategory> \"C09\" .", "<did:example:b34ca6cd37bbf23> <https://w3id.org/citizenship#lprNumber> \"999-999-999\" .", "<did:example:b34ca6cd37bbf23> <https://w3id.org/citizenship#residentSince> \"2015-01-01\"^^<http://www.w3.org/2001/XMLSchema#dateTime> .", "<https://issuer.oidp.uscis.gov/credentials/83627465> <http://schema.org/description> \"Government of Example Permanent Resident Card.\" .", "<https://issuer.oidp.uscis.gov/credentials/83627465> <http://schema.org/identifier> \"83627465\" .", "<https://issuer.oidp.uscis.gov/credentials/83627465> <http://schema.org/name> \"Permanent Resident Card\" .", "<https://issuer.oidp.uscis.gov/credentials/83627465> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://w3id.org/citizenship#PermanentResidentCard> .", "<https://issuer.oidp.uscis.gov/credentials/83627465> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://www.w3.org/2018/credentials#VerifiableCredential> .", "<https://issuer.oidp.uscis.gov/credentials/83627465> <https://www.w3.org/2018/credentials#credentialSubject> <did:example:b34ca6cd37bbf23> .", "<https://issuer.oidp.uscis.gov/credentials/83627465> <https://www.w3.org/2018/credentials#expirationDate> \"2029-12-03T12:19:52Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime> .", "<https://issuer.oidp.uscis.gov/credentials/83627465> <https://www.w3.org/2018/credentials#issuanceDate> \"2019-12-03T12:19:52Z\"^^<http://www.w3.org/2001/XMLSchema#dateTime> .", "<https://issuer.oidp.uscis.gov/credentials/83627465> <https://www.w3.org/2018/credentials#issuer> <did:example:489398593> ."
            };
            var revealed = new[] { 0, 1, 2, 3, 5, 6, 7, 9, 10, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
            var indexed = new List<ProofMessage>();
            for (int i = 0; i < messages.Length; i++)
            {
                indexed.Add(new ProofMessage { Message = messages[i], ProofType = revealed.Contains(i) ? ProofMessageType.Revealed : ProofMessageType.HiddenProofSpecificBlinding });
            };
            //var signature = Convert.FromBase64String("irlDc008kCWd0gkjjbCld/g1YtqO8vUhLKJiLW6c2pp4SYTJEhrymPb33ZL3JoHrPJIaDarhgWKpjUEcGkEuai725HB82PDYxp8zb2CfQAZMaFpMitXH8aG9vhOUHf9J7x3PUgZCZIc8YSI7i3tZiw==");
            //var bbsKey = Convert.FromBase64String("h/rkcTKXXzRbOPr9UxSfegCbid2U/cVNXQUaKeGF7UhwrMJFP70uMH0VQ9+3+/2zDPAAjflsdeLkOXW3+ShktLxuPy8UlXSNgKNmkfb+rrj+FRwbs13pv/WsIf+eV66+uJVq1z/N62FF262eZw9yB67jIJ/NEijqZZqGT6xNHZbjceDv77tyOS4q1HfJO2x6AAAAGYElQjNLZAhsh3iF5iUUaQ0JPsv9i2hB3/QuXBhSNZE4NP6j+MU0/RYngHMuVRLG4bDni2bNzaF4+Biyork5DJsv0DZ/e16UAOmM+269Iq1lOEQmB2qFnu16UW+H6lBGzpLLH6hqirPssaksFgqLySin2CR6zuCqmSY8xMl3MI0FePGm2S2L20rwtXOiUhTWwbKSVtycFI58/r587tjE27ze5I0QeuYt5rCrBX+S2j0hu95GwrWGxQOalJhUdNE786HlftIu4H6d4rJP2faoZsdDvzBu3m14EWv1s5WtnsdMTfAcQzpHjwJ+AZSOORmY1YCsjQeXWTOwMTIrX2BwxC9ZGwt4uy+SUfy6/2YXhrRwLw1u5DDXghnJtxkIW6xE8am+qO28bVWbLjRCJQj420j6UOwRCwaEOX3Wp+LV75mzIj52pGN5WjCbOJhuxS2xUo/ihAUYhu+qsn50vF5bMg26A4g2rD5Ucf4XTBpOtFX+j878or+jGZUQkwzH5OS7Qqrr1dDHhVccR1bH4X/qRLwJCQncLC9y9D9mOJf2dUUFhvzvMOT0R+nXC9bxcmuHS612vuARE2nUqwE5dRKcDRTgapPdD0EaRhBiH8DbmPyVT4sVt5+5+RFJhwZMnvqyfJCL8IAGcSLXkJWhsF6WTAc6PH8Rm0g85Bt4Q0zNlV0x/Ohzzq2mErEHZhUeu5o1GY3peqTgyYDQGtdEsHQVa7rq+/wX73PtUutWd5XV1+dQopD19UU7Xbrwes2ooR7kTqFDFaovYM4JXKwWUm1fJl+7J7yZBoIk0MPZsWRy83OMRbVKeDYfgf5iagyHWrJ6joBTSOObqafJSv8wmPVXoa2uHI6b3eFPR9mpV7rRRmraWrlPrtBWoqlnAk3/xt2leIQGRZ6gEKQSzvA9TWYKLfbODdlaDADpUMhE/Ihe3Q6K9UoodU/PSjTkaCQ8KyI4NaMKOeFG4ad+K0NsMnE1fPFuFKdx3JC24jmLtmBHs0oKk0VL6MnmkJD4u/ru28FgW6plK2NEj1FoT7a6M/gqpqfH9h4LUzUHdXRpj3IpJ+zQb77qyzwr6xaOlXsRbYov7bHogafhC2ojXFZnBVd6Kh6fG91VLoICKKYX01h5PWKC7SmLnG1XGFQP4eJA0J7TsquX30m3xdRzFVUhTDiHV1Q0cvMHwF2M28L1NZE6l5K99bFFNTsZpwlfQzAqqyPuaIdU0QLvX1BKmUBKfyNkpmtAzJHrIHPe+0wM1zblwr3FWuguqgifgy/xCTQmLKJbM7Ktv/CatHDcQCOKwodPrn52bngrqfl5ML/U9/zhN6tG8umSQiCPZaoHcq7bZVp5fok1HA6pb/9toxeHPG2w+PYqM9y2Kbg4UXcY66CeYy5VGPE1xodmjNCubS4220OQi5awqnGHMk4jnjJLVEKX8I7fddP9wSQP9yOM1NOjIJ0R+0fdUhwe0wqFJIzTMtKd/5N2tXf2Kgku5bi+0WVuwi8+Y+qspSjh3YS/hLzszwJm74o31loQgnA6USLo4yVdn7mCaWAxHn8easyfTVIbS5dZCY8XNOFWWk5XjW3P2Nvxkwvud94u47oFbA74zOOA1w==");

            var signature = Convert.FromBase64String("uPik2xKdkBtjeq4N6Z2NSaxFUy5F+Z2aAq+uVvjwwH2lpuHbBd/DBp+rpK9uNZtmXfyVg3UBxmyVkg6kMIS+d+aTaYupAcIESoK01AgKezUzd0f5sj3PtV3oYwcyhMM/Uct1yiT2GctQZ+UXdVNiyw==");
            var bbsKey = Convert.FromBase64String("h/rkcTKXXzRbOPr9UxSfegCbid2U/cVNXQUaKeGF7UhwrMJFP70uMH0VQ9+3+/2zDPAAjflsdeLkOXW3+ShktLxuPy8UlXSNgKNmkfb+rrj+FRwbs13pv/WsIf+eV66+uJVq1z/N62FF262eZw9yB67jIJ/NEijqZZqGT6xNHZbjceDv77tyOS4q1HfJO2x6AAAAGYElQjNLZAhsh3iF5iUUaQ0JPsv9i2hB3/QuXBhSNZE4NP6j+MU0/RYngHMuVRLG4bDni2bNzaF4+Biyork5DJsv0DZ/e16UAOmM+269Iq1lOEQmB2qFnu16UW+H6lBGzpLLH6hqirPssaksFgqLySin2CR6zuCqmSY8xMl3MI0FePGm2S2L20rwtXOiUhTWwbKSVtycFI58/r587tjE27ze5I0QeuYt5rCrBX+S2j0hu95GwrWGxQOalJhUdNE786HlftIu4H6d4rJP2faoZsdDvzBu3m14EWv1s5WtnsdMTfAcQzpHjwJ+AZSOORmY1YCsjQeXWTOwMTIrX2BwxC9ZGwt4uy+SUfy6/2YXhrRwLw1u5DDXghnJtxkIW6xE8am+qO28bVWbLjRCJQj420j6UOwRCwaEOX3Wp+LV75mzIj52pGN5WjCbOJhuxS2xUo/ihAUYhu+qsn50vF5bMg26A4g2rD5Ucf4XTBpOtFX+j878or+jGZUQkwzH5OS7Qqrr1dDHhVccR1bH4X/qRLwJCQncLC9y9D9mOJf2dUUFhvzvMOT0R+nXC9bxcmuHS612vuARE2nUqwE5dRKcDRTgapPdD0EaRhBiH8DbmPyVT4sVt5+5+RFJhwZMnvqyfJCL8IAGcSLXkJWhsF6WTAc6PH8Rm0g85Bt4Q0zNlV0x/Ohzzq2mErEHZhUeu5o1GY3peqTgyYDQGtdEsHQVa7rq+/wX73PtUutWd5XV1+dQopD19UU7Xbrwes2ooR7kTqFDFaovYM4JXKwWUm1fJl+7J7yZBoIk0MPZsWRy83OMRbVKeDYfgf5iagyHWrJ6joBTSOObqafJSv8wmPVXoa2uHI6b3eFPR9mpV7rRRmraWrlPrtBWoqlnAk3/xt2leIQGRZ6gEKQSzvA9TWYKLfbODdlaDADpUMhE/Ihe3Q6K9UoodU/PSjTkaCQ8KyI4NaMKOeFG4ad+K0NsMnE1fPFuFKdx3JC24jmLtmBHs0oKk0VL6MnmkJD4u/ru28FgW6plK2NEj1FoT7a6M/gqpqfH9h4LUzUHdXRpj3IpJ+zQb77qyzwr6xaOlXsRbYov7bHogafhC2ojXFZnBVd6Kh6fG91VLoICKKYX01h5PWKC7SmLnG1XGFQP4eJA0J7TsquX30m3xdRzFVUhTDiHV1Q0cvMHwF2M28L1NZE6l5K99bFFNTsZpwlfQzAqqyPuaIdU0QLvX1BKmUBKfyNkpmtAzJHrIHPe+0wM1zblwr3FWuguqgifgy/xCTQmLKJbM7Ktv/CatHDcQCOKwodPrn52bngrqfl5ML/U9/zhN6tG8umSQiCPZaoHcq7bZVp5fok1HA6pb/9toxeHPG2w+PYqM9y2Kbg4UXcY66CeYy5VGPE1xodmjNCubS4220OQi5awqnGHMk4jnjJLVEKX8I7fddP9wSQP9yOM1NOjIJ0R+0fdUhwe0wqFJIzTMtKd/5N2tXf2Kgku5bi+0WVuwi8+Y+qspSjh3YS/hLzszwJm74o31loQgnA6USLo4yVdn7mCaWAxHn8easyfTVIbS5dZCY8XNOFWWk5XjW3P2Nvxkwvud94u47oFbA74zOOA1w==");

            var result = new BbsSignatureService().CreateProof(new CreateProofRequest(
                new BbsKey(bbsKey,
                (uint)messages.Count()),
                indexed.ToArray(),
                signature,
                null,
                "123"));

            Assert.NotNull(result);
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