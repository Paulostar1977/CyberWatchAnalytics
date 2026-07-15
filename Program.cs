/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Configuración principal de la aplicación web, servicios,
 *               conexión a base de datos, sesiones y pipeline HTTP.
 **************************************************************************/

using CyberWatchAnalytics.Data;
using CyberWatchAnalytics.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//======================================================================
// REGISTRO DE SERVICIOS DE LA APLICACIÓN
//======================================================================

// Habilita el uso de controladores y vistas mediante el patrón MVC.
builder.Services.AddControllersWithViews();

// Configuración de sesiones para mantener los datos del usuario autenticado.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configuración del contexto de base de datos SQL Server.
builder.Services.AddDbContext<CyberWatchDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de los datos utilizados por el servicio de correo.
// Los valores sensibles se obtienen mediante User Secrets.
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// Registro del servicio encargado del envío de correos electrónicos.
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

//======================================================================
// CONFIGURACIÓN DEL PIPELINE HTTP
//======================================================================

// Configuración del manejo de errores para ambiente de producción.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Redirección hacia HTTPS y habilitación de archivos estáticos.
app.UseHttpsRedirection();
app.UseStaticFiles();

// Habilitación del sistema de rutas.
app.UseRouting();

// Habilitación de sesiones antes de la autorización.
app.UseSession();

// Habilitación del sistema de autorización.
app.UseAuthorization();

//======================================================================
// CONFIGURACIÓN DE RUTAS
//======================================================================

// Configuración de la ruta predeterminada de la aplicación.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();