using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGV.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExtraHourRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ExtraHourRate",
                table: "PayrollConfigs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraHourRate",
                table: "PayrollConfigs");
        }
    }
}
