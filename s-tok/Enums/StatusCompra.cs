using System.ComponentModel.DataAnnotations;

namespace s_tok.Models.Enums;

public enum StatusCompra
{
    [Display(Name = "Pendente")] Pendente = 1,
    [Display(Name = "Recebida")] Recebida = 2,
    [Display(Name = "Cancelada")] Cancelada = 3
}
