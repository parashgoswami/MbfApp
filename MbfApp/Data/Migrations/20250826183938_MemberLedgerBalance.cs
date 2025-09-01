using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MbfApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class MemberLedgerBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DepositBalance",
                table: "MemberLedgerBalances",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LoanBalance",
                table: "MemberLedgerBalances",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepositBalance",
                table: "MemberLedgerBalances");

            migrationBuilder.DropColumn(
                name: "LoanBalance",
                table: "MemberLedgerBalances");
        }
    }
}
