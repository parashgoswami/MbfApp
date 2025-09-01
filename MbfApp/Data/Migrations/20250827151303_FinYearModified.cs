using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MbfApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class FinYearModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCurrent",
                table: "FinYears",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCurrent",
                table: "FinYears");
        }
    }
}
