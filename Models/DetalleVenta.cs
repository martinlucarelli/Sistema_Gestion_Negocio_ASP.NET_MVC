using NanoidDotNet;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models
{
    public class DetalleVenta
    {
        public string IdDetalleVenta { get; set; } = Nanoid.Generate("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",8);
        public int CantidadProductos { get; set; }
        public double Subtotal { get; set; }
        public string VentaId {  get; set; } //foreign key
        public string ProductoId {  get; set; } //foreign key

        //Relacion con producto y venta
        public Producto producto { get; set; }
        public Venta venta { get; set; }
    }
}
