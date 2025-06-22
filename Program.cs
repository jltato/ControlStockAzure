using ControlStock.Areas.Infrastructure;
using ControlStock.Data;
using ControlStock.Models;
using jsreport.AspNetCore;
using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Rotativa.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddMvcCore();

// Configurar Serilog
Log.Logger = new Serilog.LoggerConfiguration()
    .WriteTo.Console() // Logging a la consola
    .WriteTo.Debug() // Logging al depurador
    .WriteTo.File("Logs/general-.log", rollingInterval: RollingInterval.Day) // Archivo para logs generales
    .WriteTo.File("Logs/errors-.log", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error) // Archivo para logs de errores
    .CreateLogger();

builder.Host.UseSerilog(); // Usar Serilog como el proveedor de logging

// Add services to the container.

builder.Services.AddHttpClient();

// configuracion de coneccion a la BBDD
var connectionString = builder.Configuration.GetConnectionString("Connection") ?? throw new InvalidOperationException("Connection string 'Connection' not found.");

builder.Services.AddIdentity<MyUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
})
    .AddErrorDescriber<CustomIdentityErrorDescriber>()  
    .AddEntityFrameworkStores<MyDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddControllersWithViews();
//builder.Services.AddScoped<StockService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de inactividad permitido
    options.SlidingExpiration = true; // Renueva la cookie si el usuario est� activo
    options.LoginPath = "/Identity/Account/Login";  // configuracion de la redireccion en caso de que no este autorizado el usuario
    options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // configuracion de la pagina de acceso denegado
}
);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddJsReport(new LocalReporting()
    .UseBinary(JsReportBinary.GetBinary())    
    .KillRunningJsReportProcesses()
    .AsUtility()
    .Create());
new LocalReporting().TempDirectory("~/temp/");

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

var app = builder.Build();

// Aplicar migraciones al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    db.Database.Migrate();
}

// Aplicar usuario admin al iniciar
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializeAsync(services);
}

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

app.MapRazorPages().RequireAuthorization();
//app.UseJsReport();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .RequireAuthorization();
app.Run();
