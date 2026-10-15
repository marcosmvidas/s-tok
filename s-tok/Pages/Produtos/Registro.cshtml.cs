using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Produtos;

public class RegistroModel : PageModel
{
    private readonly S_tokDbContext _context;

    public RegistroModel(S_tokDbContext context) => _context = context;

    [BindProperty]
    public Produto Produto { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Produto.Codigo = Produto.Codigo?.Trim() ?? string.Empty;
        Produto.Nome = Produto.Nome?.Trim() ?? string.Empty;
        Produto.Categoria = string.IsNullOrWhiteSpace(Produto.Categoria) ? null : Produto.Categoria.Trim();
        Produto.Fornecedor = string.IsNullOrWhiteSpace(Produto.Fornecedor) ? null : Produto.Fornecedor.Trim();

        if (!ModelState.IsValid)
            return Page();

        var codigo = Produto.Codigo.ToUpperInvariant();
        var codigoExiste = await _context.Produtos
            .AnyAsync(p => p.Codigo.ToUpper() == codigo);

        if (codigoExiste)
        {
            ModelState.AddModelError("Produto.Codigo", "Já existe um produto cadastrado com esse código.");
            return Page();
        }

        _context.Produtos.Add(Produto);
        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Produto cadastrado com sucesso.";
        return RedirectToPage("/Produtos/Index");
    }
}
