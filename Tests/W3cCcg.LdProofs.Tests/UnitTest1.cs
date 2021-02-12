using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;
using W3C.CCG.AuthorizationCapabilities;
using W3C.CCG.LinkedDataProofs;
using W3C.CCG.LinkedDataProofs.Purposes;
using Xunit;

namespace W3cCcg.LdProofs.Tests
{
    public class CapabilityTests
    {
        [Fact(DisplayName = "sign with capabilityInvocation proof purpose / should succeed w/key invoker")]
        public async Task Test1()
        {
            var doc = Utilities.LoadJson("TestData/example-doc.json");

            var signedDoc = await LdSignatures.SignAsync(doc, new SignatureOptions
            {
                Suite = new MockSuite
                {
                    VerificationMethod = "did:example:bob"
                },
                Purpose = new CapabilityInvocationProofPurpose()
                {
                    Controller = "did:example:bob",
                    Options = new PurposeOptions
                    {
                        Capability = "http://example/mock"
                    }
                }
            });
        }


        [Fact(DisplayName = "Null terms passes null test")]
        public void TestNullTermEquality()
        {
            var obj = new JObject
            {
                { "prop1", null },
                { "prop2", "value" }
            };

            Assert.True(obj["test"] is null);
            Assert.True(obj["test"] == null);
            Assert.Throws<ArgumentNullException>(() => obj["test"].Value<string>() == null);
            Assert.True(obj["prop1"] != null);
            Assert.True(obj["prop1"].ToObject<string>() == null);
            Assert.True(obj["prop1"].Value<string>() == null);
            Assert.True(obj["prop2"] != null);
            Assert.True(obj["prop2"].Value<string>() == "value");
            Assert.False(obj["test"]?.Value<string>() == "value");
        }

        [Fact(DisplayName = "Sign data with MockSuite")]
        public async Task SignMockSuite()
        {
            var doc = Utilities.LoadJson("TestData/example-doc.json");

            var data = await LdSignatures.SignAsync(doc, new SignatureOptions
            {
                Suite = new MockSuite
                {
                    VerificationMethod = "did:example:alice"
                },
                Purpose = new AssertionMethodPurpose()
            });

            Assert.NotNull(data);
            Assert.NotNull(data["proof"]);
        }
    }
}
