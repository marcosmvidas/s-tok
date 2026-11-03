using System.ComponentModel.DataAnnotations;

namespace s_tok.Models.Enums;

public enum TipoLancamento
{
    [Display(Name = "A receber")] Receber = 1,
    [Display(Name = "A pagar")] Pagar = 2
}
