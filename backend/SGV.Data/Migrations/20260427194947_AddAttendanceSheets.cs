using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGV.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceSheets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceSheets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SecurityGuardId = table.Column<int>(type: "int", nullable: false),
                    WorkplaceId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    TotalWorkedHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalNightHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalExtraHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceSheets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceSheets_SecurityGuards_SecurityGuardId",
                        column: x => x.SecurityGuardId,
                        principalTable: "SecurityGuards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceSheets_Workplaces_WorkplaceId",
                        column: x => x.WorkplaceId,
                        principalTable: "Workplaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceSheetRows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttendanceSheetId = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<int>(type: "int", nullable: false),
                    Entry = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Exit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsDayOff = table.Column<bool>(type: "bit", nullable: false),
                    WorkedHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NightHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceSheetRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceSheetRows_AttendanceSheets_AttendanceSheetId",
                        column: x => x.AttendanceSheetId,
                        principalTable: "AttendanceSheets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$O7O9nZDH4OOZQe25.zh3DOtt7hA8PtPjrjtoptRch2KkOszx2y1YW");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSheetRows_AttendanceSheetId",
                table: "AttendanceSheetRows",
                column: "AttendanceSheetId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSheets_SecurityGuardId",
                table: "AttendanceSheets",
                column: "SecurityGuardId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSheets_WorkplaceId",
                table: "AttendanceSheets",
                column: "WorkplaceId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSheets_Year_Month_SecurityGuardId",
                table: "AttendanceSheets",
                columns: new[] { "Year", "Month", "SecurityGuardId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceSheetRows");

            migrationBuilder.DropTable(
                name: "AttendanceSheets");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$Lajoknx33QvrEb4dfk8IGuh6lH2YVDbsgnVDxdRR86n6xm6g3Q.BW");
        }
    }
}
