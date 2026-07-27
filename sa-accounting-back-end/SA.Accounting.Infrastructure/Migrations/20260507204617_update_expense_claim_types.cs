using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SA.Accounting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_expense_claim_types : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ExpenseClaim_State_Valid",
                table: "ExpenseClaims");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ExpenseClaim_State_Valid",
                table: "ExpenseClaims",
                sql: "[CurrentState] IN (1,2,3,4,5,6,7)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ExpenseClaim_State_Valid",
                table: "ExpenseClaims");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ExpenseClaim_State_Valid",
                table: "ExpenseClaims",
                sql: "[CurrentState] IN (1,2,3,4,5,6,7,8,9)");
        }
    }
}
