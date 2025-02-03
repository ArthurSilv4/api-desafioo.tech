using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_desafioo.tech.Migrations
{
    /// <inheritdoc />
    public partial class AddChallengeidInParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChallengeParticipantId",
                table: "Challenges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ChallengeId",
                table: "ChallengeParticipants",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_ChallengeParticipantId",
                table: "Challenges",
                column: "ChallengeParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_ChallengeParticipants_ChallengeParticipantId",
                table: "Challenges",
                column: "ChallengeParticipantId",
                principalTable: "ChallengeParticipants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_ChallengeParticipants_ChallengeParticipantId",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_ChallengeParticipantId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "ChallengeParticipantId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "ChallengeId",
                table: "ChallengeParticipants");
        }
    }
}
