namespace Kliniq.Api.Models
{
    public class SubmitAccountRequestForm
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public IFormFile? PrcId { get; set; }
        public IFormFile? BoardCertificate { get; set; }
        public IFormFile? MedicalDiploma { get; set; }
        public IFormFile? CertificateOfGoodStanding { get; set; }
    }
}
