namespace s_tok.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;

        public int? CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        public int? FornecedorId { get; set; }
        public Fornecedor? Fornecedor { get; set; }

        public string UnidadeBase { get; set; } = "UN";
        public decimal PrecoCusto { get; set; }
        public decimal PrecoVenda { get; set; }
        public decimal EstoqueMinimo { get; set; }
        public bool Ativo { get; set; } = true;

        public ICollection<ProdutoItem> Itens { get; set; } = new List<ProdutoItem>();
    }
}