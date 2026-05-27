namespace Kliniq.Api.Models
{
    public class SubmitAccountRequestForm
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;

        public List<string> Specializations { get; set; } = [];

        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public double ClinicLatitude { get; set; }
        public double ClinicLongitude { get; set; }

        public IFormFile PrcLicense { get; set; } = default!;
        public IFormFile GovernmentId { get; set; } = default!;
        public IFormFile ProfessionalPhoto { get; set; } = default!;
        public IFormFile Cv { get; set; } = default!;
    }
}
