using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Produtos
{
    public class IndexModel : PageModel
    {
        private readonly S_tokDbContext _context;

        public IndexModel(S_tokDbContext context)
        {
            _context = context;
        }

        public List<ProdutoListagemItem> Produtos { get; set; } = new();

        public async Task OnGetAsync()
        {
            var produtos = await _context.Produtos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .Include(p => p.Fornecedor).ThenInclude(f => f!.Pessoa)
                .OrderBy(p => p.Nome)
                .ToListAsync();

            // soma em memória por causa do decimal no SQLite
            var saldos = (await _context.Estoques
                    .AsNoTracking()
                    .Select(e => new { e.ProdutoId, e.Quantidade })
                    .ToListAsync())
                .GroupBy(e => e.ProdutoId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantidade));

            Produtos = produtos.Select(p => new ProdutoListagemItem
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Nome = p.Nome,
                Categoria = p.Categoria?.Nome ?? "-",
                Fornecedor = p.Fornecedor?.Pessoa.Nome ?? "-",
                PrecoCusto = p.PrecoCusto,
                PrecoVenda = p.PrecoVenda,
                Estoque = (saldos.TryGetValue(p.Id, out var s) ? s : 0m).ToString("0.###")
            }).ToList();
        }
    }
}