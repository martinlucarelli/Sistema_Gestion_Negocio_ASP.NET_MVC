using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Helper;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models.ViewModels;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Services;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Controllers
{
    public class EmpleadoController : Controller
    {
        NegocioContext context;
        public ILogger<LoginController> logger;
        private readonly EmailService emailService;
        private readonly BashClaveService bashClaveService;

        public EmpleadoController(NegocioContext _context, ILogger<LoginController> _logger, EmailService _emailService, BashClaveService _bashClaveService)
        {
            context = _context;
            logger = _logger;
            emailService = _emailService;
            bashClaveService = _bashClaveService;

        }

        [HttpPost]
        public async Task<IActionResult> AgregarEmpleado(UsuarioViewModel user)
        {


            string token = Guid.NewGuid().ToString();
            logger.LogWarning($"El token que se genero fue : {token}");

            if (context.Usuarios.Any(u => u.Correo == user.correo))
            {
                TempData["DangerMessage"] = "Ya existe un usuario registrado con ese correo.";
                TempData.Keep("DangerMessage");
                return RedirectToAction("MiNegocio", "Negocio");
            }

            Usuario empleadoARegistrar = new Usuario()
            {
                Nombre = user.nombre,
                Correo = user.correo,
                Clave = "AaJDnn123",
                TokenConfirmacion = token,
                Rol = Rol.Empleado,
                NegocioId = user.idNegocio
            };
            context.Usuarios.Add(empleadoARegistrar);
            context.SaveChanges();

            //Enlace para enviar correo

            string linkRegistrarNegocio = Url.Action("CompletarRegistroEmpleado", "Empleado", new { token }, Request.Scheme);
            string mensaje = $"<p>Para terminar de registrar su cuenta, haz click <a href='{linkRegistrarNegocio}'>aquí</a>";

            await emailService.EnviarCorreo(empleadoARegistrar.Correo, "Completar registro", mensaje);

            TempData["SuccessMessage"] = "Correo enviado correctamente al usuario para completar el registro.";
            return RedirectToAction("MiNegocio","Negocio"); 
        }
    

        public IActionResult CompletarRegistroEmpleado(string token)
        {
            var usuario = context.Usuarios.FirstOrDefault(u=> u.TokenConfirmacion == token);

            if(usuario == null) 
            {
                logger.LogError($"No se encontro ningun usuario con ese token en la base de datos");
                return NotFound();
            }

            return View(new UsuarioViewModel { token= token });

        }

        [HttpPost]
        public IActionResult CompletarRegistroEmpleado(UsuarioViewModel user, string token)
        {
            var usuarioActualizar = context.Usuarios.FirstOrDefault(u => u.TokenConfirmacion == token);

            if (user.clave == user.repetirClave)
            {
                string claveCifrada = bashClaveService.ConvertirSha256(user.clave);

                usuarioActualizar.Clave = claveCifrada;
                usuarioActualizar.Confirmado = true;
                usuarioActualizar.TokenConfirmacion = null;

                context.Usuarios.Update(usuarioActualizar);
                context.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("clave", "Las claves no coinciden");
                return View(user);
            }

            return RedirectToAction("EmpleadoRegistradoConExito", "Empleado");


        }
        public IActionResult EmpleadoRegistradoConExito() { return View(); }

        [HttpPost]
        public IActionResult CambiarNombreEmpleado(string correo,string nuevoNombre) 
        {
            var usuarioEditar = context.Usuarios.FirstOrDefault(u => u.Correo == correo);

            if(usuarioEditar == null) { return NotFound(); }

            usuarioEditar.Nombre = nuevoNombre;
            context.SaveChanges();

            return RedirectToAction("MiNegocio","Negocio");
        
        }

        [HttpPost]
        public IActionResult EliminarEmpleado(string correo)
        {
            var usuarioEliminar = context.Usuarios.FirstOrDefault(u=> u.Correo== correo);

            if(usuarioEliminar == null) { return NotFound(); }

            context.Usuarios.Remove(usuarioEliminar);
            context.SaveChanges();

            return RedirectToAction("MiNegocio", "Negocio");

        }


    }
}

