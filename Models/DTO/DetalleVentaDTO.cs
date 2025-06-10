namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models.DTO
{
    public class DetalleVentaDTO
    {
        public string? ProductoId { get; set; }
        public double PrecioProducto { get;set; }
        public string? NombreProducto { get; set; }
        public int CantidadProductos { get; set; }
        public double Subtotal { get; set; }

    }
}
