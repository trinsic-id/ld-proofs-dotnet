using System;
using System.Collections.Generic;
using System.Linq;
using W3C.CCG.DidCore;

namespace W3C.CCG.AuthorizationCapabilities
{
    public static class CapabilityExtensions
    {
        /// <summary>
        /// Retrieves the authorized delegators from a capability.
        /// </summary>
        /// <param name="capability">The JSON-LD document for the object capability
        /// compacted to the security context.</param>
        /// <returns>The delegators for the capability (empty for none).</returns>
        public static IEnumerable<string> GetDelegators(this CapabilityDelegation capability)
        {
            // if neither a delegator, controller, nor id is found on the capability then
            // the capability can not be delegated
            if (capability.Delegator is null || capability.Id is null || capability.Controller is null)
            {
                throw new Exception("Delegator not found for capability.");
            }

            // if there's an invoker present and not a delegator, then this capability
            // was intentionally meant to not be delegated
            if (capability.Invoker != null && capability.Delegator is null)
            {
                return Array.Empty<string>();
            }

            return new[] { capability.Delegator ?? capability.Controller ?? capability.Id };
        }

        /// <summary>
        /// Retrieves the authorized invokers from a capability.
        /// </summary>
        /// <param name="capability">The JSON-LD document for the object capability
        /// compacted to the security context.</param>
        /// <returns>The invokers for the capability (empty for none).</returns>
        public static IEnumerable<string> GetInvokers(this CapabilityDelegation capability)
        {
            // if neither a delegator, controller, nor id is found on the capability then
            // the capability can not be delegated
            if (capability.Invoker is null || capability.Id is null || capability.Controller is null)
            {
                throw new Exception("Delegator not found for capability.");
            }

            // if there's a delegator present and not an invoker, then this capability
            // was intentionally meant to not be invoked
            if (capability.Delegator != null && capability.Invoker is null)
            {
                return Array.Empty<string>();
            }

            return new[] { capability.Invoker ?? capability.Controller ?? capability.Id };
        }

        /// <summary>
        /// Returns true if the given verification method is a delegator or is
        /// controlled by a delegator of the given capability.
        /// </summary>
        /// <param name="capability"></param>
        /// <param name="verificationMethod"></param>
        /// <returns></returns>
        public static bool HasDelegator(this CapabilityDelegation capability, VerificationMethod verificationMethod)
        {
            var delegators = capability.GetDelegators();

            return delegators.Count() > 0 &&
              (delegators.Contains(verificationMethod.Id) ||
              (verificationMethod.Controller != null && delegators.Contains(verificationMethod.Controller)));
        }

        /// <summary>
        /// Returns true if the given verification method is a invoker or is
        /// controlled by an invoker of the given capability.
        /// </summary>
        /// <param name="capability"></param>
        /// <param name="verificationMethod"></param>
        /// <returns></returns>
        public static bool HasInvoker(this CapabilityDelegation capability, VerificationMethod verificationMethod)
        {
            var invokers = capability.GetInvokers();

            return invokers.Count() > 0 &&
              (invokers.Contains(verificationMethod.Id) ||
              (verificationMethod.Controller != null && invokers.Contains(verificationMethod.Controller)));
        }

        /// <summary>
        /// Retrieves the target from a capability.
        /// </summary>
        /// <param name="capability"></param>
        /// <returns></returns>
        public static string GetTarget(this CapabilityDelegation capability)
        {
            return capability.InvocationTarget ?? capability.Id;
        }
    }
}
