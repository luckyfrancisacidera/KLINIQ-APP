namespace Kliniq.Api.Models.Requests
{
    public class SubmitAccountRequestRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public IFormFile PrcId { get; set; } = null!;
        public IFormFile BoardCertificate { get; set; } = null!;
        public IFormFile MedicalDiploma { get; set; } = null!;
        public IFormFile CertificateOfGoodStanding { get; set; } = null!;
    }
}
