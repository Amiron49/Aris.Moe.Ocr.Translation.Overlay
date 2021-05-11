using Microsoft.EntityFrameworkCore.Migrations;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Sqlite.Migrations
{
    public partial class uniqueimagehash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Images_Info_Sha256Hash",
                table: "Images");

            migrationBuilder.CreateIndex(
                name: "IX_Images_Info_Sha256Hash",
                table: "Images",
                column: "Info_Sha256Hash",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Images_Info_Sha256Hash",
                table: "Images");

            migrationBuilder.CreateIndex(
                name: "IX_Images_Info_Sha256Hash",
                table: "Images",
                column: "Info_Sha256Hash");
        }
    }
}
