using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlStock.Migrations
{
    /// <inheritdoc />
    public partial class Egresos3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Destino",
                table: "Egresos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Destino",
                table: "Egresos");
        }
    }
}
