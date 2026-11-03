using s_tok.Models.Enums;

namespace s_tok.Models
{
    public class VendaPagamento
    {
        public int Id { get; set; }
        public int VendaId { get; set; }
        public Venda Venda { get; set; } = null!;
        public FormaPagamento Forma { get; set; }
        public decimal Valor { get; set; }
    }
}
