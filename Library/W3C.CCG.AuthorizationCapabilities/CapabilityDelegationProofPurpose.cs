using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using W3C.CCG.LinkedDataProofs.Purposes;

namespace W3C.CCG.AuthorizationCapabilities
{
    public class CapabilityDelegationProofPurpose : ControllerPurpose
    {
        public CapabilityDelegationProofPurpose() : base("capabilityDelegation")
        {
        }

        public PurposeOptions Options { get; set; } = new PurposeOptions();

        public IEnumerable<string> CapabilityChain { get; }
        public JToken VerifiedParentCapability { get; internal set; }

        public override JObject Update(JObject proof)
        {
            proof = base.Update(proof);

            proof["capabilityChain"] = new JArray(CapabilityChain);
            return proof;
        }
    }
}
