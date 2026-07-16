using Kliniq.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kliniq.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260710000100_PreventAppointmentDoubleBooking")]
    public sealed class PreventAppointmentDoubleBooking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_PractitionerId_ScheduledAt",
                table: "Appointments");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PractitionerId_ScheduledAt",
                table: "Appointments",
                columns: new[] { "PractitionerId", "ScheduledAt" },
                unique: true,
                filter: "[Status] <> 'Cancelled'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_PractitionerId_ScheduledAt",
                table: "Appointments");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PractitionerId_ScheduledAt",
                table: "Appointments",
                columns: new[] { "PractitionerId", "ScheduledAt" });
        }
    }
}
