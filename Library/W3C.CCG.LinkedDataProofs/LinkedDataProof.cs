using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Text;
using Newtonsoft.Json.Linq;
using VDS.RDF;
using VDS.RDF.JsonLd;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;
using W3C.CCG.SecurityVocabulary;

namespace W3C.CCG.LinkedDataProofs
{
    public abstract class LinkedDataProof : JObject
    {
        public LinkedDataProof()
        {
            Context = Constants.SECURITY_CONTEXT_V2_URL;
        }

        public LinkedDataProof(JObject obj) : base(obj)
        {
        }

        public LinkedDataProof(string json) : this(Parse(json))
        {
        }

        protected void EnhanceContext(JToken context)
        {
            if (Context is null)
            {
                Context = context;
                return;
            }

            switch (Context)
            {
                case JValue _:
                case JObject _:
                    Context = new JArray
                    {
                        Context,
                        context
                    };
                    break;
                case JArray jarr:
                    jarr.Add(context);
                    break;
                default:
                    throw new Exception("Unknown context type");
            }
        }

        public JToken Context
        {
            get => this["@context"];
            set => this["@context"] = value;
        }

        public string TypeName
        {
            get => this["type"]?.Value<string>();
            set => this["type"] = value;
        }

        public string ProofPurpose
        {
            get => this["proofPurpose"]?.Value<string>();
            set => this["proofPurpose"] = value;
        }

        public DateTimeOffset? Created
        {
            get => this["created"]?.Value<DateTimeOffset>();
            set => this["created"] = value;
        }

        public string VerificationMethod
        {
            get => this["verificationMethod"]?.Value<string>();
            set => this["verificationMethod"] = value;
        }
    }
}
