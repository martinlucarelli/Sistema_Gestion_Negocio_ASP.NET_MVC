using Sistema_Gestion_Negocio_ASP.NET_MVC.Configuration;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Context;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Conexion con la base de datos
builder.Services.AddSqlServer<NegocioContext>(builder.Configuration.GetConnectionString("cnBaseDeDatos"));

//EmailSettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

//Inyeccion de dependencias

builder.Services.AddScoped<IProductoApiService, ProductoApiService>();
builder.Services.AddScoped<IVentaApiService, VentaApiService>();
builder.Services.AddScoped<IEstadisticaApiService, EstadisticaApiService>();
builder.Services.AddScoped<INegocioApiService, NegocioApiService>();
//Inyectar servicio de mail
builder.Services.AddScoped<EmailService>();
//Inyectar servicio de bash de clave
builder.Services.AddScoped<BashClaveService>();

//Agregar Cookie de autenticacion
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Login/Login"; //Dirige al usuario al login si no esta autenticando (logueado)
        option.ExpireTimeSpan = TimeSpan.FromMinutes(300); //La cookie dura 5 horas, luego se vence y por lo tanto se cierra sesion
        option.SlidingExpiration= true; //Si el usuario sigue activo el tiempo de la cookie se renueva
        option.AccessDeniedPath= "/Home/AccesoDenegado"; //Si el usuario no tiene acceso a alguna vista lo dirige a privacy.

    });

//Licencia de QuestPDF
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
