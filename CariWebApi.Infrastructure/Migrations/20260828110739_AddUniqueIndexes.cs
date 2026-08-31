using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CariWebApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stocks_CompanyId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_CompanyId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_CompanyId",
                table: "Accounts");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Stocks",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiptNumber",
                table: "Receipts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Accounts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_CompanyId_Code",
                table: "Stocks",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_CompanyId_ReceiptType_ReceiptNumber",
                table: "Receipts",
                columns: new[] { "CompanyId", "ReceiptType", "ReceiptNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CompanyId_Code",
                table: "Accounts",
                columns: new[] { "CompanyId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stocks_CompanyId_Code",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_CompanyId_ReceiptType_ReceiptNumber",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_CompanyId_Code",
                table: "Accounts");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Stocks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiptNumber",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_CompanyId",
                table: "Stocks",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_CompanyId",
                table: "Receipts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CompanyId",
                table: "Accounts",
                column: "CompanyId");
        }
    }
}
