using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemeNote.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnDiplomeOptionEtude : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "diplome_id",
                table: "option_etude",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_option_etude_diplome_id",
                table: "option_etude",
                column: "diplome_id");

            migrationBuilder.AddForeignKey(
                name: "FK_option_etude_diplome_diplome_id",
                table: "option_etude",
                column: "diplome_id",
                principalTable: "diplome",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_option_etude_diplome_diplome_id",
                table: "option_etude");

            migrationBuilder.DropIndex(
                name: "IX_option_etude_diplome_id",
                table: "option_etude");

            migrationBuilder.DropColumn(
                name: "diplome_id",
                table: "option_etude");
        }
    }
}
