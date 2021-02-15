using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using W3C.CCG.DidCore;

namespace W3C.CCG.LinkedDataProofs
{
    public interface ISigner
    {
        byte[] Sign(IVerifyData input);

        bool Verify(byte[] signature, IVerifyData input);
    }
}
