using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kliniq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleBreaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleBreak_Schedules_ScheduleId",
                table: "ScheduleBreak");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleBreak",
                table: "ScheduleBreak");

            migrationBuilder.RenameTable(
                name: "ScheduleBreak",
                newName: "ScheduleBreaks");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleBreak_ScheduleId",
                table: "ScheduleBreaks",
                newName: "IX_ScheduleBreaks_ScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleBreaks",
                table: "ScheduleBreaks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleBreaks_Schedules_ScheduleId",
                table: "ScheduleBreaks",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleBreaks_Schedules_ScheduleId",
                table: "ScheduleBreaks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleBreaks",
                table: "ScheduleBreaks");

            migrationBuilder.RenameTable(
                name: "ScheduleBreaks",
                newName: "ScheduleBreak");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleBreaks_ScheduleId",
                table: "ScheduleBreak",
                newName: "IX_ScheduleBreak_ScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleBreak",
                table: "ScheduleBreak",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleBreak_Schedules_ScheduleId",
                table: "ScheduleBreak",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
