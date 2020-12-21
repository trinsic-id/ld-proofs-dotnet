using System;
using System.Linq;
using System.Reflection;
using Hyperledger.Ursa.BbsSignatures;
using Multiformats.Base;
using Newtonsoft.Json.Linq;
using W3C.CCG.DidCore;

namespace BbsDataSignatures
{
    public class Bls12381VerificationKey2020 : VerificationMethod
    {
        public const string Name = "Bls12381G2Key2020";

        public Bls12381VerificationKey2020()
        {
            TypeName = Name;
        }

        public Bls12381VerificationKey2020(JObject obj) : base(obj)
        {
            TypeName ??= Name;
        }

        public string PublicKeyBase58
        {
            get => this["publicKeyBase58"]?.Value<string>();
            set => this["publicKeyBase58"] = value;
        }

        public string PrivateKeyBase58
        {
            get => this["privateKeyBase58"]?.Value<string>();
            set => this["privateKeyBase58"] = value;
        }

        public override string GetFingerprint()
        {
            // BLS12-381 cryptonyms are multicodec encoded values, specifically:
            // (multicodec('bls12_381-g2-pub') + key bytes)
            var pubkeyBytes = Multibase.Base58.Decode(PublicKeyBase58 ?? throw new Exception("Public key was not found"));
            return GetFingerprint(pubkeyBytes);
        }

        public override VerificationMethod GetPublicNode()
        {
            var cloned = (JObject)DeepClone();
            cloned.Remove("privateKeyBase58");

            return new Bls12381VerificationKey2020(cloned);
        }

        public DidDocument ToDidDocument()
        {
            var fingerprint = GetFingerprint();
            var controller = $"did:key:{fingerprint}";

            var key = GetPublicNode();
            key.Id = $"#{fingerprint}";
            key.Controller = controller;

            var document = new DidDocument
            {
                Id = $"did:key:{fingerprint}",
                PublicKey = new [] { key },
                Authentication = new VerificationMethodReference[] { key.Id },
                CapabilityDelegation = new VerificationMethodReference[] { key.Id },
                CapabilityInvocation = new VerificationMethodReference[] { key.Id },
                AssertionMethod = new VerificationMethodReference[] { key.Id }
            };
            document.EnhanceContext(new JObject { { "@base", controller } });

            return document;
        }

        public static string GetFingerprint(byte[] pubKeyBytes)
        {
            // See https://github.com/multiformats/multicodec/blob/master/table.csv
            // 0xeb is the value for BLS12-381 public key in the G2 field
            // 0x01 is from varint.encode(0xeb) -> [0xeb, 0x01]
            // See https://github.com/multiformats/unsigned-varint
            // prefix with `z` to indicate multi-base base58btc encoding
            return $"z{ Multibase.Base58.Encode(new byte[] { 0xeb, 0x01 }.Concat(pubKeyBytes).ToArray())}";
        }

        public static Bls12381VerificationKey2020 Generate(string seed = null, string controller = null)
        {
            var service = new BbsSignatureService();
            var key = seed == null ? service.GenerateBlsKey() : service.GenerateBlsKey(seed);

            var fingerprint = GetFingerprint(key.PublicKey.ToArray());
            return new Bls12381VerificationKey2020
            {
                Id =  $"{controller ?? $"did:key:{fingerprint}"}#{fingerprint}",
                Controller = controller ?? $"did:key:{fingerprint}",
                PublicKeyBase58 = Multibase.Base58.Encode(key.PublicKey.ToArray()),
                PrivateKeyBase58 = Multibase.Base58.Encode(key.SecretKey.ToArray())
            };
        }

        public BlsKeyPair ToBlsKeyPair() => new BlsKeyPair(Multibase.Base58.Decode(
            PrivateKeyBase58 ??
            PublicKeyBase58 ??
            throw new Exception("Cannot convert to BlsKeyPair. Either public or private key data must be present.")));
    }
}