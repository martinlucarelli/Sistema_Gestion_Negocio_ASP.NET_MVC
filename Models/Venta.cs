using NanoidDotNet;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models
{
    public class Venta
    {
        
        public string IdVenta {  get; set; } = Nanoid.Generate("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 2) +
                                                Nanoid.Generate("0123456789", 5) + "-" +
                                                Nanoid.Generate("0123456789", 8);
                                               //Se genera un nanoId personalizado con este formato : LLNNNNN-NNNNNNNN
        public DateTime Fecha { get; set; } = DateTime.Now; //Ya se instancia el objeto con la fecha .
        public double Total {  get; set; }
        public int FormaPagoId {  get; set; } //foreign key

        //Relacion con FormaPago
        public FormaPago FormaPago {  get; set; }

        //Relacion con detalle venta
        public ICollection<DetalleVenta> detalleVentas { get; set; }

    }
}
