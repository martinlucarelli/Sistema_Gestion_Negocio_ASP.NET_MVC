namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models
{
    public class Rubro
    {
        public int IdRubro { get; set; }
        public string Nombre { get; set; }

        //Relacion con Negocio
        public ICollection<Negocio> negocios { get; set; }

    }
}
