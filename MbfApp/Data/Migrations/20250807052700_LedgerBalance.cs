using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MbfApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class LedgerBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberLedgerBalances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FinYearId = table.Column<int>(type: "integer", nullable: false),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    OpBalDeposit = table.Column<decimal>(type: "numeric", nullable: false),
                    OpBalLoan = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberLedgerBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberLedgerBalances_FinYears_FinYearId",
                        column: x => x.FinYearId,
                        principalTable: "FinYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberLedgerBalances_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberLedgerBalances_FinYearId",
                table: "MemberLedgerBalances",
                column: "FinYearId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberLedgerBalances_MemberId",
                table: "MemberLedgerBalances",
                column: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberLedgerBalances");
        }
    }
}
