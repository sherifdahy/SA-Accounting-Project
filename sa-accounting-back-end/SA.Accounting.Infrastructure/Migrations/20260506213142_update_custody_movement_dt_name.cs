using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SA.Accounting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_custody_movement_dt_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movements");

            migrationBuilder.CreateTable(
                name: "CustodyMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExpenseClaimId = table.Column<int>(type: "int", nullable: true),
                    CustodyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustodyMovements", x => x.Id);
                    table.CheckConstraint("CK_Movement_Amount_Positive", "[Amount] > 0");
                    table.CheckConstraint("CK_Movement_ExpenseClaim_Rule", "(\r\n                ([Type] = 2 AND [ExpenseClaimId] IS NOT NULL)\r\n                OR\r\n                ([Type] <> 2 AND [ExpenseClaimId] IS NULL)\r\n            )");
                    table.CheckConstraint("CK_Movement_Type_Valid", "[Type] IN (1, 2, 3, 4, 5)");
                    table.ForeignKey(
                        name: "FK_CustodyMovements_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustodyMovements_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustodyMovements_Custodies_CustodyId",
                        column: x => x.CustodyId,
                        principalTable: "Custodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustodyMovements_ExpenseClaims_ExpenseClaimId",
                        column: x => x.ExpenseClaimId,
                        principalTable: "ExpenseClaims",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustodyMovements_CreatedById",
                table: "CustodyMovements",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CustodyMovements_CustodyId",
                table: "CustodyMovements",
                column: "CustodyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustodyMovements_ExpenseClaimId_Type",
                table: "CustodyMovements",
                columns: new[] { "ExpenseClaimId", "Type" },
                unique: true,
                filter: "[ExpenseClaimId] IS NOT NULL AND [Type] = 2");

            migrationBuilder.CreateIndex(
                name: "IX_CustodyMovements_UpdatedById",
                table: "CustodyMovements",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustodyMovements");

            migrationBuilder.CreateTable(
                name: "Movements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CustodyId = table.Column<int>(type: "int", nullable: false),
                    ExpenseClaimId = table.Column<int>(type: "int", nullable: true),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movements", x => x.Id);
                    table.CheckConstraint("CK_Movement_Amount_Positive", "[Amount] > 0");
                    table.CheckConstraint("CK_Movement_ExpenseClaim_Rule", "(\r\n                ([Type] = 2 AND [ExpenseClaimId] IS NOT NULL)\r\n                OR\r\n                ([Type] <> 2 AND [ExpenseClaimId] IS NULL)\r\n            )");
                    table.CheckConstraint("CK_Movement_Type_Valid", "[Type] IN (1, 2, 3, 4, 5)");
                    table.ForeignKey(
                        name: "FK_Movements_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Movements_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Movements_Custodies_CustodyId",
                        column: x => x.CustodyId,
                        principalTable: "Custodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Movements_ExpenseClaims_ExpenseClaimId",
                        column: x => x.ExpenseClaimId,
                        principalTable: "ExpenseClaims",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movements_CreatedById",
                table: "Movements",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_CustodyId",
                table: "Movements",
                column: "CustodyId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_ExpenseClaimId_Type",
                table: "Movements",
                columns: new[] { "ExpenseClaimId", "Type" },
                unique: true,
                filter: "[ExpenseClaimId] IS NOT NULL AND [Type] = 2");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_UpdatedById",
                table: "Movements",
                column: "UpdatedById");
        }
    }
}
