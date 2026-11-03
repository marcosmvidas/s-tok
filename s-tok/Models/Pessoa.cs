namespace s_tok.Models
{
    public class Pessoa
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        public Documento? Documento { get; set; }
        public ICollection<Contato> Contatos { get; set; } = new List<Contato>();
        public ICollection<Endereco> Enderecos { get; set; } = new List<Endereco>();

        public Equipe? Equipe { get; set; }
        public Cliente? Cliente { get; set; }
        public Fornecedor? Fornecedor { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
