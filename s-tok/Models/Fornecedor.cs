namespace s_tok.Models
{
    public class Fornecedor
    {
        public int Id { get; set; }
        public int PessoaId { get; set; }
        public Pessoa Pessoa { get; set; } = null!;
        public bool Ativo { get; set; } = true;
        public string? Observacao { get; set; }
    }
}
