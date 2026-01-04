using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemeNote.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueColumnCodeEnseignement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_unite_enseignement_code_unite_enseignement",
                table: "unite_enseignement",
                column: "code_unite_enseignement",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_unite_enseignement_code_unite_enseignement",
                table: "unite_enseignement");
        }
    }
}
