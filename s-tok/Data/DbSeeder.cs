
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Data
{
    public static class DbSeeder
    {
        public const string PerfilAdmin = "SuperAdmin";

        public static async Task SeedAsync(
            IServiceProvider services,
            IConfiguration config)
        {
            var nome = config["Seed:AdminNome"];
            var username = config["Seed:AdminUsuario"];
            var email = config["Seed:AdminEmail"];
            var senha = config["Seed:AdminSenha"];

            Console.WriteLine("=== DIAGNÓSTICO DO DBSEEDER ===");
            Console.WriteLine($"Nome configurado: {!string.IsNullOrWhiteSpace(nome)}");
            Console.WriteLine($"Usuário configurado: {!string.IsNullOrWhiteSpace(username)}");
            Console.WriteLine($"E-mail configurado: {!string.IsNullOrWhiteSpace(email)}");
            Console.WriteLine($"Senha configurada: {!string.IsNullOrWhiteSpace(senha)}");

            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(senha))
            {
                Console.WriteLine("SEED CANCELADO: faltam configurações.");
                return;
            }

            using var scope = services.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<S_tokDbContext>();

            var roles = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            var users = scope.ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            Console.WriteLine(
                $"Banco utilizado: {db.Database.GetDbConnection().DataSource}");

            if (!await db.Database.CanConnectAsync())
            {
                throw new InvalidOperationException(
                    "Não foi possível conectar ao banco de dados.");
            }

            // Garante que os perfis existam.
            foreach (var perfil in new[] { "SuperAdmin", "Admin" })
            {
                if (await roles.RoleExistsAsync(perfil))
                    continue;

                var resultadoRole = await roles.CreateAsync(
                    new IdentityRole(perfil));

                if (!resultadoRole.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Falha ao criar perfil: " +
                        string.Join("; ",
                            resultadoRole.Errors.Select(e => e.Description)));
                }
            }

            // Localiza ou cria o administrador.
            var login = await users.FindByNameAsync(username);

            if (login == null)
            {
                login = new IdentityUser
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true
                };

                var resultadoCriacao = await users.CreateAsync(login, senha);

                if (!resultadoCriacao.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Falha ao criar administrador: " +
                        string.Join("; ",
                            resultadoCriacao.Errors.Select(e => e.Description)));
                }

                Console.WriteLine("Usuário de autenticação criado.");
            }
            else
            {
                Console.WriteLine("Usuário de autenticação já existe.");
            }

            // Garante o perfil SuperAdmin.
            if (!await users.IsInRoleAsync(login, PerfilAdmin))
            {
                var resultadoPerfil = await users.AddToRoleAsync(
                    login,
                    PerfilAdmin);

                if (!resultadoPerfil.Succeeded)
                {
                    throw new InvalidOperationException(
                        "Não foi possível atribuir o perfil SuperAdmin: " +
                        string.Join("; ",
                            resultadoPerfil.Errors.Select(e => e.Description)));
                }

                Console.WriteLine("Perfil SuperAdmin atribuído com sucesso.");
            }
            else
            {
                Console.WriteLine("Administrador já possui o perfil SuperAdmin.");
            }

            // Procura o cadastro complementar pelo IdentityUserId.
            var usuario = await db.Usuarios
                .FirstOrDefaultAsync(u => u.IdentityUserId == login.Id);

            if (usuario != null)
            {
                Console.WriteLine(
                    $"Cadastro complementar já existe. Usuario ID: {usuario.Id}");
            }
            else
            {
                // Procura uma pessoa pelo nome configurado.
                var pessoa = await db.Pessoas
                    .FirstOrDefaultAsync(p => p.Nome == nome);

                if (pessoa == null)
                {
                    pessoa = new Pessoa
                    {
                        Nome = nome
                    };

                    db.Pessoas.Add(pessoa);
                    await db.SaveChangesAsync();

                    Console.WriteLine("Pessoa criada.");
                }

                // Verifica se a pessoa já pertence a outro usuário.
                var usuarioVinculado = await db.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.PessoaId == pessoa.Id);

                if (usuarioVinculado != null)
                {
                    throw new InvalidOperationException(
                        $"Não foi possível concluir o cadastro complementar. " +
                        $"A pessoa '{pessoa.Nome}', ID {pessoa.Id}, " +
                        $"já está vinculada ao usuário ID {usuarioVinculado.Id}. " +
                        $"IdentityUserId vinculado: {usuarioVinculado.IdentityUserId}. " +
                        $"IdentityUserId do administrador atual: {login.Id}. " +
                        "Os vínculos existentes foram preservados. " +
                        "É necessário verificar qual conta deve utilizar essa pessoa.");
                }

                // Garante o cadastro na equipe.
                var equipeExiste = await db.Equipe
                    .AnyAsync(e => e.PessoaId == pessoa.Id);

                if (!equipeExiste)
                {
                    db.Equipe.Add(new Equipe
                    {
                        PessoaId = pessoa.Id
                    });
                }

                // Cria o cadastro complementar.
                db.Usuarios.Add(new Usuario
                {
                    NomeCompleto = nome,
                    IdentityUserId = login.Id,
                    PessoaId = pessoa.Id
                });

                await db.SaveChangesAsync();

                Console.WriteLine(
                    "Cadastro complementar criado em Usuarios.");
            }

            Console.WriteLine("=== DBSEEDER CONCLUÍDO ===");
        }
    }
}
