using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;
using W3C.CCG.AuthorizationCapabilities;
using W3C.CCG.LinkedDataProofs;
using Xunit;

namespace W3cCcg.LdProofs.Tests
{
    public class UnitTest1
    {
        [Fact(DisplayName = "sign with capabilityInvocation proof purpose / should succeed w/key invoker")]
        public async Task Test1()
        {
            /*
             * describe('sign with capabilityInvocation proof purpose', () => {
      it('should succeed w/key invoker', async () => {
        const doc = clone(mock.exampleDoc);
        const signed = await jsigs.sign(doc, {
          suite: new Ed25519Signature2018({
            key: new Ed25519KeyPair(alice.get('publicKey', 0)),
            date: CONSTANT_DATE
          }),
          purpose: new CapabilityInvocation({
            capability: capabilities.root.alpha.id
          })
        });
        expect(signed).to.deep.equal(mock.exampleDocWithInvocation.alpha);
      });*/

            var doc = Utilities.LoadJson("TestData/example-doc.json");

            var signedDoc = await LdSignatures.SignAsync(doc, new SignatureOptions
            {
                Suite = new MockSuite(),
                Purpose = new CapabilityInvocationProofPurpose("urn:example:mock")
            });
        }
    }

    public class MockSuite : LinkedDataProof
    {
        public MockSuite() : base("MockSignature")
        {
        }

        public override async Task<JToken> CreateProofAsync(CreateProofOptions options)
        {
            await Task.Yield();

            var proof = new JObject();

            options.Purpose.Update(proof);

            return proof;
        }
    }
}
