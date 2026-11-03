using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Documento;

public class IndexModel(S_tokDbContext context) : PageModel
{
    public List<ItemDocumento> Documentos { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Documentos = await context.Documentos
            .AsNoTracking()
            .OrderBy(d => d.Pessoa.Nome)
            .Select(d => new ItemDocumento
            {
                Id = d.Id,
                Pessoa = d.Pessoa.Nome,
                Tipo = d.Tipo == s_tok.Models.Enums.TipoDocumento.Cpf ? "CPF"\n                    : d.Tipo == s_tok.Models.Enums.TipoDocumento.Cnpj ? "CNPJ"\n                    : d.Tipo == s_tok.Models.Enums.TipoDocumento.Rg ? "RG" : "CNH",
                Numero = d.Numero
            })
            .ToListAsync();
    }

    public sealed class ItemDocumento
    {
        public int Id { get; set; }
        public string Pessoa { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string Numero { get; set; } = "";
    }
}
