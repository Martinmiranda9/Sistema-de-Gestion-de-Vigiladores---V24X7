using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGV.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkplaceToShiftRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkplaceId",
                table: "ShiftRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftRecords_WorkplaceId",
                table: "ShiftRecords",
                column: "WorkplaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftRecords_Workplaces_WorkplaceId",
                table: "ShiftRecords",
                column: "WorkplaceId",
                principalTable: "Workplaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShiftRecords_Workplaces_WorkplaceId",
                table: "ShiftRecords");

            migrationBuilder.DropIndex(
                name: "IX_ShiftRecords_WorkplaceId",
                table: "ShiftRecords");

            migrationBuilder.DropColumn(
                name: "WorkplaceId",
                table: "ShiftRecords");
        }
    }
}
