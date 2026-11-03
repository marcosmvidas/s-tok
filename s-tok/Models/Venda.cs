using s_tok.Models.Enums;

namespace s_tok.Models
{
    public class Venda
    {
        public int Id { get; set; }
        public int Numero { get; set; }                  // sequencial por loja

        public int LojaId { get; set; }
        public Loja Loja { get; set; } = null!;
        public int? CaixaSessaoId { get; set; }
        public CaixaSessao? CaixaSessao { get; set; }
        public int? ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
        public int EquipeId { get; set; }                // operador/vendedor
        public Equipe Equipe { get; set; } = null!;

        public DateTime DataHora { get; set; } = DateTime.UtcNow;
        public decimal Subtotal { get; set; }
        public decimal Desconto { get; set; }
        public decimal Total { get; set; }
        public StatusVenda Status { get; set; } = StatusVenda.Aberta;

        public ICollection<VendaItem> Itens { get; set; } = new List<VendaItem>();
        public ICollection<VendaPagamento> Pagamentos { get; set; } = new List<VendaPagamento>();
    }
}
