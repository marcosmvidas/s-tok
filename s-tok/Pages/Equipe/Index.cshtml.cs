using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Equipe
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly S_tokDbContext _context;

        public IndexModel(UserManager<IdentityUser> userManager, S_tokDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public List<UsuarioEquipeItem> Usuarios { get; private set; } = [];

        public async Task OnGetAsync()
        {
            var usuarios = await _userManager.Users
                .OrderBy(usuario => usuario.Email)
                .ToListAsync();

            var cadastros = await _context.Usuarios
                .AsNoTracking()
                .ToDictionaryAsync(usuario => usuario.IdentityUserId);

            foreach (var usuario in usuarios)
            {
                var perfis = await _userManager.GetRolesAsync(usuario);
                cadastros.TryGetValue(usuario.Id, out var cadastro);

                Usuarios.Add(new UsuarioEquipeItem
                {
                    Id = usuario.Id,
                    NomeCompleto = cadastro?.NomeCompleto ?? usuario.UserName ?? "Usuário",
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
            public string Id { get; set; } = string.Empty;
            public string NomeCompleto { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Perfil { get; set; } = string.Empty;
            public bool EmailConfirmado { get; set; }
        }
    }
}
