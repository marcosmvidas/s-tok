using System.ComponentModel.DataAnnotations;

namespace s_tok.Models.Enums;

public enum TipoPlanoConta
{
    [Display(Name = "Receita")] Receita = 1,
    [Display(Name = "Despesa")] Despesa = 2
}
