using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlStock.Migrations
{
    /// <inheritdoc />
    public partial class agregoSectionAEgresos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "Egresos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Egresos_SectionId",
                table: "Egresos",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Egresos_Sections_SectionId",
                table: "Egresos",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "SectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Egresos_Sections_SectionId",
                table: "Egresos");

            migrationBuilder.DropIndex(
                name: "IX_Egresos_SectionId",
                table: "Egresos");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Egresos");
        }
    }
}
