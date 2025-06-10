namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models
{
    public class Negocio
    {

        public Guid IdNegocio { get; set; } = new Guid(); //Ya se instancia el objeto con el ID
        public string? Nombre {  get; set; }
        public string? Direccion {  get; set; }

        public int RubroId { get; set; } // foreign key

        //Relacion con rubro
        public Rubro? Rubro {  get; set; }
        
        // Relacion con usuarios y productos
        public ICollection<Usuario>? Usuarios { get; set; }
        public ICollection<Producto>? Productos { get; set; }

        //Relacion con ventas
        public ICollection<Venta> ventas { get; set; }
    }
}
