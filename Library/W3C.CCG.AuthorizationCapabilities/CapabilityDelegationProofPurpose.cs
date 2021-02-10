using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using W3C.CCG.LinkedDataProofs.Purposes;

namespace W3C.CCG.AuthorizationCapabilities
{
    public class CapabilityDelegationProofPurpose : ControllerProofPurpose
    {
        public CapabilityDelegationProofPurpose(IEnumerable<string> capabilityChain, string controller) : base("capabilityDelegation", controller)
        {
            CapabilityChain = capabilityChain;
        }

        public IEnumerable<string> CapabilityChain { get; }

        public override JToken Update(JToken proof)
        {
            proof = base.Update(proof);

            proof["capabilityChain"] = new JArray(CapabilityChain);
            return proof;
        }
    }
}
