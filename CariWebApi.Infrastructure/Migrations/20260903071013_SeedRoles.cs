using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CariWebApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "UserCompanies",
                newName: "RoleId");

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Owner" },
                    { 2, "User" },
                    { 3, "Customer" },
                    { 4, "Admin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanies_RoleId",
                table: "UserCompanies",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCompanies_Role_RoleId",
                table: "UserCompanies",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCompanies_Role_RoleId",
                table: "UserCompanies");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropIndex(
                name: "IX_UserCompanies_RoleId",
                table: "UserCompanies");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "UserCompanies",
                newName: "Role");
        }
    }
}
