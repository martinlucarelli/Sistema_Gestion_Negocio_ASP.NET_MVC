
# Documentación Técnica del Sistema de Gestión de Negocio

Este documento contiene una descripción general de los componentes técnicos del sistema.

## Indice

1. [LoginController](#logincontroller)
   - [Login(UsuarioViewModel user)](#loginusuarioviewmodel-user)
   - [RecuperarClave(string correo)](#recuperarclavestring-correo)
   - [RestablecerClave(string token)](#restablecerclavestring-token)
   - [CerrarSesion()](#cerrarsesion)
     
2. [NegocioController](#negociocontroller)
   - [Negocios()](#negocios())
   - [AgregarNegocio(string correo)](#agregarnegocio(string-correo))
   - [RegistrarAdministradorNegocio(string token)](#registraradministradornegocio(string-token))
   - [RegistrarAdministradorNegocio(UsuarioViewModel user,string token)](#registraradministradoraegocio(usuarioviewModel-user,string-token))
   - [RegistrarNegocio(string token)](#registrarnegocio(string-token))
   - [RegistrarNegocio(NegocioViewModel n ,string token)](#registrarnegocio(negocioviewmodel-n,string-token))
   - [MiNegocio()](#minegocio())
   - [EditarNegocio(string idNegocio)](#editarnegocio(string-idnegocio))
   - [EditarNegocio(string idNegocio, NegocioViewModel negocioEditar)](#editarnegocio(string-idnegocio,-negocioviewmodel-negocioeditar))

3. [EmpleadoController](#empleadocontroller)
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

7.[VentaController](#ventacontroller)
    -[RegistrarVenta()](#registrarventa())
    -[Ventas()](#ventas())

8.[VentaApiService](#ventaapiservice)
    - [GetVenta()](#getventa())
    - [GetFactura()](#getfactura())
    - [RegistrarVenta()](#registrarventa())

9. [VentaApiController](#ventaapicontroller)    

     




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
### RestablecerClave(string token)


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

### AgregarNegocio(string correo)

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

### RegistrarAdministradorNegocio(string token)

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

### RegistrarAdministradorNegocio(UsuarioViewModel user,string token)

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

### RegistrarNegocio(string token)

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

### RegistrarNegocio(NegocioViewModel n ,string token)

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
### MiNegocio()

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
### EditarNegocio(string idNegocio)

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

### EditarNegocio(string idNegocio, NegocioViewModel negocioEditar)

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

## EmpleadoController

### AgregarEmpleado(UsuarioViewModel user)

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

## ProductoApiController

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
### Funciones de Javascript relacionadas con GET:

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
### Funciones de Javascript relacionadas con POST:

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

`document.addEventListener("DOMContentLoaded", function ()` : Espera que todo el DOM este cargado antes de ejecutar la funcion.

`const form = document.getElementById("formAgregarProducto");` : Obtiene el formulario con id "formAgregarProducto" y lo guarda en
una variable.

`form.addEventListener("submit", function (e) `: Escucha el envio del formulario, mientras que `e.preventDefault()` sirve para
evitar que el navegador envie el formulario recargando la pagina.

`const producto = {` : Crea un producto y lo llena con `getElementById` de cada input del formulario, en caso de necesitar parsear
datos lo hace.

```javascript
            fetch("/ProductoApi", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(producto)
        })
```

`method: "POST"`: Indica que se esta creando un nuevo recurso.
`headers`: Indica que el contenido es JSON.
`body`: Se convierte el objeto producto en una cadena de JSON para enviarla por el body de la solicitud.


`.then(res => {`: Espera la repuesta del servidor, si no es exitosa lanza un error.

`.then(data => {`: Avisa que el producto fue agregado y cierra el modal con `const modal = bootstrap.Modal.getInstance(document.getElementById("modalAgregarProducto"));`
y luego `modal.hide();`. Tambien se encarga de volver a cargar los productos con `cargarProductos()` lo que permite que se actualice
sin recargar la pagina.

`.catch(err => {`: Avisa si hubo un error al agregar el producto.
          
---

### Funciones de Javascript relacionadas con DELETE:

```javascript 
function eliminarProducto(id, nombre) {
    console.log("ID del producto a eliminar:", id);
    idProductoAEliminar = id;
    const mensaje = `¿Estás seguro de eliminar el producto <strong>${nombre}</strong>?`;
    document.getElementById("mensajeConfirmacion").innerHTML = mensaje;

    const modal = new bootstrap.Modal(document.getElementById("modalConfirmarEliminacion"));
    modal.show();
}

```

Esta funcion se ejecuta al presionar en eliminar en la lista de prodcutos. Lo que hace es guardar el ID del producto, mostrar un 
mensaje para confirmar si se va a eliminar el producto y abre un modal para que el usuario confirme la eliminacion.
Esta funcion solo abre el modal, la logica de eliminacion esta cuando se presiona aceptar.

`const mensaje = `¿Estás seguro de eliminar...` : Crea un HTML para mostrar al usuario.

`document.getElementById("mensajeConfirmacion").innerHTML = mensaje`: Busca el elemento "mensajeConfirmacion" dentro del modal,
es un <p> y lo remplaza por el mensasje del renglon anterior. Hacerlo de esta forma permite mostrar el nombre del producto que se
esta por eliminar.

`const modal = new bootstrap.Modal(document.getElementById("modalConfirmarEliminacion"))`: Crea una insatancia de modal de bootstrap
a partir del modal de confirmacion.

`modal.show();`: Muestra el modal.

Esta funcion es llamada desde el boton que se encuentra en cada producto, en la funcion `mostrarProductos`:
`<button class="btn btn-sm btn-primary me-2" onclick="modificarProducto('${item.idProducto}')">Modificar</button>`.

```javascript
document.getElementById("btnConfirmarEliminar").addEventListener("click", function () {
    if (idProductoAEliminar !== null) {
        fetch(`/ProductoApi/${idProductoAEliminar}`, {
            method: "DELETE"
        })
            .then(res => {
                if (!res.ok) throw new Error("Error al eliminar el producto");
                return res.text(); // o .json() si devolvés algo
            })
            .then(() => {
                console.log("Producto eliminado");

                const modal = bootstrap.Modal.getInstance(document.getElementById("modalConfirmarEliminacion"));
                modal.hide();

                cargarProductos(); // Recargar la tabla
            })
            .catch(err => {
                console.error("Error al eliminar", err);
            });
    }
});
```

Esta funcion se ejecuta cuando se confirma la eliminacion del producto desde el modal.

`document.getElementById("btnConfirmarEliminar").addEventListener("click", function () `: Busca el boton con Id "btnConfirmarEliminar"
que esta en el modal le asigna un click listener.


`fetch(/ProductoApi/${idProductoAEliminar}`: Luego hace un fetch, el id del producto a eliminar que se envia en el endpoint, se
declaro en la funcion anterior (`eliminarProducto`).

`const modal = bootstrap.Modal.getInstance(document.getElementById("modalConfirmarEliminacion"))`: Se obtiene el modal, como en la
funcion anterior ya creamos el modal con new.bootstrap.Modal..., aca no hace falta crearlo otra vez, para eso utilizamos getInstance
y nos trae el modal que habiamos creado.

`modal.hide();`: Cerramos el modal, recordemos que el modal estaba abierto y a la hora de confirmar la eliminacion lo cerramos.

`cargarProductos();`: Recargamos los productos. Se recargara automaticamente la lista, sin el producto que eliminamos.

---

### Funciones de Javascript relacionadas con PUT:

```javascript 
function modificarProducto(id) {
    fetch(`/ProductoApi/Detalle/${id}`)
        .then(res => res.json())
        .then(producto => {
            document.getElementById("idModificar").value = producto.idProducto;
            document.getElementById("nombreModificar").value = producto.nombre;
            document.getElementById("precioModificar").value = producto.precio;
            document.getElementById("stockModificar").value = producto.stock;

            const modal = new bootstrap.Modal(document.getElementById("modalModificarProducto"));
            modal.show();
        })
        .catch(err => {
            console.error("Error al cargar producto para modificar", err);
        });
}
```
Esta funcion se ejecuta cuando se presiona en  modificar, se encarga de buscar los datos del producto y cargarlos en el formulario
para modificarlos. Esta funcion permite que si se quiere solamente modificar el precio, no se tengan que estar volviendo a cargar
todos los valores ya que el formulario ya viene completo con los datos del producto.

` fetch(/ProductoApi/Detalle/${id})`: Primero hace una peticion GET para obtener los datos del producto.

`.then(res => res.json())`: Convierte la repsuesta en JSON.

`.then(producto => {`: Si se reciben los datos bien, los guarda en producto y luego carga los valores de cada campo de producto 
en los input del formulario.

`const modal = new bootstrap.Modal(document.getElementById("modalModificarProducto"))`: Crea una instancia de modal que muestra el
formulario.

```javascript

document.getElementById("formModificarProducto").addEventListener("submit", function (e) {
    e.preventDefault();

    const id = document.getElementById("idModificar").value;

    const productoActualizado = {
        nombre: document.getElementById("nombreModificar").value,
        precio: parseFloat(document.getElementById("precioModificar").value),
        stock: parseInt(document.getElementById("stockModificar").value)
    };

    console.log("Enviando ID:", id);
    console.log("Producto actualizado:", productoActualizado);


    fetch(`/ProductoApi/${id}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(productoActualizado)
    })
        .then(res => {
            if (!res.ok) throw new Error("Error al actualizar el producto");
            return res.text();
        })
        .then(() => {
            console.log("Producto actualizado");
            const modal = bootstrap.Modal.getInstance(document.getElementById("modalModificarProducto"));
            modal.hide();
            cargarProductos();
        })
        .catch(err => {
            console.error("Error al actualizar", err);
        });
});
```
Esta funcion se encarga de enviar los cambios del producto cuando se presiona guardar cambios.

`document.getElementById("formModificarProducto").addEventListener("submit", function (e) {`: Escucha cuando se envia el formulario
con id "formModificarProducto".

`e.preventDefault();`: Evita que se recargue la pagina.

`const id = document.getElementById("idModificar").value`: Toma el id que se encuentra en un input oculto ya que lo utilizara para
el endpoint de la api para indicar que elemento va a modificar.

`const productoActualizado = {`: Arma un objeto con los datos nuevos del producto.

`fetch(/ProductoApi/${id}, {`: Hace una solicitud PUT y envia los datos en formato JSON.

`const modal = bootstrap.Modal.getInstance(document.getElementById("modalModificarProducto"))`: Obtiene el modal que ya se habia
creado

`modal.hide();`: Cierra el modal.

`cargarProductos()`: Actualiza la lista.

---

## VentaController

Este controlador se encarga de controlar 2 vistas, la vista que se encarga de registrar una nueva venta y la vista que se encarga
de mostrar las ventas de cada negocio.

### RegistrarVenta()

``` csharp
public IActionResult RegistrarVenta()
        {
            string negocioIdUsuario = UsuarioHelper.ObtenerNegocioIdDelUsuario(HttpContext);
            string usuarioId = UsuarioHelper.ObtenerUsuarioId(HttpContext);

            ViewBag.negocioId = negocioIdUsuario;
            ViewBag.usuarioId = usuarioId;
            ViewBag.formasDePago = context.FormasPago.ToList();
            return View();
        }
```

Esta funcion se encarga de devolver la vista que permite registrar una nueva venta, para ello envia por viewbags datos
claves para realizar esta funcionalidad como el id del negocio,el id del usuario y las formas de pago.

```html

@{
    ViewData["Title"] = "RegistrarVenta";
    Layout = "~/Views/Shared/_LayoutNavbar.cshtml";
}
<div class="card p-3 mb-3">
    <div class="row g-2 align-items-end">
        <div class="col-md-6">
            <label for="busquedaProducto" class="form-label">Buscar producto</label>
            <input type="text" id="busquedaProducto" class="form-control" placeholder="Ej: Coca, Pan, etc.">
        </div>
        <div class="col-md-3">
            <label for="cantidadInput" class="form-label">Cantidad</label>
            <input type="number" id="cantidadInput" class="form-control" min="1" value="1" />
        </div>
        <div class="col-md-3">
            <button id="btnAgregarDetalle" class="btn btn-success w-100" disabled>Agregar</button>
        </div>
    </div>

    <!-- Productos filtrados -->
    <div id="resultadosBusqueda" class="mt-2"></div>
</div>

<!-- Lista de productos agregados -->
<table class="table table-bordered" id="tablaDetalleVenta">
    <thead>
        <tr>
            <th>Producto</th>
            <th>Precio unitario</th>
            <th>Cantidad</th>
            <th>Subtotal</th>
            <th>Acciones</th>
        </tr>
    </thead>
    <tbody>
        <!-- Se llena dinámicamente -->
    </tbody>
</table>

<div class="mt-3">
    <label for="formaPagoSelect" class="form-label">Forma de pago</label>
    <select id="formaPagoSelect" class="form-select" required>
        <option value="">Seleccione una forma de pago</option>
        @foreach (var forma in ViewBag.formasDePago)
        {
            <option value="@forma.IdFormaPago">@forma.TipoFormaPago</option>
        }
    </select>
</div>

<!-- Total + Confirmar -->
<div class="d-flex justify-content-between align-items-center">
    <h5>Total: $<span id="totalVenta">0.00</span></h5>
    <button id="btnConfirmarVenta" class="btn btn-primary">Confirmar venta</button>
</div>

<script src="/js/venta.js" ></script>

<script>
    const negocioId = "@ViewBag.negocioId"
</script>
<script>
    const usuarioId = "@ViewBag.usuarioId"
</script>
```

Este es el HTML de la vista, es importante aclarar que los viewbag enviados desde el controller, luego se envian al archivo js
desde el el HTML

---

### Ventas()

``` 
        public IActionResult Ventas()
        {
            string negocioIdUsuario = UsuarioHelper.ObtenerNegocioIdDelUsuario(HttpContext);

            ViewBag.negocioId = negocioIdUsuario;

            return View();
        }
```

Esta funcion devuelve la vista de las ventas registradas en un negocio, solamente envia a la vista el id del negocio del usuario
logueado por un viewbag.

---

## VentaApiService

### GetVenta()

```csharp
        public ResumenVentasDTO Get(Guid idNegocio, DateTime? desde, DateTime? hasta)
        {
            var query = context.Ventas
                .Where(v => v.NegocioId == idNegocio)
                .Include(v => v.FormaPago)
                .Include(v => v.Usuario)
                .AsQueryable();

            if (desde.HasValue && hasta.HasValue)
            {
                var desdeFecha = desde.Value.Date;
                var hastaFecha = hasta.Value.Date.AddDays(1); // Incluye todo el día 'hasta'
                query = query.Where(v => v.Fecha >= desdeFecha && v.Fecha < hastaFecha);
            }
            else
            {
                // Ventas solo del día actual
                var hoy = DateTime.Today; // 2025-05-19 00:00:00
                var mañana = hoy.AddDays(1); // 2025-05-20 00:00:00

                query = query.Where(v => v.Fecha >= hoy && v.Fecha < mañana);

            }

            var ventas = query.ToList();

            var resumenPorFormaPago = ventas
                .GroupBy(v => v.FormaPago?.TipoFormaPago ?? "Sin forma de pago")
                .Select(g => new ResumenFormaPagoDTO
                {
                    FormaPago = g.Key,
                    Total = g.Sum(v => v.Total)
                }).ToList();

            var totalGeneral = ventas.Sum(v => v.Total);

            return new ResumenVentasDTO
            {
                Ventas = ventas.Select(v => new MostrarVentaDTO
                {
                    IdVenta = v.IdVenta,
                    Fecha = v.Fecha,
                    Total = v.Total,
                    NombreFormaPago = v.FormaPago != null ? v.FormaPago.TipoFormaPago : "Sin forma de pago",
                    NombreUsuario = v.Usuario != null ? v.Usuario.Nombre : "Usuario desconocido"
                }).ToList(),
                ResumenPorFormaPago = resumenPorFormaPago,
                TotalGeneral = totalGeneral
            };
        }
```

Esta funcion se encarga de devolver las ventas de un negocio. Antes de explicar cada parte de la funcion se debe tener en cuenta
que es lo que realiza. Esta funcion se utiliza para ver las ventas que tiene un negocio, estas ventas se muestran en una
tabla que tiene la particularidad de filtrarlas por fecha. Por defecto, al cargar la pagina, van a aparecer las ventas realizadas
ese mismo dia, pero si filtramos por fecha se vuelve a llamar a la funcion y se filtran las ventas desde y hasta la fecha
indicada. Es por eso que esa funcion recibe por parametro dos `DateTime` que pueden ser null `?`.

##### Construccion de la consulta base
``` csharp
var query = context.Ventas
    .Where(v => v.NegocioId == idNegocio)
    .Include(v => v.FormaPago)
    .Include(v => v.Usuario)
    .AsQueryable();

```

`.Where(v => v.NegocioId == idNegocio)`: Filtra solo las ventas que pertenecen al negocio con el id recibido por parametro.
`.Include(v => v.FormaPago)` y `.Include(v => v.Usuario)`: Eager loanding (carga anticipada) de datos relacionados. Significa
que, junto a cada venta, tambien trae la informacion de la forma de pago y del usuario asociado.
`.AsQueryable();`: Convierte la consulta en IQueryable, esto permite seguir agregando filtros mas adelante siempre guardando
lo que ya esta en la consulta, es decir, nos permite ir "agrandando" la consulta.

#### Filtrado por fechas

``` csharp
if (desde.HasValue && hasta.HasValue)
{
    var desdeFecha = desde.Value.Date;
    var hastaFecha = hasta.Value.Date.AddDays(1); // Incluye todo el día 'hasta'
    query = query.Where(v => v.Fecha >= desdeFecha && v.Fecha < hastaFecha);
}
```

Primero verifica si los parametros desde y hasta tienen valor. Si ambos parametros contienen valor entonces:

Para la fecha hasta, suma un día más (AddDays(1)) para incluir todas las ventas del día completo "hasta".
Ejemplo: si hasta es 2025-05-19, la comparación será v.Fecha < 2025-05-20, con lo que incluye todo el 19/05.

Luego aplica un filtro a la consulta que ya existe para que solo se incluyan ventas con fecha desdeFecha ≤ venta.Fecha < hastaFecha.

Si hasta y desde no tienen valor:

``` csharp
else
{
    var hoy = DateTime.Today;        // Ej: 2025-05-19 00:00:00
    var mañana = hoy.AddDays(1);     // Ej: 2025-05-20 00:00:00

    query = query.Where(v => v.Fecha >= hoy && v.Fecha < mañana);
}
```

Si no se pasó rango de fechas, la función toma como filtro por defecto las ventas del día actual.

La consulta trae solo las ventas que ocurren desde las 00:00 del día de hoy hasta justo antes de la medianoche
del día siguiente (no incluye el siguiente día).

#### Ejecucion de consulta

`var ventas = query.ToList();`: Ejecuta la consulta y trae todas las ventas que cumplen con los filtros que agregamos
anteriormente guardandolos en una lista.

#### Resumen agrupado por forma de pago

```csharp 
var resumenPorFormaPago = ventas
    .GroupBy(v => v.FormaPago?.TipoFormaPago ?? "Sin forma de pago")
    .Select(g => new ResumenFormaPagoDTO
    {
        FormaPago = g.Key,
        Total = g.Sum(v => v.Total)
    }).ToList();
```

`.GroupBy(v => v.FormaPago?.TipoFormaPago ?? "Sin forma de pago")`: Agrupa las ventas que se trajeron a memoria anteriormente por la forma de pago.
Usa el operador `?.` para manejar casos donde la forma de pago sea null, si llega a ocurrir eso asigna el texto "sun forma de pago".

Luego por cada gurpo de pago crea un objeto `ResumenFormaPagoDTO` con la forma de pago y la suma de todas las ventas dentro de ese grupo.
Finalmente convierte todo a lista.

ResumenFormaPagoDTO:
```csharp 
    public class ResumenFormaPagoDTO
    {
        public string FormaPago { get; set; }
        public double Total {  get; set; }

    }
``` 

#### Calculo total general

`var totalGeneral = ventas.Sum(v => v.Total);`: Suma el total de todas la ventas filtradas sin importar la forma de pago. Este total es
el  total global de ventas para el negocio y rango de fechas indicadas.

#### Retorno de datos

```csharp 
return new ResumenVentasDTO
{
    Ventas = ventas.Select(v => new MostrarVentaDTO
    {
        IdVenta = v.IdVenta,
        Fecha = v.Fecha,
        Total = v.Total,
        NombreFormaPago = v.FormaPago != null ? v.FormaPago.TipoFormaPago : "Sin forma de pago",
        NombreUsuario = v.Usuario != null ? v.Usuario.Nombre : "Usuario desconocido"
    }).ToList(),

    ResumenPorFormaPago = resumenPorFormaPago,
    TotalGeneral = totalGeneral
};

```
Esta parte lo que hace es transformar todo lo que fuimos obteniendo a lo largo de la funcion en el objeto final que se 
debe devolver. Para esto es importante tener en claro los DTO que se mencionan.

```csharp
    public class MostrarVentaDTO
    {
        public string? IdVenta { get; set; }
        public double Total { get; set; }
        public DateTime? Fecha { get; set; }
        public string? NombreUsuario { get; set; }
        public string? NombreFormaPago { get; set; }
    }
```

```csharp
    public class ResumenVentasDTO
    {
        public List<MostrarVentaDTO> Ventas { get; set; }
        public List<ResumenFormaPagoDTO> ResumenPorFormaPago { get; set; }
        public double TotalGeneral { get; set; }
    }
```
Como se puede ver `ResumenVenta` tiene una lista de `MostrarVenta` y `ResumenFormaPago`.

Por lo tanto esa ultima parte de la funcion que lo que hace es:

`Ventas = ventas.Select(v => new MostrarVentaDTO`: Se fija en la base de datos la tabla venta y por cada registro en venta
crea un MostarVentaDTO. Los datos de este nuevo objeto instanciado se igualan con lo que ya obtuvimos cuando ejecutamos la consulta
mas arriba. Luego se pasa a lista debido a que en la clase esta definida la lista.

`ResumenPorFormaPago = resumenPorFormaPago,`: El resumen ya lo habiamos obtenido anteriormente y ya lo habiamos pasado a lsita
por lo tanto solo se igualan.
`TotalGeneral = totalGeneral`: El total general ya lo obtuvimos antes asi que aca se lo asignamos al objeto que se va a devolver.

---

###  GetFactura()

```csharp
        public FacturaDTO GetFactura(string idVenta)
        {
           
            var venta = context.Ventas
                .Include(v => v.detalleVentas)
                    .ThenInclude(dv => dv.producto)
                .Include(v => v.FormaPago)
                .Include(v => v.Usuario)
                .Include(v => v.Negocio)
                .FirstOrDefault(v => v.IdVenta == idVenta);

            if (venta == null) return null;

            return new FacturaDTO
            {
                IdVenta = venta.IdVenta,
                fechaVenta = venta.Fecha,
                FormaPago = venta.FormaPago?.TipoFormaPago,
                Total = venta.Total,
                Vendedor = venta.Usuario?.Nombre,
                NombreNegocio = venta.Negocio?.Nombre,
                DireccionNegocio = venta.Negocio?.Direccion,
                Detalle = venta.detalleVentas?.Select(d => new DetalleVentaDTO
                {
                    ProductoId = d.ProductoId,
                    CantidadProductos = d.CantidadProductos,
                    Subtotal = d.Subtotal,
                    PrecioProducto = d.producto?.Precio ?? 0,
                    NombreProducto= d.producto.Nombre
                }).ToList()
            };
        }
```

Esta funcion se encarga de devolver la factura de una venta especifica, para ello es necesario recibir por parametro el id de la venta que
se quiere la factura.

#### Busca la venta
```csharp 
var venta = context.Ventas
    .Include(v => v.detalleVentas)
        .ThenInclude(dv => dv.producto)
    .Include(v => v.FormaPago)
    .Include(v => v.Usuario)
    .Include(v => v.Negocio)
    .FirstOrDefault(v => v.IdVenta == idVenta);
```

Esta parte de la funcion primero accede a la tabla ventas. Luego cada `.Include` se encarga de traer las relaciones (joins), en lugar de
traer solamente el Id de ,por ejemplo, el usuario, trae todo el usuario completo.
`.ThenInclude` se encarga de que incluir de cada detalle de vetna el producto. Como producto no es un join de venta, pero si lo es de detalle,
de cada detalle tambien va a obtener el dato completo del producto.

#### Armar y devolver DTO.

```csharp
    public class FacturaDTO
    {
        public string? NombreNegocio {  get; set; }
        public string? DireccionNegocio { get; set; }
        public string? IdVenta {  get; set; }
        public DateTime fechaVenta { get; set; }
        public  string? FormaPago {  get; set; }
        public double Total {  get; set; }
        public string? Vendedor {  get; set; }

        public List<DetalleVentaDTO>? Detalle { get; set; } 
    }
```

Asi esta estructurado el DTO.

```csharp
            return new FacturaDTO
            {
                IdVenta = venta.IdVenta,
                fechaVenta = venta.Fecha,
                FormaPago = venta.FormaPago?.TipoFormaPago,
                Total = venta.Total,
                Vendedor = venta.Usuario?.Nombre,
                NombreNegocio = venta.Negocio?.Nombre,
                DireccionNegocio = venta.Negocio?.Direccion,
                Detalle = venta.detalleVentas?.Select(d => new DetalleVentaDTO
                {
                    ProductoId = d.ProductoId,
                    CantidadProductos = d.CantidadProductos,
                    Subtotal = d.Subtotal,
                    PrecioProducto = d.producto?.Precio ?? 0,
                    NombreProducto= d.producto.Nombre
                }).ToList()
            };  
```

Primero se instancia el DTO que se va a devolver y se completan los datos con la informacion que se obtuvo de la primera consulta con los
include.

`Detalle = venta.detalleVentas?.Select(d => new DetalleVentaDTO`: Esta parte lo que hace es que por cada detalle venta que haya en venta
(el objeto resultado de la consulta anterior), cree un nuevo objeto `DetalleVentaDTO` y lo va a ir llenando con los datos del detalle venta
original. Esto es porque el detalle venta original (el que traemos de la base de datos en la primer consulta) contiene datos que no son 
necesarios para nuestro DTO, es por eso que por cada detalle venta se crea un DTO con los dato que si son necesarios para mostrar y se completa
con los datos del detalle venta original que viene de la base de datos.

---

### RegistrarVenta()

Esta funcion se encarga de guardar una nueva venta en la base de datos, para esto recibe por parametro el DTO de la venta y guarda la venta,
a partir de la venta guardada, se guardan los detalles de la venta.

```csharp
public async Task Registrar(VentaDTO nuevaVenta)
        {

            var venta = new Venta
            {
                UsuarioId = nuevaVenta.UsuarioId,
                NegocioId = nuevaVenta.NegocioId,
                FormaPagoId = nuevaVenta.FormaPagoId,
                Total = nuevaVenta.Total,
            };

            context.Ventas.Add(venta);

            foreach (var detalleDeVenta in nuevaVenta.Detalle)
            {
                var detalleVenta = new DetalleVenta
                {
                    CantidadProductos = detalleDeVenta.CantidadProductos,
                    Subtotal = detalleDeVenta.Subtotal,
                    VentaId = venta.IdVenta,
                    ProductoId = detalleDeVenta.ProductoId
                };

                context.DetalleVentas.Add(detalleVenta);
            }

            
            await context.SaveChangesAsync();
        }
```

`var venta = new Venta`: Primero crea una nueva venta (modelo original) y lo completa con lo que recibe del DTO. Luego lo guarda en la base
de datos.

`foreach (var detalleDeVenta in nuevaVenta.Detalle)`: El DTO que recibe por parametro contiene una lista con los detalles de esa venta, entonces
lo que hace aca es iterar la lista y por cada detalle, crear un objeto detalleVenta (modelo original) y guardarlo en la base de datos.

---

### VentaApiController

```csharp
        [HttpGet ("{idNegocio}")]
        public ActionResult Get(Guid idNegocio, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            var resumenVentas = ventaApiService.Get(idNegocio, desde, hasta);
            return Ok(resumenVentas);
        }
```

Esta funcion se encarga de devolver las ventas. El endpoint sera el nombre del controler y el id del negocio que contiene las ventas.
Se pasa por parametro la fecha `desde` y `hasta` pero al tener `?` pueden ser null.

```csharp
        [HttpGet("factura/{idVenta}")]
        public ActionResult GetFactura(string idVenta)
        {
            var factura = ventaApiService.GetFactura(idVenta);

            if(factura == null)
            {
                return NotFound();
            }
            else
            {
                var pdfBytes = CrearPDFHelper.crearPDFFactura(factura);

                return File(pdfBytes, "application/pdf");
                
            }
        }
```

Esta funcion se encarga de devolver la factura en formato PDF. Cuando llamaos a la API, esta nos devuelve un DTO que luego se lo enviamos por
parametro a `crearPDFFactura` que es la funcion del Helper que se encarga de crear el pdf. Por ultimo devuelve un `File`.

---

### Funciones de javascript relacionadas con GET (vista de registrar venta)

```js
document.addEventListener("DOMContentLoaded", function () {
    fetch("/ProductoApi/" + negocioId)
        .then(res => res.json())
        .then(data => {
            productosDisponibles = data;
        });

    document.getElementById("busquedaProducto").addEventListener("input", function () {
        const busqueda = this.value.toLowerCase();
        const contenedorResultados = document.getElementById("resultadosBusqueda");
        contenedorResultados.innerHTML = ""; // Limpiar resultados

        if (busqueda.length === 0) return;

        const productosFiltrados = productosDisponibles.filter(p =>
            p.nombre.toLowerCase().includes(busqueda)
        );

        productosFiltrados.forEach(p => {
            const div = document.createElement("div");
            div.className = "border rounded p-2 mb-1 producto-item";
            div.style.cursor = "pointer";
            div.textContent = `${p.nombre} - $${p.precio}`;
            div.addEventListener("click", function () {
                productoSeleccionado = p;
                document.getElementById("btnAgregarDetalle").disabled = false;
                document.getElementById("busquedaProducto").value = `${p.nombre}`;
                contenedorResultados.innerHTML = ""; // Limpiar resultados al seleccionar
            });
            contenedorResultados.appendChild(div);
        });
    });

    document.getElementById("btnAgregarDetalle").addEventListener("click", function () {
        const cantidad = parseInt(document.getElementById("cantidadInput").value);

        if (!productoSeleccionado || cantidad < 1) return;

        const subtotal = productoSeleccionado.precio * cantidad;

        detalleVenta.push({
            productoId: productoSeleccionado.idProducto,
            nombre: productoSeleccionado.nombre,
            precio: productoSeleccionado.precio,
            cantidad: cantidad,
            subtotal: subtotal
        });

        renderDetalle();

        // Resetear
        document.getElementById("busquedaProducto").value = "";
        document.getElementById("cantidadInput").value = 1;
        document.getElementById("btnAgregarDetalle").disabled = true;
        productoSeleccionado = null;
    });

    document.getElementById("btnConfirmarVenta").addEventListener("click", function () {
        confirmarVenta(); // Esto deberías adaptarlo para incluir también la forma de pago si querés
    });
});

```


#### Fetch

`fetch("/ProductoApi/" + negocioId) ...` 

- Se hace una llamada HTTP GET a la API pidiendo los productos correspondientes a ese negocio.
- `.then(res => res.json())` Convierte la respuesta en JSON que js lo puede entender.
- `.then(data => { productosDisponibles = data; })` Guarda los datos en la variable definida al principio `productosDisponibles` para utilizar
despues.

#### Listener en el input de busqueda

```js
document.getElementById("busquedaProducto").addEventListener("input", function () {
```

- Escucha cada vez que el usuario escribe o borra algo en el campo de busqueda (input).
- `this.value.toLowerCase()` Toma el texto que ingreso el usuario y lo pasa a minuscula para que la busqueda no sea sensible
a mayusculas.

#### Limpiar resultados y filtrar productos

```js
const contenedorResultados = document.getElementById("resultadosBusqueda");
contenedorResultados.innerHTML = ""; // limpia resultados anteriores
```

- Cada vez que cambia la búsqueda, limpia la lista de resultados anteriores para mostrar solo los nuevos.
- Si el texto está vacío `(busqueda.length === 0)` no hace nada más.

Luego:

```js
const productosFiltrados = productosDisponibles.filter(p =>
    p.nombre.toLowerCase().includes(busqueda)
);

```

- Filtra el arreglo `productosDisponibles` para obtener solo los productos cuyo nombre contenga la palabra buscada.

#### Mostrar resultados filtrados.

Para cada producto filtrado:

```js
productosFiltrados.forEach(p => {
    const div = document.createElement("div");
    div.className = "border rounded p-2 mb-1 producto-item";
    div.style.cursor = "pointer";
    div.textContent = `${p.nombre} - $${p.precio}`;
```

- Crea un nuevo `<div>` que muestra el nombre y precio del producto.
- Le asigna estilos para que parezca un item clickeable.

Cuando se hace click en el div:

```js
div.addEventListener("click", function () {
    productoSeleccionado = p;
    document.getElementById("btnAgregarDetalle").disabled = false;
    document.getElementById("busquedaProducto").value = `${p.nombre}`;
    contenedorResultados.innerHTML = ""; // limpia la lista de resultados
});
```

- Guarda el producto seleccionado en una variable global `productoSeleccionado`.
- Habilita el botón para agregar el producto al detalle de la venta.
- Pone el nombre del producto en el input de búsqueda.
- Limpia la lista de resultados para que no siga mostrando los productos.

Finalmente agrega ese div a la lista visible:

`contenedorResultados.appendChild(div);`

#### Listener para agregar el producto al detalle de venta 

```js
document.getElementById("btnAgregarDetalle").addEventListener("click", function () {
    const cantidad = parseInt(document.getElementById("cantidadInput").value);

    if (!productoSeleccionado || cantidad < 1) return;

    const subtotal = productoSeleccionado.precio * cantidad;

    detalleVenta.push({
        productoId: productoSeleccionado.idProducto,
        nombre: productoSeleccionado.nombre,
        precio: productoSeleccionado.precio,
        cantidad: cantidad,
        subtotal: subtotal
    });

    renderDetalle();
```

- Cuando haces click en el botón “Agregar detalle”, agarra la cantidad ingresada.
- Valida que haya un producto seleccionado y que la cantidad sea válida (>0).
- Calcula el subtotal multiplicando precio x cantidad.
- Agrega un objeto con los datos del producto y cantidad al arreglo detalleVenta, que representa los
productos agregados a la venta.
- Llama a renderDetalle() que seguramente actualiza la interfaz para mostrar la lista actualizada de
productos en la venta.

Después, resetea los inputs y el botón:

```js
    document.getElementById("busquedaProducto").value = "";
    document.getElementById("cantidadInput").value = 1;
    document.getElementById("btnAgregarDetalle").disabled = true;
    productoSeleccionado = null;
});
```

#### Listener para confirmar la venta

```js
document.getElementById("btnConfirmarVenta").addEventListener("click", function () {
    confirmarVenta();
});

```
 
- Cuando clickeas “Confirmar Venta” se llama a la función `confirmarVenta()` que debería encargarse de
enviar los datos de la venta a la API para que se guarden en la base de datos.

----

```js
function renderDetalle() {
    const tbody = document.querySelector("#tablaDetalleVenta tbody");
    tbody.innerHTML = "";
    let total = 0;

    detalleVenta.forEach((item, index) => {
        total += item.subtotal;
        tbody.innerHTML += `
            <tr>
                <td>${item.nombre}</td>
                <td>$${item.precio}</td>
                <td>${item.cantidad}</td>
                <td>$${item.subtotal.toFixed(2)}</td>
                <td><button class="btn btn-sm btn-danger" onclick="eliminarDetalle(${index})">Eliminar</button></td>
            </tr>
        `;
    });

    document.getElementById("totalVenta").textContent = total.toFixed(2);
}
```

Esta funcion se encarga de actualizar la tabla de detalles de la venta cada vez que se agrega o elimina un producto.

1. `const tbody = document.querySelector("#tablaDetalleVenta tbody");`
    
    - Agarra el `<tbody>` de la tabla con id tablaDetalleVenta. Es donde se van a mostrar las filas de productos agregados.
    


2. `tbody.innerHTML = "";`

    - impia el contenido de ese `<tbody>` antes de volver a renderizarlo. Es decir, borra todas las filas anteriores para evitar duplicados.


3. `let total = 0;`

    - Inicializa la variable total, que después se va a mostrar como el total general de la venta.


4. Recorre el array detalleVenta

    - Va producto por producto (cada item) y también guarda el index, que lo usa para poder eliminar después.

5.  Va sumando los subtotales:
    
    `total += item.subtotal;`

    - Acumula el precio total de la venta.


6. Genera una fila por cada producto:

```js
tbody.innerHTML += `
    <tr>
        <td>${item.nombre}</td>
        <td>$${item.precio}</td>
        <td>${item.cantidad}</td>
        <td>$${item.subtotal.toFixed(2)}</td>
        <td><button class="btn btn-sm btn-danger" onclick="eliminarDetalle(${index})">Eliminar</button></td>
    </tr>
`;
```

- Usa template literals para generar cada fila `<tr>`.
- Muestra:
    - Nombre del producto
    - Precio unitario
    - Cantidad
    - Subtotal (precio * cantidad), con dos decimales
    - Un botón rojo que dice Eliminar, que llama a `eliminarDetalle(index)` cuando se clickea.


7.  Muestra el total en el DOM:

`document.getElementById("totalVenta").textContent = total.toFixed(2);`

- Actualiza el valor total de la venta en el elemento con id totalVenta.
    
---

```js
function eliminarDetalle(index) {
    detalleVenta.splice(index, 1);
    renderDetalle();
}
```

`detalleVenta.splice(index, 1);`: Elimina el producto del array en la posicion Index.

`renderDetalle();`: Vuelve a renderizar la tabla con los productos actualizados.

---

### Funciones de javascript relacionadas con POST (vista de registrar venta)

```js
function confirmarVenta() {
    const formaPagoId = document.getElementById("formaPagoSelect").value;
    if (!formaPagoId) {
        alert("Seleccione una forma de pago.");
        return;
    }

    const venta = {
        usuarioId: usuarioId,
        negocioId: negocioId,
        formaPagoId: formaPagoId,
        total: detalleVenta.reduce((sum, item) => sum + item.subtotal, 0),
        detalle: detalleVenta.map(item => ({
            productoId: item.productoId,
            cantidadProductos: item.cantidad,
            subtotal: item.subtotal
        }))
    };

    fetch("/VentaApi", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(venta)
    }).then(res => {
        if (res.ok) {
            alert("Venta registrada correctamente");
            location.reload();
        } else {
            alert("Ocurrió un error al registrar la venta.");
        }
    });
}
```

#### Obtener forma de pago

```js
const formaPagoId = document.getElementById("formaPagoSelect").value;
if (!formaPagoId) {
    alert("Seleccione una forma de pago.");
    return;
}
```

- Agarra el valor seleccionado en el `<select>` de forma de pago.
- Si no se eligió nada, tira un alert y frena todo con return.

#### Armar objeto Venta

`const venta = {`:

- Crea un objeto Venta que se va enviar al servidor.

#### Manda la venta al backend

```js
fetch("/VentaApi", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(venta)
})
```

- Le pega a tu API (`/VentaApi`) con un método POST.
- Manda el objeto venta convertido a JSON (`body: JSON.stringify(venta)`).
- Le avisa al backend que lo que manda es JSON (`Content-Type: application/json`).

#### Manejo de respuesta

```js
.then(res => {
    if (res.ok) {
        alert("Venta registrada correctamente");
        location.reload();
    } else {
        alert("Ocurrió un error al registrar la venta.");
    }
});
```

- Si la respuesta del servidor es OK

    - Muestra un mensaje de exito
    - Recarga la pagina con `location.reload();`

- Si algo salio mal lanza un mensaje de error

---

### Funciones de javascript relacionadas con GET (vista de ver ventas)

```js
document.addEventListener("DOMContentLoaded", function () {
    const hoy = new Date();
    const fechaHoy = hoy.toISOString().split('T')[0];

    document.getElementById("desde").value = fechaHoy;
    document.getElementById("hasta").value = fechaHoy;

    cargarVentas(fechaHoy, fechaHoy);

    document.getElementById("btnFiltrar").addEventListener("click", () => {
        const desde = document.getElementById("desde").value;
        const hasta = document.getElementById("hasta").value;

        if (desde && hasta) {
            cargarVentas(desde, hasta);
        }
    });
});
```

#### Obtener la fecha actual

```js
const hoy = new Date();
const fechaHoy = hoy.toISOString().split('T')[0];
```   

- Creamos una instancia Date con la fecha actual.
- `toISOString()` da algo como: `"2025-06-10T12:34:56.789Z"`.
- `split('T')[0]` corta la hora y se queda solo con "2025-06-10" 

#### Cargar la fecha por defecto

```js
document.getElementById("desde").value = fechaHoy;
document.getElementById("hasta").value = fechaHoy;
```   

- Pone la fecha de hoy en los dos inputs de tipo fecha: desde y hasta.
- Esto hace que cuando entres a la vista, ya se muestren las ventas del día actual sin que el usuario toque nada.

#### Cargar ventas del dia actual

`cargarVentas(fechaHoy, fechaHoy);`

- Llama a la función cargarVentas.
- Le pasa fechaHoy como desde y hasta, así que por defecto muestra las ventas de hoy.

#### Configurar el boton "Filtrar"

```js
document.getElementById("btnFiltrar").addEventListener("click", () => {
    const desde = document.getElementById("desde").value;
    const hasta = document.getElementById("hasta").value;

    if (desde && hasta) {
        cargarVentas(desde, hasta);
    }
});
```

- Cuando el usuario hace click en el boton de filtro, toma las fechas seleccionadas por el usuario y llama a la funcion `cargarVentas` con
esas fechas.

---

```js
function cargarVentas(desde, hasta) {
    fetch(`/VentaApi/${negocioId}?desde=${desde}&hasta=${hasta}`)
        .then(res => {
            if (!res.ok) throw new Error("Error al cargar ventas");
            return res.json();
        })
        .then(data => {
            const tbody = document.querySelector("table tbody");
            tbody.innerHTML = "";

            if (data.ventas.length === 0) {
                tbody.innerHTML = `<tr><td colspan="5" class="text-center">No se encontraron ventas</td></tr>`;
                return;
            }

            data.ventas.forEach((venta, index) => {
                const fecha = venta.fecha ? new Date(venta.fecha).toLocaleString() : "Fecha desconocida";
                const total = typeof venta.total === "number" ? `$${venta.total.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }` : "$0.00";
                const formaPago = venta.nombreFormaPago || "Sin forma de pago";
                const nombreUsuario = venta.nombreUsuario || "Usuario desconocido";

                const fila = `
                    <tr>
                        <th scope="row">${index + 1}</th>
                        <td>${fecha}</td>
                        <td>${total}</td>
                        <td>${formaPago}</td>
                        <td>${nombreUsuario}</td>
                        <td> 
                              <a href="/VentaApi/factura/${venta.idVenta}" target="_blank" class="btn btn-sm btn-secondary">
                                 Ver factura
                             </a>
                        
                        </td>
                    </tr>
                `;
                tbody.innerHTML += fila;
            });

            // Mostrar resumen
            const resumenDiv = document.getElementById("resumenVentas");
            resumenDiv.innerHTML = "<h5>Resumen</h5>";

            data.resumenPorFormaPago.forEach(r => {
                resumenDiv.innerHTML += `<p><strong>${r.formaPago}:</strong> $${r.total.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }</p>`;
            });

            resumenDiv.innerHTML += `<p><strong>Total general:</strong> $${data.totalGeneral.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }</p>`;
        })
        .catch(err => {
            console.error("Error al obtener ventas:", err);
        });
}
```


#### Paso 1: Llama al backend

`fetch(/VentaApi/${negocioId}?desde=${desde}&hasta=${hasta})`

- Se hace un GET al endpoint /VentaApi/{negocioId} con parámetros de query string (desde, hasta).
- El backend filtra las ventas por esas fechas y devuelve un JSON con la info.
- Ejemplo de URL que construye: /VentaApi/5?desde=2025-06-01&hasta=2025-06-10

#### Paso 2: Validacion de respuesta

```js
.then(res => {
    if (!res.ok) throw new Error("Error al cargar ventas");
    return res.json();
})
```

- Si la respuesta no es 200 OK, lanza un error.
- Si todo va bien, convierte el resultado a objeto JavaScript (res.json()).

#### Paso 3: Limpiar la tabla 

```js
const tbody = document.querySelector("table tbody");
tbody.innerHTML = "";
```

- Limpia el contenido de la tabla donde se mostrarán las ventas.
- Así evitás que queden datos viejos mezclados.

#### Paso 4: Si no hay ventas , muestra mensaje

```js
if (data.ventas.length === 0) {
    tbody.innerHTML = `<tr><td colspan="5" class="text-center">No se encontraron ventas</td></tr>`;
    return;
}
```

- Si no se encuentran ventas, muestra un mensaje en la tabla.
- Y termina la función con return para no seguir ejecutando.

#### Paso 5: Recorrer las ventas y mostrarlas

```js
data.ventas.forEach((venta, index) => {
    const fecha = venta.fecha ? new Date(venta.fecha).toLocaleString() : "Fecha desconocida";
    const total = typeof venta.total === "number" ? `$${venta.total.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }` : "$0.00";
    const formaPago = venta.nombreFormaPago || "Sin forma de pago";
    const nombreUsuario = venta.nombreUsuario || "Usuario desconocido";
}
```

- Por cada venta: 

    - Se formatea la fecha (toLocaleString() → local friendly).
    - Se muestra el total con 2 decimales.
    - Se cargan la forma de pago y el nombre del usuario (con fallback si falta).
    
```js
    const fila = `
        <tr>
            <th scope="row">${index + 1}</th>
            <td>${fecha}</td>
            <td>${total}</td>
            <td>${formaPago}</td>
            <td>${nombreUsuario}</td>
            <td> 
                  <a href="/VentaApi/factura/${venta.idVenta}" target="_blank" class="btn btn-sm btn-secondary">
                     Ver factura
                 </a>
            </td>
        </tr>
    `;
    tbody.innerHTML += fila;
});
```

- Se genera el HTML de cada fila y se agrega al tbody.
- El botón "Ver factura" abre una vista/descarga del PDF en otra pestaña usando el idVenta.

#### Paso 6: Mostrar resumen por forma de pago.

```js
const resumenDiv = document.getElementById("resumenVentas");
resumenDiv.innerHTML = "<h5>Resumen</h5>";

data.resumenPorFormaPago.forEach(r => {
    resumenDiv.innerHTML += `<p><strong>${r.formaPago}:</strong> $${r.total.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }</p>`;
});

```

- Se muestra un bloque resumen con subtotales agrupados por forma de pago.

#### Paso 7: Mostrar total general

`resumenDiv.innerHTML += `<p><strong>Total general:</strong> $${data.totalGeneral.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) }</p>`;`

- Muestra el total general de todas las ventas del rango elegido.

#### Paso 8: Captura errores

```js
.catch(err => {
    console.error("Error al obtener ventas:", err);
});
```

- Si algo falla en el fetch o en el then, se muestra el error en consola.

