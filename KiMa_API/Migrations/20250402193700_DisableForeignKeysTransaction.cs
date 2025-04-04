using Microsoft.EntityFrameworkCore.Migrations;

namespace KiMa_API.Migrations
{
    public partial class DisableForeignKeysTransaction : Migration
    {
        // Diese Einstellung verhindert, dass EF Core diese Migration in einer Transaktion ausführt.
        

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = 0;", suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = 1;", suppressTransaction: true);
        }
    }
}

