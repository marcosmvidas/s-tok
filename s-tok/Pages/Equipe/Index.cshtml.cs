using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace s_tok.Pages.Equipe
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public List<UsuarioEquipeItem> Usuarios { get; private set; } = [];

        public async Task OnGetAsync()
        {
            var usuarios = await _userManager.Users
                .OrderBy(usuario => usuario.Email)
                .ToListAsync();

            foreach (var usuario in usuarios)
            {
                var perfis = await _userManager.GetRolesAsync(usuario);

                Usuarios.Add(new UsuarioEquipeItem
                {
                    Email = usuario.Email ?? string.Empty,
                    Perfil = perfis.Count > 0
                        ? string.Join(", ", perfis)
                        : "Sem perfil",
                    EmailConfirmado = usuario.EmailConfirmed
                });
            }
        }

        public class UsuarioEquipeItem
        {
            public string Email { get; set; } = string.Empty;

            public string Perfil { get; set; } = string.Empty;

            public bool EmailConfirmado { get; set; }
        }
    }
}
