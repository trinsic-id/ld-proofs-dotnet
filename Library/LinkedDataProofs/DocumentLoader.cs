using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;
using W3C.CCG.DidCore;
using W3C.CCG.SecurityVocabulary;

namespace LinkedDataProofs
{
    public class CachingDocumentLoader : IDocumentLoader, ICloneable
    {
        public static IDocumentLoader Default { get; set; } = new CachingDocumentLoader()
            .AddCached(Constants.DID_V1_URL, Contexts.DidContextV1)
            .AddCached(Constants.SECURITY_CONTEXT_V1_URL, Contexts.SecurityContextV1)
            .AddCached(Constants.SECURITY_CONTEXT_V2_URL, Contexts.SecurityContextV2)
            .AddCached(Constants.SECURITY_CONTEXT_V3_URL, Contexts.SecurityContextV3);


        public Dictionary<string, RemoteDocument> Documents = new Dictionary<string, RemoteDocument>();
        private readonly IEnumerable<IDocumentProvider> documentProviders;

        public CachingDocumentLoader(IEnumerable<IDocumentProvider> documentProviders)
        {
            this.documentProviders = documentProviders;
        }

        public CachingDocumentLoader() : this(Array.Empty<IDocumentProvider>())
        {
        }

        public IDocumentLoader AddCached(string uri, JObject document)
        {
            Documents.Add(uri, new RemoteDocument { Document = document });

            return this;
        }

        public RemoteDocument Load(Uri uri, JsonLdLoaderOptions options)
        {
            foreach (var item in documentProviders)
            {
                if (item.CanResolve(uri.ToString()))
                {
                    var didDocument = item.ResolveAsync(uri.ToString()).Result;
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

        public object Clone()
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
