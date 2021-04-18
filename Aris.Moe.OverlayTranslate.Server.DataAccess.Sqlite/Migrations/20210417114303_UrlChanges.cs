using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Sqlite.Migrations
{
    public partial class UrlChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalUrl",
                table: "Images");

            migrationBuilder.CreateTable(
                name: "Urls",
                columns: table => new
                {
                    UrlHash = table.Column<byte[]>(type: "BLOB", nullable: false),
                    ImageReferenceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OriginalUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urls", x => x.UrlHash);
                    table.ForeignKey(
                        name: "FK_Urls_Images_ImageReferenceId",
                        column: x => x.ImageReferenceId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Urls_ImageReferenceId",
                table: "Urls",
                column: "ImageReferenceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Urls");

            migrationBuilder.AddColumn<string>(
                name: "OriginalUrl",
                table: "Images",
                type: "TEXT",
                nullable: true);
        }
    }
}
