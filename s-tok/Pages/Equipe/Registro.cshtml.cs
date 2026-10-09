using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Equipe
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class RegistroModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly S_tokDbContext _context;

        public RegistroModel(
            UserManager<IdentityUser> userManager,
            S_tokDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public EntradaRegistro Entrada { get; set; } = new();

        public List<string> PerfisDisponiveis { get; private set; } = [];

        private async Task CarregarPerfisAsync()
        {
            PerfisDisponiveis = User.IsInRole("SuperAdmin")
                ? ["Admin", "Operador"]
                : ["Operador"];

            await Task.CompletedTask;
        }

        public async Task OnGetAsync()
        {
            await CarregarPerfisAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CarregarPerfisAsync();

            if (!ModelState.IsValid)
                return Page();

            if (!PerfisDisponiveis.Contains(Entrada.Perfil))
            {
                ModelState.AddModelError(
                    "Entrada.Perfil",
                    "Você não tem permissão para atribuir esse perfil.");

                return Page();
            }

            var emailExistente = await _userManager.FindByEmailAsync(
                Entrada.Email);

            if (emailExistente != null)
            {
                ModelState.AddModelError(
                    "Entrada.Email",
                    "Este e-mail já está cadastrado.");

                return Page();
            }

            var usuarioIdentity = new IdentityUser
            {
                UserName = Entrada.Username,
                Email = Entrada.Email,
                EmailConfirmed = false
            };

            using var transacao = await _context.Database.BeginTransactionAsync();

            try
            {
                var resultado = await _userManager.CreateAsync(
                    usuarioIdentity,
                    Entrada.Senha);

                if (!resultado.Succeeded)
                {
                    await transacao.RollbackAsync();

                    foreach (var erro in resultado.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            erro.Description);
                    }

                    return Page();
                }

                var usuario = new Usuario
                {
                    NomeCompleto = Entrada.NomeCompleto,
                    IdentityUserId = usuarioIdentity.Id
                };

                _context.Usuarios.Add(usuario);

                var resultadoPerfil = await _userManager.AddToRoleAsync(
                    usuarioIdentity,
                    Entrada.Perfil);

                if (!resultadoPerfil.Succeeded)
                {
                    await _userManager.DeleteAsync(usuarioIdentity);
                    await transacao.RollbackAsync();

                    foreach (var erro in resultadoPerfil.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            erro.Description);
                    }

                    return Page();
                }

                await _context.SaveChangesAsync();
                await transacao.CommitAsync();

                TempData["Sucesso"] =
                    "Usuário registrado com sucesso!";

                return RedirectToPage("/Equipe/Index");
            }
            catch
            {
                await transacao.RollbackAsync();

                await _userManager.DeleteAsync(usuarioIdentity);

                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível concluir o cadastro. Verifique os dados e tente novamente.");

                return Page();
            }
        }

        public class EntradaRegistro
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
            [Compare(nameof(Senha),
                ErrorMessage = "As senhas não coincidem.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar senha")]
            public string ConfirmarSenha { get; set; } = string.Empty;

            [Required(ErrorMessage = "Selecione um perfil.")]
            [Display(Name = "Perfil de acesso")]
            public string Perfil { get; set; } = string.Empty;
        }
    }
}
