using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;
using s_tok.Models.Enums;

namespace s_tok.Pages.Clientes
{
    public class RegistroModel : PageModel
    {
        private readonly S_tokDbContext _context;

        public RegistroModel(S_tokDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CadastroInput Input { get; set; } = new();

        public List<SelectListItem> PessoasDisponiveis { get; set; } = [];

        public async Task OnGetAsync()
        {
            await CarregarPessoasAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var pessoaExistente = Input.PessoaId.HasValue;
            var nomeInformado = !string.IsNullOrWhiteSpace(Input.Nome);

            if (pessoaExistente == nomeInformado)
            {
                ModelState.AddModelError(
                    nameof(Input.PessoaId),
                    "Selecione uma pessoa existente ou informe o nome de uma nova pessoa.");
            }

            var documentoNumero = NormalizarDigitos(Input.DocumentoNumero);
            var contatoValor = Input.ContatoValor?.Trim();
            var possuiEndereco = new[]
            {
                Input.Logradouro, Input.Numero, Input.Complemento,
                Input.Bairro, Input.Cidade, Input.Uf, Input.Cep
            }.Any(valor => !string.IsNullOrWhiteSpace(valor));

            if (!pessoaExistente && !string.IsNullOrWhiteSpace(documentoNumero))
            {
                if (Input.TipoDocumento == TipoDocumento.Cpf && documentoNumero.Length != 11)
                    ModelState.AddModelError(nameof(Input.DocumentoNumero), "O CPF deve conter 11 dígitos.");

                if (Input.TipoDocumento == TipoDocumento.Cnpj && documentoNumero.Length != 14)
                    ModelState.AddModelError(nameof(Input.DocumentoNumero), "O CNPJ deve conter 14 dígitos.");

                if (await _context.Documentos.AnyAsync(d => d.Numero == documentoNumero))
                    ModelState.AddModelError(nameof(Input.DocumentoNumero), "Este documento já está vinculado a outra pessoa.");
            }

            if (!pessoaExistente && possuiEndereco)
            {
                if (string.IsNullOrWhiteSpace(Input.Logradouro))
                    ModelState.AddModelError(nameof(Input.Logradouro), "Informe o logradouro.");

                if (string.IsNullOrWhiteSpace(Input.Bairro))
                    ModelState.AddModelError(nameof(Input.Bairro), "Informe o bairro.");

                if (string.IsNullOrWhiteSpace(Input.Cidade))
                    ModelState.AddModelError(nameof(Input.Cidade), "Informe a cidade.");

                if (string.IsNullOrWhiteSpace(Input.Uf) || Input.Uf.Trim().Length != 2)
                    ModelState.AddModelError(nameof(Input.Uf), "Informe a UF com duas letras.");
            }

            if (!ModelState.IsValid)
            {
                await CarregarPessoasAsync();
                return Page();
            }

            Pessoa pessoa;

            if (pessoaExistente)
            {
                pessoa = await _context.Pessoas
                    .FirstOrDefaultAsync(p => p.Id == Input.PessoaId!.Value)
                    ?? null!;

                if (pessoa is null)
                {
                    ModelState.AddModelError(nameof(Input.PessoaId), "A pessoa selecionada não foi encontrada.");
                    await CarregarPessoasAsync();
                    return Page();
                }

                if (await _context.Clientes.AnyAsync(c => c.PessoaId == pessoa.Id))
                {
                    ModelState.AddModelError(nameof(Input.PessoaId), "Esta pessoa já está cadastrada como cliente.");
                    await CarregarPessoasAsync();
                    return Page();
                }
            }
            else
            {
                pessoa = new Pessoa
                {
                    Nome = Input.Nome!.Trim(),
                    CriadoEm = DateTime.UtcNow
                };

                if (!string.IsNullOrWhiteSpace(documentoNumero))
                {
                    pessoa.Documento = new Documento
                    {
                        Tipo = Input.TipoDocumento,
                        Numero = documentoNumero
                    };
                }

                if (!string.IsNullOrWhiteSpace(contatoValor))
                {
                    pessoa.Contatos.Add(new Contato
                    {
                        Tipo = Input.TipoContato,
                        Valor = contatoValor
                    });
                }

                if (possuiEndereco)
                {
                    pessoa.Enderecos.Add(new Endereco
                    {
                        Logradouro = Input.Logradouro!.Trim(),
                        Numero = TextoOuNull(Input.Numero),
                        Complemento = TextoOuNull(Input.Complemento),
                        Bairro = Input.Bairro!.Trim(),
                        Cidade = Input.Cidade!.Trim(),
                        Uf = Input.Uf!.Trim().ToUpperInvariant(),
                        Cep = NormalizarCep(Input.Cep),
                        Principal = true
                    });
                }

                _context.Pessoas.Add(pessoa);
            }

            _context.Clientes.Add(new Cliente
            {
                Pessoa = pessoa,
                Ativo = Input.Ativo,
                Observacao = TextoOuNull(Input.Observacao)
            });

            try
            {
                // Pessoa e Cliente são persistidos juntos na mesma operação.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível salvar o cliente. Verifique se o documento ou o vínculo já existe.");
                await CarregarPessoasAsync();
                return Page();
            }

            TempData["Sucesso"] = "Cliente cadastrado com sucesso!";
            return RedirectToPage("/Cliente/Index");
        }

        private async Task CarregarPessoasAsync()
        {
            PessoasDisponiveis = await _context.Pessoas
                .AsNoTracking()
                .Where(p => !_context.Clientes.Any(c => c.PessoaId == p.Id))
                .OrderBy(p => p.Nome)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nome
                })
                .ToListAsync();
        }

