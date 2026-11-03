
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using s_tok.Data;
using s_tok.Models;
using System.ComponentModel.DataAnnotations;

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

        public class CadastroInput
        {
            public int? PessoaId { get; set; }

            public string? Nome { get; set; }

            public bool Ativo { get; set; } = true;

            public string? Observacao { get; set; }
        }

        public async Task OnGetAsync()
        {
            await CarregarPessoasAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var pessoaExistente = Input.PessoaId.HasValue;
            var novaPessoa = !string.IsNullOrWhiteSpace(Input.Nome);

            if (pessoaExistente == novaPessoa)
            {
                ModelState.AddModelError(
                    "Input.PessoaId",
                    "Selecione uma pessoa existente ou informe o nome de uma nova pessoa."
                );
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
                    .FirstOrDefaultAsync(p => p.Id == Input.PessoaId)
                    ?? null!;

                if (pessoa == null)
                {
                    ModelState.AddModelError(
                        "Input.PessoaId",
                        "A pessoa selecionada não foi encontrada."
                    );

                    await CarregarPessoasAsync();
                    return Page();
                }

                bool jaExiste = await _context.Clientes
                    .AnyAsync(c => c.PessoaId == pessoa.Id);

                if (jaExiste)
                {
                    ModelState.AddModelError(
                        "Input.PessoaId",
                        "Esta pessoa já está cadastrada como cliente."
                    );

                    await CarregarPessoasAsync();
                    return Page();
                }
            }
            else
            {
                pessoa = new Pessoa
                {
                    Nome = Input.Nome!.Trim(),
                    CriadoEm = DateTime.Now
                };

                _context.Pessoas.Add(pessoa);
            }

            var cliente = new s_tok.Models.Cliente
            {
                Pessoa = pessoa,
                Ativo = Input.Ativo,
                Observacao = Input.Observacao
            };

            _context.Clientes.Add(cliente);

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Cliente cadastrado com sucesso!";

            return RedirectToPage("/Clientes/Index");
        }

        private async Task CarregarPessoasAsync()
        {
            PessoasDisponiveis = await _context.Pessoas
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nome
                })
                .ToListAsync();
        }
    }
}
