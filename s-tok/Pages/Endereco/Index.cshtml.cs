using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Endereco;

public class IndexModel(S_tokDbContext context) : PageModel
{
    public List<ItemEndereco> Enderecos { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Enderecos = await context.Enderecos
            .AsNoTracking()
            .OrderByDescending(e => e.Principal)
            .ThenBy(e => e.Pessoa.Nome)
            .Select(e => new ItemEndereco
            {
                Id = e.Id,
                Pessoa = e.Pessoa.Nome,
                Logradouro = e.Logradouro + (e.Numero == null ? "" : ", " + e.Numero),
                Bairro = e.Bairro,
                CidadeUf = e.Cidade + "/" + e.Uf,
                Cep = e.Cep ?? "—",
                Principal = e.Principal
            })
            .ToListAsync();
    }

    public sealed class ItemEndereco
    {
        public int Id { get; set; }
        public string Pessoa { get; set; } = "";
        public string Logradouro { get; set; } = "";
        public string Bairro { get; set; } = "";
        public string CidadeUf { get; set; } = "";
        public string Cep { get; set; } = "";
        public bool Principal { get; set; }
    }
}
