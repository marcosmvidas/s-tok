using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Equipe
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class DetalhesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly S_tokDbContext _context;

        public DetalhesModel(UserManager<IdentityUser> userManager, S_tokDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public string Id { get; private set; } = string.Empty;
        public string NomeCompleto { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Username { get; private set; } = string.Empty;
        public string Perfil { get; private set; } = string.Empty;
        public bool EmailConfirmado { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var identity = await _userManager.FindByIdAsync(id);
            if (identity == null)
                return NotFound();

            var cadastro = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdentityUserId == id);

            if (cadastro == null)
                return NotFound();

            var perfis = await _userManager.GetRolesAsync(identity);
            Id = identity.Id;
            NomeCompleto = cadastro.NomeCompleto;
            Email = identity.Email ?? string.Empty;
            Username = identity.UserName ?? string.Empty;
            Perfil = perfis.Count > 0 ? string.Join(", ", perfis) : "Sem perfil";
            EmailConfirmado = identity.EmailConfirmed;

            return Page();
        }
    }
}
