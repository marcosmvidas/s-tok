using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Equipe
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AlterarModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly S_tokDbContext _context;

        public AlterarModel(UserManager<IdentityUser> userManager, S_tokDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public EntradaAlteracao Entrada { get; set; } = new();

        public List<string> PerfisDisponiveis { get; private set; } = [];

        private async Task CarregarPerfisAsync()
        {
            PerfisDisponiveis = User.IsInRole("SuperAdmin")
                ? ["Admin", "Operador"]
                : ["Operador"];
            await Task.CompletedTask;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await CarregarPerfisAsync();
            var identity = await _userManager.FindByIdAsync(id);
            if (identity == null) return NotFound();

            var perfis = await _userManager.GetRolesAsync(identity);
            if (perfis.Contains("SuperAdmin") ||
                (!User.IsInRole("SuperAdmin") && perfis.Contains("Admin")))
                return Forbid();

            var cadastro = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdentityUserId == id);
            if (cadastro == null) return NotFound();

            Entrada = new EntradaAlteracao
            {
                Id = identity.Id,
                NomeCompleto = cadastro.NomeCompleto,
                Email = identity.Email ?? string.Empty,
                Username = identity.UserName ?? string.Empty,
                Perfil = perfis.FirstOrDefault(p => p != "SuperAdmin") ?? "Operador"
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CarregarPerfisAsync();
            if (!ModelState.IsValid) return Page();

            if (!PerfisDisponiveis.Contains(Entrada.Perfil))
            {
                ModelState.AddModelError("Entrada.Perfil", "Você não tem permissão para atribuir esse perfil.");
                return Page();
            }

            var identity = await _userManager.FindByIdAsync(Entrada.Id);
            if (identity == null) return NotFound();

            var perfisAtuais = await _userManager.GetRolesAsync(identity);
            if (perfisAtuais.Contains("SuperAdmin") ||
                (!User.IsInRole("SuperAdmin") && perfisAtuais.Contains("Admin")))
                return Forbid();

            var cadastro = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdentityUserId == Entrada.Id);
            if (cadastro == null) return NotFound();

            var emailExistente = await _userManager.FindByEmailAsync(Entrada.Email);
            if (emailExistente != null && emailExistente.Id != identity.Id)
            {
                ModelState.AddModelError("Entrada.Email", "Este e-mail já está cadastrado.");
                return Page();
            }

            var usernameExistente = await _userManager.FindByNameAsync(Entrada.Username);
            if (usernameExistente != null && usernameExistente.Id != identity.Id)
            {
                ModelState.AddModelError("Entrada.Username", "Este nome de usuário já está cadastrado.");
                return Page();
            }

            await using var transacao = await _context.Database.BeginTransactionAsync();

            identity.Email = Entrada.Email.Trim();
            identity.UserName = Entrada.Username.Trim();
            identity.NormalizedEmail = _userManager.NormalizeEmail(identity.Email);
            identity.NormalizedUserName = _userManager.NormalizeName(identity.UserName);
            cadastro.NomeCompleto = Entrada.NomeCompleto.Trim();

            var update = await _userManager.UpdateAsync(identity);
            if (!update.Succeeded)
            {
                foreach (var erro in update.Errors)
                    ModelState.AddModelError(string.Empty, erro.Description);
                return Page();
            }

            var removerPerfis = await _userManager.RemoveFromRolesAsync(identity, perfisAtuais);
            if (!removerPerfis.Succeeded)
            {
                foreach (var erro in removerPerfis.Errors)
                    ModelState.AddModelError(string.Empty, erro.Description);
                return Page();
            }

            var adicionarPerfil = await _userManager.AddToRoleAsync(identity, Entrada.Perfil);
            if (!adicionarPerfil.Succeeded)
            {
                foreach (var erro in adicionarPerfil.Errors)
                    ModelState.AddModelError(string.Empty, erro.Description);
                return Page();
            }

            await _context.SaveChangesAsync();
            await transacao.CommitAsync();
            TempData["Sucesso"] = "Usuário alterado com sucesso!";
            return RedirectToPage("/Equipe/Index");
        }

        public class EntradaAlteracao
        {
            [Required]
            public string Id { get; set; } = string.Empty;

            [Required(ErrorMessage = "Informe o nome completo.")]
            [StringLength(150)]
            [Display(Name = "Nome completo")]
            public string NomeCompleto { get; set; } = string.Empty;

            [Required(ErrorMessage = "Informe o e-mail.")]
            [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
            [Display(Name = "E-mail")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Informe o nome de usuário.")]
            [StringLength(50, MinimumLength = 3)]
            [Display(Name = "Nome de usuário")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Selecione um perfil.")]
            [Display(Name = "Perfil de acesso")]
            public string Perfil { get; set; } = string.Empty;
        }
    }
}
