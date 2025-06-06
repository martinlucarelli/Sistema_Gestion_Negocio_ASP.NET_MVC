
# Documentación Técnica del Sistema de Gestión de Negocio

Este documento contiene una descripción general de los componentes técnicos del sistema.

## Indice

1.[LoginController](#logincontroller)
   - [Login(UsuarioViewModel user)](#loginusuarioviewmodel-user)
   - [RecuperarClave(string correo)](#recuperarclavestring-correo)
   - [RestablecerClave(string token)](#restablecerclavestring-token)
   - [CerrarSesion()](#cerrarsesion)
     
2.[NegocioController](#negociocontroller)
   - [Negocios()](#negocios())
   - [AgregarNegocio(string correo)](#agregarnegocio(string-correo))
   - [RegistrarAdministradorNegocio(string token)](#registraradministradornegocio(string-token))
   - [RegistrarAdministradorNegocio(UsuarioViewModel user,string token)](#registraradministradoraegocio(usuarioviewModel-user,string-token))
   - [RegistrarNegocio(string token)](#registrarnegocio(string-token))
   - [RegistrarNegocio(NegocioViewModel n ,string token)](#registrarnegocio(negocioviewmodel-n,string-token))
   - [MiNegocio()](#minegocio())
   - [EditarNegocio(string idNegocio)](#editarnegocio(string-idnegocio))
   - [EditarNegocio(string idNegocio, NegocioViewModel negocioEditar)](#editarnegocio(string-idnegocio,-negocioviewmodel-negocioeditar))

3.[EmpleadoController](#empleadocontroller)
   - [AgregarEmpleado(UsuarioViewModel user)](#agregarempleado(usuarioviewmodel-user))
   - [Negocios()](#negocios)
   - [AgregarNegocio(string correo)](#agregarnegociostring-correo)
   - [RegistrarAdministradorNegocio(string token)](#registraradministradornegociostring-token)
   - [RegistrarAdministradorNegocio(UsuarioViewModel user string token)](#registraradministradornegociousuarioviewmodel-user-string-token)
   - [RegistrarNegocio(string token)](#registrarnegociostring-token)
   - [RegistrarNegocio(NegocioViewModel n, string token)](#registrarnegocionegocioviewmodel-n-string-token)
 
4.[ProductoController](#productocontroller)
    -[Productos()](#productos())

5.[ProductoApiService](#productoapiservice)

6.[ProductoApiController](#productocontroller)


---
## SE DEBE AGREGAR LA DOCUMENTACION DE 

-Configuration (para que sirve la clase de esa carpeta)
-EmailService (explicar la funcion).
Funciones de javascript relacionadas con POST,PUT Y DELETE



---

## LoginController

### Login(UsuarioViewModel user)

```csharp
        [HttpPost]
        public async Task<IActionResult> Login(UsuarioViewModel user)
        {
          
            var usuarioIngresado = context.Usuarios.FirstOrDefault(u => u.Correo == user.correo);
          
            if(usuarioIngresado==null || usuarioIngresado.Clave != ConvertirSha256(user.clave)) 
            {
                ModelState.AddModelError("correo", "Correo o contraseña incorrectas, intente de nuevo");
                return View();
            
            }
            else
            {
              
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,usuarioIngresado.Nombre),
                    new Claim("Correo",usuarioIngresado.Correo),
                    new Claim(ClaimTypes.Role,usuarioIngresado.Rol.ToString()),

                };

                var claimsIdentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));  

                return RedirectToAction("Index","Home");
            }
        }
```

---

- `var usuarioIngresado = context.Usuarios.FirstOrDefault(u => u.Correo == user.correo);`

	Buscar en la base de de datos si hay un registro con el mismo correo y lo guarda en `usuarioIngresado`.
---

```csharp
                  var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,usuarioIngresado.Nombre),
                    new Claim("Correo",usuarioIngresado.Correo),
                    new Claim(ClaimTypes.Role,usuarioIngresado.Rol.ToString()),

                };
```

   Creamos una lista de Claims. Los claims sirven para guardad algun dato relevante sobre el usuario. Al loguearse se crea una
   lista de claims que luego se guardaran en una Cookie.

---  
   `var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);`

   Reprensenta la identidad del usuario que se acaba de loguear . Anteriormente se creo una lista con todos los datos del usuario
   y en esta linea usamos esos datos para generar una Cookie que se guarda en el navegador del usuario que inicio sesion.

---
   `await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));`

   HttpContex es un objeto que contiene toda la informacion sobre la peticion HTTP actual. En este caso lo usamos para guardar
   los datos del usuario en una Cookie con `SignInAsync` y nos permite iniciar sesion.

---
### RecuperarClave(string correo)

```csharp
            [HttpPost]
        public async Task<IActionResult> RecuperarClave(string correo)
        {
            var usuario = context.Usuarios.FirstOrDefault(u=> u.Correo == correo);

            if(usuario==null) 
            {
                ModelState.AddModelError("correo", "No existe una cuenta vinculada a ese correo");
                return View();
            
            }

            string token= Guid.NewGuid().ToString();
            usuario.TokenConfirmacion=token;
            context.SaveChanges();

            string linkRecuperacion = Url.Action("RestablecerClave", "Login", new { token }, Request.Scheme);
            string mensaje = $"<p>Para restablecer tu contraseña, haz click <a href='{linkRecuperacion}'>aquí</a>.</p>";

            await emailService.EnviarCorreo(usuario.Correo, "Recuperacion de contraseña", mensaje);

            return RedirectToAction("CorreoEnviadoCambiarClave","Login");

        }
```

---
`var usuario = context.Usuarios.FirstOrDefault(u=> u.Correo == correo);`

Primero busca a un usuario con el correo y lo guarda en `usuario`.

```
      string token= Guid.NewGuid().ToString();
      usuario.TokenConfirmacion=token;
      context.SaveChanges();
```

Genera un guid que se utiliza como un token para el usuario que quiere cambiar la contraseña y se guarda ese token en la 
base de datos.

---
`string linkRecuperacion = Url.Action("RestablecerClave", "Login", new { token }, Request.Scheme);`

Se crea un link que apunta al metodo `RestablecerClave` del controlador `Login` y le envia por parametro el token.
`Request.Scheme` sirve para devolver el protocolo que se esta usando en la app (http o https), es importante este paso ya que 
sirve para generar la URL correcta para usarse fuera de la app, como en este caso que el metodo `RestablecerClave` se enviara
por mail.

---
`string mensaje = $"<p>Para restablecer tu contraseña, haz click <a href='{linkRecuperacion}'>aquí</a>.</p>";`

Se crea el mensaje que se quiere enviar por mail, el formato de los correos son en HTML.

---
`await emailService.EnviarCorreo(usuario.Correo, "Recuperacion de contraseña", mensaje);`

Se utiliza la funcion `EnviarCorreo` del service.

---
## RestablecerClave(string token)


```csharp

        public IActionResult RestablecerClave(string token)
        {
            var usuario = context.Usuarios.FirstOrDefault(u=> u.TokenConfirmacion==token);
           
            if(usuario== null)
            {
                return NotFound();
            }

            return View(new RestablecerClaveViewModel { Token= token });
        }

        [HttpPost]
        public IActionResult RestablecerClave(RestablecerClaveViewModel model)
        {
            var usuario = context.Usuarios.FirstOrDefault(u=> u.TokenConfirmacion == model.Token);

            if(usuario== null)
            {
                return NotFound();
            }

            if(model.NuevaClave != model.ConfirmarClave)
            {
                ModelState.AddModelError("ConfirmarContraseña", "Las contraseñas no coinciden");
                return View(model);

            }

            usuario.Clave = ConvertirSha256(model.NuevaClave); //Bashea la contraseña
            usuario.TokenConfirmacion = null; //Elimina el token para que no se pueda vovler a acceder
            context.SaveChanges(); //Guarda los datos en la base de datos

            return RedirectToAction("Login");
        }
```
---
``` 
        public IActionResult RestablecerClave(string token)
        {
            var usuario = context.Usuarios.FirstOrDefault(u=> u.TokenConfirmacion==token);
           
            if(usuario== null)
            {
                return NotFound();
            }

            return View(new RestablecerClaveViewModel { Token= token });
        }
```

Esta primera funcion lo que hace es recibir por parametro el token (es el que se envio con la Url en la funcion `RecuperarClave`).
Luego busca el token en la base de datos y si lo encuentra devuelve la vista con un viewModel con el token.

---
``` 
            usuario.Clave = ConvertirSha256(model.NuevaClave); //Bashea la contraseña
            usuario.TokenConfirmacion = null; //Elimina el token para que no se pueda vovler a acceder
            context.SaveChanges(); //Guarda los datos en la base de datos
```

En la segunda funcion se convierte la nueva clave en Bash, se elimina el token de confirmacion para que no se pueda volver a 
acceder y se guardan los datos en la base de datos.

---
### CerrarSesion()

```csharp
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Login");

        }
```

Elimina los datos de la Cookie con `SignOutAsync` y nos permite cerrar sesion.

---


## NegocioController

### Negocios()

Esta funcion devuelve la vista de los negocios que estan guardados en la base de datos

```csharp 
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
```

Se utiliza LINQ para realizar una consulta en la que creamos una nueva variable que la llamamos `negocioConRubro` y en ella
se va a guardar el resultado de la consulta. La consulta que utilizamos es un inner join para que en la vista en lugar de que
aparezca el id del rubro, aparezca el nombre del mismo.
Luego devolvemos la vista con la lista de los negocios con los rubros con nombres en lugar del id.
   
---

## AgregarNegocio(string correo)

Esta funcion asincronica es utilizada desde el rol del administrador general y sirve para enviar un correo a un cliente que
haya contratado el servicio, en ese correo estaran todos los formularios para registrar su negocio y su usuario.

``` csharp
public async Task<IActionResult> AgregarNegocio(string correo)
        {
            string token = Guid.NewGuid().ToString();

            logger.LogWarning($"El token que se genero fue : {token}"); //Log para ver el token en consola
            
            Usuario usuarioBandera = new Usuario()
            {
                Correo = correo,
                Clave = "Afjsirm11K22masl1nnde",
                Nombre = "a",
                Confirmado = false,
                TokenConfirmacion = token,
                Rol = Rol.AdministradorNegocio,
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
```
---

`Usuario usuarioBandera = new Usuario()`

El usuario bandera sirve debido a que al momento de recibir el correo, el usuario debe tener un token que valide el acceso a ese
link. Por eso se crea un usuario bandera donde solamente se le asigna el correo y un token que generamos anteriormente. Los demas
datos son genericos y luego seran asignados los reales en la siguiente funcion.
En cuanto al Id del negocio al que pertenece este usuario bandera: Al momento de crear este usuario todavia no existe el negocio
al que va a pertenecer  , pero EF no nos permite asignar un Id de un negocio que no existe como clave foreanea y mucho menos
podemos dejarlo null, por lo tanto lo se hace es crear un negocio que esta en la base de datos y que solo sirve para almacenar
estos usuarios momentaneamente hasta que se cree el negocio real al que van a pertenecer y se les pueda asignar.
Por otro lado, se le asigna una clave sin bashear, por lo tanto es imposible que alguien acceda desde el login con ese usuario.

---

## RegistrarAdministradorNegocio(string token)

Esta funcion se encarga de devolver la vista donde se encuentra el formulario para registrar al usuario administrador del negocio
antes de devolver la vista corrobora si el token que tiene ese usuario es valido para darle acceso a la vista, evitando que
alguien sin permiso acceda a ella.

``` csharp

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
```

Ademas se incluye el token del UsuarioViewModel para mantenerlo en el post:

``` cshtml

    <form method="post" asp-action="RegistrarAdministradorNegocio" asp-controller="Negocio">
        <input type="hidden" name="token" value="@Model.token" />
        ...
    </form>

```
---

## RegistrarAdministradorNegocio(UsuarioViewModel user,string token)

Esta funcion es el post de la anterior y contiene la logica para terminar de guardar los datos en el usuario bandera que se creo
anteriormente en la funcion `agregarNegocio()`

``` 
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
```

Despues de pasar los condicionales para busacar el usuario con el token, verificar que no se exista esa direccion de correo en 
el sistema y verificar que no se repitan las claves, se pasa a hacer un update del usuario bandera ya guardando sus datos
reales, los unicos datos que todavia no se asignarion fueron:

`IdNegocio`: Todavia no esta creado el negocio, asi que sigue con el id del negocio del sistema que sirve para estos casos-
`Confirmado`: Como no pertenece a ningun negocio real, todavia no se confirma este usuario, esto evita que inicie sesion.

---

## RegistrarNegocio(string token)

Para acceder a esta funcion se hace desde el enlace de la vista `AdminNegocioRegistradoConExito`, en esta vista enviamos
el token en la URL

```cshtml

<a asp-action="RegistrarNegocio" asp-controller="Negocio" asp-route-token="@Model.token">
    Registrar negocio
</a>
```

Esta funcion devuelve la vista para registar al negocio y con ella el view model de negocio. Dentro del view model esta la 
lista de rubros y ademas el token del usuario. Si bien el token no es un atributo directo de negocio, ya que el negocio no tiene
token, debi ponerlo en el viewmodel ya que es necesario para enviar por parametro.

``` 
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
```

En la vista de esta funcion se incuye el token en modo hidden

`<input type="hidden" name="token" value="@Model.tokenUsuario" />`


---

## RegistrarNegocio(NegocioViewModel n ,string token)

Esta funcion finalmente es la encargada de registrar un negocio y guardarlo en la base de datos y luego, con el token que
recibe por parametro, busca al usuario que se guardo como "bandera" anteriormente y le asigna el id del nuevo negocio que
ya se creo y ademas lo marca como usuario confirmado. Aqui ya finaliza el registro de un dueño de negocio y su negocio.

``` 
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

```

---
## MiNegocio()

Esta funcion se encarga de mostrarle la vista de su negocio a los usuarios administradores de negocios, dentro de
esta vista se puede ver la informacion de negocio, modificar esta informacion, se pueden ver los empleados, agregar
empleados nuevos y modificar los que ya estan.
Para permitir que se muestren los modelos Negocio y Empleado en la misma vista, se creo un viewmodel que almacena
principalmente el negocio y la lista de empleados de ese negocio.

``` 
    public class NegocioConEmpleadoViewModel
    {
        public Negocio Negocio { get; set; }

        public List<Usuario> Empleados { get; set; }

        public string nombreRubro { get; set; }

        public UsuarioViewModel EmpleadoNuevo { get; set; } = new UsuarioViewModel();
    }
```

``` 
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

```

`string usuarioId = UsuarioHelper.ObtenerUsuarioId(HttpContext)` : Como para saber el negocio al que pertenece
el usuario que esta con la sesion iniciada debemos conocer su Id, se creo un helper que contiene una funcion
sencilla que devuelve el id del usuario que tiene la sesion iniciada a partir del `HttpContex` y los Claims que
que guardan datos en la Cookie.

---
## EditarNegocio(string idNegocio)

Esta funcion es la encargada de devolver la vista cuando el administrador de negocio quiere modificar algun dato
de su negocio como nombre,direccion etc. No solo va a devolver el formulario para que el usuario cambie los datos,
sino que ese formulario estara ya completo con los datos actuales del negocio.
El Id del negocio qeu se quiere editar se recibe por parametro, Id se pasa por parametro desde la vista `MiNegocio`
a partir del link que nos dirige a la vista para editar el negocio.

`<a asp-controller="Negocio" asp-action="EditarNegocio" asp-route-idNegocio="@Model.Negocio.IdNegocio" class="btn btn-sm btn-warning">Editar</a>`


```
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

```

` ViewBag.negocioId = idNegocio;` : Como el id del negocio no esta en el viewmodel, se pasa a la vista por un
viewBag.

---

## EditarNegocio(string idNegocio, NegocioViewModel negocioEditar)

Esta funcion es el post para editar el negocio. Hay que resaltar que como el viewModel no contiene el Id del
negocio, el Id se para a partir del viewBag de la funcion que devuelve de la vista, para pasarlo se utiliza
un input escondido (hidden).

```
        public IActionResult EditarNegocio(string idNegocio, NegocioViewModel negocioEditar)
        {
            var negocio = context.Negocios.FirstOrDefault(n => n.IdNegocio.ToString() == idNegocio);

            if(negocio== null) { return NotFound(); }

            if(!ModelState.IsValid) 
            {
                ViewBag.negocioId= idNegocio;
                return View(negocioEditar);
            }

            negocio.Nombre = negocioEditar.nombre;
            negocio.Direccion = negocioEditar.direccion;
            negocio.RubroId = negocioEditar.rubroId;

            context.SaveChanges();

            return RedirectToAction("MiNegocio");
        }
```

Forma de pasar el Id del negocio:

`<input type="hidden" name="idNegocio" value="@ViewBag.negocioId" />`

---

### EmpleadoController

## AgregarEmpleado(UsuarioViewModel user)

Esta funcion no tiene Get ya que el formulario para agregar empleado no es precisamente una vista, sino que es una
`PartialView`, que es una fragmento HTML que se muestra dentro de otra vista. En este caso se utiliza para mostrar
el forumlario para agregar un empleado en un modal. Para llamar a este PartialView se hace desde el cshtml donde
queremos que aparezca.

Mostrar la PartialView:

```cshtml
   <button class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalAgregarEmpleado">
        Agregar empleado
    </button>

    <div class="modal fade" id="modalAgregarEmpleado" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Nuevo Empleado</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>
                <div class="modal-body">
                    @await Html.PartialAsync("/Views/Empleado/AgregarEmpleado.cshtml", Model.EmpleadoNuevo)
                </div>
            </div>
        </div>
    </div>

```


```
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

```

`TempData` : El principal problema de menajar modals y partial views es como manejar cuando se completan incorrectamente
los formularios. Para manejar este tipo de errores se utiliza los temp data que sirven para guardar un mensaje entre
dos peticiones Http, por eso si utilizamos dos temp data, uno de exito y otro por si algo salio mal. Si al completar
el formulario y presionar enviar hay algun error enviamos un tempdata y aparecera un alert avisandole al usurio que
no se envio el correo, lo mismo si el correo fue enviado con exito.
`TempData.Keep("DangerMessage")` Sirve para que el temp data no se elimina al refrescar la pagina.

---

## ProductoController

A deiferencia de los controllers anteriores, este controller no contiene logica, de hecho solo contiene una funcion
que es la encargada de devolver la vista. Esto se debe a que para manipular los productos se creo una API interna
del sistema que va a ser la encargada de realizar todas las operaciones con ellos (GET,POST,PUT,DELETE).La logica
de estas operaciones esta en un service y la api se consume desde Javascript con fetch.

###  Productos()

```cs
        public IActionResult Productos()
        {
            string negocioIdUsuario = UsuarioHelper.ObtenerNegocioIdDelUsuario(HttpContext);

            ViewBag.negocioId = negocioIdUsuario;

            return View();
        }
```

Esta funcion simplemente devuelve la vista de productos, con la particularidad que envia un viewbag con el id del
negocio al que pertenece el usuario que esta logueado, eso es porque todos los productos estan en la misma tabla
de la base de datos, por lo tanto la vista solo debe devolver los productos del negocio del usuario que esta logueado.

---

## ProductoApiService

En este archivo se contiene la logica de la API, ademas de la interfaz que nos permite realizar la inyeccion de
dependencias.

```cs
    
     public class ProductoApiService : IProductoApiService
    {
        NegocioContext context;
        public readonly ILogger<ProductoApiService> logger;

        
        
        public ProductoApiService(NegocioContext _context, ILogger<ProductoApiService> _logger)
        {
            context= _context;
            logger= _logger;
        }

        public IEnumerable<Producto> Get(Guid idNegocio)
        { 
            return context.Productos.Where(p => p.NegocioId == idNegocio);
        }

        public Producto GetProductoDetalle(string id)
        {
            return context.Productos.Find(id);
        }

        public async Task Save(ProductoDTO nuevoProducto)
        {
            var producto = new Producto
            {
                Nombre=nuevoProducto.Nombre,
                Precio=nuevoProducto.Precio.Value,
                Stock=nuevoProducto.Stock.Value,
                NegocioId=nuevoProducto.NegocioId
            };
            
            context.Productos.Add(producto);
            await context.SaveChangesAsync();
        }
        public async Task Delete(string id)
        {
            var productoEliminar = await context.Productos.FindAsync(id);
            
            if(productoEliminar != null)
            {
                context.Productos.Remove(productoEliminar);
            }
            else
            {
                logger.LogError("NO SE ENCONTRO EL PRODUCTO QUE SE DESEA ELIMINAR");
            }
            
            
            await context.SaveChangesAsync();
        }
        public async Task Update(string id,ProductoDTO productoUpd)
        {
            var productoExistente = await context.Productos.FindAsync(id);

            if(productoExistente != null)
            {
                if(!string.IsNullOrEmpty(productoUpd.Nombre))
                {
                    productoExistente.Nombre = productoUpd.Nombre;
                }
                if (productoUpd.Precio.HasValue && productoUpd.Precio > 0 && productoUpd.Precio < 1000000000)
                {
                    productoExistente.Precio = productoUpd.Precio.Value;

                    logger.LogError("ERROR AL CAMBIAR PRECIO DEL PRODUCTO");
                }
                if (productoUpd.Stock.HasValue)
                {
                    productoExistente.Stock = productoUpd.Stock.Value;
                }
            }
            else
            {
                logger.LogError("NO SE ENCONTRO EL PRODUCTO QUE SE DESEA ACTUALIZAR");
            }
            
            await context.SaveChangesAsync();
        }

    }

    public interface IProductoApiService
    {

        IEnumerable<Producto> Get(Guid idNegocio);
        Producto GetProductoDetalle(string id);
        Task Save(ProductoDTO nuevoProducto);
        Task Delete(string id);
        Task Update(string id,ProductoDTO productoUpd);
    }
```

`Get :` Devuelve la lista de productos del negocio correspondiente completa, debe recibir por parametro el id del
negocio para solo devolver los productos relacionados a ese negocio.
`GetProductoDetalle :` Solamente devuelve el producto del id que se pasa por parametro.
`Save :` Agrega un nuevo producto.
`Delete :` Elimina un producto.
`Update :` Actualiza un producto existente.

---

### ProductoApiController

Este es el controller de la API de productos, no contiene demasiada logica, ya que la logica esta en el service, pero
es quien se encarga de asignar los endpoints por donde se consumira la API. Ademas la API es consumida desde el
frontend con javascript, asi que tambien se mostrara eso desde aqui.

``` cs

    [ApiController]
    [Route("[controller]")]
    public class ProductoApiController : ControllerBase
    {
        IProductoApiService productoApiService;
        NegocioContext context;
        public readonly ILogger<ProductoApiController> logger;

        public ProductoApiController(IProductoApiService _productoApiService, NegocioContext _context, ILogger<ProductoApiController> _logger)
        {
            productoApiService = _productoApiService;
            context = _context;
            logger = _logger;
        }

        [HttpGet("{idNegocio}")]
        public ActionResult Get(Guid idNegocio)
        {
            return Ok(productoApiService.Get(idNegocio));

        }
        [HttpGet("Detalle/{id}")]
        public IActionResult GetProductoDetalle(string id)
        {
            var producto = productoApiService.GetProductoDetalle(id);

            if (producto == null)
            {
                return NotFound();
            }

            var dto = new ProductoDTO
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Stock = producto.Stock,
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductoDTO nuevoProducto)
        {
            await productoApiService.Save(nuevoProducto);
            return Ok(new { message = "Producto guardado con éxito" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await productoApiService.Delete(id);
            return Ok(new { message = "Producto eliminado" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] ProductoDTO productoUpd)
        {
            var prod = context.Productos.FirstOrDefault(p=> p.IdProducto== id);

            if(prod ==null)
            {  
                return NotFound(); 
            }
            else
            { 
                await productoApiService.Update(id, productoUpd);
                return Ok();
            }
        }
    }
```

`GetProductoDetalle :` Esta funcion esta relacionada con update, sirve para que a la hora de abrir el formulario
para actualizar un producto, ya aparezcan los datos cargados, logrando que el usuario solamente modifique lo que
necesite.

---
## Funciones de Javascript relacionadas con GET:

``` javascript

document.addEventListener("DOMContentLoaded", function () 
{                                                             
    cargarProductos();
                                                           
});


function OcultarColumnaAEmpleados() {
    if (!esAdminNegocio) {
        const columnasAcciones = document.querySelectorAll("td:nth-child(5), th:nth-child(5)");
        columnasAcciones.forEach(celda => {
            celda.style.display = "none";
        });
    }
}

let productos = []; 
function cargarProductos() {
    fetch(`/ProductoApi/${negocioId}`)
        .then(res => res.json())
        .then(data => {

            productos = data;
            filtrarYOrdenar();


        })
        .catch(err => {
            console.error("Error al traer productos", err);
        });
}

function filtrarYOrdenar() {

    const texto = document.getElementById("inputBuscar").value.toLowerCase();
    const criterio = document.getElementById("selectOrdenar").value;

    let filtrados = productos.filter(p => p.nombre.toLowerCase().startsWith(texto));

    filtrados.sort((a, b) => {
        if (criterio === "nombre") return a.nombre.localeCompare(b.nombre);
        if (criterio === "precio") return a.precio - b.precio;
        if (criterio === "stock") return a.stock - b.stock;
        return 0;
    });

    mostrarProductos(filtrados);
}

function mostrarProductos(lista) {

    const tbody = document.querySelector("tbody");
    tbody.innerHTML = "";

    lista.forEach((item, index) => {
        const row = document.createElement("tr");
        const nombreSanitizado = item.nombre.replace(/'/g, "\\'");
        row.innerHTML = `
                    <td>${index + 1}</td>
                    <td>${item.nombre}</td>
                    <td>${item.precio}</td>
                    <td>${item.stock}</td>
                    <td>
                         <button class="btn btn-sm btn-primary me-2" onclick="modificarProducto('${item.idProducto}')">Modificar</button>
                         <button class="btn btn-sm btn-danger" onclick="eliminarProducto('${item.idProducto}', '${nombreSanitizado}')">Eliminar</button>
                    </td>
                `;

        tbody.appendChild(row);
    });

    OcultarColumnaAEmpleados();
}

document.getElementById("inputBuscar").addEventListener("input", filtrarYOrdenar);
document.getElementById("selectOrdenar").addEventListener("change", filtrarYOrdenar);

```

`document.addEventListener("DOMContentLoaded", function ():` DOMcontentLoaded se encarga de ejecutar esta funcion
solo cuando este cargado el HTML, por lo tanto lo que se coloque alli esperara a que se carguen todos los elementos
para luego ejecutarlo.

`function OcultarColumnaAEmpleados() :`Esta funcion se encarga de ocultar las columnas de modificar y eliminar a 
los usuarios que sean empleados. El dato del usuario empleado se envia desde el cshtml de la siguiente forma :

```cshtml
<script>
    const esAdminNegocio = @(User.IsInRole("AdministradorNegocio").ToString().ToLower());
</script>

```

`let productos = [] :`Se declara un vector que luego sera mostrado, no se muestran directamente los productos
traidos con fetch ya que los productos pueden ser filtrados u ordenados, para eso sirve esta variable.

`function cargarProductos() :` Esta funcino se encarga de consumir la API, la data que devuelve la api la guarda
en la variable productos y luego llama a la funcion filtrarYOrdenar. Por otra parte `${negocioId}` que se coloca en
el endpoint es traido desde el html al igual que el rol de usuario.
El id del negocio es fundamental para filtrar los productos del negocio correspondiente, la logica de esto se encuentra
en `ProductoController` y `ProductoApiController`

`function filtrarYOrdenar() :`Esta funcion se encarga de traer los datos del input donde se realiza la busqueda y
el dato del selector donde se indica por que se quiere ordenar la lista y a partir de ello crea una variable donde
alamacenara la lista de filtrados y va guardando en ella lo que se va filtrando, luego llama a la funcion que muestra
los productos y le envia por parametro esa lista filtrada.

`function mostrarProductos(lista) :` Esta funcion recibe por parametro la lista de productos que se va a mostrar
(ya filtrada) y va reemplazando cada elemento HTML con la informacion del producto correspondiente.

`document.getElementById("inputBuscar").addEventListener("input", filtrarYOrdenar)` y 
`document.getElementById("selectOrdenar").addEventListener("change", filtrarYOrdenar); :` Estos dos elementos
se encargar de que cada vez comienza a escribir en la parte de busqueda o cada vez que cambia el tipo de ordanamiento
se llame a las funciones para que actualicen la lista.

---
## Funciones de Javascript relacionadas con POST:

```Javascript
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("formAgregarProducto");

    form.addEventListener("submit", function (e) {
        e.preventDefault();

        const producto = {
            nombre: document.getElementById("nombre").value,
            precio: parseFloat(document.getElementById("precio").value),
            stock: parseInt(document.getElementById("stock").value),
            negocioId: negocioId
        };

        fetch("/ProductoApi", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(producto)
        })
            .then(res => {
                if (!res.ok) throw new Error("Error al agregar producto");
                
            })
            .then(data => {
                console.log("Producto agregado:", data);
                // Acá podés cerrar el modal
                const modal = bootstrap.Modal.getInstance(document.getElementById("modalAgregarProducto"));
                modal.hide();

                document.getElementById("formAgregarProducto").reset();

                cargarProductos();

            })
            .catch(err => {
                console.error("Error al enviar producto", err);
            });
    });
});

```








