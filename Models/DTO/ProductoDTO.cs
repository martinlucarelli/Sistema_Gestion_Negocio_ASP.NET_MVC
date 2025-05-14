namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models.DTO
{
    public class ProductoDTO
    {
        public string IdProducto { get; set; }
        public string? Nombre { get; set; }
        public double? Precio { get; set; }
        public int? Stock { get; set; }
        public Guid NegocioId { get; set; }


    }
}
