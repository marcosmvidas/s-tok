using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using s_tok.Models;

namespace s_tok.Pages.Fornecedores
{
    public class IndexModel : PageModel
    {
        private readonly S_tokDbContext _context;

        public IndexModel(S_tokDbContext context)
        {
            _context = context;
        }

        public List<FornecedorListagemItem> Fornecedores { get; private set; } = [];

        public async Task OnGetAsync()
        {
            var fornecedores = await _context.Fornecedores
                .AsNoTracking()
                .Include(f => f.Pessoa)
                    .ThenInclude(p => p.Documento)
                .Include(f => f.Pessoa)
                    .ThenInclude(p => p.Contatos)
                .OrderBy(f => f.Pessoa.Nome)
                .ToListAsync();

            Fornecedores = fornecedores.Select(f => new FornecedorListagemItem
            {
                Id = f.Id,
                Nome = f.Pessoa.Nome,
                Documento = f.Pessoa.Documento?.Numero ?? "—",
                Contato = f.Pessoa.Contatos
                    .OrderBy(x => x.Tipo)
                    .Select(x => x.Valor)
                    .FirstOrDefault() ?? "—",
                Ativo = f.Ativo
            }).ToList();
        }

        public sealed class FornecedorListagemItem
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public string Documento { get; set; } = string.Empty;
            public string Contato { get; set; } = string.Empty;
            public bool Ativo { get; set; }
        }
    }
}
