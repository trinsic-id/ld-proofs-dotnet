using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;

namespace W3C.CCG.LinkedDataProofs
{
    internal class CustomDocumentLoader : IDocumentLoader
    {
        public Dictionary<Uri, RemoteDocument> Documents = new Dictionary<Uri, RemoteDocument>();

        public IDocumentLoader AddCached(string uri, JObject document)
        {
            Documents.Add(new Uri(uri), new RemoteDocument { Document = document });

            return this;
        }

        public Func<Uri, JsonLdLoaderOptions, RemoteDocument> GetDocumentLoader()
        {
            return (uri, options) =>
            {
                if (Documents.TryGetValue(uri, out var document))
                {
                    return document;
                }
                var doc = DefaultDocumentLoader.LoadJson(uri, options);
                Documents.TryAdd(uri, doc);
                return doc;
            };
        }
    }

    public interface IDocumentLoader
    {
        IDocumentLoader AddCached(string uri, JObject document);

        Func<Uri, JsonLdLoaderOptions, RemoteDocument> GetDocumentLoader();
    }
}
