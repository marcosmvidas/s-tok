using System.ComponentModel.DataAnnotations;

namespace s_tok.Models.Enums;

public enum StatusLancamento
{
    [Display(Name = "Pendente")] Pendente = 1,
    [Display(Name = "Pago")] Pago = 2,
    [Display(Name = "Cancelado")] Cancelado = 3
}
