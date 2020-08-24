using System;
using System.Linq;
using BbsDataSignatures;
using Hyperledger.Ursa.BbsSignatures;
using Multiformats.Base;

namespace BbsDataSignatures
{
    public static class BlsKeyPairExtensions
    {
        public static Bls12381VerificationKey2020 ToVerificationMethod(this BlsKeyPair keyPair, string id = null, string controller = null)
        {
            var method = new Bls12381VerificationKey2020
            {
                PublicKeyBase58 = Multibase.Base58.Encode(keyPair.PublicKey.ToArray()),
                PrivateKeyBase58 = keyPair.SecretKey is null ? null : Multibase.Base58.Encode(keyPair.SecretKey.ToArray())
            };

            if (id != null) method.Id = id;
            if (controller != null) method.Controller = controller;

            return method;
        }
    }
}