using s_tok.Models.Enums;

namespace s_tok.Models
{
    /// <summary>Plano de contas em árvore (ex.: 1 Receitas, 1.01 Vendas; 2 Despesas, 2.01 Aluguel).</summary>
    public class PlanoConta
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public TipoPlanoConta Tipo { get; set; }

        public int? PaiId { get; set; }
        public PlanoConta? Pai { get; set; }
        public ICollection<PlanoConta> Filhos { get; set; } = new List<PlanoConta>();

        public bool AceitaLancamento { get; set; } = true;   // contas "grupo" não recebem lançamentos
        public bool Ativa { get; set; } = true;
    }
}
