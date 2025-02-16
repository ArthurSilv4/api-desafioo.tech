using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_desafioo.tech.Migrations
{
    /// <inheritdoc />
    public partial class AddStartsInChallenge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stars",
                table: "Challenges",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stars",
                table: "Challenges");
        }
    }
}
