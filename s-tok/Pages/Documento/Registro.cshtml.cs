using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;
using s_tok.Models.Enums;

namespace s_tok.Pages.Documento;

public class RegistroModel(S_tokDbContext context) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> Pessoas { get; private set; } = [];

    public async Task OnGetAsync() => await CarregarPessoasAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        Input.Numero = new string((Input.Numero ?? "").Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();

        if (Input.Tipo == TipoDocumento.Cpf && Input.Numero.Length != 11)
            ModelState.AddModelError("Input.Numero", "O CPF deve conter 11 dígitos.");

        if (Input.Tipo == TipoDocumento.Cnpj && Input.Numero.Length != 14)
            ModelState.AddModelError("Input.Numero", "O CNPJ deve conter 14 dígitos.");

        if (!ModelState.IsValid)
        {
            await CarregarPessoasAsync();
            return Page();
        }

        if (!await context.Pessoas.AnyAsync(p => p.Id == Input.PessoaId))
            ModelState.AddModelError("Input.PessoaId", "Selecione uma pessoa válida.");

        if (await context.Documentos.AnyAsync(d => d.PessoaId == Input.PessoaId))
            ModelState.AddModelError("Input.PessoaId", "Esta pessoa já possui um documento cadastrado.");

        if (await context.Documentos.AnyAsync(d => d.Numero == Input.Numero))
            ModelState.AddModelError("Input.Numero", "Este número de documento já está cadastrado.");

        if (!ModelState.IsValid)
        {
            await CarregarPessoasAsync();
            return Page();
        }

        context.Documentos.Add(new Documento
        {
            PessoaId = Input.PessoaId,
            Tipo = Input.Tipo,
            Numero = Input.Numero
        });

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível salvar. Verifique se a pessoa ou o documento já possui cadastro.");
            await CarregarPessoasAsync();
            return Page();
        }

        TempData["Sucesso"] = "Documento cadastrado com sucesso!";
        return RedirectToPage("/Documento/Index");
    }

    private async Task CarregarPessoasAsync()
    {
        Pessoas = await context.Pessoas.AsNoTracking()
            .Where(p => !context.Documentos.Any(d => d.PessoaId == p.Id))
            .OrderBy(p => p.Nome)
            .Select(p => new SelectListItem(p.Nome, p.Id.ToString()))
            .ToListAsync();
    }

    public sealed class InputModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Selecione a pessoa.")]
        [Display(Name = "Pessoa")]
        public int PessoaId { get; set; }

        [Display(Name = "Tipo de documento")]
        public TipoDocumento Tipo { get; set; } = TipoDocumento.Cpf;

        [Required, StringLength(20)]
        [Display(Name = "Número do documento")]
        public string Numero { get; set; } = "";
    }
}
