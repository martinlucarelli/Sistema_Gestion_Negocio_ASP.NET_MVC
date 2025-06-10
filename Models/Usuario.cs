namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models
{
    public class Usuario
    {

        public Guid IdUsuario { get; set; } = new Guid(); //Ya se instancia el objeto con el ID
        public string Correo { get; set; }
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public bool Confirmado {  get; set; } = false; //Ya se instacia el objeto con este campo como false
        public string TokenConfirmacion {  get; set; }
        public Rol Rol { get; set; }
        public Guid NegocioId { get; set; } //foreign key

        //Relacion con negocio.
        public Negocio Negocio { get; set; }

        //Relacion con Venta

        public ICollection<Venta> ventas { get; set; }
    }
}
