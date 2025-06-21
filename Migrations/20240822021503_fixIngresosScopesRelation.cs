using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlStock.Migrations
{
    /// <inheritdoc />
    public partial class fixIngresosScopesRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleIngreso_Scopes",
                table: "DetalleIngreso");

            //migrationBuilder.DropIndex(
            //    name: "IX_DetalleIngreso_ScopeId",
            //    table: "DetalleIngreso");

            migrationBuilder.DropColumn(
                name: "ScopeId",
                table: "DetalleIngreso");

            migrationBuilder.AddColumn<int>(
                name: "ScopeId",
                table: "Ingresos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Ingresos_ScopeId",
                table: "Ingresos",
                column: "ScopeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingreso_Scopes",
                table: "Ingresos",
                column: "ScopeId",
                principalTable: "Scopes",
                principalColumn: "ScopeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingreso_Scopes",
                table: "Ingresos");

            migrationBuilder.DropIndex(
                name: "IX_Ingresos_ScopeId",
                table: "Ingresos");

            migrationBuilder.DropColumn(
                name: "ScopeId",
                table: "Ingresos");

            migrationBuilder.AddColumn<int>(
                name: "ScopeId",
                table: "DetalleIngreso",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetalleIngreso_ScopeId",
                table: "DetalleIngreso",
                column: "ScopeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleIngreso_Scopes",
                table: "DetalleIngreso",
                column: "ScopeId",
                principalTable: "Scopes",
                principalColumn: "ScopeId");
        }
    }
}
