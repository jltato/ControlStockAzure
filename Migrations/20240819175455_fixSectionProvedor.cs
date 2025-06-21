using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlStock.Migrations
{
    /// <inheritdoc />
    public partial class fixSectionProvedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RubroProveedores");

            migrationBuilder.RenameColumn(
                name: "fechaAlta",
                table: "Proveedor",
                newName: "FechaAlta");

            migrationBuilder.RenameColumn(
                name: "eliminadoLogico",
                table: "Proveedor",
                newName: "EliminadoLogico");

            //migrationBuilder.AddColumn<int>(
            //    name: "RubroIdRubro",
            //    table: "Proveedor",
            //    type: "int",
            //    nullable: true);

            migrationBuilder.CreateTable(
                name: "SectionProveedores",
                columns: table => new
                {
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionProveedores", x => new { x.ProveedorId, x.SectionId });
                    table.ForeignKey(
                        name: "FK_SectionProveedores_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
                        principalColumn: "ProveedorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionProveedores_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "SectionId",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Proveedor_RubroIdRubro",
            //    table: "Proveedor",
            //    column: "RubroIdRubro");

            migrationBuilder.CreateIndex(
                name: "IX_SectionProveedores_SectionId",
                table: "SectionProveedores",
                column: "SectionId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Proveedor_Rubro_RubroIdRubro",
            //    table: "Proveedor",
            //    column: "RubroIdRubro",
            //    principalTable: "Rubro",
            //    principalColumn: "IdRubro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proveedor_Rubro_RubroIdRubro",
                table: "Proveedor");

            migrationBuilder.DropTable(
                name: "SectionProveedores");

            migrationBuilder.DropIndex(
                name: "IX_Proveedor_RubroIdRubro",
                table: "Proveedor");

            migrationBuilder.DropColumn(
                name: "RubroIdRubro",
                table: "Proveedor");

            migrationBuilder.RenameColumn(
                name: "FechaAlta",
                table: "Proveedor",
                newName: "fechaAlta");

            migrationBuilder.RenameColumn(
                name: "EliminadoLogico",
                table: "Proveedor",
                newName: "eliminadoLogico");

            migrationBuilder.CreateTable(
                name: "RubroProveedores",
                columns: table => new
                {
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    IdRubro = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RubroProveedores", x => new { x.ProveedorId, x.IdRubro });
                    table.ForeignKey(
                        name: "FK_RubroProveedores_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
                        principalColumn: "ProveedorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RubroProveedores_Rubro",
                        column: x => x.IdRubro,
                        principalTable: "Rubro",
                        principalColumn: "IdRubro");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RubroProveedores_IdRubro",
                table: "RubroProveedores",
                column: "IdRubro");
        }
    }
}
