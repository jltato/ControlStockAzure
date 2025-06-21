using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlStock.Migrations
{
    /// <inheritdoc />
    public partial class observacionesIngresos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleIngreso_Ingresos_IngresoId",
                table: "DetalleIngreso");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "DetalleIngreso");

            migrationBuilder.RenameColumn(
                name: "fehchaIngreso",
                table: "Ingresos",
                newName: "Timestamp");

            migrationBuilder.AddColumn<bool>(
                name: "EliminadoLogico",
                table: "Ingresos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "Ingresos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "ScopeId",
                table: "DetalleIngreso",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IngresoId",
                table: "DetalleIngreso",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleIngreso_Ingresos_IngresoId",
                table: "DetalleIngreso",
                column: "IngresoId",
                principalTable: "Ingresos",
                principalColumn: "IngresoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleIngreso_Ingresos_IngresoId",
                table: "DetalleIngreso");

            migrationBuilder.DropColumn(
                name: "EliminadoLogico",
                table: "Ingresos");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "Ingresos");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Ingresos",
                newName: "fehchaIngreso");

            migrationBuilder.AlterColumn<int>(
                name: "ScopeId",
                table: "DetalleIngreso",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IngresoId",
                table: "DetalleIngreso",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "DetalleIngreso",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleIngreso_Ingresos_IngresoId",
                table: "DetalleIngreso",
                column: "IngresoId",
                principalTable: "Ingresos",
                principalColumn: "IngresoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
