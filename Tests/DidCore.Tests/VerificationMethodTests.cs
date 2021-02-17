using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using W3C.CCG.DidCore;
using Xunit;

namespace W3C.DidCore.Tests
{
    public class VerificationMethodTests
    {
        [Fact(DisplayName = "Construct mock vertification method and assign values")]
        public void ConstructVerificationMethod()
        {
            var method = new VerificationMethod();

            Assert.Null(method.Id);
            Assert.Null(method.TypeName);
            Assert.Null(method.Controller);

            method.Id = "did:123#key-1";
            method.TypeName = "MockVerificationMethod";
            method.Controller = "did:123";

            Assert.Equal("did:123#key-1", method.Id);
            Assert.Equal("MockVerificationMethod", method.TypeName);
            Assert.Equal("did:123", method.Controller);
        }

        [Fact(DisplayName = "Serialize and deserialize verification method")]
        public void SerializeDeserializeVerificationMethod()
        {
            var method = new VerificationMethod();
            method.TypeName = "MockMethod";
            method.Id = "1";
            method.Controller = "a";

            var json = JsonConvert.SerializeObject(method);

            Assert.NotNull(json);

            var method1 = JsonConvert.DeserializeObject<IVerificationMethod>(json);

            Assert.NotNull(method1);
            Assert.IsType<VerificationMethod>(method1);
        }

        [Fact(DisplayName = "Serialize and deserialize verification method reference")]
        public void SerializeDeserializeVerificationMethodReference()
        {
            var method = new VerificationMethodReference("did:1234");

            var json = JsonConvert.SerializeObject(method);

            Assert.NotNull(json);

            var method1 = JsonConvert.DeserializeObject<VerificationMethodReference>(json);

            Assert.NotNull(method1);
            Assert.IsType<VerificationMethodReference>(method1);
        }
    }
}
