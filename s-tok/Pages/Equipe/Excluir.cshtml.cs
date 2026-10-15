using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Equipe
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ExcluirModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly S_tokDbContext _context;

        public ExcluirModel(UserManager<IdentityUser> userManager, S_tokDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult OnGet(string? id) => RedirectToPage("/Equipe/Index");

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var identity = await _userManager.FindByIdAsync(id);
            if (identity == null)
                return NotFound();

            if (identity.Id == _userManager.GetUserId(User))
            {
                TempData["Erro"] = "Não é possível excluir a própria conta.";
                return RedirectToPage("/Equipe/Index");
            }

            var perfis = await _userManager.GetRolesAsync(identity);
            if (perfis.Contains("SuperAdmin") ||
                (!User.IsInRole("SuperAdmin") && perfis.Contains("Admin")))
            {
                TempData["Erro"] = "Você não tem permissão para excluir essa conta.";
                return RedirectToPage("/Equipe/Index");
            }

            var cadastro = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdentityUserId == id);

            await using var transacao = await _context.Database.BeginTransactionAsync();
            try
            {
                if (cadastro != null)
                {
                    _context.Usuarios.Remove(cadastro);
                    await _context.SaveChangesAsync();
                }

                var resultado = await _userManager.DeleteAsync(identity);
                if (!resultado.Succeeded)
                {
                    await transacao.RollbackAsync();
                    TempData["Erro"] = string.Join(" ", resultado.Errors.Select(e => e.Description));
                    return RedirectToPage("/Equipe/Index");
                }

                await transacao.CommitAsync();
                TempData["Sucesso"] = "Usuário excluído com sucesso.";
            }
            catch
            {
                await transacao.RollbackAsync();
                TempData["Erro"] = "Não foi possível excluir o usuário. Tente novamente.";
            }

            return RedirectToPage("/Equipe/Index");
        }
    }
}
