using Kliniq.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kliniq.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260711000100_AddAppointmentQueueWorkflow")]
    public sealed class AddAppointmentQueueWorkflow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "QueuedAtUtc",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConsultationStartedAtUtc",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAtUtc",
                table: "Appointments",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "QueuedAtUtc", table: "Appointments");
            migrationBuilder.DropColumn(name: "ConsultationStartedAtUtc", table: "Appointments");
            migrationBuilder.DropColumn(name: "CompletedAtUtc", table: "Appointments");
        }
    }
}
