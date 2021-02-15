using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace W3C.CCG.SecurityVocabulary
{
    public class Contexts
    {
        public static JObject SecurityContextV1 = LoadFromResource("W3C.CCG.SecurityVocabulary.Data.security-v1.jsonld");

        public static JObject SecurityContextV2 = LoadFromResource("W3C.CCG.SecurityVocabulary.Data.security-v2.jsonld");

        public static JObject SecurityContextV3 = LoadFromResource("W3C.CCG.SecurityVocabulary.Data.security-v3-unstable.jsonld");

        public static JObject DidContextV1 = LoadFromResource("W3C.CCG.SecurityVocabulary.Data.did-v1.jsonld");

        private static JObject LoadFromResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var resourceStream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(resourceStream);

            return JObject.Parse(reader.ReadToEnd());
        }
    }
}
