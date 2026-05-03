using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kliniq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountRequestSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LicenseNumber",
                table: "AccountRequests",
                newName: "Country");

            migrationBuilder.AddColumn<string>(
                name: "BoardCertificatePath",
                table: "AccountRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CertificateOfGoodStandingPath",
                table: "AccountRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "AccountRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "InvitationExpiresAt",
                table: "AccountRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvitationToken",
                table: "AccountRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInvitationUsed",
                table: "AccountRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MedicalDiplomaPath",
                table: "AccountRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrcIdPath",
                table: "AccountRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "AccountRequests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRequests_InvitationToken",
                table: "AccountRequests",
                column: "InvitationToken",
                unique: true,
                filter: "[InvitationToken] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountRequests_InvitationToken",
                table: "AccountRequests");

            migrationBuilder.DropColumn(
                name: "BoardCertificatePath",
                table: "AccountRequests");

            migrationBuilder.DropColumn(
                name: "CertificateOfGoodStandingPath",
                table: "AccountRequests");

            migrationBuilder.DropColumn(
                name: "City",
                table: "AccountRequests");

            migrationBuilder.DropColumn(
                name: "InvitationExpiresAt",
                table: "AccountRequests");

            migrationBuilder.DropColumn(
                name: "InvitationToken",
                table: "AccountRequests");

            migrationBuilder.DropColumn(
                name: "IsInvitationUsed",
                table: "AccountRequests");

            migrationBuilder.DropColumn(
                name: "MedicalDiplomaPath",
                table: "AccountRequests");

            migrationBuilder.DropColumn(
                name: "PrcIdPath",
                table: "AccountRequests");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "AccountRequests");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "AccountRequests",
                newName: "LicenseNumber");
        }
    }
}
