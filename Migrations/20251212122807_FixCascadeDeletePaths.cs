using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemeNote.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeDeletePaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_note_etudiant_Promotion_promotion_id",
                table: "note_etudiant");

            migrationBuilder.DropForeignKey(
                name: "FK_note_etudiant_etudiant_etudiant_id",
                table: "note_etudiant");

            migrationBuilder.DropForeignKey(
                name: "FK_note_etudiant_parcours_etude_parcours_etudiant_id",
                table: "note_etudiant");

            migrationBuilder.DropForeignKey(
                name: "FK_parcours_etude_matiere_matiere_id",
                table: "parcours_etude");

            migrationBuilder.DropForeignKey(
                name: "FK_parcours_etude_planif_semestre_planif_semestre_id",
                table: "parcours_etude");

            migrationBuilder.DropForeignKey(
                name: "FK_parcours_etude_unite_enseignement_unite_enseignement_id",
                table: "parcours_etude");

            migrationBuilder.DropForeignKey(
                name: "FK_planif_semestre_Promotion_promotion_id",
                table: "planif_semestre");

            migrationBuilder.DropForeignKey(
                name: "FK_planif_semestre_option_etude_option_etude_id",
                table: "planif_semestre");

            migrationBuilder.DropForeignKey(
                name: "FK_planif_semestre_semestre_semestre_id",
                table: "planif_semestre");

            migrationBuilder.AddForeignKey(
                name: "FK_note_etudiant_Promotion_promotion_id",
                table: "note_etudiant",
                column: "promotion_id",
                principalTable: "Promotion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_note_etudiant_etudiant_etudiant_id",
                table: "note_etudiant",
                column: "etudiant_id",
                principalTable: "etudiant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_note_etudiant_parcours_etude_parcours_etudiant_id",
                table: "note_etudiant",
                column: "parcours_etudiant_id",
                principalTable: "parcours_etude",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_parcours_etude_matiere_matiere_id",
                table: "parcours_etude",
                column: "matiere_id",
                principalTable: "matiere",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_parcours_etude_planif_semestre_planif_semestre_id",
                table: "parcours_etude",
                column: "planif_semestre_id",
                principalTable: "planif_semestre",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_parcours_etude_unite_enseignement_unite_enseignement_id",
                table: "parcours_etude",
                column: "unite_enseignement_id",
                principalTable: "unite_enseignement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_planif_semestre_Promotion_promotion_id",
                table: "planif_semestre",
                column: "promotion_id",
                principalTable: "Promotion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_planif_semestre_option_etude_option_etude_id",
                table: "planif_semestre",
                column: "option_etude_id",
                principalTable: "option_etude",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_planif_semestre_semestre_semestre_id",
                table: "planif_semestre",
                column: "semestre_id",
                principalTable: "semestre",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_note_etudiant_Promotion_promotion_id",
                table: "note_etudiant");

            migrationBuilder.DropForeignKey(
                name: "FK_note_etudiant_etudiant_etudiant_id",
                table: "note_etudiant");

            migrationBuilder.DropForeignKey(
                name: "FK_note_etudiant_parcours_etude_parcours_etudiant_id",
                table: "note_etudiant");

            migrationBuilder.DropForeignKey(
                name: "FK_parcours_etude_matiere_matiere_id",
                table: "parcours_etude");

            migrationBuilder.DropForeignKey(
                name: "FK_parcours_etude_planif_semestre_planif_semestre_id",
                table: "parcours_etude");

            migrationBuilder.DropForeignKey(
                name: "FK_parcours_etude_unite_enseignement_unite_enseignement_id",
                table: "parcours_etude");

            migrationBuilder.DropForeignKey(
                name: "FK_planif_semestre_Promotion_promotion_id",
                table: "planif_semestre");

            migrationBuilder.DropForeignKey(
                name: "FK_planif_semestre_option_etude_option_etude_id",
                table: "planif_semestre");

            migrationBuilder.DropForeignKey(
                name: "FK_planif_semestre_semestre_semestre_id",
                table: "planif_semestre");

            migrationBuilder.AddForeignKey(
                name: "FK_note_etudiant_Promotion_promotion_id",
                table: "note_etudiant",
                column: "promotion_id",
                principalTable: "Promotion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_note_etudiant_etudiant_etudiant_id",
                table: "note_etudiant",
                column: "etudiant_id",
                principalTable: "etudiant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_note_etudiant_parcours_etude_parcours_etudiant_id",
                table: "note_etudiant",
                column: "parcours_etudiant_id",
                principalTable: "parcours_etude",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_parcours_etude_matiere_matiere_id",
                table: "parcours_etude",
                column: "matiere_id",
                principalTable: "matiere",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_parcours_etude_planif_semestre_planif_semestre_id",
                table: "parcours_etude",
                column: "planif_semestre_id",
                principalTable: "planif_semestre",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_parcours_etude_unite_enseignement_unite_enseignement_id",
                table: "parcours_etude",
                column: "unite_enseignement_id",
                principalTable: "unite_enseignement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_planif_semestre_Promotion_promotion_id",
                table: "planif_semestre",
                column: "promotion_id",
                principalTable: "Promotion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_planif_semestre_option_etude_option_etude_id",
                table: "planif_semestre",
                column: "option_etude_id",
                principalTable: "option_etude",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_planif_semestre_semestre_semestre_id",
                table: "planif_semestre",
                column: "semestre_id",
                principalTable: "semestre",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
