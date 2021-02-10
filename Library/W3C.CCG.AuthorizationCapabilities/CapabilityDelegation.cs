using Newtonsoft.Json.Linq;
using W3C.CCG.LinkedDataProofs.Purposes;
using W3C.CCG.SecurityVocabulary;

namespace W3C.CCG.AuthorizationCapabilities
{
    public class CapabilityDelegation : JObject
    {
        public CapabilityDelegation()
        {
            Context = Constants.SECURITY_CONTEXT_V2_URL;
        }

        public CapabilityDelegation(JObject obj) : base(obj)
        {
        }

        public JToken Context
        {
            get => this["@context"];
            set => this["@context"] = value;
        }

        public string Id
        {
            get => this["id"]?.Value<string>();
            set => this["id"] = value;
        }

        public string ParentCapability
        {
            get => this["parentCapability"]?.Value<string>();
            set => this["parentCapability"] = value;
        }

        public string Invoker
        {
            get => this["invoker"]?.Value<string>();
            set => this["invoker"] = value;
        }

        public string Target
        {
            get => this["target"]?.Value<string>();
            set => this["target"] = value;
        }

        public string Delegator
        {
            get => this["delegator"]?.Value<string>();
            set => this["delegator"] = value;
        }

        public JToken Caveat
        {
            get => this["caveat"];
            set => this["caveat"] = value;
        }

        public JToken Proof
        {
            get => this["proof"];
            set => this["proof"] = value;
        }
    }
}
