using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aris.Moe.OverlayTranslate.Server.DataAccess.Sqlite.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Info_Sha256Hash = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Info_AverageHash = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Info_DifferenceHash = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Info_PerceptualHash = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Info_Width = table.Column<int>(type: "INTEGER", nullable: false),
                    Info_Height = table.Column<int>(type: "INTEGER", nullable: false),
                    Info_MimeType = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalUrl = table.Column<string>(type: "TEXT", nullable: true),
                    QualityScore = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RawMachineOcrs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ForImage = table.Column<Guid>(type: "TEXT", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    Provider = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Texts = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawMachineOcrs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RawMachineOcrs_Images_ForImage",
                        column: x => x.ForImage,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsolidatedMachineOcrs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RawId = table.Column<int>(type: "INTEGER", nullable: false),
                    Consolidation = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsolidatedMachineOcrs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsolidatedMachineOcrs_RawMachineOcrs_RawId",
                        column: x => x.RawId,
                        principalTable: "RawMachineOcrs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MachineTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MachineOcrId = table.Column<int>(type: "INTEGER", nullable: false),
                    Provider = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineTranslations_ConsolidatedMachineOcrs_MachineOcrId",
                        column: x => x.MachineOcrId,
                        principalTable: "ConsolidatedMachineOcrs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpatialTexts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    BasedOnSpatialOcrText = table.Column<int>(type: "INTEGER", nullable: true),
                    MachineOcrId = table.Column<int>(type: "INTEGER", nullable: true),
                    MachineTranslationId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Rectangle_TopLeft_X = table.Column<int>(type: "INTEGER", nullable: false),
                    Rectangle_TopLeft_Y = table.Column<int>(type: "INTEGER", nullable: false),
                    Rectangle_BottomRight_X = table.Column<int>(type: "INTEGER", nullable: false),
                    Rectangle_BottomRight_Y = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpatialTexts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpatialTexts_ConsolidatedMachineOcrs_MachineOcrId",
                        column: x => x.MachineOcrId,
                        principalTable: "ConsolidatedMachineOcrs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpatialTexts_MachineTranslations_MachineTranslationId",
                        column: x => x.MachineTranslationId,
                        principalTable: "MachineTranslations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpatialTexts_SpatialTexts_BasedOnSpatialOcrText",
                        column: x => x.BasedOnSpatialOcrText,
                        principalTable: "SpatialTexts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    For = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false),
                    ChangedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => new { x.For, x.UserId });
                    table.ForeignKey(
                        name: "FK_Votes_SpatialTexts_For",
                        column: x => x.For,
                        principalTable: "SpatialTexts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsolidatedMachineOcrs_RawId",
                table: "ConsolidatedMachineOcrs",
                column: "RawId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_Info_AverageHash",
                table: "Images",
                column: "Info_AverageHash");

            migrationBuilder.CreateIndex(
                name: "IX_Images_Info_Sha256Hash",
                table: "Images",
                column: "Info_Sha256Hash");

            migrationBuilder.CreateIndex(
                name: "IX_MachineTranslations_MachineOcrId",
                table: "MachineTranslations",
                column: "MachineOcrId");

            migrationBuilder.CreateIndex(
                name: "IX_RawMachineOcrs_ForImage",
                table: "RawMachineOcrs",
                column: "ForImage");

            migrationBuilder.CreateIndex(
                name: "IX_SpatialTexts_BasedOnSpatialOcrText",
                table: "SpatialTexts",
                column: "BasedOnSpatialOcrText");

            migrationBuilder.CreateIndex(
                name: "IX_SpatialTexts_MachineOcrId",
                table: "SpatialTexts",
                column: "MachineOcrId");

            migrationBuilder.CreateIndex(
                name: "IX_SpatialTexts_MachineTranslationId",
                table: "SpatialTexts",
                column: "MachineTranslationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "SpatialTexts");

            migrationBuilder.DropTable(
                name: "MachineTranslations");

            migrationBuilder.DropTable(
                name: "ConsolidatedMachineOcrs");

            migrationBuilder.DropTable(
                name: "RawMachineOcrs");

            migrationBuilder.DropTable(
                name: "Images");
        }
    }
}
