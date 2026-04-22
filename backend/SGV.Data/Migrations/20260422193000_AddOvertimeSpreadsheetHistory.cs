using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SGV.Data;

#nullable disable

namespace SGV.Data.Migrations
{
    [DbContext(typeof(SGVDbContext))]
    [Migration("20260422193000_AddOvertimeSpreadsheetHistory")]
    public partial class AddOvertimeSpreadsheetHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OvertimeSpreadsheets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkplaceId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    ExtraHourRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RateValidFrom = table.Column<DateTime>(type: "date", nullable: true),
                    TotalHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvertimeSpreadsheets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OvertimeSpreadsheets_Workplaces_WorkplaceId",
                        column: x => x.WorkplaceId,
                        principalTable: "Workplaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OvertimeSpreadsheetRows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OvertimeSpreadsheetId = table.Column<int>(type: "int", nullable: false),
                    SecurityGuardId = table.Column<int>(type: "int", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Dni = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FileNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Verified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvertimeSpreadsheetRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OvertimeSpreadsheetRows_OvertimeSpreadsheets_OvertimeSpreadsheetId",
                        column: x => x.OvertimeSpreadsheetId,
                        principalTable: "OvertimeSpreadsheets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OvertimeSpreadsheetRows_SecurityGuards_SecurityGuardId",
                        column: x => x.SecurityGuardId,
                        principalTable: "SecurityGuards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeSpreadsheetRows_OvertimeSpreadsheetId",
                table: "OvertimeSpreadsheetRows",
                column: "OvertimeSpreadsheetId");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeSpreadsheetRows_SecurityGuardId",
                table: "OvertimeSpreadsheetRows",
                column: "SecurityGuardId");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeSpreadsheets_WorkplaceId",
                table: "OvertimeSpreadsheets",
                column: "WorkplaceId");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeSpreadsheets_Year_Month_WorkplaceId",
                table: "OvertimeSpreadsheets",
                columns: new[] { "Year", "Month", "WorkplaceId" });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OvertimeSpreadsheetRows");

            migrationBuilder.DropTable(
                name: "OvertimeSpreadsheets");
        }
    }
}
