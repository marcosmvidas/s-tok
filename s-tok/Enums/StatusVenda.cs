using System.ComponentModel.DataAnnotations;

namespace s_tok.Models.Enums;

public enum StatusVenda
{
    [Display(Name = "Aberta")] Aberta = 1,
    [Display(Name = "Concluída")] Concluida = 2,
    [Display(Name = "Cancelada")] Cancelada = 3
}
