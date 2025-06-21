using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlStock.Migrations
{
    /// <inheritdoc />
    public partial class MarcaUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EliminadoLogico",
                table: "Marca",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Marca",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Marca_UserId",
                table: "Marca",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Marca_AspNetUsers_UserId",
                table: "Marca",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Marca_AspNetUsers_UserId",
                table: "Marca");

            migrationBuilder.DropIndex(
                name: "IX_Marca_UserId",
                table: "Marca");

            migrationBuilder.DropColumn(
                name: "EliminadoLogico",
                table: "Marca");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Marca");
        }
    }
}
