using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlStock.Migrations
{
    /// <inheritdoc />
    public partial class rubrosFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EliminadoLogico",
                table: "Rubro",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Rubro",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Rubro_UserId",
                table: "Rubro",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rubro_AspNetUsers_UserId",
                table: "Rubro",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rubro_AspNetUsers_UserId",
                table: "Rubro");

            migrationBuilder.DropIndex(
                name: "IX_Rubro_UserId",
                table: "Rubro");

            migrationBuilder.DropColumn(
                name: "EliminadoLogico",
                table: "Rubro");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Rubro");
        }
    }
}
