using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace s_tok.Services
{
    public class InitialSetupService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public InitialSetupService(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<bool> CanInitializeAsync()
        {
            return !await _userManager.Users.AnyAsync();
        }
        
        public async Task<bool> RolesExistAsync()
        {
            return await _roleManager.RoleExistsAsync("SuperAdmin")
                && await _roleManager.RoleExistsAsync("Admin")
                && await _roleManager.RoleExistsAsync("Operador");
        }

        public async Task<IdentityResult> CreateRolesAsync()
        {
            string[] roles =
            {
                "SuperAdmin",
                "Admin",
                "Operador"
            };

            foreach (var role in roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    continue;
                }

                var result = await _roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!result.Succeeded)
                {
                    return result;
                }
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> CreateSuperAdminAsync(
            string email,
            string password)
        {
            if (!await CanInitializeAsync())
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description =
                            "A inicialização só é permitida quando não existem usuários cadastrados."
                    });
            }

            if (!await RolesExistAsync())
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description =
                            "As funções do sistema ainda não foram configuradas."
                    });
            }

            var existingAdmins = await _userManager.GetUsersInRoleAsync(
                "SuperAdmin");

            if (existingAdmins.Count > 0)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description =
                            "O SuperAdmin inicial já foi cadastrado."
                    });
            }

            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser is not null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description =
                            "Já existe um usuário com esse e-mail."
                    });
            }

            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(
                user,
                password);

            if (!createResult.Succeeded)
            {
                return createResult;
            }

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                "SuperAdmin");

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return roleResult;
            }

            return IdentityResult.Success;
        }
    }
}
