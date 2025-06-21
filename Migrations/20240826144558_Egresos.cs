using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlStock.Migrations
{
    /// <inheritdoc />
    public partial class Egresos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProveedorMail",
                table: "Proveedor",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Ingresos",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Comprobante",
                table: "Ingresos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Egresos",
                columns: table => new
                {
                    EgresoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaEgreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScopeId = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, defaultValue: ""),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EliminadoLogico = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Egresos", x => x.EgresoId);
                    table.ForeignKey(
                        name: "FK_Egresos_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Scopes",
                        principalColumn: "ScopeId");
                });

            migrationBuilder.CreateTable(
                name: "DetalleEgresos",
                columns: table => new
                {
                    DetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EgresoId = table.Column<int>(type: "int", nullable: true),
                    ArticuloId = table.Column<int>(type: "int", nullable: false),
                    LoteId = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleEgresos", x => x.DetalleId);
                    table.ForeignKey(
                        name: "FK_DetalleEgresos_Articulo_ArticuloId",
                        column: x => x.ArticuloId,
                        principalTable: "Articulo",
                        principalColumn: "IdArticulo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleEgresos_Egresos_EgresoId",
                        column: x => x.EgresoId,
                        principalTable: "Egresos",
                        principalColumn: "EgresoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleEgresos_Lote_LoteId",
                        column: x => x.LoteId,
                        principalTable: "Lote",
                        principalColumn: "LoteId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleEgreso_ArticuloId",
                table: "DetalleEgresos",
                column: "ArticuloId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleEgreso_IngresoId",
                table: "DetalleEgresos",
                column: "EgresoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleEgreso_LoteId",
                table: "DetalleEgresos",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Egresos_ScopeId",
                table: "Egresos",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_Egresos_UserId",
                table: "Egresos",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleEgresos");

            migrationBuilder.DropTable(
                name: "Egresos");

            migrationBuilder.DropColumn(
                name: "ProveedorMail",
                table: "Proveedor");

            migrationBuilder.DropColumn(
                name: "Comprobante",
                table: "Ingresos");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Ingresos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "");
        }
    }
}
