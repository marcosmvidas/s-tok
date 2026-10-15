using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Produtos;

public class DetalhesModel : PageModel
{
    private readonly S_tokDbContext _context;

    public DetalhesModel(S_tokDbContext context) => _context = context;

    public Produto Produto { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var produto = await _context.Produtos.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto is null)
            return NotFound();

        Produto = produto;
        return Page();
    }
}
