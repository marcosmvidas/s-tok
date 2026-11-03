using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Clientes
{
    public class IndexModel : PageModel
    {
        private readonly S_tokDbContext _context;

        public IndexModel(S_tokDbContext context)
        {
            _context = context;
        }

        public List<ClienteListagemItem> Clientes { get; private set; } = [];

        public async Task OnGetAsync()
        {
            var clientes = await _context.Clientes
                .AsNoTracking()
                .Include(c => c.Pessoa)
                    .ThenInclude(p => p.Documento)
                .Include(c => c.Pessoa)
                    .ThenInclude(p => p.Contatos)
                .OrderBy(c => c.Pessoa.Nome)
                .ToListAsync();

            Clientes = clientes.Select(c => new ClienteListagemItem
            {
                Id = c.Id,
                Nome = c.Pessoa.Nome,
                Documento = c.Pessoa.Documento?.Numero ?? "—",
                Contato = c.Pessoa.Contatos
                    .OrderBy(x => x.Tipo)
                    .Select(x => x.Valor)
                    .FirstOrDefault() ?? "—",
                Ativo = c.Ativo
            }).ToList();
        }

        public sealed class ClienteListagemItem
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public string Documento { get; set; } = string.Empty;
            public string Contato { get; set; } = string.Empty;
            public bool Ativo { get; set; }
        }
    }
}
