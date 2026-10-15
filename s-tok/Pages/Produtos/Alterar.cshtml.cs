using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Produtos;

public class AlterarModel : PageModel
{
    private readonly S_tokDbContext _context;

    public AlterarModel(S_tokDbContext context) => _context = context;

    [BindProperty]
    public Produto Produto { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var produto = await _context.Produtos.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produto is null)
            return NotFound();

        Produto = produto;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (id != Produto.Id)
            return BadRequest();

        Produto.Nome = Produto.Nome?.Trim() ?? string.Empty;
        Produto.Codigo = Produto.Codigo?.Trim() ?? string.Empty;
        Produto.Categoria = string.IsNullOrWhiteSpace(Produto.Categoria) ? null : Produto.Categoria.Trim();
        Produto.Fornecedor = string.IsNullOrWhiteSpace(Produto.Fornecedor) ? null : Produto.Fornecedor.Trim();

        if (!ModelState.IsValid)
            return Page();

        var produtoExistente = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id);

        if (produtoExistente is null)
            return NotFound();

        var codigo = Produto.Codigo.ToUpperInvariant();
        var codigoDuplicado = await _context.Produtos
            .AnyAsync(p => p.Id != id && p.Codigo.ToUpper() == codigo);

        if (codigoDuplicado)
        {
            ModelState.AddModelError("Produto.Codigo", "Já existe outro produto cadastrado com esse código.");
            return Page();
        }

        produtoExistente.Nome = Produto.Nome;
        produtoExistente.Codigo = Produto.Codigo;
        produtoExistente.Categoria = Produto.Categoria;
        produtoExistente.Fornecedor = Produto.Fornecedor;
        produtoExistente.PrecoCusto = Produto.PrecoCusto;
        produtoExistente.PrecoVenda = Produto.PrecoVenda;
        produtoExistente.QuantidadeEstoque = Produto.QuantidadeEstoque;

        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Produto atualizado com sucesso.";
        return RedirectToPage("/Produtos/Index");
    }
}
