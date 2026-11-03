namespace s_tok.Models
{
    public class VendaItem
    {
        public int Id { get; set; }
        public int VendaId { get; set; }
        public Venda Venda { get; set; } = null!;
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;
        public int? ProdutoItemId { get; set; }          // forma de venda (caixa, fardo...)
        public ProdutoItem? ProdutoItem { get; set; }

        public decimal Quantidade { get; set; }          // na forma de venda escolhida
        public decimal PrecoUnitario { get; set; }
        public decimal Desconto { get; set; }
        public decimal Total { get; set; }
    }
}
