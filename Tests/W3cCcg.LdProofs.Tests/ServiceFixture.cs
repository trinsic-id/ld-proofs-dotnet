using System;
using Microsoft.Extensions.DependencyInjection;
using W3C.CCG.LinkedDataProofs;
using Xunit;
using W3C.CCG.SecurityVocabulary;

namespace W3cCcg.LdProofs.Tests
{
    [CollectionDefinition(CollectionDefinitionName)]
    public class ServiceFixture : ICollectionFixture<ServiceFixture>
    {
        public const string CollectionDefinitionName = "Service Collection";

        public ServiceFixture()
        {
            var services = new ServiceCollection();
            services.AddLinkedDataProofs();

            Provider = services.BuildServiceProvider();

            Provider.GetRequiredService<IDocumentLoader>()
                .AddCached("https://schema.org", Utilities.LoadJson("Data/schemaorgcontext.jsonld"))
                .AddCached("http://schema.org", Utilities.LoadJson("Data/schemaorgcontext.jsonld"))
                .AddCached("did:example:489398593#test", Utilities.LoadJson("Data/did_example_489398593_test.json"))
                .AddCached("https://w3c-ccg.github.io/ldp-bbs2020/context/v1", Utilities.LoadJson("Data/lds-bbsbls2020-v0.0.json"))
                .AddCached(Constants.SECURITY_CONTEXT_V1_URL, Contexts.SecurityContextV1)
                .AddCached(Constants.SECURITY_CONTEXT_V2_URL, Contexts.SecurityContextV2);
        }

        public ServiceProvider Provider { get; }
    }
}
