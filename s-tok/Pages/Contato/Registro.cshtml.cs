using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;
using s_tok.Models.Enums;

namespace s_tok.Pages.Contato;

public class RegistroModel(S_tokDbContext context) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> Pessoas { get; private set; } = [];

    public async Task OnGetAsync() => await CarregarPessoasAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        Input.Valor = Input.Valor?.Trim() ?? "";
        Input.Observacao = string.IsNullOrWhiteSpace(Input.Observacao) ? null : Input.Observacao.Trim();

        if (Input.Tipo == TipoContato.Email &&
            !string.IsNullOrWhiteSpace(Input.Valor) &&
            !new EmailAddressAttribute().IsValid(Input.Valor))
            ModelState.AddModelError("Input.Valor", "Informe um endereço de e-mail válido.");

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

        context.Contatos.Add(new Contato
        {
            PessoaId = Input.PessoaId,
            Tipo = Input.Tipo,
            Valor = Input.Valor,
            Observacao = Input.Observacao
        });

        await context.SaveChangesAsync();
        TempData["Sucesso"] = "Contato cadastrado com sucesso!";
        return RedirectToPage("/Contato/Index");
    }

    private async Task CarregarPessoasAsync()
    {
        Pessoas = await context.Pessoas.AsNoTracking()
            .OrderBy(p => p.Nome)
            .Select(p => new SelectListItem(p.Nome, p.Id.ToString()))
            .ToListAsync();
    }

    public sealed class InputModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Selecione a pessoa.")]
        [Display(Name = "Pessoa")]
        public int PessoaId { get; set; }

        [Display(Name = "Tipo de contato")]
        public TipoContato Tipo { get; set; } = TipoContato.Telefone;

        [Required, StringLength(150)]
        [Display(Name = "Contato")]
        public string Valor { get; set; } = "";

        [StringLength(250)]
        [Display(Name = "Observação")]
        public string? Observacao { get; set; }
    }
}
