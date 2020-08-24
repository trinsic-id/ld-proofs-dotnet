using BbsDataSignatures;
using Multiformats.Base;
using W3C.CCG.LinkedDataProofs;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LdProofsBuilderExtensions
    {
        public static ILinkedDataProofsBuilder AddBbsSuite(this ILinkedDataProofsBuilder builder)
        {
            builder.Services.AddSingleton<ILinkedDataSuite, BbsBlsSignature2020Suite>();
            builder.Services.AddSingleton<ILinkedDataSuite, BbsBlsSignatureProof2020Suite>();

            return builder;
        }
    }
}

namespace System
{
    public static class StringExtensions
    {
        public static byte[] AsBytesFromBase58(this string message) => Multibase.Base58.Decode(message);
    }
}