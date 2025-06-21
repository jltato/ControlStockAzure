using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlStock.Migrations
{
    /// <inheritdoc />
    public partial class Egresos2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_DetalleEgreso_IngresoId",
                table: "DetalleEgresos",
                newName: "IX_DetalleEgreso_EgresoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_DetalleEgreso_EgresoId",
                table: "DetalleEgresos",
                newName: "IX_DetalleEgreso_IngresoId");
        }
    }
}
