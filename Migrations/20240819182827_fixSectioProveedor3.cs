using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlStock.Migrations
{
    /// <inheritdoc />
    public partial class fixSectioProveedor3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proveedor_Rubro_RubroIdRubro",
                table: "Proveedor");

            migrationBuilder.DropIndex(
                name: "IX_Proveedor_RubroIdRubro",
                table: "Proveedor");

            migrationBuilder.DropColumn(
                name: "RubroIdRubro",
                table: "Proveedor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RubroIdRubro",
                table: "Proveedor",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proveedor_RubroIdRubro",
                table: "Proveedor",
                column: "RubroIdRubro");

            migrationBuilder.AddForeignKey(
                name: "FK_Proveedor_Rubro_RubroIdRubro",
                table: "Proveedor",
                column: "RubroIdRubro",
                principalTable: "Rubro",
                principalColumn: "IdRubro");
        }
    }
}
