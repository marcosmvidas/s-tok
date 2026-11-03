using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace s_tok.Models
{
    public class S_tokDbContext : IdentityDbContext
    {
        public S_tokDbContext(DbContextOptions<S_tokDbContext> options) : base(options) { }

        // Usuários
        public DbSet<Usuario> Usuarios { get; set; }

        // Pessoas
        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<Contato> Contatos { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Equipe> Equipe { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Loja> Lojas { get; set; }

        // Produtos e estoque
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<ProdutoItem> ProdutoItens { get; set; }
        public DbSet<Estoque> Estoques { get; set; }
        public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<CompraItem> CompraItens { get; set; }

        // PDV
        public DbSet<CaixaSessao> CaixaSessoes { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<VendaItem> VendaItens { get; set; }
        public DbSet<VendaPagamento> VendaPagamentos { get; set; }

        // Financeiro
        public DbSet<PlanoConta> PlanoContas { get; set; }
        public DbSet<Lancamento> Lancamentos { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            builder.Properties<decimal>().HavePrecision(18, 2);
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            var ns = typeof(Pessoa).Namespace;
            foreach (var fk in b.Model.GetEntityTypes()
                                 .Where(t => t.ClrType.Namespace == ns)
                                 .SelectMany(t => t.GetForeignKeys())
                                 .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade))
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            // ---------- Pessoas ----------
            b.Entity<Pessoa>().Property(x => x.Nome).IsRequired().HasMaxLength(150);

            b.Entity<Documento>(e =>
            {
                e.Property(x => x.Numero).IsRequired().HasMaxLength(20);
                e.HasOne(x => x.Pessoa).WithOne(p => p.Documento)
                 .HasForeignKey<Documento>(x => x.PessoaId).OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(x => x.PessoaId).IsUnique();
                e.HasIndex(x => x.Numero).IsUnique();
            });

            b.Entity<Contato>(e =>
            {
                e.Property(x => x.Valor).IsRequired().HasMaxLength(150);
                e.Property(x => x.Observacao).HasMaxLength(250);
                e.HasOne(x => x.Pessoa).WithMany(p => p.Contatos)
                 .HasForeignKey(x => x.PessoaId).OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<Endereco>(e =>
            {
                e.Property(x => x.Logradouro).IsRequired().HasMaxLength(150);
                e.Property(x => x.Bairro).IsRequired().HasMaxLength(80);
                e.Property(x => x.Cidade).IsRequired().HasMaxLength(80);
                e.Property(x => x.Uf).IsRequired().HasMaxLength(2);
                e.Property(x => x.Cep).HasMaxLength(9);
                e.HasOne(x => x.Pessoa).WithMany(p => p.Enderecos)
                 .HasForeignKey(x => x.PessoaId).OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<Equipe>(e =>
            {
                e.ToTable("Equipe");
                e.HasOne(x => x.Pessoa).WithOne(p => p.Equipe).HasForeignKey<Equipe>(x => x.PessoaId);
                e.HasIndex(x => x.PessoaId).IsUnique();
            });

            b.Entity<Cliente>(e =>
            {
                e.HasOne(x => x.Pessoa).WithOne(p => p.Cliente).HasForeignKey<Cliente>(x => x.PessoaId);
                e.HasIndex(x => x.PessoaId).IsUnique();
            });

            b.Entity<Fornecedor>(e =>
            {
                e.HasOne(x => x.Pessoa).WithOne(p => p.Fornecedor).HasForeignKey<Fornecedor>(x => x.PessoaId);
                e.HasIndex(x => x.PessoaId).IsUnique();
            });

            b.Entity<Loja>(e =>
            {
                e.Property(x => x.Nome).IsRequired().HasMaxLength(100);
                e.HasIndex(x => x.Nome).IsUnique();
            });

            // ---------- Produtos e estoque ----------
            b.Entity<Categoria>(e =>
            {
                e.Property(x => x.Nome).IsRequired().HasMaxLength(80);
                e.HasIndex(x => x.Nome).IsUnique();
            });

            b.Entity<Produto>(e =>
            {
                e.Property(x => x.Codigo).IsRequired().HasMaxLength(40);
                e.Property(x => x.Nome).IsRequired().HasMaxLength(120);
                e.Property(x => x.UnidadeBase).IsRequired().HasMaxLength(10);
                e.Property(x => x.EstoqueMinimo).HasPrecision(18, 3);
                e.HasIndex(x => x.Codigo).IsUnique();
            });

            b.Entity<ProdutoItem>(e =>
            {
                e.Property(x => x.Descricao).IsRequired().HasMaxLength(100);
                e.Property(x => x.CodigoBarras).HasMaxLength(20);
                e.Property(x => x.FatorConversao).HasPrecision(18, 3);
                e.HasOne(x => x.Produto).WithMany(p => p.Itens)
                 .HasForeignKey(x => x.ProdutoId).OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(x => x.CodigoBarras).IsUnique();
            });

            b.Entity<Estoque>(e =>
            {
                e.Property(x => x.Quantidade).HasPrecision(18, 3);
                e.HasIndex(x => new { x.ProdutoId, x.LojaId }).IsUnique();
            });

            b.Entity<MovimentacaoEstoque>(e =>
            {
                e.Property(x => x.Quantidade).HasPrecision(18, 3);
                e.Property(x => x.Observacao).HasMaxLength(250);
                e.HasIndex(x => new { x.ProdutoId, x.LojaId, x.DataHora });
            });

            b.Entity<Compra>(e =>
            {
                e.Property(x => x.NumeroNota).HasMaxLength(40);
            });

            b.Entity<CompraItem>(e =>
            {
                e.Property(x => x.Quantidade).HasPrecision(18, 3);
                e.HasOne(x => x.Compra).WithMany(c => c.Itens)
                 .HasForeignKey(x => x.CompraId).OnDelete(DeleteBehavior.Cascade);
            });

            // ---------- PDV ----------
            b.Entity<Venda>(e =>
            {
                e.HasIndex(x => new { x.LojaId, x.Numero }).IsUnique();
                e.HasIndex(x => x.DataHora);
            });

            b.Entity<VendaItem>(e =>
            {
                e.Property(x => x.Quantidade).HasPrecision(18, 3);
                e.HasOne(x => x.Venda).WithMany(v => v.Itens)
                 .HasForeignKey(x => x.VendaId).OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<VendaPagamento>(e =>
            {
                e.HasOne(x => x.Venda).WithMany(v => v.Pagamentos)
                 .HasForeignKey(x => x.VendaId).OnDelete(DeleteBehavior.Cascade);
            });

            // ---------- Financeiro ----------
            b.Entity<PlanoConta>(e =>
            {
                e.Property(x => x.Codigo).IsRequired().HasMaxLength(20);
                e.Property(x => x.Nome).IsRequired().HasMaxLength(100);
                e.HasIndex(x => x.Codigo).IsUnique();
                e.HasOne(x => x.Pai).WithMany(x => x.Filhos).HasForeignKey(x => x.PaiId);
            });

            b.Entity<Lancamento>(e =>
            {
                e.Property(x => x.Descricao).IsRequired().HasMaxLength(200);
                e.HasIndex(x => new { x.Status, x.Vencimento });
            });

            // ---------- Usuário ----------
            b.Entity<Usuario>(e =>
            {
                e.HasOne(x => x.Pessoa).WithOne(p => p.Usuario).HasForeignKey<Usuario>(x => x.PessoaId);
                e.HasIndex(x => x.PessoaId).IsUnique();
                e.HasIndex(x => x.IdentityUserId).IsUnique();
            });
        }
    }
}