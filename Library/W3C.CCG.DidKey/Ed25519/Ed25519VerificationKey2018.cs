using Multiformats.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using W3C.CCG.DidCore;

namespace W3C.CCG.DidKey.Ed25519
{
    public class Ed25519VerificationKey2018 : VerificationMethod
    {
        public const string Name = "Ed25519VerificationKey2018";

        public Ed25519VerificationKey2018()
        {
            TypeName = Name;
        }

        public Ed25519VerificationKey2018(JObject obj) : base(obj)
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

        public static string GetFingerprint(byte[] pubKeyBytes)
        {
            // See https://github.com/multiformats/multicodec/blob/master/table.csv
            // 0xed is the value for Ed25519 public key public key in the G2 field
            // 0x01 is from varint.encode(0xed) -> [0xed, 0x01]
            // See https://github.com/multiformats/unsigned-varint
            // prefix with `z` to indicate multi-base base58btc encoding
            return $"z{ Multibase.Base58.Encode(new byte[] { 0xed, 0x01 }.Concat(pubKeyBytes).ToArray())}";
        }
    }
}