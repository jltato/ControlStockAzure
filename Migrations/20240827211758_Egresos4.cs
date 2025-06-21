using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlStock.Migrations
{
    /// <inheritdoc />
    public partial class Egresos4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Egresos_Destino",
                table: "Egresos",
                column: "Destino");

            migrationBuilder.AddForeignKey(
                name: "FK_Egresos_Scopes_Destino",
                table: "Egresos",
                column: "Destino",
                principalTable: "Scopes",
                principalColumn: "ScopeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Egresos_Scopes_Destino",
                table: "Egresos");

            migrationBuilder.DropIndex(
                name: "IX_Egresos_Destino",
                table: "Egresos");
        }
    }
}
