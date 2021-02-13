using System;
using Microsoft.Extensions.DependencyInjection;
using W3C.CCG.LinkedDataProofs;
using Xunit;
using W3C.CCG.SecurityVocabulary;
using W3C.CCG.DidCore;
using System.Linq;
using Newtonsoft.Json.Linq;
using W3C.CCG.AuthorizationCapabilities;

namespace W3cCcg.LdProofs.Tests
{
    [CollectionDefinition(CollectionDefinitionName)]
    public class MockDataFixture : ICollectionFixture<MockDataFixture>
    {
        public const string CollectionDefinitionName = "Mock Data Collection";

        public MockDataFixture()
        {
            Alice_Keys = new DidDocument(Utilities.LoadJson("TestData/ed25519-alice-keys.json"));
            Bob_Keys = new DidDocument(Utilities.LoadJson("TestData/ed25519-bob-keys.json"));
            Diana_Keys = new DidDocument(Utilities.LoadJson("TestData/ed25519-diana-keys.json"));

            ExampleDoc = Utilities.LoadJson("TestData/example-doc.json");
            ExampleDocAlphaInvocation = Utilities.LoadJson("TestData/example-doc-with-alpha-invocation.json");

            RootCapAlpha = new CapabilityDelegation
            {
                Context = Constants.SECURITY_CONTEXT_V2_URL,
                Id = "https://example.org/alice/caps#1",
                Invoker = "https://example.com/i/alice/keys/1",
                Delegator = "https://example.com/i/alice/keys/1"
            };

            DocumentLoader = new CachingDocumentLoader(Array.Empty<IDidDriver>())
                .AddCached(Constants.DID_V1_URL, Contexts.DidContextV1)
                .AddCached(Constants.SECURITY_CONTEXT_V1_URL, Contexts.SecurityContextV1)
                .AddCached(Constants.SECURITY_CONTEXT_V2_URL, Contexts.SecurityContextV2)
                .AddCached(Alice_Keys.Id, Alice_Keys)
                .AddCached((Alice_Keys.VerificationMethod.First() as VerificationMethod).Id, Alice_Keys)
                .AddCached(Bob_Keys.Id, Bob_Keys)
                .AddCached((Bob_Keys.CapabilityDelegation.First() as VerificationMethod).Id, Bob_Keys)
                .AddCached((Bob_Keys.CapabilityInvocation.First() as VerificationMethod).Id, Bob_Keys)
                .AddCached(Diana_Keys.Id, Diana_Keys)
                .AddCached((Diana_Keys.VerificationMethod.First() as VerificationMethod).Id, Diana_Keys)
                .AddCached((Diana_Keys.CapabilityDelegation.First() as VerificationMethod).Id, Diana_Keys)
                .AddCached(ExampleDoc["id"].ToString(), ExampleDoc)
                .AddCached(RootCapAlpha.Id, RootCapAlpha);
        }

        public DidDocument Alice_Keys { get; }
        public DidDocument Bob_Keys { get; }
        public DidDocument Diana_Keys { get; }
        public JObject ExampleDoc { get; }
        public JObject ExampleDocAlphaInvocation { get; }
        public IDocumentLoader DocumentLoader { get; }
        public CapabilityDelegation RootCapAlpha { get; }
    }
}
