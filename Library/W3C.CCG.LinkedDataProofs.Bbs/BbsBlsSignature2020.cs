using Newtonsoft.Json.Linq;
using W3C.CCG.LinkedDataProofs;

namespace BbsDataSignatures
{
    public class BbsBlsSignature2020 : LinkedDataProof
    {
        public const string Name = "BbsBlsSignature2020";

        public BbsBlsSignature2020()
        {
            TypeName = Name;
            Context = "https://w3c-ccg.github.io/ldp-bbs2020/context/v1";
        }

        public BbsBlsSignature2020(JObject obj) : base(obj)
        {
        }

        public string ProofValue
        {
            get => this["proofValue"]?.Value<string>();
            set => this["proofValue"] = value;
        }
    }
}
