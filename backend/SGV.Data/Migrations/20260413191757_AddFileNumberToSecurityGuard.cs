using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGV.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFileNumberToSecurityGuard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileNumber",
                table: "SecurityGuards",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileNumber",
                table: "SecurityGuards");
        }
    }
}
