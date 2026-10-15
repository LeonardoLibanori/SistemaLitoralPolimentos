using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SistemaLitoralPolimentos.Data;
using SistemaLitoralPolimentos.Services;

var builder = WebApplication.CreateBuilder(args);

// Em produção os cookies usam Secure=Always (corrige login via PWA instalado, commit 5477f00).
// Em Development isso quebra o perfil "http" do dotnet run: o antiforgery lança exceção quando
// SecurePolicy=Always e a requisição não é SSL. SameAsRequest mantém Secure nas requisições
// HTTPS (localhost:7034) e evita a exceção em HTTP puro (localhost:5057).
var cookieSecurePolicy = builder.Environment.IsDevelopment()
    ? CookieSecurePolicy.SameAsRequest
    : CookieSecurePolicy.Always;

builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Autenticação baseada em Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = cookieSecurePolicy;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<GoogleCalendarService>();
builder.Services.AddHttpClient();

// Proteção contra CSRF global
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = cookieSecurePolicy;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Normaliza status legado: "Aprovado" passa a ser "Feito"
    await db.Database.ExecuteSqlRawAsync(
        "UPDATE \"Documentos\" SET \"Status\" = 'Feito' WHERE \"Status\" = 'Aprovado'");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
var uploadsPath = Path.Combine(app.Environment.WebRootPath, "uploads", "solicitacoes");
Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Headers de segurança
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();