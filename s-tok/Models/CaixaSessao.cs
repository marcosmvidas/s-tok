namespace s_tok.Models
{
    /// <summary>Abertura/fechamento de caixa por operador.</summary>
    public class CaixaSessao
    {
        public int Id { get; set; }
        public int LojaId { get; set; }
        public Loja Loja { get; set; } = null!;
        public int EquipeId { get; set; }
        public Equipe Equipe { get; set; } = null!;

        public DateTime AbertoEm { get; set; } = DateTime.UtcNow;
        public decimal ValorAbertura { get; set; }
        public DateTime? FechadoEm { get; set; }
        public decimal? ValorFechamento { get; set; }
    }
}
