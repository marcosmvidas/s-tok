namespace s_tok.Models
{
    public class CompraItem
    {
        public int Id { get; set; }
        public int CompraId { get; set; }
        public Compra Compra { get; set; } = null!;
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;

        public decimal Quantidade { get; set; }      // na unidade base
        public decimal CustoUnitario { get; set; }
        public decimal Total { get; set; }
    }
}
