using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kliniq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountRequestShape : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "AccountRequests",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "AccountRequests");
        }
    }
}
