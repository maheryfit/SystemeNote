using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemeNote.Migrations
{
    /// <inheritdoc />
    public partial class DiplomeUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Diplome_NomDiplome",
                table: "Diplome");

            migrationBuilder.CreateIndex(
                name: "IX_Diplome_NomDiplome",
                table: "Diplome",
                column: "NomDiplome",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Diplome_NomDiplome",
                table: "Diplome");

            migrationBuilder.CreateIndex(
                name: "IX_Diplome_NomDiplome",
                table: "Diplome",
                column: "NomDiplome");
        }
    }
}
