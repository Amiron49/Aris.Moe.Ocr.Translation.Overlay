using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Sqlite.Migrations
{
    public partial class QuotaStuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuotaUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstimatedCost = table.Column<double>(type: "REAL", nullable: false),
                    Units = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotaUsages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuotaUsages_Time",
                table: "QuotaUsages",
                column: "Time");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuotaUsages");
        }
    }
}
