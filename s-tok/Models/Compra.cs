using s_tok.Models.Enums;

namespace s_tok.Models
{
    public class Compra
    {
        public int Id { get; set; }
        public int FornecedorId { get; set; }
        public Fornecedor Fornecedor { get; set; } = null!;
        public int LojaId { get; set; }
        public Loja Loja { get; set; } = null!;

        public DateTime DataCompra { get; set; } = DateTime.UtcNow;
        public string? NumeroNota { get; set; }
        public decimal Total { get; set; }
        public StatusCompra Status { get; set; } = StatusCompra.Pendente;

        public ICollection<CompraItem> Itens { get; set; } = new List<CompraItem>();
    }
}
