using DriveNow.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

var connStr = builder.Configuration.GetConnectionString("DriveNowDB") ?? "Data Source=drivenow.db";
Banco.Configurar(connStr);
DatabaseInitializer.Inicializar(connStr);

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "login",
    pattern: "",
    defaults: new { controller = "Usuarios", action = "Login" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.Run();
