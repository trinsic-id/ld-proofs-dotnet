﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using W3C.CCG.DidCore;

namespace W3C.CCG.LinkedDataProofs.Purposes
{
    public abstract class ProofPurpose
    {
        public ProofPurpose(string term)
        {
            Term = term ?? throw new ArgumentNullException(nameof(term), "this field is required");
        }

        public string Term { get; }

        public virtual Task<ValidationResult> ValidateAsync(JToken proof, ValidateOptions options)
        {
            return Task.FromResult(new ValidationResult { Valid = proof["proofPurpose"].Equals(Term) });
        }

        public virtual JObject Update(JObject proof)
        {
            proof["proofPurpose"] = Term;
            return proof;
        }

        public virtual bool Match(JObject proof)
        {
            return true;
        }
    }

    public class ValidationResult
    {
        public string Controller { get; set; }
        public bool Valid { get; set; }
        public string Invoker { get; set; }
    }

    public class ValidateOptions
    {
        public VerificationMethod VerificationMethod { get; set; }
        public IDocumentLoader DocumentLoader { get; set; }
    }
}
