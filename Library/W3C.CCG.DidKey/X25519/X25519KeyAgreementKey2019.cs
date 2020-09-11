using System;
using System.Linq;
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
        /// Generates and returns a multiformats encoded
        /// X25519 public key fingerprint(for use with cryptonyms, for example).
        /// </summary>
        /// <returns></returns>
        public override string GetFingerprint()
        {
            // X25519 cryptonyms are multicodec encoded values, specifically:
            // (multicodec('x25519-pub') + key bytes)
            var pubkeyBytes = Multibase.Base58.Decode(PublicKeyBase58 ?? throw new Exception("Public key was not found"));
            // See https://github.com/multiformats/multicodec/blob/master/table.csv
            // 0xec is the value for X25519 public key
            // 0x01 is from varint.encode(0xec) -> [0xec, 0x01]
            // See https://github.com/multiformats/unsigned-varint
            // prefix with `z` to indicate multi-base base58btc encoding
            return $"z{ Multibase.Base58.Encode(new byte[] { 0xec, 0x01 }.Concat(pubkeyBytes).ToArray())}";
        }

        /// <summary>
        /// Generate new <see cref="X25519KeyAgreementKey2019"/> key
        /// </summary>
        /// <param name="id"></param>
        /// <param name="controller"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static X25519KeyAgreementKey2019 GenerateKey(string seed = null, string controller = null)
        {
            var privateKey = seed is null ? Curve25519.CreateRandomPrivateKey() : Curve25519.CreateRandomPrivateKey(seed);
            var publicKey = Curve25519.GetPublicKey(privateKey);

            var key = new X25519KeyAgreementKey2019
            {
                Controller = controller,
                PrivateKeyBase58 = Multibase.Base58.Encode(privateKey),
                PublicKeyBase58 = Multibase.Base58.Encode(publicKey)
            };
            key.Id = controller != null ? $"{controller}#{key.GetFingerprint()}" : $"#{key.GetFingerprint()}";

            return key;
        }
    }
}
