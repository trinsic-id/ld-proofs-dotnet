using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using W3C.CCG.DidCore;
using Xunit;

namespace W3C.DidCore.Tests
{
    public class DidDocumentTests
    {
        [Fact(DisplayName = "Read and write Created and Updated values as datetime")]
        public void CreateUpdatedAsDateTimeOffset()
        {
            var didDoc = new DidDocument();

            Assert.Null(didDoc.Created);
            Assert.Null(didDoc.Updated);

            var now = DateTimeOffset.UtcNow;

            didDoc.Created = didDoc.Updated = now;

            Assert.Equal(now, didDoc.Created);
            Assert.Equal(now, didDoc.Updated);
        }

        [Fact(DisplayName = "Test serialization of VerificationMethod properties")]
        public void VerificationMethodSerialization()
        {
            var didDoc = new DidDocument();
            didDoc.Authentication = new IVerificationMethod[]
            {
                new VerificationMethod { Id = "did:123" },
                new VerificationMethodReference("did:123#key-1")
            };

            var json = JsonConvert.SerializeObject(didDoc);

            var didDoc1 = new DidDocument(JObject.Parse(json));
            var auth = didDoc1.Authentication.ToList();

            Assert.Equal(2, auth.Count);
            Assert.IsType<VerificationMethod>(auth[0]);
            Assert.IsType<VerificationMethodReference>(auth[1]);
        }
    }
}
