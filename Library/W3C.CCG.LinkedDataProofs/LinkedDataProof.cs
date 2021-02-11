using System;
using System.IO;
using System.Threading.Tasks;
using AngleSharp.Text;
using Newtonsoft.Json.Linq;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;
using W3C.CCG.LinkedDataProofs.Purposes;

namespace W3C.CCG.LinkedDataProofs
{
    //public class LinkedDataProof : JObject
    //{
    //    public LinkedDataProof()
    //    {
    //        Context = Constants.SECURITY_CONTEXT_V2_URL;
    //    }

    //    public LinkedDataProof(JObject obj) : base(obj)
    //    {
    //    }

    //    public LinkedDataProof(string json) : this(Parse(json))
    //    {
    //    }

    //    protected void EnhanceContext(JToken context)
    //    {
    //        if (Context is null)
    //        {
    //            Context = context;
    //            return;
    //        }

    //        switch (Context)
    //        {
    //            case JValue _:
    //            case JObject _:
    //                Context = new JArray
    //                {
    //                    Context,
    //                    context
    //                };
    //                break;
    //            case JArray jarr:
    //                jarr.Add(context);
    //                break;
    //            default:
    //                throw new Exception("Unknown context type");
    //        }
    //    }

    //    public JToken Context
    //    {
    //        get => this["@context"];
    //        set => this["@context"] = value;
    //    }

    //    public string TypeName
    //    {
    //        get => this["type"]?.Value<string>();
    //        set => this["type"] = value;
    //    }

    //    public string ProofPurpose
    //    {
    //        get => this["proofPurpose"]?.Value<string>();
    //        set => this["proofPurpose"] = value;
    //    }

    //    public DateTimeOffset? Created
    //    {
    //        get => this["created"]?.Value<DateTimeOffset>();
    //        set => this["created"] = value;
    //    }

    //    public string Creator
    //    {
    //        get => this["creator"]?.Value<string>();
    //        set => this["creator"] = value;
    //    }

    //    public string Capability
    //    {
    //        get => this["capability"]?.Value<string>();
    //        set => this["capability"] = value;
    //    }

    //    public string VerificationMethod
    //    {
    //        get => this["verificationMethod"]?.Value<string>();
    //        set => this["verificationMethod"] = value;
    //    }
    //}

    public abstract class LinkedDataProof
    {
        public LinkedDataProof(string typeName)
        {
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName), "Type name must be specified.");
        }

        public string TypeName { get; }

        public abstract Task<JObject> CreateProofAsync(CreateProofOptions options);

        public abstract Task<VerifyProofResult> VerifyProofAsync(JToken proof, VerifyProofOptions options);

        public virtual Task<bool> MatchProofAsync(MatchProofOptions options)
        {
            return Task.FromResult(options.TypeName == TypeName);
        }
    }

    public class MatchProofOptions
    {
        public string TypeName { get; set; }
    }

    public class VerifyProofResult
    {

    }

    public class SignatureOptions
    {
        public LinkedDataProof Suite { get; set; }

        public ProofPurpose Purpose { get; set; }

        public bool CompactProof { get; set; }

        public IDocumentLoader DocumentLoader { get; set; }
    }
}
