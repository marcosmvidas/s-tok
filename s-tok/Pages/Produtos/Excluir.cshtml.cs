using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Produtos;

public class ExcluirModel : PageModel
{
    private readonly S_tokDbContext _context;

    public ExcluirModel(S_tokDbContext context) => _context = context;

    public IActionResult OnGet(int? id) => RedirectToPage("/Produtos/Index");

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);

        if (produto is null)
        {
            TempData["Erro"] = "O produto não foi encontrado.";
            return RedirectToPage("/Produtos/Index");
        }

        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Produto excluído com sucesso.";
        return RedirectToPage("/Produtos/Index");
    }
}
