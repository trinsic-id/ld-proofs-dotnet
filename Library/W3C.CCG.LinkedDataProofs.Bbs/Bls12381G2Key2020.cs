using System;
using BbsSignatures;
using Multiformats.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using W3C.CCG.DidCore;
using W3C.CCG.LinkedDataProofs;

namespace BbsDataSignatures
{
    public class Bls12381G2Key2020 : VerificationMethod, ISigner
    {
        public const string Name = "Bls12381G2Key2020";
        private readonly IBbsSignatureService Service = new BbsSignatureService();

        public Bls12381G2Key2020()
        {
            TypeName = Name;
            this["@context"] = "https://w3c-ccg.github.io/ldp-bbs2020/contexts/v1";
        }

        public Bls12381G2Key2020(JObject obj) : base(obj)
        {
        }

        public Bls12381G2Key2020(BlsKeyPair keyPair)
        {
            PublicKeyBase58 = Multibase.Base58.Encode(keyPair.PublicKey);
            if (keyPair.SecretKey != null)
            {
                PrivateKeyBase58 = Multibase.Base58.Encode(keyPair.SecretKey);
            }
            KeyPair = keyPair;
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

        [JsonIgnore]
        public BlsKeyPair? KeyPair { get; }

        /// <summary>
        /// Gets the public data for this method
        /// </summary>
        /// <returns></returns>
        public override VerificationMethod GetPublicNode()
        {
            var clone = DeepClone();
            clone.Remove("privateKeyBase58");

            return new Bls12381G2Key2020(clone as JObject);
        }

        public byte[] Sign(IVerifyData input)
        {
            if (input is StringArray stringArray)
            {
                return Service.Sign(new SignRequest(KeyPair, stringArray.Data));
            }
            throw new Exception("Invalid input data type");
        }

        public bool Verify(byte[] signature, IVerifyData input)
        {
            if (input is StringArray stringArray)
            {
                return Service.Verify(new VerifyRequest(KeyPair, signature, stringArray.Data));
            }
            throw new Exception("Invalid input data type");
        }
    }
}
