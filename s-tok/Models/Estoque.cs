namespace s_tok.Models
{
    /// <summary>Saldo atual do produto por loja (na unidade base).</summary>
    public class Estoque
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;
        public int LojaId { get; set; }
        public Loja Loja { get; set; } = null!;
        public decimal Quantidade { get; set; }
    }
}
