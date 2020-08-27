using System;
using Multiformats.Base;
using Newtonsoft.Json.Linq;

namespace W3C.CCG.DidCore.X25519
{
    public class X25519KeyAgreementKey2019 : VerificationMethod
    {
        public const string Name = "X25519KeyAgreementKey2019";

        public X25519KeyAgreementKey2019()
        {
            TypeName = Name;
        }

        public X25519KeyAgreementKey2019(JObject obj) : base(obj)
        {
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

        /// <summary>
        /// Get a shared secret
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public byte[] SharedSecret(byte[] publicKey) => Curve25519.GetSharedSecret(Multibase.Base58.Decode(PrivateKeyBase58), publicKey);

        /// <summary>
        /// Generate new <see cref="X25519KeyAgreementKey2019"/> key
        /// </summary>
        /// <param name="id"></param>
        /// <param name="controller"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static X25519KeyAgreementKey2019 GenerateKey(string id, string controller, string seed = null)
        {
            var privateKey = seed is null ? Curve25519.CreateRandomPrivateKey() : Curve25519.CreateRandomPrivateKey(seed);
            var publicKey = Curve25519.GetPublicKey(privateKey);

            return new X25519KeyAgreementKey2019
            {
                Id = id,
                Controller = controller,
                PrivateKeyBase58 = Multibase.Base58.Encode(privateKey),
                PublicKeyBase58 = Multibase.Base58.Encode(publicKey)
            };
        }
    }
}
