using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGV.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollConfigAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChangedBy",
                table: "PayrollConfigs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PayrollConfigs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "PayrollConfigs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$Lajoknx33QvrEb4dfk8IGuh6lH2YVDbsgnVDxdRR86n6xm6g3Q.BW");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "PayrollConfigs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PayrollConfigs");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "PayrollConfigs");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$l2nNVBZE3iziLhdNC3ict.Y7aAeAyDAH2sDfP5lRP81yr/eESG0uK");
        }
    }
}
