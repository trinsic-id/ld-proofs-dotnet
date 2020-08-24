
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AngleSharp.Text;
using Newtonsoft.Json.Linq;
using VDS.RDF;
using VDS.RDF.JsonLd;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace W3C.CCG.LinkedDataProofs
{
    public static class Helpers
    {
        public static IEnumerable<string> ToRdf(JToken token, JsonLdProcessorOptions options)
        {
            var jsonLdParser = new JsonLdParser(options);
            var store = new TripleStore();
            jsonLdParser.Load(store, new StringReader(token.ToString(Newtonsoft.Json.Formatting.None)));

            var nqWriter = new NQuadsWriter(NQuadsSyntax.Rdf11);
            using var expectedTextWriter = new System.IO.StringWriter();
            nqWriter.Save(store, expectedTextWriter);
            return expectedTextWriter.ToString().Split(Environment.NewLine).Where(x => !string.IsNullOrWhiteSpace(x));
        }

        public static IEnumerable<string> Canonize(JToken token, JsonLdProcessorOptions options)
        {
            return ToRdf(token, options)
                .Select(x => x.StartsWith("_:b") ? x.ReplaceFirst("_:b", "_:c14n") : x)
                .OrderBy(x => x)
                .Select(x => x.Replace("^^<http://www.w3.org/2001/XMLSchema#string>", ""));
        }

        public static JToken FromRdf(IEnumerable<string> statements)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item in statements)
            {
                stringBuilder.AppendLine(item);
            }
            var reader = new StringReader(stringBuilder.ToString());

            var store = new TripleStore();
            var parser = new NQuadsParser(NQuadsSyntax.Rdf11);
            parser.Load(store, reader);

            var ldWriter = new JsonLdWriter();
            var stringWriter = new System.IO.StringWriter();
            ldWriter.Save(store, stringWriter);

            return JToken.Parse(stringWriter.ToString());
        }
    }
}