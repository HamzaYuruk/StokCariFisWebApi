using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CariWebApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameToUserCompanyRoleAndAddRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCompanies_Companies_CompanyId",
                table: "UserCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCompanies_Role_RoleId",
                table: "UserCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCompanies_Users_UserId",
                table: "UserCompanies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCompanies",
                table: "UserCompanies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                table: "Role");

            migrationBuilder.RenameTable(
                name: "UserCompanies",
                newName: "UserCompanyRoles");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "Roles");

            migrationBuilder.RenameIndex(
                name: "IX_UserCompanies_UserId",
                table: "UserCompanyRoles",
                newName: "IX_UserCompanyRoles_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCompanies_RoleId",
                table: "UserCompanyRoles",
                newName: "IX_UserCompanyRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCompanies_CompanyId",
                table: "UserCompanyRoles",
                newName: "IX_UserCompanyRoles_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCompanyRoles",
                table: "UserCompanyRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCompanyRoles_Companies_CompanyId",
                table: "UserCompanyRoles",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCompanyRoles_Roles_RoleId",
                table: "UserCompanyRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCompanyRoles_Users_UserId",
                table: "UserCompanyRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCompanyRoles_Companies_CompanyId",
                table: "UserCompanyRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCompanyRoles_Roles_RoleId",
                table: "UserCompanyRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCompanyRoles_Users_UserId",
                table: "UserCompanyRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCompanyRoles",
                table: "UserCompanyRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "UserCompanyRoles",
                newName: "UserCompanies");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Role");

            migrationBuilder.RenameIndex(
                name: "IX_UserCompanyRoles_UserId",
                table: "UserCompanies",
                newName: "IX_UserCompanies_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCompanyRoles_RoleId",
                table: "UserCompanies",
                newName: "IX_UserCompanies_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCompanyRoles_CompanyId",
                table: "UserCompanies",
                newName: "IX_UserCompanies_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCompanies",
                table: "UserCompanies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                table: "Role",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCompanies_Companies_CompanyId",
                table: "UserCompanies",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCompanies_Role_RoleId",
                table: "UserCompanies",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCompanies_Users_UserId",
                table: "UserCompanies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
