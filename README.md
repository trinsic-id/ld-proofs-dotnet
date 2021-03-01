# Linked Data Proofs for .NET

[![Build, Test & Package](https://github.com/trinsic-id/ld-proofs-dotnet/actions/workflows/ci.yml/badge.svg?branch=master)](https://github.com/trinsic-id/ld-proofs-dotnet/actions/workflows/ci.yml)

.NET Core implementation of [Linked Data Proofs](https://w3c-ccg.github.io/ld-proofs/). This includes complete set of feature for the below specifications:

- [Linked Data Proofs 1.0](https://w3c-ccg.github.io/ld-proofs/)
    - [BBS+ Signatures 2020](https://w3c-ccg.github.io/ldp-bbs2020/)
    - [Ed25519 Signature 2018](https://w3c-ccg.github.io/lds-ed25519-2018/)
    - [JCS Ed25519 Signature 2020](https://identity.foundation/JcsEd25519Signature2020/)
- [Authorization Capabilities for Linked Data v0.3](https://w3c-ccg.github.io/zcap-ld/)
- [Decentralized Identifiers (DIDs) v1.0](https://www.w3.org/TR/did-core/)
    - [The did:key Method v0.7](https://w3c-ccg.github.io/did-method-key/)
- [Verifiable Credentials Data Model 1.0](https://www.w3.org/TR/vc-data-model/)

## Installation

Using Nuget, the following packages are available

- LinkedDataProofs
- LinkedDataProofs.Bbs
- LinkedDataProofs.Zcaps

## Usage

To understand different use case, the best place is to explore the tests found under [Tests](Tests) directory.

### Creating a proof

To create new linked data proof, use the static methods in the `LdSignatures` class.

```cs
// an example document to sign
var document = JObject.Parse(@"{
    'id': 'Alice'
}");

// signer key
var key = Ed25519VerificationKey2018.Generate();

// create proof
var signedDocument = await LdSignatures.SignAsync(
    document,
    new ProofOptions
    {
        Suite = new Ed25519Signature2018
        {
            Signer = key,
            VerificationMethod = key.Id
        },
        Purpose = new AssertionMethodPurpose()
    });
```

This will create a proof and return the fully signed document

```json
{
  "id": "Alice",
  "proof": {
    "type": "Ed25519Signature2018",
    "created": "2021-02-28T22:43:43",
    "verificationMethod": "#key-1",
    "proofPurpose": "assertionMethod",
    "jws": "eyJhbGciOiJFZERTQSIsImI2NCI6ZmFsc2UsImNyaXQiOlsiYjY0Il19..P8VSTUDCxaSHztIbFEGkwqn+KAbEQwGxvhNsqxgnCNJ/BnP7PDTJffHcielev2D7nRP9QK1wdXbkJvMmvumQCQ=="
  }
}
```

### Verifiying a proof

To verify a proof, use the `LdSignatures.VerifyAsync` method with the input document and the required suite.

```cs
var verifyResult = await LdSignatures.VerifyAsync(
    signedDocument,
    new ProofOptions
    {
        Suite = new Ed25519Signature2018(),
        Purpose = new AssertionMethodPurpose()
    });
```