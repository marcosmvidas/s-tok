using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Produtos
{
    public class RegistroModel : PageModel
    {
        private readonly S_tokDbContext _context;

        public RegistroModel(S_tokDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Produto Produto { get; set; } = new();

        public SelectList Categorias { get; set; } = null!;
        public SelectList Fornecedores { get; set; } = null!;

        public async Task OnGetAsync()
        {
            await CarregarListasAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Produto.Codigo = Produto.Codigo?.Trim() ?? string.Empty;
            Produto.Nome = Produto.Nome?.Trim() ?? string.Empty;

            if (Produto.Codigo.Length > 0 &&
                await _context.Produtos.AnyAsync(p => p.Codigo == Produto.Codigo))
            {
                ModelState.AddModelError("Produto.Codigo", "Já existe um produto com este código.");
            }

            if (!ModelState.IsValid)
            {
                await CarregarListasAsync();
                return Page();
            }

            _context.Produtos.Add(Produto);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        private async Task CarregarListasAsync()
        {
            Categorias = new SelectList(
                await _context.Categorias.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome");

            Fornecedores = new SelectList(
                await _context.Fornecedores
                    .Where(f => f.Ativo)
                    .Select(f => new { f.Id, f.Pessoa.Nome })
                    .OrderBy(f => f.Nome)
                    .ToListAsync(), "Id", "Nome");
        }
    }
}