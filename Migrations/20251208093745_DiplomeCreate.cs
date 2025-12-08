using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemeNote.Migrations
{
    /// <inheritdoc />
    public partial class DiplomeCreate : Migration
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

            migrationBuilder.CreateIndex(
                name: "IX_Diplome_NomDiplome",
                table: "Diplome",
                column: "NomDiplome");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diplome");
        }
    }
}
