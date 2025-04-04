using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KiMa_API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    VehicleCategory = table.Column<int>(type: "INTEGER", nullable: false),
                    VehicleDetails = table.Column<string>(type: "TEXT", nullable: true),
                    Occupation = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EducationLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Region = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Age = table.Column<int>(type: "INTEGER", nullable: true),
                    IncomeLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsInterestedInTechnology = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsInterestedInSports = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsInterestedInEntertainment = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsInterestedInTravel = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
