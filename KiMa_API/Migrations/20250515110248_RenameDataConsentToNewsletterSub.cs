using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KiMa_API.Migrations
{
    /// <inheritdoc />
    public partial class RenameDataConsentToNewsletterSub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataConsent",
                table: "AspNetUsers",
                newName: "NewsletterSub");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NewsletterSub",
                table: "AspNetUsers",
                newName: "DataConsent");
        }
    }
}
