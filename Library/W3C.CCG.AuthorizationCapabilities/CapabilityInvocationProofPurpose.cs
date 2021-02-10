using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using W3C.CCG.LinkedDataProofs;
using W3C.CCG.LinkedDataProofs.Purposes;

namespace W3C.CCG.AuthorizationCapabilities
{
    public class CapabilityInvocationProofPurpose : ControllerProofPurpose
    {
        public CapabilityInvocationProofPurpose(string capability, string capabilityAction = null, string controller = null) : base("capabilityInvocation", controller)
        {
            Capability = capability ?? throw new ArgumentNullException(nameof(capability), "Capability must be specified");
            CapabilityAction = capabilityAction;
        }

        public string Capability { get; }
        public string CapabilityAction { get; }

        public override JToken Update(JToken proof)
        {
            proof = base.Update(proof);

            proof["capability"] = Capability;
            if (CapabilityAction != null)
            {
                proof["capabilityAction"] = CapabilityAction;
            }
            return proof;
        }
    }

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
