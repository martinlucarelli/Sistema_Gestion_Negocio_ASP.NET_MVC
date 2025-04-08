namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Models
{
    //El enum no se crea dentro de una clase porque EF puede mappear enum pero no si estan dentro de una clase.
        public enum Rol
        {
            AdministradorGeneral =1,
            AdministradorNegocio=2,
            Empleado=3


        }


}
