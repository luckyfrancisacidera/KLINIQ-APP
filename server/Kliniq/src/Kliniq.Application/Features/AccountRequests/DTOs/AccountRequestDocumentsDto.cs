namespace Kliniq.Application.Features.AccountRequests.DTOs
{
    public class AccountRequestDocumentsDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string PrcIdPath { get; set; } = string.Empty;
        public string BoardCertificatePath { get; set; } = string.Empty;
        public string MedicalDiplomaPat { get; set; } = string.Empty;
        public string CertificateOfGoodStandingPath { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; } 
    }
}