        private static string NormalizarDigitos(string? valor) =>
            new((valor ?? string.Empty).Where(char.IsDigit).ToArray());

        private static string? NormalizarCep(string? valor)
        {
            var digitos = NormalizarDigitos(valor);
            return digitos.Length == 8 ? digitos : TextoOuNull(valor);
        }

        private static string? TextoOuNull(string? valor) =>
            string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

        public class CadastroInput
        {
            [Display(Name = "Pessoa já cadastrada")]
            public int? PessoaId { get; set; }

            [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
            [Display(Name = "Nome da nova pessoa")]
            public string? Nome { get; set; }

            [Display(Name = "Tipo de documento")]
            public TipoDocumento TipoDocumento { get; set; } = TipoDocumento.Cpf;

            [StringLength(20)]
            [Display(Name = "Número do documento")]
            public string? DocumentoNumero { get; set; }

            [Display(Name = "Tipo de contato")]
            public TipoContato TipoContato { get; set; } = TipoContato.Telefone;

            [StringLength(150)]
            [Display(Name = "Telefone, WhatsApp ou e-mail")]
            public string? ContatoValor { get; set; }

            [StringLength(150)]
            [Display(Name = "Logradouro")]
            public string? Logradouro { get; set; }

            [StringLength(30)]
            [Display(Name = "Número")]
            public string? Numero { get; set; }

            [StringLength(100)]
            [Display(Name = "Complemento")]
            public string? Complemento { get; set; }

            [StringLength(80)]
            [Display(Name = "Bairro")]
            public string? Bairro { get; set; }

            [StringLength(80)]
            [Display(Name = "Cidade")]
            public string? Cidade { get; set; }

            [StringLength(2)]
            [Display(Name = "UF")]
            public string? Uf { get; set; }

            [StringLength(9)]
            [Display(Name = "CEP")]
            public string? Cep { get; set; }

            [Display(Name = "Cliente ativo")]
            public bool Ativo { get; set; } = true;

            [StringLength(250)]
            [Display(Name = "Observações")]
            public string? Observacao { get; set; }
        }
    }
}
