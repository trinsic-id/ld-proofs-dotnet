using System.Threading.Tasks;

namespace W3C.CCG.LinkedDataProofs.Purposes
{
    public class ControllerProofPurpose : ProofPurpose
    {
        public ControllerProofPurpose(string term, string controller) : base(term)
        {
            Controller = controller;
        }

        public string Controller { get; }

        public override async Task ValidateAsync()
        {
            await base.ValidateAsync();
        }
    }
}
