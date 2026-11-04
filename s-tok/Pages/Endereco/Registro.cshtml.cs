using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Endereco;

public class RegistroModel(S_tokDbContext context) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> Pessoas { get; private set; } = [];

    public async Task OnGetAsync() => await CarregarPessoasAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        Input.Cep = NormalizarCep(Input.Cep);
        Input.Uf = Input.Uf?.Trim().ToUpperInvariant();

        if (!ModelState.IsValid)
        {
            await CarregarPessoasAsync();
            return Page();
        }

        if (!await context.Pessoas.AnyAsync(p => p.Id == Input.PessoaId))
        {
            ModelState.AddModelError("Input.PessoaId", "Selecione uma pessoa válida.");
            await CarregarPessoasAsync();
            return Page();
        }

        if (Input.Principal)
        {
            var anteriores = await context.Enderecos
                .Where(e => e.PessoaId == Input.PessoaId && e.Principal)
                .ToListAsync();
            anteriores.ForEach(e => e.Principal = false);
        }

        context.Enderecos.Add(new s_tok.Models.Endereco
        {
            PessoaId = Input.PessoaId,
            Logradouro = Input.Logradouro.Trim(),
            Numero = TextoOuNull(Input.Numero),
            Complemento = TextoOuNull(Input.Complemento),
            Bairro = Input.Bairro.Trim(),
            Cidade = Input.Cidade.Trim(),
            Uf = Input.Uf!,
            Cep = Input.Cep,
            Principal = Input.Principal
        });

        await context.SaveChangesAsync();
        TempData["Sucesso"] = "Endereço cadastrado com sucesso!";
        return RedirectToPage("/Endereco/Index");
    }

    private async Task CarregarPessoasAsync()
    {
        Pessoas = await context.Pessoas.AsNoTracking()
            .OrderBy(p => p.Nome)
            .Select(p => new SelectListItem(p.Nome, p.Id.ToString()))
            .ToListAsync();
    }

    private static string? NormalizarCep(string? cep)
    {
        var digitos = new string((cep ?? "").Where(char.IsDigit).ToArray());
        return digitos.Length == 8 ? digitos : TextoOuNull(cep);
    }

    private static string? TextoOuNull(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    public sealed class InputModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Selecione a pessoa.")]
        [Display(Name = "Pessoa")]
        public int PessoaId { get; set; }

        [Required, StringLength(150)]
        [Display(Name = "Logradouro")]
        public string Logradouro { get; set; } = "";

        [StringLength(30)]
        [Display(Name = "Número")]
        public string? Numero { get; set; }

        [StringLength(100)]
        [Display(Name = "Complemento")]
        public string? Complemento { get; set; }

        [Required, StringLength(80)]
        [Display(Name = "Bairro")]
        public string Bairro { get; set; } = "";

        [Required, StringLength(80)]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; } = "";

        [Required, StringLength(2, MinimumLength = 2, ErrorMessage = "Informe a UF com duas letras.")]
        [Display(Name = "UF")]
        public string Uf { get; set; } = "";

        [StringLength(9)]
        [Display(Name = "CEP")]
        public string? Cep { get; set; }

        [Display(Name = "Endereço principal")]
        public bool Principal { get; set; }
    }
}
