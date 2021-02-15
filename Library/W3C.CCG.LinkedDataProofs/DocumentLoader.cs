using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;
using W3C.CCG.DidCore;
using W3C.CCG.SecurityVocabulary;

namespace W3C.CCG.LinkedDataProofs
{
    public class CachingDocumentLoader : IDocumentLoader
    {
        public static IDocumentLoader Default { get; set; } = new CachingDocumentLoader(Array.Empty<IDidDriver>())
            .AddCached(Constants.DID_V1_URL, Contexts.DidContextV1)
            .AddCached(Constants.SECURITY_CONTEXT_V1_URL, Contexts.SecurityContextV1)
            .AddCached(Constants.SECURITY_CONTEXT_V2_URL, Contexts.SecurityContextV2)
            .AddCached(Constants.SECURITY_CONTEXT_V3_URL, Contexts.SecurityContextV3);


        public Dictionary<string, RemoteDocument> Documents = new Dictionary<string, RemoteDocument>();
        private readonly IEnumerable<IDidDriver> didDrivers;

        public CachingDocumentLoader(IEnumerable<IDidDriver> didDrivers)
        {
            this.didDrivers = didDrivers;
        }

        public IDocumentLoader AddCached(string uri, JObject document)
        {
            Documents.Add(uri, new RemoteDocument { Document = document });

            return this;
        }

        public RemoteDocument Load(Uri uri, JsonLdLoaderOptions options)
        {
            foreach (var item in didDrivers)
            {
                if (item.CanResolve(uri))
                {
                    var didDocument = item.Resolve(uri);
                    return new RemoteDocument { Document = didDocument };
                }
            }
            if (Documents.TryGetValue(uri.ToString(), out var document))
            {
                return document;
            }
            var doc = DefaultDocumentLoader.LoadJson(uri, options);
            Documents.TryAdd(uri.ToString(), doc);
            return doc;
        }

        public Task<JObject> LoadAsync(string documentUri)
        {
            throw new NotImplementedException();
        }
    }

    public interface IDocumentLoader
    {
        IDocumentLoader AddCached(string uri, JObject document);

        Task<JObject> LoadAsync(string documentUri);

        RemoteDocument Load(Uri uri, JsonLdLoaderOptions options);
    }
}
