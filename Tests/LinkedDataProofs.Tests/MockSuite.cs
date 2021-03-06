﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using LinkedDataProofs;
using LinkedDataProofs.Purposes;

namespace LinkedDataProfss.Tests
{
    public class MockSuite : LinkedDataSignature
    {
        public MockSuite() : base("https://example.com/MockSignature")
        {
        }

        public override Task<ValidationResult> VerifyProofAsync(JToken proof, ProofOptions options)
        {
            throw new NotImplementedException();
        }

        protected override Task<JObject> SignAsync(IVerifyData verifyData, JObject proof, ProofOptions options)
        {
            proof["proofValue"] = Convert.ToBase64String((verifyData as ByteArray).Data);
            return Task.FromResult(proof);
        }

        protected override Task VerifyAsync(IVerifyData verifyData, JToken proof, JToken verificationMethod, ProofOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
