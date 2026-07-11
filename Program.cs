/**************************************************************************
 * Proyecto    : CyberWatch Analytics
 * Autor       : Paulo Estrella
 * Institución : IPLACEX
 * Descripción : Configuración principal de la aplicación web, servicios,
 *               conexión a base de datos, sesiones y pipeline HTTP.
 **************************************************************************/

using CyberWatchAnalytics.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registro de servicios utilizados por la aplicación.
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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configuración del manejo de errores para ambiente de producción.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Configuración del pipeline de procesamiento de solicitudes HTTP.
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Habilitación de sesiones antes de la autorización.
app.UseSession();

app.UseAuthorization();

// Configuración de la ruta predeterminada de la aplicación.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();