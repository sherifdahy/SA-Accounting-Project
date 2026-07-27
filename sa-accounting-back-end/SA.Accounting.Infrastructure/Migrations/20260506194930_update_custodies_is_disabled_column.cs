using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SA.Accounting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_custodies_is_disabled_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ExpenseCategory_Name_NotEmpty",
                table: "ExpenseCategories");

            migrationBuilder.DropIndex(
                name: "IX_Custodies_UserId",
                table: "Custodies");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Custodies");

            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "Custodies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Custodies_UserId",
                table: "Custodies",
                column: "UserId",
                unique: true,
                filter: "[IsDisabled] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Custodies_UserId",
                table: "Custodies");

            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "Custodies");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Custodies",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ExpenseCategory_Name_NotEmpty",
                table: "ExpenseCategories",
                sql: "LEN(LTRIM(RTRIM([Name]))) > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Custodies_UserId",
                table: "Custodies",
                column: "UserId",
                unique: true,
                filter: "[IsActive] = 1");
        }
    }
}
