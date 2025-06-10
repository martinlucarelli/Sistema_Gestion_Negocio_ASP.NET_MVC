using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Migrations
{
    /// <inheritdoc />
    public partial class relacion_usuario_ventas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "Venta",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Venta_UsuarioId",
                table: "Venta",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Venta_Usuario_UsuarioId",
                table: "Venta",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Venta_Usuario_UsuarioId",
                table: "Venta");

            migrationBuilder.DropIndex(
                name: "IX_Venta_UsuarioId",
                table: "Venta");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Venta");
        }
    }
}
