using System;
using Microsoft.Extensions.DependencyInjection;
using LinkedDataProofs;
using Xunit;
using W3C.CCG.SecurityVocabulary;

namespace LinkedDataProofs.Bbs.Tests
{
    [CollectionDefinition(CollectionDefinitionName)]
    public class ServiceFixture : ICollectionFixture<ServiceFixture>
    {
        public const string CollectionDefinitionName = "Service Collection";

        public ServiceFixture()
        {
            CachingDocumentLoader.Default
                .AddCached("https://www.w3.org/2018/credentials/v1", Utilities.LoadJson("Data/credential_vocab.jsonld"))
                .AddCached("https://w3id.org/citizenship/v1", Utilities.LoadJson("Data/citizenship-v1.jsonld"))
                .AddCached("did:example:489398593", Utilities.LoadJson("Data/did_example_489398593.json"))
                .AddCached("https://schema.org/", Utilities.LoadJson("Data/schemaorgcontext.jsonld"))
                .AddCached("did:example:489398593#test", Utilities.LoadJson("Data/did_example_489398593_test.json"));
        }
    }
}
