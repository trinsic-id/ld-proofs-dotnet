using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using W3C.CCG.LinkedDataProofs;
using W3C.CCG.LinkedDataProofs.Purposes;

namespace W3C.CCG.AuthorizationCapabilities
{
    public class PurposeOptions
    {
        public string CapabilityAction { get; set; }
        public string ExpectedAction { get; set; }
        public IJEnumerable<JToken> CapabilityChain { get; set; }
        public string Capability { get; set; }
        public string ExpectedTarget { get; set; }
        public string ExpectedRootCapability { get; set; }
        public LinkedDataProof Suite { get; set; }
    }

    public class CapabilityInvocationProofPurpose : ControllerProofPurpose
    {
        public CapabilityInvocationProofPurpose() : base("capabilityInvocation")
        {
        }

        public PurposeOptions Options { get; set; } = new PurposeOptions();

        public override async Task<ValidationResult> ValidateAsync(JToken proof, ProofOptions options)
        {
            if (proof["capability"] is null)
            {
                throw new Exception("'capability' was not found in the capability invocation proof.");
            }

            if (Options.ExpectedTarget == null)
            {
                throw new ArgumentNullException("ExpectedTarget is required.");
            }

            // 1. get the capability in the security v2 context
            var result = await Helpers.FetchInSecurityContextAsync(proof["capability"], false, null);
            var capability = new CapabilityDelegation(result as JObject);

            // 2. verify the capability delegation chain
            await CapabilityExtensions.VerifyCapabilityChain(capability, Options);

            // 3. verify the invoker...
            // authorized invoker must match the verification method itself OR
            // the controller of the verification method
            //if (capability.HasInvoker(options.AdditonalData["verificationMethod"] as JToken))
            //{
            //    throw new Exception("The authorized invoker does not match the " +
            //        "verification method or its controller.");
            //}

            var validateResult = await base.ValidateAsync(proof, options);

            validateResult.Invoker = validateResult.Controller;

            return validateResult;
        }

        public override JObject Update(JObject proof)
        {
            proof = base.Update(proof);

            proof["capability"] = Options.Capability;
            if (Options.CapabilityAction != null)
            {
                proof["capabilityAction"] = Options.CapabilityAction;
            }
            return proof;
        }
    }
}
