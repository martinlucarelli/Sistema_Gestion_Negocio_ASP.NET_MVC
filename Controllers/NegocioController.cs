using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Common;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Models.ViewModels;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Helper;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Controllers
{
    

    public class NegocioController : Controller
    {


        NegocioContext context;
        public ILogger<LoginController> logger;
        private readonly EmailService emailService;
        private readonly BashClaveService bashClaveService;

        public NegocioController(NegocioContext _context, ILogger<LoginController> _logger, EmailService _emailService, BashClaveService _bashClaveService)
        {
            context = _context;
            logger = _logger;
            emailService = _emailService;
            bashClaveService = _bashClaveService;

        }

        public IActionResult Negocios()
        {
            var negocioConRubro = context.Negocios.Join(context.Rubros,
                negocio => negocio.RubroId,
                rubro => rubro.IdRubro,
                (negocio, rubro) => new
                {
                    negocio.IdNegocio,
                    negocio.Nombre,
                    negocio.Direccion,
                    rubroNombre = rubro.Nombre

                })
                .ToList();
               
            return View(negocioConRubro);
        }

        public IActionResult AgregarNegocio()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgregarNegocio(string correo)
        {
            string token = Guid.NewGuid().ToString();

            logger.LogWarning($"El token que se genero fue : {token}"); //Log para ver el token en consola
            
            Usuario usuarioBandera = new Usuario()
            {
                Correo = correo,
                Clave = "Afjsirm11K22masl1nnde",
                Nombre = "a",
                Confirmado = false, //NO SE CONFIRMA HASTA QUE SE CREE EL NEGOCIO
                TokenConfirmacion = token,
                Rol = Rol.AdministradorNegocio,
                //NegocioId = Guid.NewGuid() //Se crea un Guid solo para que nos deje guardar los datos, luego se cambia por el id real del negocio.
                NegocioId = Guid.Parse("06FDE092-6A5A-4156-B9A0-6327D8BB2FAD")
            };
            context.Usuarios.Add(usuarioBandera);
            context.SaveChanges();

            //Enlace para enviar correo de registro
            string linkRegistrarNegocio = Url.Action("RegistrarAdministradorNegocio", "Negocio", new { token }, Request.Scheme);
            string mensaje = $"<p>Para comenzar a registrar su negocio, haz click <a href='{linkRegistrarNegocio}'>aquí</a>.</p>";

            await emailService.EnviarCorreo(usuarioBandera.Correo, "Registrar negocio", mensaje);

            return View();
        }

            [HttpGet]
            public IActionResult RegistrarAdministradorNegocio(string token)
            {
                logger.LogWarning($"El token que se recibio en RegistrarAdministradorNegocio fue : {token}");

                var usuario = context.Usuarios.FirstOrDefault(u => u.TokenConfirmacion == token);


                if (usuario == null) 
                {
                    logger.LogError($"No se encontro ningun usuario con ese token en la base de datos");
                    return NotFound(); 
            
                } //Se puede agregar una vista que diga que el link ya fue usado.

                return View(new UsuarioViewModel {token=token});
            }


        [HttpPost]
        public  IActionResult RegistrarAdministradorNegocio(UsuarioViewModel user,string token)
        {
            var usuarioActualizar =context.Usuarios.FirstOrDefault(u=> u.TokenConfirmacion == token);
            if(usuarioActualizar == null)
            {
                ModelState.AddModelError("correo", "no encotramos un usuario con esas");
                return View(user);
            }

            if (context.Usuarios.Any(u => u.Correo == user.correo))
            {
                ModelState.AddModelError("correo", "Ya existe un usuario registrado con ese correo");
                return View(user);
            }

            if (user.clave == user.repetirClave)
            {
                string claveCifrada= bashClaveService.ConvertirSha256(user.clave);
                

                usuarioActualizar.Correo = user.correo;
                usuarioActualizar.Nombre = user.nombre;
                usuarioActualizar.Clave = claveCifrada;
                usuarioActualizar.Rol = Rol.AdministradorNegocio;
                //CONFIRMADO Y NEGOCIOID se terminan de completar cuando se cree el negocio, por ahora tiene un
                // id de nogocio temporal que esta relacionado a un nogocio "fantasma" para poder completar el registro.
               
                context.Usuarios.Update(usuarioActualizar);
                context.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("clave", "Las claves no coinciden");
                return View(user);
            }

            return RedirectToAction("AdminNegocioRegistradoConExito", "Negocio", new {token});
        }

        [HttpGet]
        public IActionResult RegistrarNegocio(string token)
        {
            var usuario = context.Usuarios.FirstOrDefault(u => u.TokenConfirmacion == token);
            if (usuario == null) return NotFound();

            var modelo = new NegocioViewModel
            {
                rubros = context.Rubros.ToList(),
                tokenUsuario = token
            };

            return View(modelo);
        }

        [HttpPost]
        public IActionResult RegistrarNegocio(NegocioViewModel n ,string token)
        {
            Negocio negocioFinal = new Negocio()
            {
                Nombre = n.nombre,
                Direccion = n.direccion,
                RubroId = n.rubroId
            };
            context.Negocios.Add(negocioFinal);
            context.SaveChanges();

            Usuario usuarioActualizar = context.Usuarios.FirstOrDefault(u=> u.TokenConfirmacion == token);
            
            if(usuarioActualizar == null)
            {
                return NotFound();
            }
            else 
            {
                usuarioActualizar.NegocioId = negocioFinal.IdNegocio;
                usuarioActualizar.TokenConfirmacion = null;
                usuarioActualizar.Confirmado = true;
                context.Usuarios.Update(usuarioActualizar);
                context.SaveChanges();
            }

            return RedirectToAction("NegocioRegistradoConExito", "Negocio");
        }

        public IActionResult AdminNegocioRegistradoConExito(){ return View();}
        public IActionResult NegocioRegistradoConExito() { return View(); }

        public IActionResult MiNegocio() 
        {
            string usuarioId = UsuarioHelper.ObtenerUsuarioId(HttpContext);

            var usuario = context.Usuarios.Include(u => u.Negocio).FirstOrDefault(u => u.IdUsuario.ToString() == usuarioId);

            if(usuario==null || usuario.Rol != Rol.AdministradorNegocio)
            {
               return Unauthorized(); 
            }

            var empleados = context.Usuarios.Where(u=> u.NegocioId== usuario.NegocioId  && u.Rol==Rol.Empleado).ToList();
            var RubroDelNegocio = context.Rubros.FirstOrDefault(r=> r.IdRubro == usuario.Negocio.RubroId);
            
            var modelo = new NegocioConEmpleadoViewModel
            {
                Negocio = usuario.Negocio,
                Empleados = empleados,
                nombreRubro=RubroDelNegocio.Nombre,
                EmpleadoNuevo = new UsuarioViewModel
                {
                    idNegocio=usuario.NegocioId,
                }
            };

            return View(modelo); 
        }

        [HttpGet]
        public IActionResult EditarNegocio(string idNegocio)
        {

            var negocio = context.Negocios.FirstOrDefault(n=> n.IdNegocio.ToString()== idNegocio);
            
            if(negocio== null)
            {
                return NotFound();
            }

            var negocioModel = new NegocioViewModel
            {
                nombre = negocio.Nombre,
                direccion = negocio.Direccion,
                rubroId = negocio.RubroId,
                rubros = context.Rubros.ToList()
            };

            ViewBag.negocioId = idNegocio;
          
            return View(negocioModel);
        }

        [HttpPost]
        public IActionResult EditarNegocio(string idNegocio, NegocioViewModel negocioEditar)
        {
            var negocio = context.Negocios.FirstOrDefault(n => n.IdNegocio.ToString() == idNegocio);

            if(negocio== null) { return NotFound(); }

            if(!ModelState.IsValid) 
            {
                negocioEditar.rubros = context.Rubros.ToList();
                ViewBag.negocioId= idNegocio;
                return View(negocioEditar);
            }

            negocio.Nombre = negocioEditar.nombre;
            negocio.Direccion = negocioEditar.direccion;
            negocio.RubroId = negocioEditar.rubroId;

            context.SaveChanges();

            return RedirectToAction("MiNegocio");
        }





    }
}
