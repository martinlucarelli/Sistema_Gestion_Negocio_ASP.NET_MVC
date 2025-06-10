using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models.DTO;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Helper
{
    public class CrearPDFHelper
    {

        public static byte[] crearPDFFactura(FacturaDTO venta)
        {

            var document =Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

                    // ENCABEZADO
                    page.Header().ShowOnce().Row(row =>
                    {
                        row.RelativeItem().Border(1).Padding(5).Column(col =>
                        {
                            col.Item().AlignCenter().Text($"{venta.NombreNegocio}").Bold().FontSize(14).FontColor("#2F4F4F");
                            col.Item().AlignCenter().Text($"{venta.DireccionNegocio}").FontSize(9).FontColor(Colors.Grey.Darken2);
                        });
                    });

                    // CUERPO
                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        // NRO VENTA Y FECHA
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text(txt =>
                            {
                                txt.Span("Nro Venta: ").SemiBold();
                                txt.Span($"{venta.IdVenta}");
                            });

                            row.RelativeItem().AlignRight().Text(txt =>
                            {
                                txt.Span("Fecha: ").SemiBold();
                                txt.Span($"{venta.fechaVenta}");
                            });
                        });

                        col.Item().PaddingVertical(10).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);

                        // CUADRO CONTENEDOR: Detalles + Total + Método de pago
                        col.Item().Border(1).BorderColor("#CCCCCC").Padding(5).Column(cuadro =>
                        {
                            // TABLA DE DETALLES
                            cuadro.Item().Table(tabla =>
                            {
                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3); // Producto
                                    columns.RelativeColumn();   // Precio Unit
                                    columns.RelativeColumn();   // Cantidad
                                    columns.RelativeColumn();   // Total
                                });

                                // ENCABEZADO
                                tabla.Header(header =>
                                {
                                    string headerColor = "#1F4E79"; // Azul oscuro
                                    string fontColor = "#FFFFFF";

                                    header.Cell().Background(headerColor).Padding(3).Text("Producto").FontColor(fontColor).Bold();
                                    header.Cell().Background(headerColor).Padding(3).AlignRight().Text("Precio Unit").FontColor(fontColor).Bold();
                                    header.Cell().Background(headerColor).Padding(3).AlignRight().Text("Cantidad").FontColor(fontColor).Bold();
                                    header.Cell().Background(headerColor).Padding(3).AlignRight().Text("Total").FontColor(fontColor).Bold();
                                });

                                // FILAS
                                decimal totalGeneral = 0;
                                foreach (var item in venta.Detalle)
                                {
                                    string borderColor = "#d9d9d9";

                                    tabla.Cell().BorderBottom(0.5f).BorderColor(borderColor).Padding(2).Text($"{item.NombreProducto}").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor(borderColor).Padding(2).AlignRight().Text($"{item.PrecioProducto:F2}").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor(borderColor).Padding(2).AlignRight().Text($"{item.CantidadProductos}").FontSize(10);
                                    tabla.Cell().BorderBottom(0.5f).BorderColor(borderColor).Padding(2).AlignRight().Text($"{item.Subtotal:F2}").FontSize(10);
                                    
                                }
                                tabla.Cell().ColumnSpan(4).Text(" ");
                                // FILA TOTAL con fondo oscuro solo en las últimas 2 columnas
                                tabla.Cell().ColumnSpan(2); // celda vacía para alinear
                                tabla.Cell().Background("#1F4E79").Padding(5).AlignRight().Text(txt =>
                                {
                                    txt.Span("Total a pagar:").FontColor("#FFFFFF").Bold();
                                });

                                tabla.Cell().Background("##1F4E79").Padding(4).AlignRight().Text(txt =>
                                {
                                    txt.Span($"{venta.Total:F2}").FontColor("#FFFFFF").Bold();
                                });

                                
                            });

                            // MÉTODO DE PAGO Y VENDEDOR (dentro del mismo recuadro)
                            cuadro.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Text($"Método de pago: {venta.FormaPago}").FontSize(10);
                                row.RelativeItem().AlignRight().Text($"Vendedor: {venta.Vendedor}").FontSize(10).Italic();
                            });
                        });
                    });

                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            return ms.ToArray();
        }


    }
}
