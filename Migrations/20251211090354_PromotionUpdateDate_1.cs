using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemeNote.Migrations
{
    /// <inheritdoc />
    public partial class PromotionUpdateDate_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diplome",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomDiplome = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diplome", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Promotion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomPromotion = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateCreation = table.Column<DateOnly>(type: "date", nullable: false),
                    CodePromotion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DiplomeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promotion_Diplome_DiplomeId",
                        column: x => x.DiplomeId,
                        principalTable: "Diplome",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diplome_NomDiplome",
                table: "Diplome",
                column: "NomDiplome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promotion_CodePromotion",
                table: "Promotion",
                column: "CodePromotion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promotion_DiplomeId",
                table: "Promotion",
                column: "DiplomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotion_NomPromotion",
                table: "Promotion",
                column: "NomPromotion",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Promotion");

            migrationBuilder.DropTable(
                name: "Diplome");
        }
    }
}
