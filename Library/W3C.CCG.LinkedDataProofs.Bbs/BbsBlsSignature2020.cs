using BbsSignatures;
using Multiformats.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF.JsonLd;
using W3C.CCG.LinkedDataProofs;

namespace BbsDataSignatures
{
    public class BbsBlsSignature2020 : LinkedDataSignature
    {
        public const string Name = "https://w3c-ccg.github.io/ldp-bbs2020/contexts/v1#BbsBlsSignature2020";

        public IBbsSignatureService SignatureService { get; }
        public BlsKeyPair KeyPair { get; set; }

        public BbsBlsSignature2020() : base(Name)
        {
            SignatureService = new BbsSignatureService();
        }

        protected override Task<JObject> SignAsync(IVerifyData payload, JObject proof, ProofOptions options)
        {
            var verifyData = payload as StringArray ?? throw new ArgumentException("Invalid data type");

            if (KeyPair?.SecretKey == null)
            {
                throw new Exception("Private key not found.");
            }

            Console.WriteLine($"Sign: {verifyData.Data.Count()}");
            var proofValue = SignatureService.Sign(new SignRequest(KeyPair, verifyData.Data));
            proof["proofValue"] = Convert.ToBase64String(proofValue);

            return Task.FromResult(proof);
        }

        protected override Task VerifyAsync(IVerifyData payload, JToken proof, JToken verificationMethod, ProofOptions options)
        {
            var verifyData = payload as StringArray ?? throw new ArgumentException("Invalid data type");

            var blsVerificationMethod = new Bls12381G2Key2020(verificationMethod as JObject);

            var key = new BlsKeyPair(Multibase.Base58.Decode(blsVerificationMethod.PublicKeyBase58));
            var signature = Helpers.FromBase64String(proof["proofValue"]?.ToString() ?? throw new Exception("Proof value not found"));

            var valid = SignatureService.Verify(new VerifyRequest(key, signature, verifyData.Data));
            if (!valid)
            {
                throw new Exception("Invalid signature");
            }
            return Task.CompletedTask;
        }

        protected override IVerifyData CreateVerifyData(JObject proof, ProofOptions options)
        {
            var c14nProofOptions = CanonizeProof(proof);
            var c14nDocument = Helpers.CanonizeStatements(options.Input, new JsonLdProcessorOptions());

            return (StringArray)c14nProofOptions.Concat(c14nDocument).ToArray();
        }

        internal static IEnumerable<string> CanonizeProof(JObject proof)
        {
            proof = proof.DeepClone() as JObject;

            proof.Remove("jws");
            proof.Remove("signatureValue");
            proof.Remove("proofValue");

            return Helpers.CanonizeStatements(proof, new JsonLdProcessorOptions());
        }
    }
}
