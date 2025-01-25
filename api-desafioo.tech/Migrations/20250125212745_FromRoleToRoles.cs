using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_desafioo.tech.Migrations
{
    /// <inheritdoc />
    public partial class FromRoleToRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "Roles");

            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "Users",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Users",
                newName: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Roles",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "UserEmail");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "Role");
        }
    }
}
