using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;
using LinkedDataProofs;
using LinkedDataProofs.Purposes;

namespace W3C.CCG.AuthorizationCapabilities
{
    public class CapabilityDelegationProofPurpose : ControllerPurpose
    {
        public CapabilityDelegationProofPurpose() : base("capabilityDelegation")
        {
        }

        public CapabilityDelegationProofPurpose(IEnumerable<string> capabilityChain) : base("capabilityDelegation")
        {
            CapabilityChain = capabilityChain;
        }

        public IEnumerable<string> CapabilityChain { get; set; }
        public JToken VerifiedParentCapability { get; internal set; }

        public override async Task<JObject> UpdateAsync(JObject proof, ProofOptions options)
        {
            proof = await base.UpdateAsync(proof, options);

            // no capability chain given, attempt to compute from parent
            if (CapabilityChain == null)
            {
                var processorOptions = new JsonLdProcessorOptions
                {
                    CompactToRelative = false,
                    DocumentLoader = options.DocumentLoader.Load
                };

                var capability = await Utils.FetchInSecurityContextAsync(
                    options.Input, false, processorOptions);
                CapabilityChain = await Utils.ComputeCapabilityChainAsync(
                    capability, processorOptions);
            }

            proof["capabilityChain"] = new JArray(CapabilityChain);
            return proof;
        }
    }
}
