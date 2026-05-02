using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kliniq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePractitionerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SChedules_Practioners_PractitionerId",
                table: "SChedules");

            migrationBuilder.DropTable(
                name: "Practioners");

            migrationBuilder.CreateTable(
                name: "Practitioners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClinicID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Practitioners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Practitioners_Clinics_ClinicID",
                        column: x => x.ClinicID,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Practitioners_ClinicID",
                table: "Practitioners",
                column: "ClinicID");

            migrationBuilder.CreateIndex(
                name: "IX_Practitioners_UserId",
                table: "Practitioners",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SChedules_Practitioners_PractitionerId",
                table: "SChedules",
                column: "PractitionerId",
                principalTable: "Practitioners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SChedules_Practitioners_PractitionerId",
                table: "SChedules");

            migrationBuilder.DropTable(
                name: "Practitioners");

            migrationBuilder.CreateTable(
                name: "Practioners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClinicID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Practioners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Practioners_Clinics_ClinicID",
                        column: x => x.ClinicID,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Practioners_ClinicID",
                table: "Practioners",
                column: "ClinicID");

            migrationBuilder.CreateIndex(
                name: "IX_Practioners_UserId",
                table: "Practioners",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SChedules_Practioners_PractitionerId",
                table: "SChedules",
                column: "PractitionerId",
                principalTable: "Practioners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
