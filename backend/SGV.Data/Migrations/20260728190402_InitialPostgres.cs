using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SGV.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsRecurring = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayrollConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NormalHourRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NightSurchargeRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    HolidayHourRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ExtraHourRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ChangedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workplaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workplaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OvertimeSpreadsheets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkplaceId = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    ExtraHourRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    RateValidFrom = table.Column<DateTime>(type: "date", nullable: true),
                    TotalHours = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "SecurityGuards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DNI = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FileNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    WorkplaceId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityGuards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecurityGuards_Workplaces_WorkplaceId",
                        column: x => x.WorkplaceId,
                        principalTable: "Workplaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceSheets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SecurityGuardId = table.Column<int>(type: "integer", nullable: false),
                    WorkplaceId = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    TotalWorkedHours = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalNightHours = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalExtraHours = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "OvertimeSpreadsheetRows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OvertimeSpreadsheetId = table.Column<int>(type: "integer", nullable: false),
                    SecurityGuardId = table.Column<int>(type: "integer", nullable: true),
                    FullName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Dni = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FileNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Hours = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Verified = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvertimeSpreadsheetRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OvertimeSpreadsheetRows_OvertimeSpreadsheets_OvertimeSpread~",
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

            migrationBuilder.CreateTable(
                name: "ShiftRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SecurityGuardId = table.Column<int>(type: "integer", nullable: false),
                    WorkplaceId = table.Column<int>(type: "integer", nullable: true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftRecords_SecurityGuards_SecurityGuardId",
                        column: x => x.SecurityGuardId,
                        principalTable: "SecurityGuards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShiftRecords_Workplaces_WorkplaceId",
                        column: x => x.WorkplaceId,
                        principalTable: "Workplaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceSheetRows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AttendanceSheetId = table.Column<int>(type: "integer", nullable: false),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    Entry = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Exit = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    IsDayOff = table.Column<bool>(type: "boolean", nullable: false),
                    WorkedHours = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NightHours = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
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

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, true, "$2a$11$FDSV0GhcbmMekNymOyUKVeHspWvl.kiHr5.wByZX9LYSAHRPvvWQK", "Admin", "admin" });

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

            migrationBuilder.CreateIndex(
                name: "IX_SecurityGuards_DNI",
                table: "SecurityGuards",
                column: "DNI",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SecurityGuards_WorkplaceId",
                table: "SecurityGuards",
                column: "WorkplaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftRecords_SecurityGuardId",
                table: "ShiftRecords",
                column: "SecurityGuardId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftRecords_WorkplaceId",
                table: "ShiftRecords",
                column: "WorkplaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceSheetRows");

            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.DropTable(
                name: "OvertimeSpreadsheetRows");

            migrationBuilder.DropTable(
                name: "PayrollConfigs");

            migrationBuilder.DropTable(
                name: "ShiftRecords");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "AttendanceSheets");

            migrationBuilder.DropTable(
                name: "OvertimeSpreadsheets");

            migrationBuilder.DropTable(
                name: "SecurityGuards");

            migrationBuilder.DropTable(
                name: "Workplaces");
        }
    }
}
