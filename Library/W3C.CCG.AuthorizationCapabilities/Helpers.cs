using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;
using W3C.CCG.SecurityVocabulary;

namespace W3C.CCG.AuthorizationCapabilities
{
    public class Helpers
    {
        /// <summary>
        /// Fetches a JSON-LD document from a URL and, if necessary, compacts it to
        /// the security v2 context.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="isRoot"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task<JObject> FetchInSecurityContextAsync(JToken document, bool isRoot = false, JsonLdProcessorOptions options = null)
        {
            await Task.Yield();

            if (document.Type == JTokenType.Object &&
                document["@context"]?.Value<string>() == Constants.SECURITY_CONTEXT_V2_URL &&
                !isRoot)
            {
                return document as JObject;
            }

            return JsonLdProcessor.Compact(document, Constants.SECURITY_CONTEXT_V2_URL, options);
        }
    }
}
