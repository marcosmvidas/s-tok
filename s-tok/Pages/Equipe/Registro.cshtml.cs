using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;
using s_tok.Pages.Shared;

namespace s_tok.Pages.Equipe;

[Authorize(Roles = "SuperAdmin,Admin")]
public class RegistroModel(
    UserManager<IdentityUser> userManager,
    S_tokDbContext context) : PageModel
{
    [BindProperty]
    public EntradaRegistro Entrada { get; set; } = new();

    public List<string> PerfisDisponiveis { get; private set; } = [];
    public DadosComplementaresPessoaModel? DadosComplementares { get; private set; }

    private Task CarregarPerfisAsync()
    {
        PerfisDisponiveis = User.IsInRole("SuperAdmin")
            ? ["Admin", "Operador"]
            : ["Operador"];
        return Task.CompletedTask;
    }

    public async Task<IActionResult> OnGetAsync(int? pessoaId)
    {
        await CarregarPerfisAsync();

        if (pessoaId.HasValue)
        {
            var pessoa = await context.Pessoas.AsNoTracking()
                .Where(p => p.Id == pessoaId.Value && context.Equipe.Any(e => e.PessoaId == p.Id))
                .Select(p => new { p.Id, p.Nome })
                .FirstOrDefaultAsync();

            if (pessoa is null)
                return NotFound();

            DadosComplementares = new DadosComplementaresPessoaModel
            {
                PessoaId = pessoa.Id,
                NomePessoa = pessoa.Nome,
                PaginaConcluir = "/Equipe/Index",
                PossuiDocumento = await context.Documentos.AnyAsync(d => d.PessoaId == pessoa.Id),
                QuantidadeContatos = await context.Contatos.CountAsync(c => c.PessoaId == pessoa.Id),
                QuantidadeEnderecos = await context.Enderecos.CountAsync(e => e.PessoaId == pessoa.Id)
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await CarregarPerfisAsync();

        if (!ModelState.IsValid)
            return Page();

        if (!PerfisDisponiveis.Contains(Entrada.Perfil))
        {
            ModelState.AddModelError("Entrada.Perfil", "Você não tem permissão para atribuir esse perfil.");
            return Page();
        }

        if (await userManager.FindByEmailAsync(Entrada.Email) is not null)
        {
            ModelState.AddModelError("Entrada.Email", "Este e-mail já está cadastrado.");
            return Page();
        }

        var usuarioIdentity = new IdentityUser
        {
            UserName = Entrada.Username,
            Email = Entrada.Email,
            EmailConfirmed = false
        };

        await using var transacao = await context.Database.BeginTransactionAsync();
        try
        {
            var resultado = await userManager.CreateAsync(usuarioIdentity, Entrada.Senha);
            if (!resultado.Succeeded)
            {
                await transacao.RollbackAsync();
                foreach (var erro in resultado.Errors)
                    ModelState.AddModelError(string.Empty, erro.Description);
                return Page();
            }

            var pessoa = new Pessoa
            {
                Nome = Entrada.NomeCompleto.Trim(),
                CriadoEm = DateTime.UtcNow
            };

            context.Pessoas.Add(pessoa);
            context.Usuarios.Add(new Usuario
            {
                NomeCompleto = Entrada.NomeCompleto.Trim(),
                IdentityUserId = usuarioIdentity.Id,
                Pessoa = pessoa
            });
            context.Equipe.Add(new Equipe
            {
                Pessoa = pessoa,
                Ativo = true
            });

            var resultadoPerfil = await userManager.AddToRoleAsync(usuarioIdentity, Entrada.Perfil);
            if (!resultadoPerfil.Succeeded)
            {
                await transacao.RollbackAsync();
                foreach (var erro in resultadoPerfil.Errors)
                    ModelState.AddModelError(string.Empty, erro.Description);
                return Page();
            }

            await context.SaveChangesAsync();
            await transacao.CommitAsync();

            TempData["Sucesso"] = "Acesso e cadastro da equipe criados. Agora você pode incluir documento, contatos e endereços.";
            return RedirectToPage("/Equipe/Registro", new { pessoaId = pessoa.Id });
        }
        catch
        {
            await transacao.RollbackAsync();
            ModelState.AddModelError(string.Empty,
                "Não foi possível concluir o cadastro. Verifique os dados e tente novamente.");
            return Page();
        }
    }

    public sealed class EntradaRegistro
    {
        [Required(ErrorMessage = "Informe o nome completo.")]
        [StringLength(150)]
        [Display(Name = "Nome completo")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o nome de usuário.")]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Nome de usuário")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a senha.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme a senha.")]
        [Compare(nameof(Senha), ErrorMessage = "As senhas não coincidem.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar senha")]
        public string ConfirmarSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione um perfil.")]
        [Display(Name = "Perfil de acesso")]
        public string Perfil { get; set; } = string.Empty;
    }
}
