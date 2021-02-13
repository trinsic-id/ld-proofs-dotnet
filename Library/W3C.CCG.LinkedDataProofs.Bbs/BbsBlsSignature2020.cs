using Newtonsoft.Json.Linq;
using W3C.CCG.LinkedDataProofs;

namespace BbsDataSignatures
{
    public class BbsBlsSignature2020 : SignerVerificationMethod
    {
        public const string Name = "BbsBlsSignature2020";

        public BbsBlsSignature2020()
        {
            TypeName = Name;
            this["@context"] = "https://w3c-ccg.github.io/ldp-bbs2020/contexts/v1";
        }

        public BbsBlsSignature2020(JObject obj) : base(obj)
        {
        }
    }
}
