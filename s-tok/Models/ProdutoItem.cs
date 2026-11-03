namespace s_tok.Models
{
    /// <summary>Forma de venda do produto (ex.: Unidade, Caixa c/ 100, Fardo c/ 10 caixas).</summary>
    public class ProdutoItem
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;

        public string Descricao { get; set; } = string.Empty;
        public string? CodigoBarras { get; set; }
        public decimal FatorConversao { get; set; } = 1;   // quantas unidades base tem este item
        public decimal PrecoVenda { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
