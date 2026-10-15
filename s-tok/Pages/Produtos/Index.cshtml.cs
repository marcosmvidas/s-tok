using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Produtos;

public class IndexModel : PageModel
{
    private readonly S_tokDbContext _context;

    public IndexModel(S_tokDbContext context) => _context = context;

    public List<Produto> Produtos { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Produtos = await _context.Produtos
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }
}
