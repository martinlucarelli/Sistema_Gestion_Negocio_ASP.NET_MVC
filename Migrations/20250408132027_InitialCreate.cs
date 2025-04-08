using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Forma_Pago",
                columns: table => new
                {
                    IdFormaPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "100, 1"),
                    TipoFormaPago = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forma_Pago", x => x.IdFormaPago);
                });

            migrationBuilder.CreateTable(
                name: "Rubro",
                columns: table => new
                {
                    IdRubro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1000, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rubro", x => x.IdRubro);
                });

            migrationBuilder.CreateTable(
                name: "Venta",
                columns: table => new
                {
                    IdVenta = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false),
                    FormaPagoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venta", x => x.IdVenta);
                    table.ForeignKey(
                        name: "FK_Venta_Forma_Pago_FormaPagoId",
                        column: x => x.FormaPagoId,
                        principalTable: "Forma_Pago",
                        principalColumn: "IdFormaPago",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Negocio",
                columns: table => new
                {
                    IdNegocio = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RubroId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Negocio", x => x.IdNegocio);
                    table.ForeignKey(
                        name: "FK_Negocio_Rubro_RubroId",
                        column: x => x.RubroId,
                        principalTable: "Rubro",
                        principalColumn: "IdRubro",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    IdProducto = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Precio = table.Column<double>(type: "float", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    NegocioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.IdProducto);
                    table.ForeignKey(
                        name: "FK_Producto_Negocio_NegocioId",
                        column: x => x.NegocioId,
                        principalTable: "Negocio",
                        principalColumn: "IdNegocio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Clave = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Confirmado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TokenConfirmacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rol = table.Column<int>(type: "int", nullable: false),
                    NegocioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Usuario_Negocio_NegocioId",
                        column: x => x.NegocioId,
                        principalTable: "Negocio",
                        principalColumn: "IdNegocio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Detalle_Ventas",
                columns: table => new
                {
                    IdDetalleVenta = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CantidadProductos = table.Column<int>(type: "int", nullable: false),
                    Subtotal = table.Column<double>(type: "float", nullable: false),
                    VentaId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProductoId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detalle_Ventas", x => x.IdDetalleVenta);
                    table.ForeignKey(
                        name: "FK_Detalle_Ventas_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "IdProducto");
                    table.ForeignKey(
                        name: "FK_Detalle_Ventas_Venta_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Venta",
                        principalColumn: "IdVenta");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Ventas_ProductoId",
                table: "Detalle_Ventas",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Ventas_VentaId",
                table: "Detalle_Ventas",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Negocio_RubroId",
                table: "Negocio",
                column: "RubroId");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_NegocioId",
                table: "Producto",
                column: "NegocioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_NegocioId",
                table: "Usuario",
                column: "NegocioId");

            migrationBuilder.CreateIndex(
                name: "IX_Venta_FormaPagoId",
                table: "Venta",
                column: "FormaPagoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Detalle_Ventas");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Venta");

            migrationBuilder.DropTable(
                name: "Negocio");

            migrationBuilder.DropTable(
                name: "Forma_Pago");

            migrationBuilder.DropTable(
                name: "Rubro");
        }
    }
}
