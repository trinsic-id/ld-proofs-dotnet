using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace W3C.CCG.DidCore
{
    public class ServiceEndpoint : JObject
    {
        public string Id
        {
            get => this["id"]?.Value<string>();
            set => this["id"] = value;
        }

        public string ServiceEndpointType
        {
            get => this["type"]?.Value<string>();
            set => this["type"] = value;
        }

        [JsonProperty("serviceEndpoint")]
        public IEndpoint Endpoint
        {
            get;
            set;
        }
    }

    public interface IEndpoint
    {
    }

    public class EndpointUri : Uri, IEndpoint
    {
        public EndpointUri(string uriString) : base(uriString)
        {
        }

        public EndpointUri(Uri baseUri, string relativeUri) : base(baseUri, relativeUri)
        {
        }
    }

    public class Endpoint : JObject, IEndpoint
    {
    }
}