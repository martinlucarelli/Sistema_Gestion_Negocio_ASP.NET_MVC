using System.ComponentModel.DataAnnotations;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models.ViewModels
{
    public class RestablecerClaveViewModel
    {

        public string Token { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string NuevaClave { get; set; }

        [Required(ErrorMessage = "Debes confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare("NuevaClave", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarClave { get; set; }

        public string Correo { get; set; }



    }
}
