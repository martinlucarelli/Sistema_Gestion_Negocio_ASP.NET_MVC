namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models.ViewModels
{
    public class NegocioConEmpleadoViewModel
    {
        public Negocio Negocio { get; set; }

        public List<Usuario> Empleados { get; set; }

        public string nombreRubro { get; set; }

        public UsuarioViewModel EmpleadoNuevo { get; set; } = new UsuarioViewModel(); //Empleado utilizado para el modal
    }
}
