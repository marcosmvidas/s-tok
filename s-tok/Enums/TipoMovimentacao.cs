using System.ComponentModel.DataAnnotations;

namespace s_tok.Models.Enums;

public enum TipoMovimentacao
{
    [Display(Name = "Entrada")] Entrada = 1,
    [Display(Name = "Saída")] Saida = 2,
    [Display(Name = "Ajuste")] Ajuste = 3,
    [Display(Name = "Venda")] Venda = 4,
    [Display(Name = "Devolução")] Devolucao = 5
}
