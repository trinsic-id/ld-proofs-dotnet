using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using W3C.CCG.DidCore;

namespace W3C.VerifiableCredentials
{
    /// <summary>
    /// Verifiable Credential Data Model
    /// </summary>
    public class VerifiableCredential : JObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VerifiableCredential"/> class
        /// </summary>
        public VerifiableCredential()
        {
            Context = new JArray { "https://www.w3.org/2018/credentials/v1" };
            TypeName = new JArray { "VerifiableCredential" };
        }

        /// <summary>
        /// The value of the @context property MUST be an ordered set where the first item is a URI
        /// with the value https://www.w3.org/2018/credentials/v1. For reference, a copy of the
        /// base context is provided in Appendix § B. Base Context - https://www.w3.org/TR/vc-data-model/#base-context.
        /// Subsequent items in the array
        /// MUST express context information and be composed of any combination of URIs or objects.
        /// It is RECOMMENDED that each URI in the @context be one which, if dereferenced, results
        /// in a document containing machine-readable information about the @context. 
        /// </summary>
        public JArray Context
        {
            get => this["@context"] as JArray ?? throw new Exception("Invalid '@context' property");
            set => this["@context"] = value;
        }

        public string Id
        {
            get => this["id"]?.Value<string>();
            set => this["id"] = value;
        }

        public JArray TypeName
        {
            get => this["type"] as JArray;
            set => this["type"] = value;
        }

        /// <summary>
        /// The value of the issuer property MUST be either a URI or an object containing an id property.
        /// It is RECOMMENDED that the URI in the issuer or its id be one which, if dereferenced,
        /// results in a document containing machine-readable information about the issuer that can be
        /// used to verify the information expressed in the credential. 
        /// </summary>
        /// <example>
        /// <code>"issuer": "https://example.edu/issuers/14"</code>
        /// or can be expressed as object
        /// <code>
        /// "issuer": {
        ///     "id": "did:example:76e12ec712ebc6f1c221ebfeb1f",
        ///     "name": "Example University"
        /// }
        /// </code>
        /// </example>
        public JToken Issuer
        {
            get => this["issuer"];
            set => this["issuer"] = value;
        }

        /// <summary>
        /// The value of the credentialSubject property is defined as a set of objects that contain one
        /// or more properties that are each related to a subject of the verifiable credential.
        /// Each object MAY contain an id, as described in Section § 4.2 Identifiers
        /// https://www.w3.org/TR/vc-data-model/#identifiers. 
        /// </summary>
        /// <example>
        /// <code>
        /// "credentialSubject": {
        ///    "id": "did:example:ebfeb1f712ebc6f1c276e12ec21",
        ///    "degree": {
        ///      "type": "BachelorDegree",
        ///      "name": "Bachelor of Science and Arts"
        ///     }
        /// }
        /// </code>
        /// It is possible to express information related to multiple subjects in a verifiable credential. 
        /// The example below specifies two subjects who are spouses. Note the use of array notation to 
        /// associate multiple subjects with the credentialSubject property. 
        /// <code>
        /// "credentialSubject": [{
        ///    "id": "did:example:ebfeb1f712ebc6f1c276e12ec21",
        ///    "name": "Jayden Doe",
        ///    "spouse": "did:example:c276e12ec21ebfeb1f712ebc6f1"
        /// }, {
        ///    "id": "did:example:c276e12ec21ebfeb1f712ebc6f1",
        ///    "name": "Morgan Doe",
        ///    "spouse": "did:example:ebfeb1f712ebc6f1c276e12ec21"
        /// }]
        /// </code>
        /// </example>
        public JToken CredentialSubject
        {
            get => this["credentialSubject"];
            set => this["credentialSubject"] = value;
        }

        /// <summary>
        /// The value of the credentialStatus property MUST include the:
        /// - id property, which MUST be a URL.
        /// - type property, which expresses the credential status type (also referred to as the credential status method).
        ///     It is expected that the value will provide enough information to determine the current status of the credential.
        ///     For example, the object could contain a link to an external document noting whether or not the credential is suspended or revoked.
        ///
        ///  The precise contents of the credential status information is determined by the specific credentialStatus type definition,
        ///  and varies depending on factors such as whether it is simple to implement or if it is privacy-enhancing.
        /// </summary>
        public JToken CredentialStatus
        {
            get => this["credentialStatus"];
            set => this["credentialStatus"] = value;
        }
    }
}
