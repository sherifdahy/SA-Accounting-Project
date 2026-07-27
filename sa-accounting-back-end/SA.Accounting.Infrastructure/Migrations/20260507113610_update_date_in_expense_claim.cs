using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SA.Accounting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_date_in_expense_claim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ExpenseClaim_Number_NotEmpty",
                table: "ExpenseClaims");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ExpenseClaim_State_Valid",
                table: "ExpenseClaims");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ExpenseClaimItem_RejectionReason_Required_WhenRejected",
                table: "ExpenseClaimItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ExpenseClaimItem_State_Valid",
                table: "ExpenseClaimItems");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ClaimDate",
                table: "ExpenseClaims",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "ExpenseClaimItems",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ExpenseClaim_State_Valid",
                table: "ExpenseClaims",
                sql: "[CurrentState] IN (1,2,3,4,5,6,7,8,9)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ExpenseClaimItem_RejectionReason_Required_WhenRejected",
                table: "ExpenseClaimItems",
                sql: "\r\n                (\r\n                    [State] <> 3\r\n                    OR\r\n                    (\r\n                        [RejectionReason] IS NOT NULL\r\n                        AND LEN(LTRIM(RTRIM([RejectionReason]))) > 0\r\n                    )\r\n                )");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ExpenseClaimItem_State_Valid",
                table: "ExpenseClaimItems",
                sql: "[State] IN (1,2,3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ExpenseClaim_State_Valid",
                table: "ExpenseClaims");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ExpenseClaimItem_RejectionReason_Required_WhenRejected",
                table: "ExpenseClaimItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ExpenseClaimItem_State_Valid",
                table: "ExpenseClaimItems");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ClaimDate",
                table: "ExpenseClaims",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "ExpenseClaimItems",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ExpenseClaim_Number_NotEmpty",
                table: "ExpenseClaims",
                sql: "LEN(LTRIM(RTRIM([Number]))) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ExpenseClaim_State_Valid",
                table: "ExpenseClaims",
                sql: "[CurrentState] IN (1, 2, 3, 4, 5, 6, 7, 8, 9)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ExpenseClaimItem_RejectionReason_Required_WhenRejected",
                table: "ExpenseClaimItems",
                sql: "(\r\n                [State] <> 3\r\n                OR\r\n                (\r\n                    [State] = 3 \r\n                    AND [RejectionReason] IS NOT NULL \r\n                    AND LEN(LTRIM(RTRIM([RejectionReason]))) > 0\r\n                )\r\n            )");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ExpenseClaimItem_State_Valid",
                table: "ExpenseClaimItems",
                sql: "[State] IN (1, 2, 3)");
        }
    }
}
