using NanoidDotNet;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models
{
    public class Producto
    {
        
        public string IdProducto { get; set; } = Nanoid.Generate("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 4) + "-" +
                                                Nanoid.Generate("0123456789", 8);
                                            //Se genera un nanoId personalizado con este formato :LLLL-NNNNNNNN
                                                
        public string? Nombre { get; set;}
        public double Precio { get; set;}
        public int Stock { get; set;} = 0; //Se instacia el objeto con stock 0
        public Guid? NegocioId { get; set; } //foreign key

        //Relacion con negocio
        public Negocio? Negocio {  get; set;}

        //Relacion con Detalle venta
        public ICollection<DetalleVenta>? detalleVentas { get; set; }

    }
}
