using s_tok.Models.Enums;

namespace s_tok.Models
{
    /// <summary>Histórico de movimentações. Quantidade é com sinal: positiva entra, negativa sai.</summary>
    public class MovimentacaoEstoque
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;
        public int LojaId { get; set; }
        public Loja Loja { get; set; } = null!;

        public TipoMovimentacao Tipo { get; set; }
        public decimal Quantidade { get; set; }
        public decimal? CustoUnitario { get; set; }
        public DateTime DataHora { get; set; } = DateTime.UtcNow;

        public int? EquipeId { get; set; }
        public Equipe? Equipe { get; set; }
        public int? VendaId { get; set; }
        public Venda? Venda { get; set; }
        public int? CompraId { get; set; }
        public Compra? Compra { get; set; }
        public string? Observacao { get; set; }
    }
}
