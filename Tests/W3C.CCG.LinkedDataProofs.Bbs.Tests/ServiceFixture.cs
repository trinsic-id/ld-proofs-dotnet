﻿using System;
using Microsoft.Extensions.DependencyInjection;
using W3C.CCG.LinkedDataProofs;
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
                .AddCached("did:example:489398593", Utilities.LoadJson("Data/did_example_489398593.json"))
                .AddCached("did:example:489398593#test", Utilities.LoadJson("Data/did_example_489398593_test.json"));
        }
    }
}
