using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemeNote.Migrations
{
    /// <inheritdoc />
    public partial class CreateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "administrateur",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom_admin = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    prenom_admin = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_administrateur", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "config",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    valeur = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_config", x => x.Id);
                });

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
                name: "matiere",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom_matiere = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    code_matiere = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matiere", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "option_etude",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom_option_etude = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_option_etude", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "semestre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    nom_semestre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_semestre", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "unite_enseignement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code_unite_enseignement = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    credits = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unite_enseignement", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "planif_semestre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom_planif_semestre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    date_debut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    date_fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    semestre_id = table.Column<int>(type: "int", nullable: false),
                    option_etude_id = table.Column<int>(type: "int", nullable: false),
                    total_credit = table.Column<int>(type: "int", nullable: false),
                    promotion_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planif_semestre", x => x.Id);
                    table.ForeignKey(
                        name: "FK_planif_semestre_Promotion_promotion_id",
                        column: x => x.promotion_id,
                        principalTable: "Promotion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_planif_semestre_option_etude_option_etude_id",
                        column: x => x.option_etude_id,
                        principalTable: "option_etude",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_planif_semestre_semestre_semestre_id",
                        column: x => x.semestre_id,
                        principalTable: "semestre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "etudiant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    matricule = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    nom = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    prenom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    date_naissance = table.Column<DateTime>(type: "datetime2", nullable: false),
                    promotion_id = table.Column<int>(type: "int", nullable: false),
                    genre = table.Column<int>(type: "int", nullable: false),
                    is_actif = table.Column<bool>(type: "bit", nullable: false),
                    date_admission = table.Column<DateTime>(type: "datetime2", nullable: false),
                    administrateur_id = table.Column<int>(type: "int", nullable: false),
                    planif_semestre_id = table.Column<int>(type: "int", nullable: false),
                    mot_de_passe = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_etudiant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_etudiant_Promotion_promotion_id",
                        column: x => x.promotion_id,
                        principalTable: "Promotion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_etudiant_administrateur_administrateur_id",
                        column: x => x.administrateur_id,
                        principalTable: "administrateur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_etudiant_planif_semestre_planif_semestre_id",
                        column: x => x.planif_semestre_id,
                        principalTable: "planif_semestre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "parcours_etude",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    matiere_id = table.Column<int>(type: "int", nullable: false),
                    unite_enseignement_id = table.Column<int>(type: "int", nullable: false),
                    planif_semestre_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parcours_etude", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parcours_etude_matiere_matiere_id",
                        column: x => x.matiere_id,
                        principalTable: "matiere",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_parcours_etude_planif_semestre_planif_semestre_id",
                        column: x => x.planif_semestre_id,
                        principalTable: "planif_semestre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_parcours_etude_unite_enseignement_unite_enseignement_id",
                        column: x => x.unite_enseignement_id,
                        principalTable: "unite_enseignement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "historique_semestre_etudiant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    etudiant_id = table.Column<int>(type: "int", nullable: false),
                    planif_semetre_id = table.Column<int>(type: "int", nullable: false),
                    date_debut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    date_fin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historique_semestre_etudiant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_historique_semestre_etudiant_etudiant_etudiant_id",
                        column: x => x.etudiant_id,
                        principalTable: "etudiant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_historique_semestre_etudiant_planif_semestre_planif_semetre_id",
                        column: x => x.planif_semetre_id,
                        principalTable: "planif_semestre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "note_etudiant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    etudiant_id = table.Column<int>(type: "int", nullable: false),
                    parcours_etudiant_id = table.Column<int>(type: "int", nullable: false),
                    note = table.Column<double>(type: "float", nullable: false),
                    promotion_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_note_etudiant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_note_etudiant_Promotion_promotion_id",
                        column: x => x.promotion_id,
                        principalTable: "Promotion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_note_etudiant_etudiant_etudiant_id",
                        column: x => x.etudiant_id,
                        principalTable: "etudiant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_note_etudiant_parcours_etude_parcours_etudiant_id",
                        column: x => x.parcours_etudiant_id,
                        principalTable: "parcours_etude",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diplome_NomDiplome",
                table: "Diplome",
                column: "NomDiplome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_etudiant_administrateur_id",
                table: "etudiant",
                column: "administrateur_id");

            migrationBuilder.CreateIndex(
                name: "IX_etudiant_planif_semestre_id",
                table: "etudiant",
                column: "planif_semestre_id");

            migrationBuilder.CreateIndex(
                name: "IX_etudiant_promotion_id",
                table: "etudiant",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "IX_historique_semestre_etudiant_etudiant_id",
                table: "historique_semestre_etudiant",
                column: "etudiant_id");

            migrationBuilder.CreateIndex(
                name: "IX_historique_semestre_etudiant_planif_semetre_id",
                table: "historique_semestre_etudiant",
                column: "planif_semetre_id");

            migrationBuilder.CreateIndex(
                name: "IX_note_etudiant_etudiant_id",
                table: "note_etudiant",
                column: "etudiant_id");

            migrationBuilder.CreateIndex(
                name: "IX_note_etudiant_parcours_etudiant_id",
                table: "note_etudiant",
                column: "parcours_etudiant_id");

            migrationBuilder.CreateIndex(
                name: "IX_note_etudiant_promotion_id",
                table: "note_etudiant",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "IX_parcours_etude_matiere_id",
                table: "parcours_etude",
                column: "matiere_id");

            migrationBuilder.CreateIndex(
                name: "IX_parcours_etude_planif_semestre_id",
                table: "parcours_etude",
                column: "planif_semestre_id");

            migrationBuilder.CreateIndex(
                name: "IX_parcours_etude_unite_enseignement_id",
                table: "parcours_etude",
                column: "unite_enseignement_id");

            migrationBuilder.CreateIndex(
                name: "IX_planif_semestre_option_etude_id",
                table: "planif_semestre",
                column: "option_etude_id");

            migrationBuilder.CreateIndex(
                name: "IX_planif_semestre_promotion_id",
                table: "planif_semestre",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "IX_planif_semestre_semestre_id",
                table: "planif_semestre",
                column: "semestre_id");

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
                name: "config");

            migrationBuilder.DropTable(
                name: "historique_semestre_etudiant");

            migrationBuilder.DropTable(
                name: "note_etudiant");

            migrationBuilder.DropTable(
                name: "etudiant");

            migrationBuilder.DropTable(
                name: "parcours_etude");

            migrationBuilder.DropTable(
                name: "administrateur");

            migrationBuilder.DropTable(
                name: "matiere");

            migrationBuilder.DropTable(
                name: "planif_semestre");

            migrationBuilder.DropTable(
                name: "unite_enseignement");

            migrationBuilder.DropTable(
                name: "Promotion");

            migrationBuilder.DropTable(
                name: "option_etude");

            migrationBuilder.DropTable(
                name: "semestre");

            migrationBuilder.DropTable(
                name: "Diplome");
        }
    }
}
