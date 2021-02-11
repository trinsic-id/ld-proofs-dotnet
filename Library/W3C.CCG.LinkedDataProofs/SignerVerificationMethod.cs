using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using W3C.CCG.DidCore;

namespace W3C.CCG.LinkedDataProofs
{
    public abstract class SignerVerificationMethod : VerificationMethod
    {
        protected SignerVerificationMethod()
        {
        }

        protected SignerVerificationMethod(JObject obj) : base(obj)
        {
        }

        public virtual byte[] Sign(byte[] input)
        {
            throw new NotImplementedException();
        }

        public virtual bool Verify(byte[] signature, byte[] input)
        {
            throw new NotImplementedException();
        }
    }
}
