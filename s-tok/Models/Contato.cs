using s_tok.Models.Enums;

namespace s_tok.Models
{
    public class Contato
    {
        public int Id { get; set; }
        public int PessoaId { get; set; }
        public Pessoa Pessoa { get; set; } = null!;

        public TipoContato Tipo { get; set; }
        public string Valor { get; set; } = string.Empty;
        public string? Observacao { get; set; }
    }
}
