using System.Text;
using W3C.CCG.LinkedDataProofs.Suites;
using Xunit;

namespace W3cCcg.LdProofs.Tests
{
    public class Ed25519KeyTests
    {
        [Fact(DisplayName = "Generate random key")]
        public void GenerateRandomKey()
        {
            var method = Ed25519VerificationKey2018.Generate();

            Assert.NotNull(method);
            Assert.NotNull(method.PublicKeyBase58);
            Assert.NotNull(method.PrivateKeyBase58);
            Assert.Equal(Ed25519VerificationKey2018.Name, method.TypeName);
        }

        [Fact(DisplayName = "Sign payload")]
        public void SignPayload()
        {
            var method = Ed25519VerificationKey2018.Generate();
            var payload = Encoding.UTF8.GetBytes("my message");

            var signature = method.Sign(payload);

            Assert.NotNull(signature);
            Assert.Equal(Chaos.NaCl.Ed25519.SignatureSizeInBytes, signature.Length);
        }

        [Fact(DisplayName = "Verify signature")]
        public void VerifyPayload()
        {
            var method = Ed25519VerificationKey2018.Generate();
            var payload = Encoding.UTF8.GetBytes("my message");
            var signature = method.Sign(payload);

            var verified = method.Verify(signature, payload);

            Assert.True(verified);
        }
    }
}
