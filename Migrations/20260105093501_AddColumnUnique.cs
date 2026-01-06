using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemeNote.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_planif_semestre_nom_planif_semestre",
                table: "planif_semestre",
                column: "nom_planif_semestre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_etudiant_matricule",
                table: "etudiant",
                column: "matricule",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_administrateur_nom_admin_prenom_admin",
                table: "administrateur",
                columns: new[] { "nom_admin", "prenom_admin" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_planif_semestre_nom_planif_semestre",
                table: "planif_semestre");

            migrationBuilder.DropIndex(
                name: "IX_etudiant_matricule",
                table: "etudiant");

            migrationBuilder.DropIndex(
                name: "IX_administrateur_nom_admin_prenom_admin",
                table: "administrateur");
        }
    }
}
