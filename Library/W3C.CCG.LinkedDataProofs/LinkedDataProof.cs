using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Text;
using Newtonsoft.Json.Linq;
using VDS.RDF;
using VDS.RDF.JsonLd;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;
using W3C.CCG.LinkedDataProofs.Purposes;
using W3C.CCG.SecurityVocabulary;

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

    public abstract class LinkedDataSignature : LinkedDataProof
    {
        public LinkedDataSignature(string typeName) : base(typeName)
        {
        }

        public JToken VerificationMethod { get; set; }

        public override async Task<JObject> CreateProofAsync(CreateProofOptions options)
        {
            if (VerificationMethod == null) throw new ArgumentNullException(nameof(VerificationMethod), "VerificationMethod must be specified.");
            if (TypeName == null) throw new ArgumentNullException(nameof(TypeName), "TypeName must be specified.");

            var proof = options.Proof != null
                ? JsonLdProcessor.Compact(options.Proof, Constants.SECURITY_CONTEXT_V2_URL, new JsonLdProcessorOptions())
                : new JObject { { "@context", Constants.SECURITY_CONTEXT_V2_URL } };

            proof["type"] = TypeName;
            proof["created"] = DateTime.Now.ToString("s");
            proof["verificationMethod"] = VerificationMethod;

            var verifyData = CreateVerifyData(proof, options);
            proof = await SignAsync(verifyData, proof, options);

            proof.Remove("@context");
            return proof;
        }

        protected abstract Task<JObject> SignAsync(JToken verifyData, JObject proof, CreateProofOptions options);

        protected virtual JToken CreateVerifyData(JObject proof, CreateProofOptions options)
        {
            var c14nProofOptions = CanonizeProof(proof, options);
            var c14nDocument = Canonize(options.Document, options);

            var sha256 = SHA256.Create();

            return new JArray(sha256.ComputeHash(Encoding.UTF8.GetBytes(String.Join(string.Empty, c14nProofOptions)))
                .Concat(sha256.ComputeHash(Encoding.UTF8.GetBytes(String.Join(string.Empty, c14nDocument)))));
        }

        protected virtual IEnumerable<string> Canonize(JToken document, CreateProofOptions options)
        {
            return Helpers.Canonize(document, new JsonLdProcessorOptions());
        }

        protected virtual IEnumerable<string> CanonizeProof(JObject proof, CreateProofOptions options)
        {
            proof = proof.DeepClone() as JObject;

            proof.Remove("jws");
            proof.Remove("signatureValue");
            proof.Remove("proofValue");

            return Canonize(proof, options);
        }
    }

    public class LdSignatures
    {
        public static async Task<JToken> SignAsync(JToken document, SignatureOptions options)
        {
            var proof = await options.Suite.CreateProofAsync(new CreateProofOptions
            {
                Document = document,
                Purpose = options.Purpose,
                Suite = options.Suite
            });

            document["proof"] = proof;

            return document;
        }

        public static Task<VerifyProofResult> VerifyAsync(JToken document, SignatureOptions options)
        {
            return Task.FromResult(new VerifyProofResult());
        }
    }

    public class SignatureOptions
    {
        public LinkedDataProof Suite { get; set; }

        public ProofPurpose Purpose { get; set; }

        public bool CompactProof { get; set; }

        public IDocumentLoader DocumentLoader { get; set; }
    }
}
