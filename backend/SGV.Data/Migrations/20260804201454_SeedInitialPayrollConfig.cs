using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGV.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialPayrollConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PayrollConfigs",
                columns: new[] { "Id", "ChangedBy", "CreatedAt", "ExtraHourRate", "HolidayHourRate", "NightSurchargeRate", "NormalHourRate", "Reason", "ValidFrom" },
                values: new object[] { 1, "Sistema", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1500m, 1500m, 0m, 1000m, "Configuración inicial del sistema", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$6Pk.qcmcfpyR3uzil75PSOt1spzBHGJbhS8c1lrImUuZDRXS6fibG");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PayrollConfigs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$FDSV0GhcbmMekNymOyUKVeHspWvl.kiHr5.wByZX9LYSAHRPvvWQK");
        }
    }
}
