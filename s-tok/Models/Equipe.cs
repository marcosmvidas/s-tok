namespace s_tok.Models
{
    public class Equipe
    {
        public int Id { get; set; }

        public int PessoaId { get; set; }
        public Pessoa Pessoa { get; set; } = null!;

        public int? LojaId { get; set; }
        public Loja? Loja { get; set; }

        public bool Ativo { get; set; } = true;
    }
}