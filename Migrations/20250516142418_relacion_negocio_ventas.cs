using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Migrations
{
    /// <inheritdoc />
    public partial class relacion_negocio_ventas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Detalle_Ventas_Producto_ProductoId",
                table: "Detalle_Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Detalle_Ventas_Venta_VentaId",
                table: "Detalle_Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Producto_Negocio_NegocioId",
                table: "Producto");

            migrationBuilder.DropForeignKey(
                name: "FK_Venta_Usuario_UsuarioId",
                table: "Venta");

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuarioId",
                table: "Venta",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "NegocioId",
                table: "Venta",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "NegocioId",
                table: "Producto",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "VentaId",
                table: "Detalle_Ventas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Venta_NegocioId",
                table: "Venta",
                column: "NegocioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Detalle_Ventas_Producto_ProductoId",
                table: "Detalle_Ventas",
                column: "ProductoId",
                principalTable: "Producto",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Detalle_Ventas_Venta_VentaId",
                table: "Detalle_Ventas",
                column: "VentaId",
                principalTable: "Venta",
                principalColumn: "IdVenta",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_Negocio_NegocioId",
                table: "Producto",
                column: "NegocioId",
                principalTable: "Negocio",
                principalColumn: "IdNegocio");

            migrationBuilder.AddForeignKey(
                name: "FK_Venta_Negocio_NegocioId",
                table: "Venta",
                column: "NegocioId",
                principalTable: "Negocio",
                principalColumn: "IdNegocio");

            migrationBuilder.AddForeignKey(
                name: "FK_Venta_Usuario_UsuarioId",
                table: "Venta",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Detalle_Ventas_Producto_ProductoId",
                table: "Detalle_Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Detalle_Ventas_Venta_VentaId",
                table: "Detalle_Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Producto_Negocio_NegocioId",
                table: "Producto");

            migrationBuilder.DropForeignKey(
                name: "FK_Venta_Negocio_NegocioId",
                table: "Venta");

            migrationBuilder.DropForeignKey(
                name: "FK_Venta_Usuario_UsuarioId",
                table: "Venta");

            migrationBuilder.DropIndex(
                name: "IX_Venta_NegocioId",
                table: "Venta");

            migrationBuilder.DropColumn(
                name: "NegocioId",
                table: "Venta");

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuarioId",
                table: "Venta",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "NegocioId",
                table: "Producto",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VentaId",
                table: "Detalle_Ventas",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Detalle_Ventas_Producto_ProductoId",
                table: "Detalle_Ventas",
                column: "ProductoId",
                principalTable: "Producto",
                principalColumn: "IdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_Detalle_Ventas_Venta_VentaId",
                table: "Detalle_Ventas",
                column: "VentaId",
                principalTable: "Venta",
                principalColumn: "IdVenta");

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_Negocio_NegocioId",
                table: "Producto",
                column: "NegocioId",
                principalTable: "Negocio",
                principalColumn: "IdNegocio",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Venta_Usuario_UsuarioId",
                table: "Venta",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
