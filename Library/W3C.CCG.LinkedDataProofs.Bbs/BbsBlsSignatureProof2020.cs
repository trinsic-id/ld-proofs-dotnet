using Newtonsoft.Json.Linq;
using W3C.CCG.LinkedDataProofs;

namespace BbsDataSignatures
{
    public class BbsBlsSignatureProof2020 : LinkedDataProof
    {
        public const string Name = "BbsBlsSignatureProof2020";

        public BbsBlsSignatureProof2020() : base()
        {
            TypeName = Name;
            EnhanceContext("https://w3c-ccg.github.io/ldp-bbs2020/context/v1");
        }

        public BbsBlsSignatureProof2020(JObject obj) : base(obj)
        {
        }

        public string Nonce
        {
            get => this["nonce"]?.Value<string>();
            set => this["nonce"] = value;
        }

        public string ProofValue
        {
            get => this["proofValue"]?.Value<string>();
            set => this["proofValue"] = value;
        }
    }
}
