using System;
using System.Runtime.Serialization;

namespace LinkedDataProofs.Purposes
{
    public class ProofValidationException : Exception
    {
        public ProofValidationException()
        {
        }

        public ProofValidationException(string message) : base(message)
        {
        }

        public ProofValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ProofValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
