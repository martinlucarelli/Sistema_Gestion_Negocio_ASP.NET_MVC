using System.ComponentModel.DataAnnotations;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models.ViewModels
{
    public class UsuarioViewModel
    {
        [EmailAddress]
        [Required]
        public string correo { get; set; }
        [Required]
        public string clave {  get; set; }

        public string recuperarClave { get; set; } 


    }
}
