using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KiMa_API.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureUserProfileRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserProfileUserId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserProfileUserId",
                table: "AspNetUsers",
                column: "UserProfileUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserProfiles_UserProfileUserId",
                table: "AspNetUsers",
                column: "UserProfileUserId",
                principalTable: "UserProfiles",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserProfiles_UserProfileUserId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserProfileUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserProfileUserId",
                table: "AspNetUsers");
        }
    }
}
