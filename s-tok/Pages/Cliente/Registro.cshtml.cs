using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;
using s_tok.Pages.Shared;

namespace s_tok.Pages.Cliente;

public class RegistroModel(S_tokDbContext context) : PageModel
{
    [BindProperty]
    public CadastroInput Input { get; set; } = new();

    public List<SelectListItem> PessoasDisponiveis { get; private set; } = [];
    public DadosComplementaresPessoaModel? DadosComplementares { get; private set; }

    public async Task<IActionResult> OnGetAsync(int? pessoaId)
    {
        if (pessoaId.HasValue)
        {
            var pessoa = await context.Pessoas.AsNoTracking()
                .Where(p => p.Id == pessoaId.Value && context.Clientes.Any(c => c.PessoaId == p.Id))
                .Select(p => new { p.Id, p.Nome })
                .FirstOrDefaultAsync();

            if (pessoa is null)
                return NotFound();

            DadosComplementares = await MontarDadosComplementaresAsync(pessoa.Id, pessoa.Nome, "/Cliente/Index");
            return Page();
        }

        await CarregarPessoasAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var pessoaExistente = Input.PessoaId.HasValue;
        if (!pessoaExistente && string.IsNullOrWhiteSpace(Input.Nome))
            ModelState.AddModelError("Input.Nome", "Informe o nome da pessoa ou selecione uma pessoa existente.");

        if (!ModelState.IsValid)
        {
            await CarregarPessoasAsync();
            return Page();
        }

        s_tok.Models.Pessoa pessoa;
        if (pessoaExistente)
        {
            pessoa = await context.Pessoas.FirstOrDefaultAsync(p => p.Id == Input.PessoaId!.Value)
                ?? null!;

            if (pessoa is null)
            {
                ModelState.AddModelError("Input.PessoaId", "A pessoa selecionada não foi encontrada.");
                await CarregarPessoasAsync();
                return Page();
            }

            if (await context.Clientes.AnyAsync(c => c.PessoaId == pessoa.Id))
            {
                ModelState.AddModelError("Input.PessoaId", "Esta pessoa já está cadastrada como cliente.");
                await CarregarPessoasAsync();
                return Page();
            }
        }
        else
        {
            pessoa = new s_tok.Models.Pessoa
            {
                Nome = Input.Nome!.Trim(),
                CriadoEm = DateTime.UtcNow
            };
            context.Pessoas.Add(pessoa);
        }

        context.Clientes.Add(new s_tok.Models.Cliente
        {
            Pessoa = pessoa,
            Ativo = Input.Ativo,
            Observacao = TextoOuNull(Input.Observacao)
        });

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível salvar o cliente. Verifique se a pessoa já está vinculada.");
            await CarregarPessoasAsync();
            return Page();
        }

        TempData["Sucesso"] = "Dados principais do cliente salvos. Agora você pode incluir documento, contatos e endereços.";
        return RedirectToPage("/Cliente/Registro", new { pessoaId = pessoa.Id });
    }

    private async Task<DadosComplementaresPessoaModel> MontarDadosComplementaresAsync(
        int pessoaId, string nome, string paginaConcluir)
    {
        return new DadosComplementaresPessoaModel
        {
            PessoaId = pessoaId,
            NomePessoa = nome,
            PaginaConcluir = paginaConcluir,
            PossuiDocumento = await context.Documentos.AnyAsync(d => d.PessoaId == pessoaId),
            QuantidadeContatos = await context.Contatos.CountAsync(c => c.PessoaId == pessoaId),
            QuantidadeEnderecos = await context.Enderecos.CountAsync(e => e.PessoaId == pessoaId)
        };
    }

    private async Task CarregarPessoasAsync()
    {
        PessoasDisponiveis = await context.Pessoas.AsNoTracking()
            .Where(p => !context.Clientes.Any(c => c.PessoaId == p.Id))
            .OrderBy(p => p.Nome)
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nome })
            .ToListAsync();
    }

    private static string? TextoOuNull(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    public sealed class CadastroInput
    {
        [Display(Name = "Pessoa já cadastrada")]
        public int? PessoaId { get; set; }

        [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
        [Display(Name = "Nome da nova pessoa")]
        public string? Nome { get; set; }

        [Display(Name = "Cliente ativo")]
        public bool Ativo { get; set; } = true;

        [StringLength(250)]
        [Display(Name = "Observações")]
        public string? Observacao { get; set; }
    }
}
