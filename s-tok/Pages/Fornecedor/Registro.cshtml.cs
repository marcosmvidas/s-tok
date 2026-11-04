
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;
using s_tok.Pages.Shared;

namespace s_tok.Pages.Fornecedor;

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
            var pessoa = await context.Pessoas
                .AsNoTracking()
                .Where(p =>
                    p.Id == pessoaId.Value &&
                    context.Fornecedores.Any(f => f.PessoaId == p.Id))
                .Select(p => new { p.Id, p.Nome })
                .FirstOrDefaultAsync();

            if (pessoa is null)
                return NotFound();

            DadosComplementares = await MontarDadosComplementaresAsync(
                pessoa.Id,
                pessoa.Nome,
                "/Fornecedor/Index");

            return Page();
        }

        await CarregarPessoasAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var pessoaExistente = Input.PessoaId.HasValue;

        if (!pessoaExistente && string.IsNullOrWhiteSpace(Input.Nome))
        {
            ModelState.AddModelError(
                "Input.Nome",
                "Informe o nome da pessoa ou selecione uma pessoa existente.");
        }

        if (!ModelState.IsValid)
        {
            await CarregarPessoasAsync();
            return Page();
        }

        s_tok.Models.Pessoa pessoa;

        if (pessoaExistente)
        {
            pessoa = await context.Pessoas
                .FirstOrDefaultAsync(p => p.Id == Input.PessoaId!.Value);

            if (pessoa is null)
            {
                ModelState.AddModelError(
                    "Input.PessoaId",
                    "A pessoa selecionada não foi encontrada.");

                await CarregarPessoasAsync();
                return Page();
            }

            var fornecedorExistente = await context.Fornecedores
                .AnyAsync(f => f.PessoaId == pessoa.Id);

            if (fornecedorExistente)
            {
                ModelState.AddModelError(
                    "Input.PessoaId",
                    "Esta pessoa já está cadastrada como fornecedor.");

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

        var fornecedor = new s_tok.Models.Fornecedor
        {
            Pessoa = pessoa,
            Ativo = Input.Ativo,
            Observacao = TextoOuNull(Input.Observacao)
        };

        context.Fornecedores.Add(fornecedor);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Não foi possível salvar o fornecedor. Verifique se a pessoa já está vinculada.");

            await CarregarPessoasAsync();
            return Page();
        }

        TempData["Sucesso"] =
            "Dados principais do fornecedor salvos. Agora você pode incluir documento, contatos e endereços.";

        return RedirectToPage("/Fornecedor/Registro", new
        {
            pessoaId = pessoa.Id
        });
    }

    private async Task<DadosComplementaresPessoaModel> MontarDadosComplementaresAsync(
        int pessoaId,
        string nome,
        string paginaConcluir)
    {
        return new DadosComplementaresPessoaModel
        {
            PessoaId = pessoaId,
            NomePessoa = nome,
            PaginaConcluir = paginaConcluir,

            PossuiDocumento = await context.Documentos
                .AnyAsync(d => d.PessoaId == pessoaId),

            QuantidadeContatos = await context.Contatos
                .CountAsync(c => c.PessoaId == pessoaId),

            QuantidadeEnderecos = await context.Enderecos
                .CountAsync(e => e.PessoaId == pessoaId)
        };
    }

    private async Task CarregarPessoasAsync()
    {
        PessoasDisponiveis = await context.Pessoas
            .AsNoTracking()
            .Where(p => !context.Fornecedores.Any(f => f.PessoaId == p.Id))
            .OrderBy(p => p.Nome)
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Nome
            })
            .ToListAsync();
    }

    private static string? TextoOuNull(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor.Trim();
    }

    public sealed class CadastroInput
    {
        [Display(Name = "Pessoa já cadastrada")]
        public int? PessoaId { get; set; }

        [StringLength(
            150,
            ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
        [Display(Name = "Nome da nova pessoa")]
        public string? Nome { get; set; }

        [Display(Name = "Fornecedor ativo")]
        public bool Ativo { get; set; } = true;

        [StringLength(250)]
        [Display(Name = "Observações")]
        public string? Observacao { get; set; }
    }
}
