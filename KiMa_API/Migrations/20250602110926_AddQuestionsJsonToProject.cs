using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KiMa_API.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionsJsonToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuestionsJson",
                table: "Projects",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionsJson",
                table: "Projects");
        }
    }
}
