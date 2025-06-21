using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlStock.Migrations
{
    /// <inheritdoc />
    public partial class userpermissionsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_Scopes_ScopeId",
                table: "UserPermissions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAlta",
                table: "Proveedor",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Proveedor",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_Scopes_ScopeId",
                table: "UserPermissions",
                column: "ScopeId",
                principalTable: "Scopes",
                principalColumn: "ScopeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_Scopes_ScopeId",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Proveedor");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAlta",
                table: "Proveedor",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_Scopes_ScopeId",
                table: "UserPermissions",
                column: "ScopeId",
                principalTable: "Scopes",
                principalColumn: "ScopeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
