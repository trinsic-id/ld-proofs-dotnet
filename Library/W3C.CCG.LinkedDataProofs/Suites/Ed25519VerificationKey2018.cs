﻿using Multiformats.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using W3C.CCG.DidCore;

namespace W3C.CCG.LinkedDataProofs.Suites
{
    public class Ed25519VerificationKey2018 : SignerVerificationMethod
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

        /// <summary>
        /// Sign a payload using this method's private key
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override byte[] Sign(byte[] input)
        {
            if (PrivateKeyBase58 == null)
            {
                throw new Exception("Private key not found.");
            }

            return Chaos.NaCl.Ed25519.Sign(input, Multibase.Base58.Decode(PrivateKeyBase58));
        }

        /// <summary>
        /// Gets the public data for this method
        /// </summary>
        /// <returns></returns>
        public override VerificationMethod GetPublicNode()
        {
            var clone = DeepClone();
            clone.Remove("privateKeyBase58");

            return new Ed25519VerificationKey2018(clone as JObject);
        }

        /// <summary>
        /// Verify a signature using this method's public key
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Verify(byte[] signature, byte[] input)
        {
            if (PublicKeyBase58 == null)
            {
                throw new Exception("Public key not found.");
            }

            return Chaos.NaCl.Ed25519.Verify(signature, input, Multibase.Base58.Decode(PublicKeyBase58));
        }

        /// <summary>
        /// Generate new random key
        /// </summary>
        /// <returns></returns>
        public static Ed25519VerificationKey2018 Generate()
        {
            var edKey = Chaos.NaCl.Ed25519KeyPair.Generate();

            return new Ed25519VerificationKey2018
            {
                PublicKeyBase58 = Multibase.Base58.Encode(edKey.PublicKey.Array),
                PrivateKeyBase58 = Multibase.Base58.Encode(edKey.PrivateKey.Value.Array),
            };
        }
    }
}