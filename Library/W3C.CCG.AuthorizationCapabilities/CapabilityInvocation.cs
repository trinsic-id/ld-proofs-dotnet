using System;
using Newtonsoft.Json.Linq;

namespace W3C.CCG.AuthorizationCapabilities
{
    public class CapabilityInvocation : JObject
    {
        public CapabilityInvocation()
        {
        }

        public CapabilityInvocation(JObject other) : base(other)
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

        public string Action
        {
            get => this["action"]?.Value<string>();
            set => this["action"] = value;
        }

        public JToken Proof
        {
            get => this["proof"];
            set => this["proof"] = value;
        }
    }
}
