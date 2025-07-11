# Sistema_Gestion_Negocio_ASP.NET_MVC



Aplicación web desarrollada con ASP.NET Core MVC para gestionar ventas, productos y usuarios de distintos negocios. 
Diseñada para ser utilizada por múltiples empresas con un único sistema, mediante roles y permisos.

---
## Indice

- [Tecnologías utilizadas](#tecnologías-utilizadas)
- [Arquitectura](#arquitectura)
- [Roles de usuario](#roles-de-usuario)
- [Funcionalidades principales](#funcionalidades-principales)
- [Base de datos](#base-de-datos)
- [Controllers](#controllers)
- [Services](#cervices)
- [Vistas Principales](#vistas-principales)
- [Notas de desarrollo](#notas-de-desarrollo)
- [En desarrollo](#en-desarrollo)
- [Autor](#autor)

## PROXIMO A HACER:

- Achicar el panel de registrar venta y mejorar estilos
- Mejorar el estilo de las vistas en general (agregarle color de la aplicacion, bordes, tamaños)
- Agregar en la parte superior derecha, de todas las vistas (en el shared) informacion del usuario en sesion.
- Colocar una vista al iniciar la aplicacion, esta vista debe contner el nombre del sistema, alguna imagen, informacion y la
posibilidad de que inicien sesion los usuarios.
- Permitir que se registre un negocio desde la vista principal.





## Tecnologías utilizadas

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- C#
- Razor Pages
- Bootstrap 
- API REST
- Javascript

---

## Arquitectura

- Patrón MVC (Modelo-Vista-Controlador)
- Autenticación y autorización con roles
- Relación multi-negocio (cada negocio tiene sus usuarios, productos y ventas)
- Generación de PDFs

---

## Roles de usuario


|		Rol					|						Descripción 
|---------------------------|-------------------------------------------------------------------------|
| Administrador General		| Acceso completo a todo el sistema. Solo se crea desde la base de datos. |
|                           |                                                                         |
| Administrador de Negocio	| Dueño del negocio. Gestiona productos, ventas, empleados, etc.		  |
|                           |                                                                         |
| Empleado					| Acceso limitado. Puede registrar ventas, ver stock, etc.				  |
|                           |                                                                         |


---

## Funcionalidades principales

1. **Registrar nueva venta**
2. **Gestión de productos/stock**
3. **Historial de ventas**
   - Filtro por fechas
   - Detalle de venta (PDF)
4. **Estadísticas con gráficos**

---

## Base de datos

- SQL Server
- Relaciones:
  - Un `Negocio` tiene muchos `Usuarios`, cada `Usuario` pertenece a un solo `Negocio`  
  - Un `Negocio` tiene muchos `Productos`, cada `Productos` pertenece a un solo `Negocio`
  - Las `Ventas` están relacionadas muchos-a-muchos con `Productos` mediante `DetalleVenta`

---

## Controllers
- **LoginController**  
	Encargado del inicio de sesion y restablecer contraseña. Tambien tiene una funcion que se encarga de cifrar las contraseñas.
- **NegocioController**
	Encargado de mostrar los negocios que hay en la base de datos al administrador general. Ademas se encarga de enviar un correo
	a un cliente para poder realizar el registro de su usuario y de su negocio.

---

## Services
-**EmailService**
	Tiene la funcion que se encarga de enviar los mail para recuperar contraseña o registrar usuario.

---

## Helper


---

## Vistas principales

- **Login.cshtml**: Inicio de sesión.
- **RecuperarClave.cshtml**: Correo donde se va enviar el cambio de contraseña
- **RestablecerClave.cshtml**: Formulario para cambiar la contraseña.
- **Negocios**:Le muestran al Admin general los negocios del sistema.
- **AgregarNegocio.cshtml**:La usa el Admin general para mandar un mail a un cliente que contrato el servicio.
- **RegistrarAdministradorNegocio.cshtml**: Formulario para registrar al usuario dueño o administrador del negocio (se recibe por correo)
- **RegistrarNegocio**: Formulario para registrar negocio.

---

## Notas de desarrollo

- Se usa Guid para entidades principales y NanoId para otras tablas como Venta y Producto
- Los roles están definidos mediante un enum
- Autenticación por correo y contraseña
- El campo Confirmado arranca en false y se activa vía token de email
- Las credenciales del correo se definen por dotnet user-secret para que no haya datos sensibles a la vista
- Rol, al ser un enum, se convierte con fluent api en el context para que EF lo tome como int
- Si el usuario esta 3 horas inactivo se le cierra la sesion automaticamente.
- Se utilizan Claims para guardar informacion del usuario en una Cookie.
- Las opereciones de los productos y las ventas se hacen a traves de una API.
- Se utiliza Chart.js para manejar las estadisticas y graficos.



## En dasarrollo

- [X] Login, confirmacion por correo y restablecer contraseña por correo.
- [X] Autorizacion mediante roles del usuario, utilizando Cokies.
- [X] Form para que un cliente pueda completar los datos de su negocio (se envia por mail).
- [X] CRUD para que usuarios administradores de negocios registren usuarios empleados.
- [X] Mostrar productos en seccion Stock.
- [X] CRUD para productos (solo accesible para usuarios administradores de negocios).
- [X] Mostrar lista de ventas, posibilidad de imprimir el detalle de una venta (tipo facutra).
- [X] Mostrar lista de ventas especificando entre que fechas mostrar la lista, por defecto debe estar en el dia actual,pero se puede
cambiar las fechas con un calendario.
- [X] API REST para actualizar el DOM sin recargar la página
- [X] Estadísticas con gráficos

## Autor

- Martin Lucarelli 
- Proyecto educativo

