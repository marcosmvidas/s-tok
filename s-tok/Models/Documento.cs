using s_tok.Models.Enums;

namespace s_tok.Models
{
    public class Documento
    {
        public int Id { get; set; }
        public int PessoaId { get; set; }
        public Pessoa Pessoa { get; set; } = null!;

        public TipoDocumento Tipo { get; set; } = TipoDocumento.Cpf;
        public string Numero { get; set; } = string.Empty;   // somente dígitos
    }
}
