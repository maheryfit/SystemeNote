using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemeNote.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueColumnMatiere : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_matiere_code_matiere",
                table: "matiere",
                column: "code_matiere",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_matiere_nom_matiere",
                table: "matiere",
                column: "nom_matiere",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_matiere_code_matiere",
                table: "matiere");

            migrationBuilder.DropIndex(
                name: "IX_matiere_nom_matiere",
                table: "matiere");
        }
    }
}
