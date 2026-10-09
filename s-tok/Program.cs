using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;
using s_tok.Services;

var builder = WebApplication.CreateBuilder(args);

// Banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=stok.db";

builder.Services.AddDbContext<S_tokDbContext>(options =>
    options.UseSqlite(connectionString));

// ASP.NET Core Identity
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;

        options.Password.RequiredLength = 12;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<S_tokDbContext>();

// Configuração do cookie de autenticação
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Serviço de configuração inicial
builder.Services.AddScoped<InitialSetupService>();

// Razor Pages: proteger o sistema por padrão
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");

    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Error");
    options.Conventions.AllowAnonymousToPage("/Privacy");
});

var app = builder.Build();


if (args.Contains("--setup-superadmin", StringComparer.OrdinalIgnoreCase))
{
    await using var scope = app.Services.CreateAsyncScope();

    var setupService = scope.ServiceProvider
        .GetRequiredService<InitialSetupService>();

    if (!await setupService.CanInitializeAsync())
    {
        Console.WriteLine(
            "Inicialização cancelada: já existem usuários cadastrados.");

        return;
    }

    Console.Write("E-mail do SuperAdmin: ");
    var email = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(email) ||
        !new System.ComponentModel.DataAnnotations.EmailAddressAttribute()
            .IsValid(email))
    {
        Console.WriteLine("Informe um e-mail válido.");
        return;
    }

    Console.Write("Senha do SuperAdmin: ");
    var password = ConsolePasswordReader.ReadPassword();

    Console.Write("Confirme a senha: ");
    var confirmPassword = ConsolePasswordReader.ReadPassword();

    if (string.IsNullOrWhiteSpace(password))
    {
        Console.WriteLine("A senha não pode ficar vazia.");
        return;
    }

    if (password != confirmPassword)
    {
        Console.WriteLine("As senhas não coincidem.");
        return;
    }

    var rolesResult = await setupService.CreateRolesAsync();

    if (!rolesResult.Succeeded)
    {
        Console.WriteLine("Não foi possível criar as funções:");

        foreach (var error in rolesResult.Errors)
        {
            Console.WriteLine($"- {error.Description}");
        }

        return;
    }

    var result = await setupService.CreateSuperAdminAsync(
        email,
        password);

    if (!result.Succeeded)
    {
        Console.WriteLine("Não foi possível criar o SuperAdmin:");

        foreach (var error in result.Errors)
        {
            Console.WriteLine($"- {error.Description}");
        }

        return;
    }

    Console.WriteLine("SuperAdmin criado com sucesso!");
    return;
}
// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
