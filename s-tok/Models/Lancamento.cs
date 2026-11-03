using s_tok.Models.Enums;

namespace s_tok.Models
{
    /// <summary>Contas a receber / a pagar.</summary>
    public class Lancamento
    {
        public int Id { get; set; }
        public TipoLancamento Tipo { get; set; }
        public int PlanoContaId { get; set; }
        public PlanoConta PlanoConta { get; set; } = null!;

        public int? LojaId { get; set; }
        public Loja? Loja { get; set; }
        public int? PessoaId { get; set; }               // cliente ou fornecedor
        public Pessoa? Pessoa { get; set; }

        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateOnly Vencimento { get; set; }
        public DateTime? PagoEm { get; set; }
        public decimal? ValorPago { get; set; }
        public FormaPagamento? Forma { get; set; }
        public StatusLancamento Status { get; set; } = StatusLancamento.Pendente;

        public int? VendaId { get; set; }
        public Venda? Venda { get; set; }
        public int? CompraId { get; set; }
        public Compra? Compra { get; set; }
    }
}
