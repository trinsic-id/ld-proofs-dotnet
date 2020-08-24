using System;
using Hyperledger.Ursa.BbsSignatures;
using Newtonsoft.Json.Linq;
using W3C.CCG.DidCore;

namespace BbsDataSignatures
{
    public class Bls12381VerificationKey2020 : VerificationMethod
    {
        public const string Name = "Bls12381VerificationKey2020";

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

        internal BlsKeyPair ToBlsKeyPair() => new BlsKeyPair(
            PrivateKeyBase58?.AsBytesFromBase58() ??
            PublicKeyBase58?.AsBytesFromBase58() ??
            throw new Exception("Cannot convert to BlsKeyPair. Either public or private key data must be present."));
    }
}