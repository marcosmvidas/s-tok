using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;
using s_tok.Models.Enums;

namespace s_tok.Pages.Pessoa;

public class DadosComplementaresModel(S_tokDbContext context) : PageModel
{
    public async Task<IActionResult> OnPostDocumentoAsync([FromForm] DocumentoInput input)
    {
        if (!TryValidateModel(input))
            return ErrosValidacao();

        var numero = ApenasLetrasENumeros(input.Numero).ToUpperInvariant();
        if (input.Tipo == TipoDocumento.Cpf && numero.Length != 11)
            ModelState.AddModelError(nameof(input.Numero), "O CPF deve conter 11 dígitos.");
        if (input.Tipo == TipoDocumento.Cnpj && numero.Length != 14)
            ModelState.AddModelError(nameof(input.Numero), "O CNPJ deve conter 14 dígitos.");

        if (!await context.Pessoas.AnyAsync(p => p.Id == input.PessoaId))
            ModelState.AddModelError(nameof(input.PessoaId), "A pessoa informada não foi encontrada.");
        if (await context.Documentos.AnyAsync(d => d.PessoaId == input.PessoaId))
            ModelState.AddModelError(nameof(input.PessoaId), "Esta pessoa já possui um documento cadastrado.");
        if (await context.Documentos.AnyAsync(d => d.Numero == numero))
            ModelState.AddModelError(nameof(input.Numero), "Este número de documento já está vinculado a outra pessoa.");

        if (!ModelState.IsValid)
            return ErrosValidacao();

        context.Documentos.Add(new Documento
        {
            PessoaId = input.PessoaId,
            Tipo = input.Tipo,
            Numero = numero
        });

        return await SalvarAsync("Documento cadastrado com sucesso.");
    }

    public async Task<IActionResult> OnPostContatoAsync([FromForm] ContatoInput input)
    {
        if (!TryValidateModel(input))
            return ErrosValidacao();

        input.Valor = input.Valor.Trim();
        input.Observacao = TextoOuNull(input.Observacao);

        if (input.Tipo == TipoContato.Email &&
            !new EmailAddressAttribute().IsValid(input.Valor))
            ModelState.AddModelError(nameof(input.Valor), "Informe um endereço de e-mail válido.");

        if (!await context.Pessoas.AnyAsync(p => p.Id == input.PessoaId))
            ModelState.AddModelError(nameof(input.PessoaId), "A pessoa informada não foi encontrada.");

        if (!ModelState.IsValid)
            return ErrosValidacao();

        context.Contatos.Add(new Contato
        {
            PessoaId = input.PessoaId,
            Tipo = input.Tipo,
            Valor = input.Valor,
            Observacao = input.Observacao
        });

        return await SalvarAsync("Contato cadastrado com sucesso.");
    }

    public async Task<IActionResult> OnPostEnderecoAsync([FromForm] EnderecoInput input)
    {
        if (!TryValidateModel(input))
            return ErrosValidacao();

        input.Logradouro = input.Logradouro.Trim();
        input.Bairro = input.Bairro.Trim();
        input.Cidade = input.Cidade.Trim();
        input.Uf = input.Uf.Trim().ToUpperInvariant();
        input.Cep = NormalizarCep(input.Cep);

        if (!await context.Pessoas.AnyAsync(p => p.Id == input.PessoaId))
            ModelState.AddModelError(nameof(input.PessoaId), "A pessoa informada não foi encontrada.");

        if (!ModelState.IsValid)
            return ErrosValidacao();

        await using var transacao = await context.Database.BeginTransactionAsync();

        if (input.Principal)
        {
            var anteriores = await context.Enderecos
                .Where(e => e.PessoaId == input.PessoaId && e.Principal)
                .ToListAsync();
            anteriores.ForEach(e => e.Principal = false);
        }

        context.Enderecos.Add(new Endereco
        {
            PessoaId = input.PessoaId,
            Logradouro = input.Logradouro,
            Numero = TextoOuNull(input.Numero),
            Complemento = TextoOuNull(input.Complemento),
            Bairro = input.Bairro,
            Cidade = input.Cidade,
            Uf = input.Uf,
            Cep = input.Cep,
            Principal = input.Principal
        });

        try
        {
            await context.SaveChangesAsync();
            await transacao.CommitAsync();
            return new JsonResult(new { sucesso = true, mensagem = "Endereço cadastrado com sucesso." });
        }
        catch (DbUpdateException)
        {
            await transacao.RollbackAsync();
            return new ObjectResult(new { sucesso = false, mensagem = "Não foi possível salvar o endereço. Confira os dados e tente novamente." }) { StatusCode = 500 };
        }
    }

    private async Task<IActionResult> SalvarAsync(string mensagem)
    {
        try
        {
            await context.SaveChangesAsync();
            return new JsonResult(new { sucesso = true, mensagem });
        }
        catch (DbUpdateException)
        {
            return new ObjectResult(new { sucesso = false, mensagem = "Não foi possível salvar. Verifique se o dado já está cadastrado." }) { StatusCode = 500 };
        }
    }

    private IActionResult ErrosValidacao() =>
        BadRequest(new
        {
            sucesso = false,
            erros = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e =>
                    string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Confira os dados informados." : e.ErrorMessage))
                .Distinct()
                .ToArray()
        });

    private static string ApenasLetrasENumeros(string? valor) =>
        new((valor ?? string.Empty).Where(char.IsLetterOrDigit).ToArray());

    private static string? NormalizarCep(string? valor)
    {
        var digitos = new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());
        return digitos.Length == 8 ? digitos : TextoOuNull(valor);
    }

    private static string? TextoOuNull(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    public sealed class DocumentoInput
    {
        [Range(1, int.MaxValue)]
        public int PessoaId { get; set; }

        public TipoDocumento Tipo { get; set; } = TipoDocumento.Cpf;

        [Required, StringLength(20)]
        public string Numero { get; set; } = string.Empty;
    }

    public sealed class ContatoInput
    {
        [Range(1, int.MaxValue)]
        public int PessoaId { get; set; }

        public TipoContato Tipo { get; set; } = TipoContato.Telefone;

        [Required, StringLength(150)]
        public string Valor { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Observacao { get; set; }
    }

    public sealed class EnderecoInput
    {
        [Range(1, int.MaxValue)]
        public int PessoaId { get; set; }

        [Required, StringLength(150)]
        public string Logradouro { get; set; } = string.Empty;

        [StringLength(30)]
        public string? Numero { get; set; }

        [StringLength(100)]
        public string? Complemento { get; set; }

        [Required, StringLength(80)]
        public string Bairro { get; set; } = string.Empty;

        [Required, StringLength(80)]
        public string Cidade { get; set; } = string.Empty;

        [Required, StringLength(2, MinimumLength = 2)]
        public string Uf { get; set; } = string.Empty;

        [StringLength(9)]
        public string? Cep { get; set; }

        public bool Principal { get; set; }
    }
}
