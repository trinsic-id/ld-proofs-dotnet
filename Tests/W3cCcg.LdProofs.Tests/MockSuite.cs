using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using W3C.CCG.LinkedDataProofs;

namespace W3cCcg.LdProofs.Tests
{
    public class MockSuite : LinkedDataSignature
    {
        public MockSuite() : base("https://example.com/MockSignature")
        {
        }

        public override Task<VerifyProofResult> VerifyProofAsync(JToken proof, VerifyProofOptions options)
        {
            throw new NotImplementedException();
        }

        protected override async Task<JObject> SignAsync(JToken verifyData, JObject proof, CreateProofOptions options)
        {
            await Task.Yield();

            if (verifyData is JArray data)
            {
                proof["proofValue"] = Convert.ToBase64String(data.ToObject<byte[]>());
                return proof;
            }
            throw new NotSupportedException();
        }
    }
}
