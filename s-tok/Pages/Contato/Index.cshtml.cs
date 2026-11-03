using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Contato;

public class IndexModel(S_tokDbContext context) : PageModel
{
    public List<ItemContato> Contatos { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Contatos = await context.Contatos
            .AsNoTracking()
            .OrderBy(c => c.Pessoa.Nome)
            .ThenBy(c => c.Tipo)
            .Select(c => new ItemContato
            {
                Id = c.Id,
                Pessoa = c.Pessoa.Nome,
                Tipo = c.Tipo == s_tok.Models.Enums.TipoContato.Telefone ? "Telefone"\n                    : c.Tipo == s_tok.Models.Enums.TipoContato.WhatsApp ? "WhatsApp"\n                    : c.Tipo == s_tok.Models.Enums.TipoContato.Email ? "e-mail" : "Outro",
                Valor = c.Valor,
                Observacao = c.Observacao ?? "—"
            })
            .ToListAsync();
    }

    public sealed class ItemContato
    {
        public int Id { get; set; }
        public string Pessoa { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string Valor { get; set; } = "";
        public string Observacao { get; set; } = "";
    }
}
